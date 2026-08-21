namespace BaseModel
{
	public sealed class WorldBossUpdateGuildNameOperationRequest
	{
		public string GroupId { get; set; }

		public string PlayerHashedId { get; set; }

		public int SeasonId { get; set; }

		public int CycleId { get; set; }

		public string GuildName { get; set; }
	}
}
