using System;
using BaseModel;

public static class GuildWorldBossMembershipLockPolicy
{
	public static GuildWorldBossMembershipLockDecision Evaluate(WorldBossGuildBaseSnapshot snapshot, string expectedGuildId, int expectedSeasonId, int expectedCycleId, long nowUtcMs, long cycleEndUtcMs)
	{
		if (snapshot == null || string.IsNullOrEmpty(expectedGuildId) || expectedSeasonId <= 0 || expectedCycleId <= 0)
		{
			return GuildWorldBossMembershipLockDecision.InvalidSnapshot;
		}
		WorldBossGuildBaseState guildBaseState = snapshot.GuildBaseState;
		if (guildBaseState == null)
		{
			return GuildWorldBossMembershipLockDecision.Allowed;
		}
		if (!string.Equals(guildBaseState.GroupId, expectedGuildId, StringComparison.Ordinal) || guildBaseState.SeasonId != expectedSeasonId || guildBaseState.CycleId != expectedCycleId)
		{
			return GuildWorldBossMembershipLockDecision.InvalidSnapshot;
		}
		if (cycleEndUtcMs > 0 && nowUtcMs >= cycleEndUtcMs)
		{
			return GuildWorldBossMembershipLockDecision.Allowed;
		}
		switch (guildBaseState.Status)
		{
		case WorldBossCycleStatus.SignedUp:
		case WorldBossCycleStatus.DifficultySelected:
		case WorldBossCycleStatus.Matchmaking:
		case WorldBossCycleStatus.Matched:
			return GuildWorldBossMembershipLockDecision.Locked;
		case WorldBossCycleStatus.None:
		case WorldBossCycleStatus.SigningUp:
		case WorldBossCycleStatus.Settled:
			return GuildWorldBossMembershipLockDecision.Allowed;
		default:
			return GuildWorldBossMembershipLockDecision.InvalidSnapshot;
		}
	}
}
