using BaseModel;

namespace TWDModel
{
	public class PreDealDamageAction : ModelAction
	{
		public DamageAction DamageAction { get; set; }

		public PreDealDamageAction(DamageAction damageAction)
			: base(damageAction.TargetActor)
		{
			DamageAction = damageAction;
		}

		public override bool Execute(ModelManager manager)
		{
			return true;
		}
	}
}
