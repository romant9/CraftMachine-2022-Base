using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class BadgeSupportExecution : ISupportExecution
	{
		public IEnumerable<ModelAction> Execute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target, out ICollection<ActorModel> affectedTargets)
		{
			FixedPoint fixedPoint = attachedSurvivor.GetWeaponEquipment().Damage * supportModel.GetParameter(0) * 0.01;
			FixedPoint successProbability = supportModel.GetParameter(1) / 100.0;
			LinkedList<ModelAction> linkedList = new LinkedList<ModelAction>();
			affectedTargets = GetTargets(supportModel, attachedSurvivor, target);
			foreach (ActorModel affectedTarget in affectedTargets)
			{
				FixedPoint fixedPoint2 = fixedPoint;
				linkedList.AddLast(new DamageAction(affectedTarget, attachedSurvivor, (int)fixedPoint2, 0, bodyShot: false, critical: false, PlayerRandomChanceResult.Success, DamageType.Base, Faction.Any, null, noChargeGain: true, supportModel));
			}
			if (supportModel.manager != null)
			{
				FixedPoint value = 0L;
				supportModel.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, attachedSurvivor);
				if (supportModel.manager.Player.RollDice(RollDiceType.Stun, successProbability, value) != PlayerRandomChanceResult.Failed)
				{
					foreach (ActorModel item in (IEnumerable<ActorModel>)GetPossibleTargets(supportModel.manager, attachedSurvivor, supportModel))
					{
						linkedList.AddLast(new StunAction(attachedSurvivor, item, 1));
					}
				}
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

		private List<ActorModel> GetPossibleTargets(TWDModelManager manager, SurvivorModel attachedSurvivor, SupportModel supportModel)
		{
			List<ActorModel> list = new List<ActorModel>();
			if (manager == null)
			{
				return list;
			}
			CombatModel combatModel = manager.CombatModel;
			if (combatModel.Walkers == null)
			{
				return list;
			}
			IEnumerable<ActorModel> enumerable = new List<ActorModel>();
			if (combatModel.Walkers != null)
			{
				enumerable = enumerable.Union(combatModel.Walkers);
			}
			foreach (ActorModel item in enumerable)
			{
				if (!item.IsDead && combatModel.IsGridCellVisibleByAnySurvivor(item.GridCoordinate) && !item.IsEnvironmental && !item.IsStunned)
				{
					list.Add(item);
				}
			}
			list.StableSort(delegate(ActorModel actor1, ActorModel actor2)
			{
				int num3 = attachedSurvivor.GridCoordinate.SquaredDistanceTo(actor1.GridCoordinate);
				int num4 = attachedSurvivor.GridCoordinate.SquaredDistanceTo(actor2.GridCoordinate);
				if (num3 == num4)
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
				return (num3 > num4) ? 1 : (-1);
			});
			int num = (int)FixedPoint.Min(list.Count, supportModel.GetParameter(2));
			for (int num2 = list.Count - 1; num2 >= num; num2--)
			{
				list.RemoveAt(num2);
			}
			return list;
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
