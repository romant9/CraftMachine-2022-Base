namespace TWDModel
{
	public class GuardianVowBinding
	{
		public string GuardianActorDefinitionID { get; set; }

		public string SovereignActorDefinitionID { get; set; }

		public int LeftTurns { get; set; }

		public int PursuitTriggeredCount { get; set; }

		public int ChargeAttackMaxTimes { get; set; }

		public int ChargeRefreshUsedThisTurn { get; set; }

		public FixedPoint ChargeGain { get; set; }

		public GuardianVowBinding()
		{
		}

		public GuardianVowBinding(GuardianVowBinding other)
		{
			GuardianActorDefinitionID = other.GuardianActorDefinitionID;
			SovereignActorDefinitionID = other.SovereignActorDefinitionID;
			LeftTurns = other.LeftTurns;
			PursuitTriggeredCount = other.PursuitTriggeredCount;
			ChargeAttackMaxTimes = other.ChargeAttackMaxTimes;
			ChargeRefreshUsedThisTurn = other.ChargeRefreshUsedThisTurn;
			ChargeGain = other.ChargeGain;
		}
	}
}
