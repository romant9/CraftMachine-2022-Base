namespace TWDModel
{
	public class AbilityModifierIncreaseAvoidHerdModifer : ParameterModifier
	{
		private FixedPoint probabilityIncrease;

		public static string AvoidHerdChance = "AvoidHerdChance";

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == AvoidHerdChance)
			{
				value += probabilityIncrease;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[1] { AvoidHerdChance };
		}

		public AbilityModifierIncreaseAvoidHerdModifer()
		{
		}

		public AbilityModifierIncreaseAvoidHerdModifer(FixedPoint arg)
		{
			probabilityIncrease = arg;
		}
	}
}
