namespace BaseModel
{
	public enum GameHostState : byte
	{
		Normal = 0,
		Maintenance = 1,
		NoNewConnections = 2,
		Redirect = 3,
		Upgrade = 4,
		UpgradedClientUsed = 5,
		Erased = 6
	}
}
