namespace TWDModel
{
	public enum PurchaseValidationResult
	{
		None = 0,
		OK = 1,
		Error = 2,
		ClientValidationError = 3,
		ClientValidationFailed = 4,
		ClientValidationDuplicate = 5,
		ClientCommandFailed = 6,
		ServerValidationError = 7,
		ServerValidationFailed = 8,
		ServerCommandFailed = 9,
		ClientValidationNullPurchase = 10
	}
}
