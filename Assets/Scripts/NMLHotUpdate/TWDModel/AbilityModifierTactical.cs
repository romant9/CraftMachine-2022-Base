namespace TWDModel
{
	public class AbilityModifierTactical : ParameterModifier
	{
		private FixedPoint extraMoveRange;

		public AbilityModifierTactical()
		{
		}

		public AbilityModifierTactical(FixedPoint extraMoveRange)
		{
			this.extraMoveRange = extraMoveRange;
		}

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == "AbilityModifierIncreaseMoveRangeForSecondMove" || paramName == "AbilityModifierIncreaseMoveRangeForSecondMoveTacticalArmor")
			{
				value += extraMoveRange;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[2] { "AbilityModifierIncreaseMoveRangeForSecondMove", "AbilityModifierIncreaseMoveRangeForSecondMoveTacticalArmor" };
		}
	}
}
