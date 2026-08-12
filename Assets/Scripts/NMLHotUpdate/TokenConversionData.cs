using TWDModel;

public class TokenConversionData
{
	public CurrencyType CurrencyType;

	public int Amount { get; set; }

	public int ConversionAmount { get; set; }

	public int TotalConvertedAmount { get; set; }

	public TokenConversionData(CurrencyType type, int amount, int conversionAmount, int total)
	{
		CurrencyType = type;
		Amount = amount;
		ConversionAmount = conversionAmount;
		TotalConvertedAmount = total;
	}
}
