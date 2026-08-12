using TWDModel;
using UnityEngine;

public class SurvivorAcceptPanel : MonoBehaviourExtended
{
	[SerializeField]
	private UISprite currencyIconSprite;

	[SerializeField]
	private UILabel tokenAmountLabel;

	[SerializeField]
	private GameObject SlotsFullParent;

	public void UpdateWithSurvivor(SurvivorModel survivorModel, bool canAddSurvivor)
	{
		if (IsNotNull(survivorModel, "ShowWithSurvivor()"))
		{
			Helpers.GameObjectSetActive(SlotsFullParent, !canAddSurvivor);
			CurrencyType classAsCurrency = SurvivorToken.GetClassAsCurrency(survivorModel.SurvivorClass);
			int totalCost = survivorModel.GetDemoteCashier().GetTotalCost(classAsCurrency);
			HelpersUI.SetContentToLabel(tokenAmountLabel, totalCost.ToString());
			HelpersUI.SetSprite(currencyIconSprite, HelpersGfx.GetCurrencyIconName(classAsCurrency));
		}
	}
}
