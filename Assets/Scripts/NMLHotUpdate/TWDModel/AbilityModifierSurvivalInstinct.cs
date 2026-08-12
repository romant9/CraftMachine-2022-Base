namespace TWDModel
{
	public class AbilityModifierSurvivalInstinct : ParameterModifier
	{
		private FixedPoint percentMoreDamageDone;

		private FixedPoint percentLessDamageTaken;

		public static string FetchIncreaseDamageDone = "FetchIncreaseDamageDone";

		public static string FetchReduceDamageTaken = "FetchReduceDamageTaken";

		public AbilityModifierSurvivalInstinct()
		{
		}

		public AbilityModifierSurvivalInstinct(FixedPoint percentMoreDamageDoneIn, FixedPoint percentLessDamageTakenIn)
		{
			percentMoreDamageDone = percentMoreDamageDoneIn;
			percentLessDamageTaken = percentLessDamageTakenIn;
		}

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == FetchIncreaseDamageDone)
			{
				value += percentMoreDamageDone;
				return true;
			}
			if (paramName == FetchReduceDamageTaken)
			{
				value += percentLessDamageTaken;
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[2] { FetchIncreaseDamageDone, FetchReduceDamageTaken };
		}
	}
}
