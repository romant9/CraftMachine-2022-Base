using TWDModel;
using UnityEngine;

public class SPRemoldScrapItem : MonoBehaviour
{
	[SerializeField]
	private GameObject Currency_Container;

	[SerializeField]
	private UILabel CurrencyNum;

	[SerializeField]
	private UISprite CurrencyIcon;

	[SerializeField]
	private UISprite SkilltokenIcon;

	[SerializeField]
	private UISprite SkilltokenIconBg;

	[SerializeField]
	private EquipmentTokenButton Apocalyptic_TokenEquipmentButton;

	public void Setup(IReward iReward)
	{
		Helpers.GameObjectSetActive(Currency_Container, value: false);
		Helpers.GameObjectSetActive(Apocalyptic_TokenEquipmentButton, value: false);
		if (!(iReward is RewardCurrency rewardCurrency))
		{
			if (iReward is RewardEquipToken upForCampaign)
			{
				Helpers.GameObjectSetActive(Apocalyptic_TokenEquipmentButton, value: true);
				Apocalyptic_TokenEquipmentButton.SetUpForCampaign(upForCampaign);
			}
			return;
		}
		CurrencyNum.text = rewardCurrency.Amount.ToString();
		Helpers.GameObjectSetActive(Currency_Container, value: true);
		Helpers.GameObjectSetActive(CurrencyIcon, value: false);
		Helpers.GameObjectSetActive(SkilltokenIcon, value: false);
		Helpers.GameObjectSetActive(SkilltokenIconBg, value: false);
		if (HelpersGfx.IsSkillTonkenCurrencyType(rewardCurrency.CurrencyType))
		{
			Helpers.GameObjectSetActive(SkilltokenIcon, value: true);
			Helpers.GameObjectSetActive(SkilltokenIconBg, value: true);
			SPTraitsSkillKitTokenSet skillKitTokenSetDefinition = HelpersGfx.GetSkillKitTokenSetDefinition(rewardCurrency.CurrencyType);
			HelpersUI.SetTraitsIconOnSprite(SkilltokenIcon, skillKitTokenSetDefinition.TopIcon, skillKitTokenSetDefinition.TopIconOnCloud);
			SkilltokenIconBg.spriteName = skillKitTokenSetDefinition.BGIcon;
		}
		else
		{
			Helpers.GameObjectSetActive(CurrencyIcon, value: true);
			CurrencyIcon.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType);
		}
	}
}
