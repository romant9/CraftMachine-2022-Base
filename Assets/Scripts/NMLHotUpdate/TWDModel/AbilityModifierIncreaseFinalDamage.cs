namespace TWDModel
{
	public class AbilityModifierIncreaseFinalDamage : ParameterModifier
	{
		private FixedPoint damageIncreaseValue;

		public static string FinalDamage = "FinalDamage";

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == FinalDamage)
			{
				value += damageIncreaseValue;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[1] { FinalDamage };
		}

		public AbilityModifierIncreaseFinalDamage()
		{
		}

		public AbilityModifierIncreaseFinalDamage(FixedPoint arg)
		{
			damageIncreaseValue = arg;
		}
	}
}
