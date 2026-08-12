namespace TWDModel
{
	public class AbilityModifierIncreaseMeleeBodyShot : ParameterModifier
	{
		private FixedPoint chance;

		private FixedPoint multiplier;

		public static string FetchIncreaseMeleeBodyShotChance = "FetchIncreaseMeleeBodyShotChance";

		public static string FetchIncreaseMeleeBodyShotMultiplier = "FetchIncreaseMeleeBodyShotMultiplier";

		public AbilityModifierIncreaseMeleeBodyShot()
		{
		}

		public AbilityModifierIncreaseMeleeBodyShot(FixedPoint bodyShotChance, FixedPoint bodyShotMultiplier)
		{
			chance = bodyShotChance;
			multiplier = bodyShotMultiplier;
		}

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == FetchIncreaseMeleeBodyShotChance)
			{
				value += chance;
				return true;
			}
			if (paramName == FetchIncreaseMeleeBodyShotMultiplier)
			{
				value += multiplier;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[2] { FetchIncreaseMeleeBodyShotChance, FetchIncreaseMeleeBodyShotMultiplier };
		}
	}
}
