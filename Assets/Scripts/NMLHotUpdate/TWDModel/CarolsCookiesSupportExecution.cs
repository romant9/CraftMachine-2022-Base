using System.Collections.Generic;

namespace TWDModel
{
	public class CarolsCookiesSupportExecution : ISupportExecution
	{
		public IEnumerable<ModelAction> Execute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target, out ICollection<ActorModel> affectedTargets)
		{
			affectedTargets = GetTargets(supportModel, attachedSurvivor, target);
			foreach (ActorModel affectedTarget in affectedTargets)
			{
				long duration = (long)supportModel.GetParameter(2);
				affectedTarget.AddTemporaryTrait("CarolsCookiesActive", default(FixedPoint), null, duration);
			}
			return new List<ModelAction>();
		}

		public bool CanExecute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			return true;
		}

		public ICollection<ActorModel> GetTargets(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			return new SurvivorModel[1] { attachedSurvivor };
		}
	}
}
