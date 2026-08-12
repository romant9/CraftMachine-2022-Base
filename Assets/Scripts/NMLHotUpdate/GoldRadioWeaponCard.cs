using TWDModel;
using UnityEngine;

public class GoldRadioWeaponCard : UIListCard<EquipPrizeWheelDefinition>
{
	[SerializeField]
	private UILabel buttonLabel1;

	[SerializeField]
	public UIButtonToggle toggle;

	[SerializeField]
	private UITexture texture;

	[SerializeField]
	private GameObject NotificationContainer;

	[SerializeField]
	private GameObject Limited1;

	private void Awake()
	{
		toggle.onClick.Clear();
		toggle.onClick.Add(new EventDelegate(OnClickPhoneWeaponCard));
	}

	public void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
		if (OfflineManager.IsLoadDataManager)
		{
			UpdateCDN();
		}
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
		else
		{
			OnRefreshScale();
		}
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		UpdateUI();
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "PhoneGoldRadioSelected" && parameter is EquipPrizeWheelDefinition equipPrizeWheelDefinition)
		{
			bool flag = equipPrizeWheelDefinition.Identifier.Equals(base.Item.Identifier);
			toggle.SetToggled(flag);
			if (flag)
			{
				MarkGoldRadioBannerNotificationData(equipPrizeWheelDefinition.Identifier);
			}
			UpdateGoldRadioBannerNotification();
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.Item != null)
		{
			GoldRadioCallDenifition goldRadioCallDenifitionByID = GameManager.Instance.playerModel.gameEconomyData.GetGoldRadioCallDenifitionByID(base.Item.Identifier);
			if (goldRadioCallDenifitionByID != null)
			{
				LoadImageFromCdn.LoadImageToTarget(texture, goldRadioCallDenifitionByID.TabPic);
				Helpers.GameObjectSetActive(Limited1, goldRadioCallDenifitionByID.Type == 2);
			}
			buttonLabel1.text = LocalizationManager.GetText(base.Item.ButtonsLocKey);
			toggle.SetToggled(toggled: false);
			UpdateGoldRadioBannerNotification();
		}
	}

	private void UpdateButtonSprites(string normalSpriteName)
	{
		if (!(toggle == null))
		{
			bool isToggled = toggle.IsToggled;
			toggle.normalSprite = normalSpriteName;
			toggle.SetToggled(isToggled);
		}
	}

	public void OnClickPhoneWeaponCard()
	{
		if (base.Item != null)
		{
			UIEvent.Send("PhoneGoldRadioSelected", base.Item);
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
		OnRefreshScale();
	}

	private void OnRefreshScale()
		{
			bool isToggled = toggle.IsToggled;
			Vector3 localScale = base.gameObject.transform.localScale;
			if (isToggled)
			{
				base.gameObject.transform.localScale = Vector3.one * 1.3f;
			}
			else
			{
				base.gameObject.transform.localScale = Vector3.one;
			}
			if (localScale != base.gameObject.transform.localScale)
			{
				UIEvent.Send("PhoneGoldRadioSelectedReposition");
			}
		}


	#region mycode
	private void UpdateCDN()
	{
		if (base.Item != null && texture.mainTexture == null)
		{
			GoldRadioCallDenifition goldRadioCallDenifitionByID = GameManager.Instance.playerModel.gameEconomyData.GetGoldRadioCallDenifitionByID(base.Item.Identifier);
			if (goldRadioCallDenifitionByID != null)
			{
				LoadImageFromCdn.LoadImageToTarget(texture, goldRadioCallDenifitionByID.TabPic); //GoldRadioCall_ShooterNWarrior //GoldRadioCall_Hunter
			}
		}
	}
	#endregion
}
