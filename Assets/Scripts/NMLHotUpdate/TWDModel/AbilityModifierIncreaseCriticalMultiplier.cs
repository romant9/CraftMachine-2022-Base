namespace TWDModel
{
	public class AbilityModifierIncreaseCriticalMultiplier : ParameterModifier
	{
		private FixedPoint multiplierIncrease;

		public static string CriticalMultiplier = "CriticalMultiplier";

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == CriticalMultiplier)
			{
				value += multiplierIncrease;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[1] { CriticalMultiplier };
		}

		public AbilityModifierIncreaseCriticalMultiplier()
		{
		}

		public AbilityModifierIncreaseCriticalMultiplier(FixedPoint arg)
		{
			multiplierIncrease = arg;
		}
	}
}
