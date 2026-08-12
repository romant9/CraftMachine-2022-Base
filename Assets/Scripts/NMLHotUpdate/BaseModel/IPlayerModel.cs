using System;

namespace BaseModel
{
	public interface IPlayerModel
	{
		string Name { get; }

		string Language { get; }

		int Score { get; }

		int Level { get; }

		int SecondaryScore { get; }

		string HashedId { get; set; }

		string GuildAnalyticId { get; }

		string GuildName { get; }

		int CouncilLevel { get; }

		PcPlatform PcPlatform { get; set; }

		DateTime Created { get; set; }
	}
}
