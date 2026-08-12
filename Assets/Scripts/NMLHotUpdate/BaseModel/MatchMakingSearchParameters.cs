namespace BaseModel
{
	public sealed class MatchMakingSearchParameters
	{
		public int MinRating { get; set; }

		public int MaxRating { get; set; }

		public int MinSecondaryRating { get; set; }

		public int MaxSecondaryRating { get; set; }

		public string[] ExcludedPlayerIds { get; set; }
	}
}
