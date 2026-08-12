namespace TWDModel
{
	public enum CombatSupportAvailability
	{
		Executable = 0,
		AnotherSupportUsedThisTurn = 1,
		OnCooldown = 2,
		SurvivorIsUnavailable = 3,
		SupportNotEquipped = 4,
		AlreadyUsed = 5,
		SurvivorIsDead = 6
	}
}
