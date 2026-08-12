using System.Collections.Generic;

namespace TWDModel
{
	public interface ConditionContext
	{
		ActorModel GetBadgeOwner();

		List<ActorModel> GetSurvivors();
	}
}
