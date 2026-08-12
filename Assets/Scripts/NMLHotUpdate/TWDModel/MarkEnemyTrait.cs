using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class MarkEnemyTrait : ActionModifier
	{
		private int leaderMarkAmount;

		private int partyMarkAmount;

		public MarkEnemyTrait()
		{
		}

		public MarkEnemyTrait(int partyMarkAmount, int leaderMarkAmount)
		{
			this.partyMarkAmount = partyMarkAmount;
			this.leaderMarkAmount = leaderMarkAmount;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostChangeTurnAction)
			{
				CombatModel combatModel = actor.manager.CombatModel;
				if (combatModel != null && !combatModel.MissionCompleted)
				{
					TraitEntry traitWithTraitIdentifier = actor.GetTraitWithTraitIdentifier("LeaderBuffMarkEnemy");
					if (actor.Faction != combatModel.TurnManager.ActiveFaction || traitWithTraitIdentifier == null)
					{
						return ActionListClearFlag.Keep;
					}
					int traitLevelIdentifier = UpgradeTraitsData.GetTraitLevelIdentifier(traitWithTraitIdentifier.TraitIdentifier);
					string traitIdentifier = UpgradeTraitsData.CompileUpgradeTraitIdentifier("DebuffMarkEnemy", traitLevelIdentifier, isLocked: false);
					int canMarkAmount = GetCanMarkAmount(actor);
					if (canMarkAmount == 0)
					{
						return ActionListClearFlag.Keep;
					}
					List<ActorModel> targets = new List<ActorModel>();
					foreach (ActorModel allActor in combatModel.GetAllActors())
					{
						if (allActor.HasAnyLevelTrait("DebuffMarkEnemy"))
						{
							TraitEntry trait = allActor.TraitContainer.GetTrait(traitIdentifier);
							if (trait != null && trait.Tag == actor.Faction.ToString())
							{
								allActor.RemoveTrait(traitIdentifier);
							}
						}
						if (allActor.IsEnemy(actor) && !allActor.IsEnvironmental && !allActor.IsDead && !allActor.HasAnyLevelTrait("DebuffMarkEnemy"))
						{
							targets.Add(allActor);
						}
					}
					if (targets.Count == 0)
					{
						return ActionListClearFlag.Keep;
					}
					GetSortedActors(actor, in targets);
					for (int i = 0; i < Math.Min(canMarkAmount, targets.Count); i++)
					{
						ActorModel actorModel = targets[i];
						if (actorModel != null)
						{
							string tag = actor.Faction.ToString();
							actorModel.AddTemporaryTrait(traitIdentifier, default(FixedPoint), null, 0L, tag);
						}
					}
					actor.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffMarkEnemy", false });
				}
			}
			return ActionListClearFlag.Keep;
		}

		private int GetCanMarkAmount(ActorModel actorModel)
		{
			CombatModel combatModel = actorModel.manager.CombatModel;
			foreach (SurvivorModel item in (actorModel.Faction == Faction.Survivor) ? combatModel.Survivors : combatModel.Raiders)
			{
				if (item.Definition.ID == actorModel.Definition.ID)
				{
					return item.IsLeader ? leaderMarkAmount : partyMarkAmount;
				}
			}
			return 0;
		}

		private List<ActorModel> GetSortedActors(ActorModel actorModel, in List<ActorModel> targets)
		{
			targets.StableSort(delegate(ActorModel actor1, ActorModel actor2)
			{
				int num = actorModel.GridCoordinate.ChebyshevDistance(actor1.GridCoordinate);
				int num2 = actorModel.GridCoordinate.ChebyshevDistance(actor2.GridCoordinate);
				if (num == num2)
				{
					if (actor1.Definition.IsSpecial && !actor2.Definition.IsSpecial)
					{
						return -1;
					}
					if (!actor1.Definition.IsSpecial && actor2.Definition.IsSpecial)
					{
						return 1;
					}
				}
				FixedVec2 fixedVec = actorModel.GridCoordinate.ToVector2() - actor1.GridCoordinate.ToVector2();
				FixedVec2 fixedVec2 = actorModel.GridCoordinate.ToVector2() - actor2.GridCoordinate.ToVector2();
				return (fixedVec.SqrMagnitude >= fixedVec2.SqrMagnitude) ? 1 : (-1);
			});
			return targets;
		}
	}
}
