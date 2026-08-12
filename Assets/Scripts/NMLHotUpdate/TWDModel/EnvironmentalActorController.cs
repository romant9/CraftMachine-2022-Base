using System.Collections.Generic;

namespace TWDModel
{
	public class EnvironmentalActorController : AIController
	{
		public override bool CanPerformOOT => false;

		public EnvironmentalActorController(ActorModel actor)
			: base(actor)
		{
		}

		public override void AttackTarget(ActorModel actor)
		{
		}

		public override void ClearFollowTarget()
		{
		}

		public override void FollowTarget(ActorModel target)
		{
		}

		public override void HeardNoise(GridCoordinate source)
		{
		}

		public override bool IsFighting()
		{
			return false;
		}

		public override void ReceiveDamage(ActorModel attacker, DamageType damageType)
		{
		}

		public override void SeeEnemy(ActorModel enemy)
		{
		}

		protected override List<BehaviorBase> CreateSystemicBehaviors()
		{
			return new List<BehaviorBase>
			{
				new ActorEndTurnBehavior(this)
			};
		}

		protected override void OnPreExecuteBehavior()
		{
		}
	}
}
