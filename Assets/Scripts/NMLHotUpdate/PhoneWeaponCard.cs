using TWDModel;
using UnityEngine;

public class PhoneWeaponCard : UIListCard<EquipPrizeWheelDefinition>
{
	[SerializeField]
	private UILabel buttonLabel;

	[SerializeField]
	public UIButtonToggle toggle;

	[SerializeField]
	private GameObject NotificationContainer;

	public void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
	}

	public void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
	}

	public void Start()
	{
		if (toggle.IsToggled)
		{
			MarkGoldRadioBannerNotificationData(base.Item.Identifier);
			UpdateGoldRadioBannerNotification();
		}
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		UpdateUI();
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "PhoneWeaponSelected" && parameter is EquipPrizeWheelDefinition equipPrizeWheelDefinition)
		{
			toggle.SetToggled(equipPrizeWheelDefinition.Identifier.Equals(base.Item.Identifier));
			MarkGoldRadioBannerNotificationData(equipPrizeWheelDefinition.Identifier);
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.Item != null)
		{
			buttonLabel.text = LocalizationManager.GetText(base.Item.ButtonsLocKey);
			if (base.Item.RadioType == RadioType.GoldRadio)
			{
				UpdateButtonSprites("Ui_Premium_Gray_Button_Bg", "Ui_Premium_Button_Bg_Pressed", "Ui_Premium_Button_Bg_Pressed", "Ui_Premium_Button_Bg_Pressed");
			}
			UpdateGoldRadioBannerNotification();
			toggle.SetToggled(toggled: false);
		}
	}

	private void UpdateButtonSprites(string normalSpriteName, string hoverSpriteName = null, string pressedSpriteName = null, string disabledSpriteName = null)
	{
		if (!(toggle == null))
		{
			bool isToggled = toggle.IsToggled;
			toggle.normalSprite = normalSpriteName;
			if (!string.IsNullOrEmpty(hoverSpriteName))
			{
				toggle.hoverSprite = hoverSpriteName;
			}
			if (!string.IsNullOrEmpty(pressedSpriteName))
			{
				toggle.pressedSprite = pressedSpriteName;
			}
			if (!string.IsNullOrEmpty(disabledSpriteName))
			{
				toggle.disabledSprite = disabledSpriteName;
			}
			toggle.SetToggled(isToggled);
		}
	}

	public void OnClickPhoneWeaponCard()
	{
		if (base.Item != null)
		{
			UIEvent.Send("PhoneWeaponSelected", base.Item);
			UpdateGoldRadioBannerNotification();
		}
	}

	public override int GetSortValue()
	{
		if (base.Item == null)
		{
			return -1;
		}
		return base.Item.Order;
	}

	public void MarkGoldRadioBannerNotificationData(string Identifier)
	{
		Helpers.ExecuteCommand(new MarkGoldRadioPoolViewedCommand(Identifier));
		UIEvent.Send("MarkGoldRadioBanner");
	}

	public void UpdateGoldRadioBannerNotification()
	{
		if (GameManager.Instance != null)
		{
			GameManager instance = GameManager.Instance;
			if (instance.playerModel != null && instance.playerModel.EquipPrizeWheelModel != null)
			{
				bool value = instance.playerModel.EquipPrizeWheelModel.ShouldShowRedDotForPool(base.Item.Identifier);
				Helpers.GameObjectSetActive(NotificationContainer, value);
			}
		}
	}
}
