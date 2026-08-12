namespace TWDModel
{
	public class AbilityModifierClassIncrementer : ParameterModifier
	{
		private FixedPoint increment;

		private string parameterName;

		private SurvivorClass survivorClass;

		public AbilityModifierClassIncrementer()
		{
		}

		public AbilityModifierClassIncrementer(string inParameterName, FixedPoint inIncrement, SurvivorClass inSurvivorClass)
		{
			parameterName = inParameterName;
			increment = inIncrement;
			survivorClass = inSurvivorClass;
		}

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == parameterName && actor is SurvivorModel survivorModel && survivorModel.SurvivorClass == survivorClass)
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
