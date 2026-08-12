namespace BaseModel
{
	public interface IGvgBattleOpponentMatchmakingEntry
	{
		long StartBattleTimestamp { get; set; }

		int RandomSeed { get; set; }

		bool IsFakeBattle { get; set; }

		string GuildBattleMatchmakingInfo { get; set; }
	}
}
