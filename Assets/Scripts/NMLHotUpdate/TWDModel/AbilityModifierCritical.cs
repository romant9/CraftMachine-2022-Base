namespace TWDModel
{
	public class AbilityModifierCritical : ParameterModifier
	{
		private FixedPoint chance;

		private FixedPoint multiplier;

		public static string FetchCriticalChance = "FetchCriticalChance";

		public static string FetchCriticalMultiplier = "FetchCriticalMultiplier";

		public AbilityModifierCritical()
		{
		}

		public AbilityModifierCritical(FixedPoint criticalChance, FixedPoint criticalMultiplier)
		{
			chance = criticalChance;
			multiplier = criticalMultiplier;
		}

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == FetchCriticalChance)
			{
				value += chance;
				return true;
			}
			if (paramName == FetchCriticalMultiplier)
			{
				value += multiplier;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[2] { FetchCriticalChance, FetchCriticalMultiplier };
		}
	}
}
