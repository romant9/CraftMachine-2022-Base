namespace TWDModel
{
	public class SurvivalCharacterStateModel
	{
		public bool OutOfAction { get; set; }

		public int ChargePoints { get; set; }

		public FixedPoint HealthPercentage { get; set; }

		public int HealthPercentageBeforeCombat { get; set; }

		public int StrugglesLeft { get; set; }

		public int StrugglesLeftBeforeCombat { get; set; }

		public void ResetToInitial()
		{
			OutOfAction = false;
			ChargePoints = 0;
			HealthPercentage = 100L;
			HealthPercentageBeforeCombat = 100;
			StrugglesLeft = 1;
			StrugglesLeftBeforeCombat = 1;
		}
	}
}
