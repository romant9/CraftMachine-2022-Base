using TWDModel;
using UnityEngine;

public class SpeedUpTitle : MonoBehaviour
{
	[SerializeField]
	private UILabel TimeTitle;

	[SerializeField]
	private GameObject BG;

	public void OnEnable()
	{
		Helpers.GameObjectSetActive(TimeTitle, value: false);
		Helpers.GameObjectSetActive(BG, value: false);
	}

	public void UpdateUI(CurrencyModel currencyModel)
	{
		Helpers.GameObjectSetActive(TimeTitle, value: false);
		Helpers.GameObjectSetActive(BG, value: false);
		if (currencyModel != null)
		{
			UpdateUI(currencyModel.Type);
		}
	}

	public void UpdateUI(IReward reward)
	{
		Helpers.GameObjectSetActive(TimeTitle, value: false);
		Helpers.GameObjectSetActive(BG, value: false);
		if (reward is RewardCurrency rewardCurrency)
		{
			UpdateUI(rewardCurrency.CurrencyType);
		}
	}

	public void UpdateUI(CurrencyType currencyType)
	{
		Helpers.GameObjectSetActive(TimeTitle, value: false);
		Helpers.GameObjectSetActive(BG, value: false);
		if (ComponentHelper.IsSpeedUpToken(currencyType))
		{
			SpeedupTokenTimeDefinition speedupTokenTimeDefinitionByCurrency = GameManager.Instance.playerModel.gameEconomyData.GetSpeedupTokenTimeDefinitionByCurrency(currencyType.ToString());
			if (speedupTokenTimeDefinitionByCurrency != null)
			{
				TimeTitle.text = speedupTokenTimeDefinitionByCurrency.Title;
				Helpers.GameObjectSetActive(TimeTitle, value: true);
				Helpers.GameObjectSetActive(BG, value: true);
			}
		}
	}
}
