namespace TWDModel
{
	public class AbilityModifierMeleeBodyShot : ParameterModifier
	{
		private FixedPoint chance;

		private FixedPoint multiplier;

		public static string FetchMeleeBodyShotChance = "FetchMeleeBodyShotChance";

		public static string FetchMeleeBodyShotMultiplier = "FetchMeleeBodyShotChance";

		public AbilityModifierMeleeBodyShot()
		{
		}

		public AbilityModifierMeleeBodyShot(FixedPoint bodyShotChance, FixedPoint bodyShotMultiplier)
		{
			chance = bodyShotChance;
			multiplier = bodyShotMultiplier;
		}

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == FetchMeleeBodyShotChance)
			{
				value += chance;
				return true;
			}
			if (paramName == FetchMeleeBodyShotMultiplier)
			{
				value += multiplier;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[2] { FetchMeleeBodyShotChance, FetchMeleeBodyShotMultiplier };
		}
	}
}
