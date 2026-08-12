namespace BaseModel
{
	public sealed class SubscriptionRPCCommand
	{
		public string SubscriptionId { get; set; }

		public int Platform { get; set; }

		public long ExpiryTimeMillis { get; set; }

		public int GiveExtraReward { get; set; }
	}
}
