namespace TWDModel
{
	public class AbilityModifierIncreaseDamageVariation : ParameterModifier
	{
		private FixedPoint damageVariationIncreaseValue;

		public static string DamageVariation = "DamageVariation";

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == DamageVariation)
			{
				value += damageVariationIncreaseValue;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[1] { DamageVariation };
		}

		public AbilityModifierIncreaseDamageVariation()
		{
		}

		public AbilityModifierIncreaseDamageVariation(FixedPoint arg)
		{
			damageVariationIncreaseValue = arg;
		}
	}
}
