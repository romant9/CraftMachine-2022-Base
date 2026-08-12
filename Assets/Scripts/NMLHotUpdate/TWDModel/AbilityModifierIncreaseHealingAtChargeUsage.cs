namespace TWDModel
{
	public class AbilityModifierIncreaseHealingAtChargeUsage : ParameterModifier
	{
		private FixedPoint chance;

		private FixedPoint multiplier;

		public static string FetchIncreaseHealingAtChargeUsageChance = "FetchIncreaseHealingAtChargeUsageChance";

		public static string FetchIncreaseHealingAtChargeUsageMultiplier = "FetchIncreaseHealingAtChargeUsageMultiplier";

		public AbilityModifierIncreaseHealingAtChargeUsage()
		{
		}

		public AbilityModifierIncreaseHealingAtChargeUsage(FixedPoint healingChance, FixedPoint healingMultiplier)
		{
			chance = healingChance;
			multiplier = healingMultiplier;
		}

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == FetchIncreaseHealingAtChargeUsageChance)
			{
				value += chance;
				return true;
			}
			if (paramName == FetchIncreaseHealingAtChargeUsageMultiplier)
			{
				value += multiplier;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[2] { FetchIncreaseHealingAtChargeUsageChance, FetchIncreaseHealingAtChargeUsageMultiplier };
		}
	}
}
