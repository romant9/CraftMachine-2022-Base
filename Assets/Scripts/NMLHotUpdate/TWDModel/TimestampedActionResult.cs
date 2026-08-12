namespace TWDModel
{
	public class TimestampedActionResult
	{
		public bool ActionTaken { get; set; }

		public bool Accepted { get; set; }

		public long Timestamp { get; set; }
	}
}
