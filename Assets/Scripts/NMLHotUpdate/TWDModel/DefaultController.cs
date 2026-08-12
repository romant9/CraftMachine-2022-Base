using System.Collections.Generic;

namespace TWDModel
{
	public class DefaultController : AIController
	{
		public DefaultController(ActorModel actor)
			: base(actor)
		{
			base.Enabled = true;
		}

		protected override List<BehaviorBase> CreateSystemicBehaviors()
		{
			return new List<BehaviorBase>
			{
				new ActorEndTurnBehavior(this)
			};
		}
	}
}
