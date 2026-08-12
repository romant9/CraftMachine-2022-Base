using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class DogSupportExecution : ISupportExecution
	{
		public IEnumerable<ModelAction> Execute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target, out ICollection<ActorModel> affectedTargets)
		{
			LinkedList<ModelAction> linkedList = new LinkedList<ModelAction>();
			if (supportModel.manager.CombatModel.Walkers == null)
			{
				affectedTargets = new ActorModel[0];
				return linkedList;
			}
			List<ActorModel> possibleTargets = GetPossibleTargets(supportModel.manager, attachedSurvivor, supportModel);
			int turns = (int)supportModel.GetParameter(1);
			foreach (ActorModel item in possibleTargets)
			{
				linkedList.AddLast(new CrippleAction(attachedSurvivor, item, turns, ignoreSourceBeingDead: false, supportModel));
			}
			affectedTargets = possibleTargets;
			return linkedList;
		}

		private List<ActorModel> GetPossibleTargets(TWDModelManager manager, SurvivorModel attachedSurvivor, SupportModel supportModel)
		{
			CombatModel combatModel = manager.CombatModel;
			List<ActorModel> list = new List<ActorModel>();
			if (combatModel.Walkers == null && combatModel.Raiders == null)
			{
				return list;
			}
			IEnumerable<ActorModel> enumerable = new List<ActorModel>();
			if (combatModel.Walkers != null)
			{
				enumerable = enumerable.Union(combatModel.Walkers);
			}
			if (combatModel.Raiders != null)
			{
				enumerable = enumerable.Union(combatModel.Raiders);
			}
			foreach (ActorModel item in enumerable)
			{
				if (!item.IsDead && combatModel.IsGridCellVisibleByAnySurvivor(item.GridCoordinate) && !item.IsEnvironmental && !item.IsStunned && !item.IsRooted && !item.IsPitfalled)
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
			int num = (int)FixedPoint.Min(list.Count, supportModel.GetParameter(0));
			for (int num2 = list.Count - 1; num2 >= num; num2--)
			{
				list.RemoveAt(num2);
			}
			return list;
		}

		public bool CanExecute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			return GetPossibleTargets(supportModel.manager, attachedSurvivor, supportModel).Count > 0;
		}

		public ICollection<ActorModel> GetTargets(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			return GetPossibleTargets(supportModel.manager, attachedSurvivor, supportModel);
		}
	}
}
