namespace TWDModel
{
	public class AbilityModifierIncreaseDodgeChance : ParameterModifier
	{
		private FixedPoint probabilityIncrease;

		public static string DodgeChance = "DodgeChance";

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == DodgeChance)
			{
				value += probabilityIncrease;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[1] { DodgeChance };
		}

		public AbilityModifierIncreaseDodgeChance()
		{
		}

		public AbilityModifierIncreaseDodgeChance(FixedPoint arg)
		{
			probabilityIncrease = arg;
		}
	}
}
