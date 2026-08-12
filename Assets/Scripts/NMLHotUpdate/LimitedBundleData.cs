public class LimitedBundleData
{
	public string BundleID { get; set; }

	public bool IsAvailable { get; set; }

	public long Timer { get; set; }

	public string StartTimestamp { get; set; }

	public string EndTimestamp { get; set; }

	public long AvailabilityTime { get; set; }

	public long MinTimeFromLastCategoryBought { get; set; }
}
