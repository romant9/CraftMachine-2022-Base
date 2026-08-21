using System.Collections.Generic;

namespace BaseModel
{
	public class WorldBossGuildFullState
	{
		public string GroupId { get; set; }

		public int SeasonId { get; set; }

		public int CycleId { get; set; }

		public WorldBossCycleStatus Status { get; set; }

		public List<string> SignedUpMemberIds { get; set; }

		public int Difficulty { get; set; }

		public string DifficultySelectedByMemberId { get; set; }

		public long DifficultySelectedAtUtcMs { get; set; }

		public long SignedUpAtUtcMs { get; set; }

		public long MatchmakingEpochMsec { get; set; }

		public string OpponentGroupId { get; set; }

		public bool IsFakeBattle { get; set; }

		public string OpponentGuildBattleMatchmakingInfo { get; set; }

		public int MatchRandomSeed { get; set; }

		public long MatchedAtUtcMs { get; set; }

		public long UpdatedUtcMs { get; set; }

		public Dictionary<string, long> MemberScores { get; set; }

		public Dictionary<string, long> MemberBattleCounts { get; set; }

		public Dictionary<string, long> MemberMaxDamages { get; set; }

		public long GuildScore { get; set; }

		public long PassScoreReachedUtcMs { get; set; }

		public WorldBossSettlementResult SettlementResult { get; set; }

		public List<string> ClaimedMemberIds { get; set; }

		public WorldBossGuildFullState()
		{
			SignedUpMemberIds = new List<string>();
			MemberScores = new Dictionary<string, long>();
			MemberBattleCounts = new Dictionary<string, long>();
			MemberMaxDamages = new Dictionary<string, long>();
			ClaimedMemberIds = new List<string>();
		}
	}
}
