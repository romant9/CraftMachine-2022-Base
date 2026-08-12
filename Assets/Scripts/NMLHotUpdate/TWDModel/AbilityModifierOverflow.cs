namespace TWDModel
{
	public class AbilityModifierOverflow : ParameterModifier
	{
		private FixedPoint chance;

		private FixedPoint overflowHealthChance;

		public static string FetchOverflowMultiplier = "FetchOverflowMultiplier";

		public AbilityModifierOverflow()
		{
		}

		public AbilityModifierOverflow(FixedPoint chance, FixedPoint overflowHealthChance)
		{
			this.chance = chance;
			this.overflowHealthChance = overflowHealthChance;
		}

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor = null)
		{
			if (paramName == FetchOverflowMultiplier && base.manager.Player.RollDice(RollDiceType.Overflow, chance, 0.0) != PlayerRandomChanceResult.Failed)
			{
				value += overflowHealthChance;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[1] { FetchOverflowMultiplier };
		}
	}
}
