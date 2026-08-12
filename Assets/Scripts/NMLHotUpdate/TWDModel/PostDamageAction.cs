using BaseModel;

namespace TWDModel
{
	public class PostDamageAction : ModelAction
	{
		public DamageAction DamageAction { get; private set; }

		public ActorModel TargetActor { get; private set; }

		public ActorModel DamagerActor { get; private set; }

		public bool IsMainTarget { get; private set; }

		public bool IsChargeAttack { get; private set; }

		public bool IsTriggerExtraAttackDamage { get; private set; }

		public PostDamageAction(DamageAction dmgAction, ActorModel target, ActorModel damager, bool isMainTarget = false, bool isChargeAttack = false, bool isTriggerExtraAttackDamage = false)
			: base(target)
		{
			DamageAction = dmgAction;
			TargetActor = target;
			DamagerActor = damager;
			IsMainTarget = isMainTarget;
			IsChargeAttack = isChargeAttack;
			IsTriggerExtraAttackDamage = isTriggerExtraAttackDamage;
		}

		public override bool Execute(ModelManager manager)
		{
			return true;
		}

		public override string ToString()
		{
			return "DamagerActor = " + ((DamagerActor != null) ? DamagerActor.DebugInfo : "null") + ", TargetActor = " + ((TargetActor != null) ? TargetActor.DebugInfo : "null");
		}
	}
}
