namespace TWDModel
{
	public class AbilityModifierIncrementer : ParameterModifier
	{
		private FixedPoint increment;

		private string parameterName;

		public AbilityModifierIncrementer()
		{
		}

		public AbilityModifierIncrementer(string inParameterName, FixedPoint inIncrement)
		{
			parameterName = inParameterName;
			increment = inIncrement;
		}

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == parameterName)
			{
				value += increment;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[1] { parameterName };
		}
	}
}
