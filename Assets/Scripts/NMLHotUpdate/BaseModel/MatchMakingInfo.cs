namespace BaseModel
{
	public sealed class MatchMakingInfo
	{
		public string PlayerHashedId { get; set; }

		public string Nickname { get; set; }

		public string PlayerInformation { get; set; }

		public int Rating { get; set; }

		public int SecondaryRating { get; set; }

		public int Priority { get; set; }

		public long Availability { get; set; }

		public int Version { get; set; }
	}
}
