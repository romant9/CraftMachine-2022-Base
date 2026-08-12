namespace TWDModel
{
	public class AbilityModifierIncreaseSecondaryHitsChance : ParameterModifier
	{
		private FixedPoint probabilityIncrease;

		public static string SecondaryHitsChance = "SecondaryHitsChance";

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == SecondaryHitsChance)
			{
				value += probabilityIncrease;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[1] { SecondaryHitsChance };
		}

		public AbilityModifierIncreaseSecondaryHitsChance()
		{
		}

		public AbilityModifierIncreaseSecondaryHitsChance(FixedPoint arg)
		{
			probabilityIncrease = arg;
		}
	}
}
