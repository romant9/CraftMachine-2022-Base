using System.Collections.Generic;
using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class DetonateGrenadeAction : ModelAction
	{
		public const string DetonatedNotification = "DelayedActionGrenadeDetonated";

		public DelayedActionGrenadeArea Bomb { get; private set; }

		public DetonateGrenadeAction(DelayedActionGrenadeArea bomb)
			: base(bomb)
		{
			Bomb = bomb;
		}

		public override bool CanExecute()
		{
			return Bomb != null;
		}

		public override bool Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager tWDModelManager) || Bomb == null)
			{
				return false;
			}
			CombatModel combatModel = tWDModelManager.CombatModel;
			if (combatModel == null)
			{
				return false;
			}
			if (!combatModel.Models.Contains(Bomb))
			{
				return true;
			}
			combatModel.RemoveModel(Bomb);
			GridCoordinate effectiveAreaGridCoordinate = Bomb.EffectiveAreaGridCoordinate;
			ActorModel owner = Bomb.Owner;
			int throwerPanelDamage = GetThrowerPanelDamage(owner);
			bool flag = HasFlameTrapAt(combatModel, effectiveAreaGridCoordinate);
			List<GridCoordinate> explosionCells = GetExplosionCells(combatModel, effectiveAreaGridCoordinate, Bomb.ExplosionRadius);
			List<TraitApplyEntry> entries = DelayedActionGrenadeArea.ParseTraitsApply(Bomb.TargetTraitsApply);
			List<string> grantedSelfTraits = TryGrantSelfTraits(owner);
			HashSet<ActorModel> hashSet = new HashSet<ActorModel>();
			foreach (GridCoordinate item in explosionCells)
			{
				ActorModel occupier = combatModel.GetOccupier(item);
				if (occupier != null && !occupier.IsDead && owner != null && owner.IsEnemy(occupier) && hashSet.Add(occupier))
				{
					FixedPoint fixedPoint = throwerPanelDamage * Bomb.PanelDamagePercent + occupier.MaxHitPoints * Bomb.MaxHpDamagePercent;
					if (flag)
					{
						fixedPoint *= 1L + Bomb.OnFlameTrapExtraPercent;
					}
					CombatHelpers.ExecuteDamage(combatModel, owner, occupier, (int)fixedPoint, 0, DamageType.Explosion, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
					if (!occupier.IsDead)
					{
						ApplyTargetTraits(tWDModelManager, occupier, entries);
					}
				}
			}
			RemoveGrantedSelfTraits(owner, grantedSelfTraits);
			TryCreateFlameTraps(tWDModelManager, combatModel, owner, explosionCells);
			Bomb.NotifyChange("DelayedActionGrenadeDetonated", effectiveAreaGridCoordinate);
			return true;
		}

		private static int GetThrowerPanelDamage(ActorModel thrower)
		{
			if (thrower == null)
			{
				return 0;
			}
			if (thrower is SurvivorModel survivorModel)
			{
				return survivorModel.GetDamageForPreferredWeapon();
			}
			FixedPoint value = 0.0;
			thrower.Modifiers?.VisitParameter("AddMeleeDamage", ref value, thrower);
			EquipmentItemModel weaponEquipment = thrower.GetWeaponEquipment();
			if (weaponEquipment != null)
			{
				value += (FixedPoint)weaponEquipment.Damage;
			}
			return (int)value;
		}

		private static bool HasFlameTrapAt(CombatModel combat, GridCoordinate coord)
		{
			return combat.Models.OfType<TrapFlameArea>().Any((TrapFlameArea x) => x.EffectiveAreaGridCoordinate == coord);
		}

		private List<string> TryGrantSelfTraits(ActorModel thrower)
		{
			List<string> list = new List<string>();
			if (thrower == null || thrower.IsDead)
			{
				return list;
			}
			foreach (TraitApplyEntry item in DelayedActionGrenadeArea.ParseTraitsApply(Bomb.SelfTraitsApply))
			{
				if (!thrower.HasTrait(item.TraitId))
				{
					thrower.AddTemporaryTrait(item.TraitId, default(FixedPoint), item.HasChanceOverride ? new FixedPoint?(item.Chance) : ((FixedPoint?)null), 0L);
					list.Add(item.TraitId);
				}
			}
			return list;
		}

		private void RemoveGrantedSelfTraits(ActorModel thrower, List<string> grantedSelfTraits)
		{
			if (thrower == null)
			{
				return;
			}
			foreach (string grantedSelfTrait in grantedSelfTraits)
			{
				thrower.RemoveTrait(grantedSelfTrait);
			}
		}

		private void ApplyTargetTraits(TWDModelManager twd, ActorModel victim, List<TraitApplyEntry> entries)
		{
			foreach (TraitApplyEntry entry in entries)
			{
				if (twd?.Player == null || twd.Player.RollDice(RollDiceType.GainTrait, (int)entry.Chance) != PlayerRandomChanceResult.Failed)
				{
					if (entry.HasTurns)
					{
						victim.AddTemporaryTrait(entry.TraitId, default(FixedPoint), null, entry.Turns);
					}
					else
					{
						victim.AddTemporaryTrait(entry.TraitId, default(FixedPoint), null, 0L);
					}
				}
			}
		}

		private void TryCreateFlameTraps(TWDModelManager twd, CombatModel combat, ActorModel thrower, List<GridCoordinate> cells)
		{
			if (thrower != null && twd?.Player != null && Bomb.FlameTrapTurns > 0 && !(Bomb.FlameTrapChancePercent <= 0L) && twd.Player.RollDice(RollDiceType.TrapFlame, Bomb.FlameTrapChancePercent) != PlayerRandomChanceResult.Failed && cells != null && cells.Count != 0)
			{
				TrapFlameAreaManager trapFlameAreaManager = combat.GetModel<TrapFlameAreaManager>();
				if (trapFlameAreaManager == null)
				{
					trapFlameAreaManager = new TrapFlameAreaManager();
					trapFlameAreaManager.SetManager(twd);
					combat.AddModel(trapFlameAreaManager);
				}
				int expiryTurn = combat.TurnManager.TurnCount + Bomb.FlameTrapTurns;
				List<TrapFlameArea> trapFlameAreasFromGridCoordinates = TrapFlameAreaManager.GetTrapFlameAreasFromGridCoordinates(thrower, Bomb.EffectiveAreaGridCoordinate, expiryTurn, cells, Bomb.FlameTrapInjuryPercent);
				trapFlameAreaManager.UpdateWhenNewAreaGenerated(trapFlameAreasFromGridCoordinates);
			}
		}

		private static List<GridCoordinate> GetExplosionCells(CombatModel combat, GridCoordinate center, int radius)
		{
			List<GridCoordinate> list = new List<GridCoordinate>();
			foreach (GridCoordinate coordinate in combat.Grid.Coordinates)
			{
				if (!combat.IsBlocked(coordinate) && center.ChebyshevDistance(coordinate) <= radius)
				{
					list.Add(coordinate);
				}
			}
			return list;
		}

		public override string ToString()
		{
			return "DetonateGrenadeAction at " + ((Bomb != null) ? Bomb.EffectiveAreaGridCoordinate.ToString() : "null");
		}
	}
}
