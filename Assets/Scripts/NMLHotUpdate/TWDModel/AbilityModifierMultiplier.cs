namespace TWDModel
{
	public class AbilityModifierMultiplier : ParameterModifier
	{
		private FixedPoint multiplier;

		private string parameterName;

		public AbilityModifierMultiplier()
		{
		}

		public AbilityModifierMultiplier(string inParameterName, FixedPoint inMultiplier)
		{
			parameterName = inParameterName;
			multiplier = inMultiplier;
		}

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == parameterName)
			{
				value *= multiplier;
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
