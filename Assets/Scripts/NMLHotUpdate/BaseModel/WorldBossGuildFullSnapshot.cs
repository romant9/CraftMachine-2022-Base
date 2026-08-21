using System.Collections.Generic;

namespace BaseModel
{
	public class WorldBossGuildFullSnapshot
	{
		public WorldBossMatchSnapshot Match { get; set; }

		public WorldBossGuildFullState GuildFullState { get; set; }

		public List<WorldBossCapturePointSnapshot> CapturePoints { get; set; }

		public WorldBossGuildBuffStateModel GuildBuffState { get; set; }

		public long SnapshotUtcMs { get; set; }

		public int MaxUnlockedDifficulty { get; set; }

		public long OpponentGuildScore { get; set; }

		public long OpponentPassScoreReachedUtcMs { get; set; }

		public Dictionary<string, long> OpponentMemberScores { get; set; }

		public WorldBossGuildFullSnapshot()
		{
			CapturePoints = new List<WorldBossCapturePointSnapshot>();
			OpponentMemberScores = new Dictionary<string, long>();
		}
	}
}
