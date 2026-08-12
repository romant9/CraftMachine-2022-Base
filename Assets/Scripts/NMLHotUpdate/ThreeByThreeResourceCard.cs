using TWDModel;
using UnityEngine;

public class ThreeByThreeResourceCard : MonoBehaviour
{
	[Tooltip("SPIcon")]
	[SerializeField]
	private GameObject SPIcon;

	[Tooltip("GemIcon")]
	[SerializeField]
	private GameObject GemIcon;

	[Tooltip("SupplyIcon")]
	[SerializeField]
	private GameObject SupplyIcon;

	[Tooltip("SupplyIcon")]
	[SerializeField]
	private UILabel AmountLabel;

	public void SetResource(CurrencyType currencyType, int amount)
	{
		SPIcon.SetActive(value: false);
		SupplyIcon.SetActive(value: false);
		GemIcon.SetActive(value: false);
		switch (currencyType)
		{
		case CurrencyType.Diamonds:
			GemIcon.SetActive(value: true);
			break;
		case CurrencyType.Supplies:
			SupplyIcon.SetActive(value: true);
			break;
		case CurrencyType.SurvivalPoints:
			SPIcon.SetActive(value: true);
			break;
		}
		AmountLabel.text = $"{amount:N0}";
	}
}
