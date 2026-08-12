namespace TWDModel
{
	public class SurvivalCharacterShieldStateModel
	{
		public int MaxShieldPoints { get; set; }

		public int ShieldPoints { get; set; }

		public void ResetToInitial()
		{
			MaxShieldPoints = 0;
			ShieldPoints = 0;
		}
	}
}
