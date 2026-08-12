namespace BaseModel
{
	public class BuyBundleResultInfo
	{
		public bool IsFreeDailyBundle;

		public string TransactionId { get; set; }

		public string BundleId { get; set; }

		public string HashId { get; set; }

		public int State { get; set; }

		public string PurchaseSource { get; set; }

		public int PeriodId { get; set; }

		public string RandomResultBundleId { get; set; }

		public double PayPrice { get; set; }

		public long BuyTime { get; set; }
	}
}
