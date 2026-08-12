namespace TWDModel
{
	public class AbilityModifierFirstStrike : ParameterModifier
	{
		private FixedPoint damageMultiplier;

		private FixedPoint healthThreshold;

		public static string FirstStrikeAbilityDamageMultiplier = "FirstStrikeAbilityDamageMultiplier";

		public static string FirstStrikeAbilityHealthThreshold = "FirstStrikeAbilityHealthThreshold";

		public AbilityModifierFirstStrike()
		{
		}

		public AbilityModifierFirstStrike(FixedPoint damageMultiplier, FixedPoint healthThreshold)
		{
			this.damageMultiplier = damageMultiplier;
			this.healthThreshold = healthThreshold;
		}

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == FirstStrikeAbilityDamageMultiplier)
			{
				value += damageMultiplier;
				return true;
			}
			if (paramName == FirstStrikeAbilityHealthThreshold)
			{
				value += healthThreshold;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[2] { FirstStrikeAbilityDamageMultiplier, FirstStrikeAbilityHealthThreshold };
		}
	}
}
