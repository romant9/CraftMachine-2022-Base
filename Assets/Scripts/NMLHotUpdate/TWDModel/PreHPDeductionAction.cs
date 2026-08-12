using BaseModel;

namespace TWDModel
{
	public class PreHPDeductionAction : ModelAction
	{
		public ActorModel Attacker { get; set; }

		public ActorModel Target { get; set; }

		public int Damage { get; set; }

		public DamageType DamageType { get; set; }

		public bool Avoided { get; set; }

		public PreHPDeductionAction(ActorModel targetActor, ActorModel attacker, int damage, DamageType damageType)
			: base(targetActor)
		{
			Attacker = attacker;
			Target = targetActor;
			Damage = damage;
			DamageType = damageType;
			Avoided = false;
		}

		public override bool Execute(ModelManager manager)
		{
			return true;
		}
	}
}
