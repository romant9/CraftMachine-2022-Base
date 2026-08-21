namespace BaseModel
{
	public class BuyBundleLoadQueueMessage : LoadQueueMessage
	{
		public string BundleId { get; set; }

		public double PaidPrice { get; set; }

		public long SupportGivenTimestamp { get; set; }

		public string SupportEntityGUID { get; set; }

		public string PurchaseSource { get; set; }
	}
}
