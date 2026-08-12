namespace BaseModel
{
	public interface IGvgBattleEntry
	{
		string GroupId { get; set; }

		long MatchmakingEpochMsec { get; set; }

		int MatchmakingVersion { get; set; }

		int Tier { get; set; }

		long StartBattleTimestamp { get; set; }

		string GuildBattleMatchmakingInfo { get; set; }

		int RegisteredPlayers { get; set; }

		string LastOpponents { get; set; }

		int VictoryPoints { get; set; }

		string RegisteredPlayersList { get; set; }
	}
}
