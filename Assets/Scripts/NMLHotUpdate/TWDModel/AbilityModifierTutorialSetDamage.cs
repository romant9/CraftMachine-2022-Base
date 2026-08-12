namespace TWDModel
{
	public class AbilityModifierTutorialSetDamage : ParameterModifier
	{
		public const string ParameterName = "TutorialSetDamage";

		private int damageToSet;

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == "TutorialSetDamage")
			{
				value = damageToSet;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[1] { "TutorialSetDamage" };
		}

		public AbilityModifierTutorialSetDamage()
		{
		}

		public AbilityModifierTutorialSetDamage(int damageToSet)
		{
			this.damageToSet = damageToSet;
		}
	}
}
