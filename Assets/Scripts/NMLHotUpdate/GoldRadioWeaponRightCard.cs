using UnityEngine;

public class GoldRadioWeaponRightCard : UIListCard<string>
{
	[SerializeField]
	private UIButton button;

	[SerializeField]
	private UISprite toggleBG;

	private void Awake()
	{
		button.onClick.Add(new EventDelegate(OnClickPhoneWeaponCard));
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.Item != null)
		{
			toggleBG.spriteName = HelpersGfx.GetSurvivorEventIconName(base.Item);
		}
	}

	public void OnClickPhoneWeaponCard()
	{
		if (base.Item != null)
		{
			string survivorClassName = HelpersLocalization.GetSurvivorClassName(base.Item);
			string text = LocalizationManager.GetText("GoldRadioCall.SkillClass.Tips", survivorClassName);
			TooltipManager.OpenTextBoxWithText(base.gameObject, text, TooltipManager.Prefabs.TooltipTextboxGold);
		}
	}
}
