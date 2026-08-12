using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class ShivaSupportExecution : ISupportExecution
	{
		public IEnumerable<ModelAction> Execute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target, out ICollection<ActorModel> affectedTargets)
		{
			FixedPoint fixedPoint = attachedSurvivor.GetWeaponEquipment().Damage * supportModel.GetParameter(1) / 100.0;
			FixedPoint fixedPoint2 = supportModel.GetParameter(0) / 100.0;
			LinkedList<ModelAction> linkedList = new LinkedList<ModelAction>();
			affectedTargets = GetTargets(supportModel, attachedSurvivor, target);
			foreach (ActorModel affectedTarget in affectedTargets)
			{
				FixedPoint fixedPoint3 = fixedPoint + affectedTarget.MaxHitPoints * fixedPoint2;
				linkedList.AddLast(new DamageAction(affectedTarget, attachedSurvivor, (int)fixedPoint3, 0, bodyShot: false, critical: false, PlayerRandomChanceResult.Success, DamageType.Base, Faction.Any, null, noChargeGain: true, supportModel));
			}
			return linkedList;
		}

		private bool IsViable(ActorModel actor, CombatModel combatModel)
		{
			if (!actor.IsDead && combatModel.IsGridCellVisibleByAnySurvivor(actor.GridCoordinate))
			{
				return !actor.IsEnvironmental;
			}
			return false;
		}

		private bool HasAnyViableTargets(IEnumerable<ActorModel> possibleTargets, CombatModel combatModel)
		{
			if (possibleTargets == null)
			{
				return false;
			}
			foreach (ActorModel possibleTarget in possibleTargets)
			{
				if (IsViable(possibleTarget, combatModel))
				{
					return true;
				}
			}
			return false;
		}

		public bool CanExecute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			CombatModel combatModel = supportModel.manager.CombatModel;
			if (!HasAnyViableTargets(combatModel.Walkers, combatModel))
			{
				return HasAnyViableTargets(combatModel.Raiders, combatModel);
			}
			return true;
		}

		public ICollection<ActorModel> GetTargets(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			CombatModel combatModel = supportModel.manager.CombatModel;
			IEnumerable<ActorModel> enumerable = combatModel.Walkers;
			if (combatModel.Raiders != null)
			{
				enumerable = enumerable.Union(combatModel.Raiders);
			}
			return enumerable.Where((ActorModel actor) => IsViable(actor, combatModel)).ToList();
		}
	}
}
