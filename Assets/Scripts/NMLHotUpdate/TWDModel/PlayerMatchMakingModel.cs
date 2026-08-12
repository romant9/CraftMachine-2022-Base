using System;
using BaseModel;

namespace TWDModel
{
	public class PlayerMatchMakingModel : IPlayerModel
	{
		public string Name { get; set; }

		public string Language { get; set; }

		public int Score { get; set; }

		public int Level { get; set; }

		public string HashedId { get; set; }

		public int SecondaryScore { get; set; }

		public DateTime Created { get; set; }

		public string GuildAnalyticId { get; }

		public string GuildName { get; }

		public int CouncilLevel { get; }

		public string MatchInformation { get; set; }

		public int MatchMakingRating { get; set; }

		public int MinAttackMatchMakingRating { get; set; }

		public int MaxAttackMatchMakingRating { get; set; }

		public int MatchMakingPriority { get; set; }

		public long MatchMakingAvailability { get; set; }

		public string[] ExcludedMatchMakingTargets { get; set; }

		public PcPlatform PcPlatform { get; set; }
	}
}
