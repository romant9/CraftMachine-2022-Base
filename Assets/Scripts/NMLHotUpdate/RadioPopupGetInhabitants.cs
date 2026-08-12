using TWDModel;
using UnityEngine;

public class RadioPopupGetInhabitants : HUDElement
{
	[SerializeField]
	private UILabel inhabitantsCountLabel;

	[SerializeField]
	private GameObject currencyFlyStart;

	public int NumberInhabitants { get; set; }

	public override void Open()
	{
		base.Open();
		inhabitantsCountLabel.text = NumberInhabitants.ToString();
	}

	public override void Close()
	{
		base.Close();
		CampView.Instance.Hud.PauseCurrencyMeters = false;
		CampView.Instance.Hud.UpdateCurrencies();
		CurrencyModel currency = GameManager.Instance.playerModel.GetCurrency(CurrencyType.Inhabitants);
		if (currency.LastAdded != NumberInhabitants)
		{
			HUDNotification.Error(LocalizationManager.GetText("Error.NoStorage." + CurrencyType.Inhabitants));
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/invalid_action");
		}
		_ = currency.LastAdded;
	}
}
