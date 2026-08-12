namespace BaseModel
{
	public sealed class GuildBattleHighscoresChangedNotification
	{
		public string BattleId { get; set; }

		public int WarId { get; set; }

		public string SourceGuildId { get; set; }

		public string TargetGuildId { get; set; }

		public long Timestamp { get; set; }
	}
}
