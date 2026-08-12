using System.Collections.Generic;

namespace TWDModel
{
	public interface ISupportExecution
	{
		IEnumerable<ModelAction> Execute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target, out ICollection<ActorModel> affectedTargets);

		bool CanExecute(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target);

		ICollection<ActorModel> GetTargets(SupportModel supportModel, SurvivorModel attachedSurvivor, GridCoordinate target);
	}
}
