using TWDModel;

public class TokenInfoLocalizationLabel : LocalizationUIUpdaterWithParams
{
	public CurrencyType Currency;

	protected override void Awake()
	{
		base.Awake();
		parameters = new string[1] { GameManager.Instance.modelManager.Player.GetCurrency(Currency).Max.ToString() };
	}
}
