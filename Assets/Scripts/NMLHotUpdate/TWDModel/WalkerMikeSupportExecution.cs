using System.Collections.Generic;

namespace TWDModel
{
	public class WalkerMikeSupportExecution : ISupportExecution
	{
		public IEnumerable<ModelAction> Execute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target, out ICollection<ActorModel> affectedTargets)
		{
			affectedTargets = GetTargets(supportModel, attachedSurvivor, target);
			foreach (ActorModel affectedTarget in affectedTargets)
			{
				long duration = (long)supportModel.GetParameter(2);
				affectedTarget.AddTemporaryTrait("WalkerMikeActive", default(FixedPoint), null, duration);
			}
			supportModel.manager.CombatModel.NotifyChange("supportExecuted", this);
			return new List<ModelAction>();
		}

		public bool CanExecute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			return true;
		}

		public ICollection<ActorModel> GetTargets(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			return GetTargetsInternal(supportModel, target);
		}

		private List<ActorModel> GetTargetsInternal(SupportModel supportModel, GridCoordinate target)
		{
			List<ActorModel> list = new List<ActorModel>();
			CombatModel combatModel = supportModel.manager.CombatModel;
			FixedPoint fixedPoint = supportModel.GetParameter(1) + 1L;
			FixedPoint fixedPoint2 = fixedPoint * fixedPoint;
			int num = (int)supportModel.GetParameter(0) + 1;
			foreach (ActorModel survivor in combatModel.Survivors)
			{
				if (!survivor.IsDead && !survivor.IsEnvironmental && !survivor.IsStruggling && target.SquaredDistanceTo(survivor.GridCoordinate) <= fixedPoint2 && combatModel.IsGridCellVisibleByAnySurvivor(survivor.GridCoordinate))
				{
					list.Add(survivor);
				}
			}
			list.StableSort((ActorModel a, ActorModel b) => target.SquaredDistanceTo(a.GridCoordinate).CompareTo(target.SquaredDistanceTo(b.GridCoordinate)));
			if (list.Count > num)
			{
				list = list.GetRange(0, num);
			}
			return list;
		}
	}
}
