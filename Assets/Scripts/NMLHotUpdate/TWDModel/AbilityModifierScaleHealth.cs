namespace TWDModel
{
	public class AbilityModifierScaleHealth : ParameterModifier
	{
		private FixedPoint scalePercentage;

		public AbilityModifierScaleHealth()
		{
		}

		public AbilityModifierScaleHealth(FixedPoint scalePercentage)
		{
			this.scalePercentage = scalePercentage * 0.009999999776482582;
		}

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == "AbilityModifierDamageScaleOnTargetHealth")
			{
				value += scalePercentage;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[1] { "AbilityModifierDamageScaleOnTargetHealth" };
		}
	}
}
