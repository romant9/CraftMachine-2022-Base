namespace TWDModel
{
	public class AbilityModifierBodyShot : ParameterModifier
	{
		private FixedPoint chance;

		private FixedPoint multiplier;

		public static string FetchBodyShotChance = "FetchBodyShotChance";

		public static string FetchBodyShotMultiplier = "FetchBodyShotMultiplier";

		public AbilityModifierBodyShot()
		{
		}

		public AbilityModifierBodyShot(FixedPoint bodyShotChance, FixedPoint bodyShotMultiplier)
		{
			chance = bodyShotChance;
			multiplier = bodyShotMultiplier;
		}

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == FetchBodyShotChance)
			{
				value += chance;
				return true;
			}
			if (paramName == FetchBodyShotMultiplier)
			{
				value += multiplier;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[2] { FetchBodyShotChance, FetchBodyShotMultiplier };
		}
	}
}
