namespace TWDModel
{
	public class LimitedCustomBundleData
	{
		public string Identifier { get; set; }

		public bool IsAvailable { get; set; }

		public bool IsCanBy { get; set; }

		public long Timer { get; set; }

		public string StartTimestamp { get; set; }

		public string EndTimestamp { get; set; }

		public long RefreshTime { get; set; }

		public CustomizedBundleType customType { get; set; }

		public long MinTimeFromLastCategoryBought { get; set; }
	}
}
