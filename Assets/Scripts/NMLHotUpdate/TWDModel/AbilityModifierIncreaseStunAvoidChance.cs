namespace TWDModel
{
	public class AbilityModifierIncreaseStunAvoidChance : ParameterModifier
	{
		private FixedPoint probabilityIncrease;

		public static string StunAvoidChance = "StunAvoidChance";

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == StunAvoidChance)
			{
				value += probabilityIncrease;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[1] { StunAvoidChance };
		}

		public AbilityModifierIncreaseStunAvoidChance()
		{
		}

		public AbilityModifierIncreaseStunAvoidChance(FixedPoint arg)
		{
			probabilityIncrease = arg;
		}
	}
}
