namespace BaseModel
{
	public sealed class GuildBattleHighscoresChangedNotificationResponse
	{
		public int Delivered { get; set; }

		public string[] MissingPlayers { get; set; }
	}
}
