using System;
using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class SufferAreasManager : CombatAreasManager
	{
		public int MaxAreaCount;

		protected override CombatAreaType CombatAreaType => CombatAreaType.Suffer;

		protected override Faction ExpirationCheckFactionTurn => Faction.Survivor;

		public SufferAreasManager()
		{
		}

		public SufferAreasManager(int maxAreaCount)
		{
			MaxAreaCount = maxAreaCount;
		}

		protected override void RefreshActorAreaStateInternal(ActorModel actor, GridCoordinate coord, ModelAction actorAction = null)
		{
			if ((base.manager != null && actor.Faction == Faction.Walker) || actor.Faction == Faction.Raider)
			{
				IChallengeDebuffProvider challengeDebuffProvider = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
				if (challengeDebuffProvider != null && ChallengeDebufHelps.GetDebufConfig(challengeDebuffProvider.GetChallengeDebuffs(), ChallengeDebuffType.WalkerMoveLess) != null)
				{
					return;
				}
			}
			if (!actor.IsDead)
			{
				List<GridCoordinate> list = ((actorAction is MoveAction moveAction) ? moveAction.Path.Path : new List<GridCoordinate> { coord });
				Faction activeFaction = base.manager.CombatModel.TurnManager.ActiveFaction;
				foreach (TWDModelObject model in base.manager.CombatModel.Models)
				{
					if (model is SufferArea sufferArea && actor.Faction != sufferArea.Faction && list.Any(sufferArea.IsInArea))
					{
						if (EquipmentPassivePreventControlTrait.TryResistEffect(actor, "SufferActive"))
						{
							return;
						}
						if (actor.Faction == activeFaction)
						{
							TruncatedPath(sufferArea, actor, actorAction);
						}
					}
				}
				GridPath gridPath = GridPath.Create(list);
				CalibratePath(gridPath, base.manager.CombatModel);
				foreach (TWDModelObject model2 in base.manager.CombatModel.Models)
				{
					if (model2 is SufferArea sufferArea2 && actor.Faction != sufferArea2.Faction && gridPath.Path.Any(sufferArea2.IsInArea))
					{
						if (!actor.HasTrait("SufferActive") && actor.Faction == activeFaction)
						{
							ApplyEffect(sufferArea2, actor, actorAction);
						}
						if (!ResistNegativeEffectsTrait.TryResist(actor, "SufferActive"))
						{
							actor.AddTemporaryTrait("SufferActive", default(FixedPoint), null, 0L);
						}
						return;
					}
				}
			}
			actor.RemoveTrait("SufferActive");
		}

		protected override bool ShouldAdd(CombatArea newArea)
		{
			int num = 0;
			foreach (TWDModelObject model in base.manager.CombatModel.Models)
			{
				if (model is SufferArea sufferArea && sufferArea.Faction == newArea.Faction)
				{
					num++;
					if (sufferArea.Coordinate == newArea.Coordinate)
					{
						sufferArea.ExpiryTurn = newArea.ExpiryTurn;
						return false;
					}
				}
			}
			return num < MaxAreaCount;
		}

		protected override void Tick(IEnumerable<ActorModel> actorModels)
		{
			if (actorModels == null)
			{
				return;
			}
			foreach (ActorModel item in new List<ActorModel>(actorModels))
			{
				if (!actorModels.Contains(item))
				{
					continue;
				}
				foreach (TWDModelObject model in base.manager.CombatModel.Models)
				{
					if (model is SufferArea sufferArea && sufferArea.Faction != item.Faction && sufferArea.IsInArea(item.GridCoordinate))
					{
						if (!EquipmentPassivePreventControlTrait.TryResistEffect(item, "SufferActive"))
						{
							ApplyEffect(sufferArea, item);
						}
						break;
					}
				}
			}
		}

		private void ApplyEffect(SufferArea sufferArea, ActorModel actor, ModelAction currentAction = null)
		{
			CombatModel combatModel = base.manager.CombatModel;
			FixedPoint value = 0.0;
			combatModel.AbilityManager.VisitParameter("LeaderBuffMadeToSufferDotRatio", ref value, sufferArea.Owner);
			actor.MoveRangeConsumed = Math.Max(actor.MoveRangeConsumed, actor.MoveRange - 1);
			int num = 0;
			IChallengeDebuffProvider challengeDebuffProvider = MapMissionDebuffHelper.CanUseDebuffMission(combatModel.manager);
			if (challengeDebuffProvider != null && sufferArea.Faction == Faction.Survivor && sufferArea.Owner is SurvivorModel survivorModel)
			{
				TraitEntry traitAnyLevel = sufferArea.Owner.TraitContainer.GetTraitAnyLevel("LeaderBuffMadeToSuffer");
				if (traitAnyLevel == null)
				{
					foreach (ActorModel factionActor in combatModel.GetFactionActors(sufferArea.Faction))
					{
						if (factionActor is SurvivorModel { IsLeader: not false })
						{
							traitAnyLevel = factionActor.TraitContainer.GetTraitAnyLevel("LeaderBuffMadeToSuffer");
							if (traitAnyLevel != null)
							{
								break;
							}
						}
					}
				}
				if (traitAnyLevel != null)
				{
					FixedPoint minDebuffParamPercentageByTraitId = ChallengeDebufHelps.GetMinDebuffParamPercentageByTraitId(challengeDebuffProvider.GetChallengeDebuffs(), ChallengeDebuffType.DebuffTyreeseLT, traitAnyLevel.TraitIdentifier);
					if (minDebuffParamPercentageByTraitId > 0L)
					{
						int val = Math.Min((int)(survivorModel.GetDamageForPreferredWeapon() * minDebuffParamPercentageByTraitId), actor.Hitpoints - 1);
						int val2 = Math.Min((int)(actor.MaxHitPoints * value), actor.Hitpoints - 1);
						num = Math.Min(val, val2);
					}
					else
					{
						num = Math.Min((int)(actor.MaxHitPoints * value), actor.Hitpoints - 1);
					}
				}
			}
			else
			{
				num = Math.Min((int)(actor.MaxHitPoints * value), actor.Hitpoints - 1);
			}
			if (num > 0)
			{
				base.manager.ExecuteAction(new DamageAction(actor, sufferArea.Owner, num, 0, bodyShot: false, critical: false, PlayerRandomChanceResult.Success, DamageType.Suffer));
			}
		}

		private void TruncatedPath(SufferArea sufferArea, ActorModel actor, ModelAction currentAction = null)
		{
			CombatModel combatModel = base.manager.CombatModel;
			if (!(currentAction is MoveAction moveAction))
			{
				return;
			}
			for (int i = 1; i < moveAction.Path.Count - 1; i++)
			{
				GridCoordinate gridCoordinate = moveAction.Path[i];
				if (sufferArea.IsInArea(gridCoordinate))
				{
					int num = i;
					while (num >= 0 && CombatHelpers.IsOccupiedOrBlocked(combatModel, moveAction.Path[num], actor))
					{
						gridCoordinate = moveAction.Path[num];
						num--;
					}
					moveAction.Path.ClipTo(gridCoordinate);
					break;
				}
			}
		}

		private bool CalibratePath(GridPath path, CombatModel combatModel)
		{
			if (path.IsValid)
			{
				if (path.Count > 1 && combatModel.GetOccupier(path.End) != null)
				{
					path.RemoveLast();
					return CalibratePath(path, combatModel);
				}
				return true;
			}
			return false;
		}
	}
}
