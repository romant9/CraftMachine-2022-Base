using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class WhisperersMaskSupportExecution : ISupportExecution
	{
		public IEnumerable<ModelAction> Execute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target, out ICollection<ActorModel> affectedTargets)
		{
			LinkedList<ModelAction> linkedList = new LinkedList<ModelAction>();
			FixedPoint parameter = supportModel.GetParameter(0);
			ModelRandom playerRandom = supportModel.manager.Player.PlayerRandom;
			List<ActorModel> targetsInternal = GetTargetsInternal(supportModel, attachedSurvivor, target);
			affectedTargets = new LinkedList<ActorModel>();
			for (int i = 0; i < parameter; i++)
			{
				if (targetsInternal.Count <= 0)
				{
					break;
				}
				ActorModel randomElement = playerRandom.GetRandomElement(targetsInternal, remove: true);
				linkedList.AddLast(new HerdAction(attachedSurvivor, randomElement, 1, 0, supportModel));
				affectedTargets.Add(randomElement);
			}
			return linkedList;
		}

		private List<ActorModel> GetTargetsInternal(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			FixedPoint parameter = supportModel.GetParameter(1);
			FixedPoint fixedPoint = parameter * parameter;
			CombatModel combatModel = supportModel.manager.CombatModel;
			List<ActorModel> list = new List<ActorModel>();
			foreach (ActorModel walker in combatModel.Walkers)
			{
				if (!walker.IsDead && !walker.IsEnvironmental && walker.ExclusiveTimedEffect == null && target.SquaredDistanceTo(walker.GridCoordinate) <= fixedPoint && combatModel.IsGridCellVisibleByAnySurvivor(walker.GridCoordinate) && !walker.IsImmuneToStun)
				{
					list.Add(walker);
				}
			}
			return list;
		}

		public bool CanExecute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			return GetTargetsInternal(supportModel, attachedSurvivor, target).Count > 0;
		}

		public ICollection<ActorModel> GetTargets(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			return GetTargetsInternal(supportModel, attachedSurvivor, target);
		}
	}
}
