namespace TWDModel
{
	public class AbilityModifierHeroIncrementer : ParameterModifier
	{
		private FixedPoint increment;

		private string parameterName;

		private string heroDefinition;

		private bool includeAltVersion;

		public AbilityModifierHeroIncrementer()
		{
		}

		public AbilityModifierHeroIncrementer(string inParameterName, FixedPoint inIncrement, string inHeroDefinition, bool inIncludeAltVersion = false)
		{
			parameterName = inParameterName;
			increment = inIncrement;
			heroDefinition = inHeroDefinition;
			includeAltVersion = inIncludeAltVersion;
		}

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == parameterName && actor is SurvivorModel survivorModel && (survivorModel.ActorDefinitionID == heroDefinition || (includeAltVersion && survivorModel.Definition.GetNonAlternativeHeroDefinition() == heroDefinition)))
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
