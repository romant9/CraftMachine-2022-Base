namespace BaseModel
{
	public class BuySubscriptionLoadQueueMessage : LoadQueueMessage
	{
		public string SubscriptionId { get; set; }

		public int SubscriptionPlatform { get; set; }

		public long ExpiryTimeMillis { get; set; }

		public long BuyTime { get; set; }

		public int GiveExtraReward { get; set; }

		public long SupportGivenTimestamp { get; set; }

		public string SupportEntityGUID { get; set; }
	}
}
