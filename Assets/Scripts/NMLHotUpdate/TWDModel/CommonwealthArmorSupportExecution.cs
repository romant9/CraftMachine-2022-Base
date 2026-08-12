using System.Collections.Generic;

namespace TWDModel
{
	public class CommonwealthArmorSupportExecution : ISupportExecution
	{
		public IEnumerable<ModelAction> Execute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target, out ICollection<ActorModel> affectedTargets)
		{
			long duration = (long)supportModel.GetParameter(0);
			attachedSurvivor.AddTemporaryTrait("CommonwealthArmorActive", default(FixedPoint), null, duration);
			affectedTargets = GetTargets(supportModel, attachedSurvivor, target);
			return new List<ModelAction>();
		}

		public bool CanExecute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			return !(attachedSurvivor?.IsDead ?? true);
		}

		public ICollection<ActorModel> GetTargets(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target)
		{
			return new SurvivorModel[1] { attachedSurvivor };
		}
	}
}
