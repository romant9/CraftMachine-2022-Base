using System.Collections.Generic;
using System.Linq;

namespace BaseModel
{
	public class GvgBattleEntry : IGvgBattleEntry
	{
		public string GroupId { get; set; }

		public long MatchmakingEpochMsec { get; set; }

		public int MatchmakingVersion { get; set; }

		public int Tier { get; set; }

		public int VictoryPoints { get; set; }

		public long StartBattleTimestamp { get; set; }

		public string GuildBattleMatchmakingInfo { get; set; }

		public int RegisteredPlayers { get; set; }

		public string LastOpponents { get; set; }

		public string RegisteredPlayersList { get; set; }

		public void SetRegisteredPlayersList(List<string> list)
		{
			RegisteredPlayersList = string.Join(",", list ?? Enumerable.Empty<string>());
		}
	}
}
