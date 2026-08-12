namespace BaseModel
{
	public sealed class ValidateReceiptResponse
	{
		public InAppPurchaseState State { get; set; }

		public ValidateReceiptNextAction NextAction { get; set; }

		public string TransactionId { get; set; }
	}
}
