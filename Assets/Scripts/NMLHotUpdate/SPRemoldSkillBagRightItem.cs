using TWDModel;
using UnityEngine;

public class SPRemoldSkillBagRightItem : MonoBehaviour
{
	[SerializeField]
	private UILabel CurrencyNum;

	[SerializeField]
	private UISprite CurrencyIcon;

	[SerializeField]
	private UISprite SkilltokenIcon;

	[SerializeField]
	private UISprite SkilltokenIconBg;

	[SerializeField]
	private UISprite ImageProgress;

	private CurrencyType currencyType;

	public void Setup(CurrencyType currencyType, int count)
	{
		Helpers.GameObjectSetActive(SkilltokenIcon, value: false);
		Helpers.GameObjectSetActive(SkilltokenIconBg, value: false);
		Helpers.GameObjectSetActive(CurrencyIcon, value: false);
		this.currencyType = currencyType;
		if (HelpersGfx.IsSkillTonkenCurrencyType(currencyType))
		{
			Helpers.GameObjectSetActive(SkilltokenIcon, value: true);
			Helpers.GameObjectSetActive(SkilltokenIconBg, value: true);
			SPTraitsSkillKitTokenSet skillKitTokenSetDefinition = HelpersGfx.GetSkillKitTokenSetDefinition(currencyType);
			HelpersUI.SetTraitsIconOnSprite(SkilltokenIcon, skillKitTokenSetDefinition.TopIcon, skillKitTokenSetDefinition.TopIconOnCloud);
			SkilltokenIconBg.spriteName = skillKitTokenSetDefinition.BGIcon;
		}
		else
		{
			Helpers.GameObjectSetActive(CurrencyIcon, value: true);
			CurrencyIcon.spriteName = HelpersGfx.GetCurrencyIconName(currencyType);
		}
		int value = GameManager.Instance.playerModel.GetCurrency(currencyType).Value;
		CurrencyNum.text = $"{value}/{count}";
		if (value < count)
		{
			CurrencyNum.color = Color.red;
		}
		else
		{
			CurrencyNum.color = Color.green;
		}
		ImageProgress.fillAmount = (float)value / (float)count;
	}

	public void Onclick()
	{
		if (HelpersGfx.IsSkillTonkenCurrencyType(currencyType))
		{
			SPRemoldSkillTokenTipsPopup sPRemoldSkillTokenTipsPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldSkillTokenTipsPopup) as SPRemoldSkillTokenTipsPopup;
			if (sPRemoldSkillTokenTipsPopup != null)
			{
				sPRemoldSkillTokenTipsPopup.Setup(currencyType);
				sPRemoldSkillTokenTipsPopup.Open();
			}
		}
	}
}
