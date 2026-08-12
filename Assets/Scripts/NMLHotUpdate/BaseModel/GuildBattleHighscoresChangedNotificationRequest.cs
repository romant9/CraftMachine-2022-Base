namespace BaseModel
{
	public sealed class GuildBattleHighscoresChangedNotificationRequest
	{
		public string[] PlayerHashedIds { get; set; }

		public GuildBattleHighscoresChangedNotification Notification { get; set; }
	}
}
