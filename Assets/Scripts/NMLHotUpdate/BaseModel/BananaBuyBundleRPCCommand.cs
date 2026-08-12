namespace BaseModel
{
	public sealed class BananaBuyBundleRPCCommand
	{
		public string BundleId { get; set; }

		public double PaidPrice { get; set; }

		public long SupportEntityTimestamp { get; set; }

		public string PurchaseSource { get; set; }
	}
}
