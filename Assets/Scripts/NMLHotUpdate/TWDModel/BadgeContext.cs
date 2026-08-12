using System.Collections.Generic;

namespace TWDModel
{
	public class BadgeContext : ConditionContext
	{
		private ActorModel actorModel;

		private List<ActorModel> survivors;

		public BadgeContext(ActorModel actor, IList<ActorModel> actorList)
		{
			actorModel = actor;
			if (actorList != null && actorList.Count > 0)
			{
				survivors = new List<ActorModel>();
				survivors.AddRange(actorList);
			}
		}

		public ActorModel GetBadgeOwner()
		{
			return actorModel;
		}

		public List<ActorModel> GetSurvivors()
		{
			return survivors;
		}
	}
}
