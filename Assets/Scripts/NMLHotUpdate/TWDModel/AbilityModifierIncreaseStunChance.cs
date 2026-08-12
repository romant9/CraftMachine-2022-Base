namespace TWDModel
{
	public class AbilityModifierIncreaseStunChance : ParameterModifier
	{
		private FixedPoint stunChanceIncrease;

		public static string IncreaseStunChance = "IncreaseStunChance";

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == IncreaseStunChance)
			{
				value += stunChanceIncrease;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[1] { IncreaseStunChance };
		}

		public AbilityModifierIncreaseStunChance()
		{
		}

		public AbilityModifierIncreaseStunChance(FixedPoint arg)
		{
			stunChanceIncrease = arg;
		}
	}
}
