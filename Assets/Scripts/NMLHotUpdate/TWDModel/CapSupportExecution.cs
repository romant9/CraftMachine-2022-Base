using System.Collections.Generic;

namespace TWDModel
{
	public class CapSupportExecution : ISupportExecution
	{
		public IEnumerable<ModelAction> Execute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target, out ICollection<ActorModel> affectedTargets)
		{
			long duration = (long)supportModel.GetParameter(2);
			attachedSurvivor.AddTemporaryTrait("TemFullyStateTrait", default(FixedPoint), null, duration);
			attachedSurvivor.CapFirstAttack = true;
			attachedSurvivor.CapFirstHeal = true;
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
