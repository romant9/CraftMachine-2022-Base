namespace TWDModel
{
	public class AbilityModifierIncreaseCriticalChance : ParameterModifier
	{
		private FixedPoint probabilityIncrease;

		public static string CriticalChance = "CriticalChance";

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == CriticalChance)
			{
				value += probabilityIncrease;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[1] { CriticalChance };
		}

		public AbilityModifierIncreaseCriticalChance()
		{
		}

		public AbilityModifierIncreaseCriticalChance(FixedPoint arg)
		{
			probabilityIncrease = arg;
		}
	}
}
