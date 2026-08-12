using System.Collections.Generic;

namespace TWDModel
{
	public class CivilianController : AIController
	{
		public CivilianController(ActorModel actor)
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
