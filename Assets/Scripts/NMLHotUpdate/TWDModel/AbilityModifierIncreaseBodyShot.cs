namespace TWDModel
{
	public class AbilityModifierIncreaseBodyShot : ParameterModifier
	{
		private FixedPoint chance;

		private FixedPoint multiplier;

		public static string FetchIncreaseBodyShotChance = "FetchIncreaseBodyShotChance";

		public static string FetchIncreaseBodyShotMultiplier = "FetchIncreaseBodyShotMultiplier";

		public AbilityModifierIncreaseBodyShot()
		{
		}

		public AbilityModifierIncreaseBodyShot(FixedPoint bodyShotChance, FixedPoint bodyShotMultiplier)
		{
			chance = bodyShotChance;
			multiplier = bodyShotMultiplier;
		}

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == FetchIncreaseBodyShotChance)
			{
				value += chance;
				return true;
			}
			if (paramName == FetchIncreaseBodyShotMultiplier)
			{
				value += multiplier;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[2] { FetchIncreaseBodyShotChance, FetchIncreaseBodyShotMultiplier };
		}
	}
}
