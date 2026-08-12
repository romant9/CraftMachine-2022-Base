namespace BaseModel
{
	public sealed class LoginResponse
	{
		public string Identification;

		public string Address;

		public GameHostState State;

		public string SessionToken;

		public LockRespond LockState;

		public MaintenanceInfo Maintenance;

		public string currency;
	}
}
