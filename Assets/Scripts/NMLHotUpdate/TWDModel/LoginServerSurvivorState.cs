namespace TWDModel
{
	public class LoginServerSurvivorState
	{
		public string SurvivorId { get; set; }

		public int SurvivorTotalLevel { get; set; }

		public LoginServerSurvivorState(string survivorId, int survivorTotalLevel)
		{
			SurvivorId = survivorId;
			SurvivorTotalLevel = survivorTotalLevel;
		}
	}
}
