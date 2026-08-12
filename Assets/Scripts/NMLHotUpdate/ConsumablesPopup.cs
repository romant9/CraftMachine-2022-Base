using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ConsumablesPopup : HUDElement
{
	public delegate void ConfirmationCallback(TWDModelResult result);

	[Header("Order should be Grenade, Medkit, Flare, Blast, Gore")]
	[SerializeField]
	private UILabel[] consumableAmount;

	[SerializeField]
	private UILabel[] consumableAmountBigCard;

	[SerializeField]
	private UISprite[] consumableAmountBg;

	[SerializeField]
	private UISprite[] consumableAmountBgBigCard;

	[SerializeField]
	private GameObject[] bigCards;

	[SerializeField]
	private GameObject[] consumableSelectedHighlights;

	[SerializeField]
	private UILabel[] cooldownAfterUseDescriptionConsumable;

	[SerializeField]
	private UILabel grenadeDescription;

	[SerializeField]
	private UILabel medKitDescription;

	[SerializeField]
	private UILabel flareDescription;

	[SerializeField]
	private UILabel blastGrenadeDescription;

	[SerializeField]
	private UILabel goreDescription;

	[SerializeField]
	private UILabel grenadeThreatDescription;

	[SerializeField]
	private UILabel blastGrenadeThreatDescription;

	[Header("Amount Label Colors")]
	[SerializeField]
	private Color amountTextColorDefault;

	[SerializeField]
	private Color amountTextColorZero;

	[SerializeField]
	private Color amountTextBgColorDefault;

	[SerializeField]
	private Color amountTextBgColorZero;

	[SerializeField]
	private List<TokenHUDMeter> tokenMeters = new List<TokenHUDMeter>();

	private const string ConsumablesCampHowTo = "ConsumablesCampHowTo";

	private const string LocalizationKeyCooldownAfterUse = "Consumable.Menu.Stat.CooldownAfterUse{Turns}";

	private const string LocalizationKeyThreatGenerated = "Consumable.Menu.Stat.ThreatGenerated{Amount}";

	private const string LocalizationKeyGrenadeDescription = "Consumable.Grenade.Description{DamageFlat}{DamagePercentage}";

	private const string LocalizationKeyMedKitDescription = "Consumable.Medkit.Description{HealPercentage}";

	private const string LocalizationKeyBlastGrenadeDescription = "Consumable.BlastGrenade.Description{Parameters}";

	private const string LocalizationKeyFlareDescription = "Consumable.Flare.Description{Duration}";

	private const string LocalizationKeyGoreDescription = "Consumable.Gore.Description{Duration}";

	private const string NewSpeedUpTokenAcquiredAmount = "NewSpeedUpTokenAcquiredAmount";

	private const string NewConsumablesAcquiredAmount = "NewConsumablesAcquiredAmount";

	[SerializeField]
	private UILabel tokenNotificationAmount;

	[SerializeField]
	private UILabel consumableNotificationAmount;

	[SerializeField]
	private UIInterceptableTabs tabs;

	[SerializeField]
	private UIButtonToggleSet toolBagToggleSet;

	[SerializeField]
	private UILabel TokenName;

	[SerializeField]
	private UILabel TokenNum;

	[SerializeField]
	private UILabel TokenDec;

	[SerializeField]
	private UISprite TokenIcon;

	[SerializeField]
	private GameObject skillKitsNotice;

	[SerializeField]
	private GameObject tabSkillToken;

	public override void Open()
	{
		base.Open();
		bool flag = false;
		int num = TWDPlayerPrefs.GetInt("NewSpeedUpTokenAcquiredAmount");
		int num2 = TWDPlayerPrefs.GetInt("NewConsumablesAcquiredAmount");
		Helpers.GameObjectSetActive(tokenNotificationAmount, num > 0);
		tokenNotificationAmount.text = num.ToString();
		Helpers.GameObjectSetActive(consumableNotificationAmount, num2 > 0);
		consumableNotificationAmount.text = num2.ToString();
		UpdateSkillKitsNotice();
		UpdateTabSkillToken();
		foreach (EquipmentModel.ConsumableType value in Enum.GetValues(typeof(EquipmentModel.ConsumableType)))
		{
			if (value == EquipmentModel.ConsumableType.Unknown)
			{
				continue;
			}
			int num3 = (int)(value - 1);
			int count = GameManager.Instance.playerModel.Equipment.GetConsumablesOfType(value).Count;
			consumableAmount[num3].text = "x" + count;
			consumableAmountBigCard[num3].text = "x" + count;
			bool flag2 = count == 0;
			consumableAmount[num3].color = (flag2 ? amountTextColorZero : amountTextColorDefault);
			consumableAmountBg[num3].color = (flag2 ? amountTextBgColorZero : amountTextBgColorDefault);
			consumableAmountBigCard[num3].color = (flag2 ? amountTextColorZero : amountTextColorDefault);
			consumableAmountBgBigCard[num3].color = (flag2 ? amountTextBgColorZero : amountTextBgColorDefault);
			if (!flag && count > 0)
			{
				flag = true;
				switch (value)
				{
				case EquipmentModel.ConsumableType.Grenade:
					OnGrenadeClick();
					break;
				case EquipmentModel.ConsumableType.MedKit:
					OnMedkitClick();
					break;
				case EquipmentModel.ConsumableType.Flare:
					OnFlareClick();
					break;
				case EquipmentModel.ConsumableType.BlastGrenade:
					OnBlastGrenadeClick();
					break;
				case EquipmentModel.ConsumableType.Gore:
					OnGoreClick();
					break;
				default:
					flag = false;
					break;
				}
			}
		}
		if (!flag)
		{
			OnMedkitClick();
		}
		UIScrollView componentInChildren = GetComponentInChildren<UIScrollView>();
		if (componentInChildren != null)
		{
			componentInChildren.ResetPosition();
		}
		if (PlayerPrefs.GetInt("ConsumablesCampHowTo", 0) == 0)
		{
			EnableHowTo();
		}
		SetupTokenMeters();
		UpdateTokenMeters();
		CurrencyModel currency = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.SuperEquipmentTokenBP);
		UpdateTokenTitles(currency);
	}

	private void OnEnable()
	{
		UpdateSkillKitsNotice();
		tabs.OnNewTabSelectedEvent += OnToolBagTabSwitched;
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		tabs.OnNewTabSelectedEvent -= OnToolBagTabSwitched;
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		switch (type)
		{
		case "ConsumablesItemClickEvent":
			if (parameter != null && parameter is CurrencyModel)
			{
				CurrencyModel currency = parameter as CurrencyModel;
				UpdateTokenTitles(currency);
			}
			break;
		case "SPRemoldKitUpgradeNotice":
		case "SPRemoldKitUnlockNotice":
		case "SPRemoldMakeModSkillSuccess":
		case "SPRemoldUpgradeModSkillSuccess":
			UpdateSkillKitsNotice();
			break;
		}
	}

	public void OnGrenadeClick()
	{
		SetBigCardAndHighlightVisibility(EquipmentModel.ConsumableType.Grenade);
		SetCooldownAfterUse(EquipmentModel.ConsumableType.Grenade);
		grenadeDescription.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Consumable.Grenade.Description{DamageFlat}{DamagePercentage}", (int)ConsumableUtils.GetFlatDamage(GameManager.Instance.modelManager, EquipmentModel.ConsumableType.Grenade), ConsumableUtils.GetPercentageDamageDefinition(GameManager.Instance.modelManager, EquipmentModel.ConsumableType.Grenade));
		grenadeThreatDescription.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Consumable.Menu.Stat.ThreatGenerated{Amount}", ConsumableUtils.GetThreatDefinition(GameManager.Instance.modelManager, EquipmentModel.ConsumableType.Grenade));
	}

	public void OnMedkitClick()
	{
		SetBigCardAndHighlightVisibility(EquipmentModel.ConsumableType.MedKit);
		SetCooldownAfterUse(EquipmentModel.ConsumableType.MedKit);
		medKitDescription.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Consumable.Medkit.Description{HealPercentage}", ConsumableUtils.GetMedKitRecoveredHealthDefinition(GameManager.Instance.modelManager));
	}

	public void OnFlareClick()
	{
		SetBigCardAndHighlightVisibility(EquipmentModel.ConsumableType.Flare);
		SetCooldownAfterUse(EquipmentModel.ConsumableType.Flare);
		flareDescription.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Consumable.Flare.Description{Duration}", ConsumableUtils.GetFlareDuration(GameManager.Instance.modelManager));
	}

	public void OnBlastGrenadeClick()
	{
		SetBigCardAndHighlightVisibility(EquipmentModel.ConsumableType.BlastGrenade);
		SetCooldownAfterUse(EquipmentModel.ConsumableType.BlastGrenade);
		blastGrenadeDescription.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Consumable.BlastGrenade.Description{Parameters}", ConsumableUtils.GetBlastGrenadePushDistance(GameManager.Instance.modelManager), (int)ConsumableUtils.GetFlatDamage(GameManager.Instance.modelManager, EquipmentModel.ConsumableType.BlastGrenade), ConsumableUtils.GetPercentageDamageDefinition(GameManager.Instance.modelManager, EquipmentModel.ConsumableType.BlastGrenade));
		blastGrenadeThreatDescription.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Consumable.Menu.Stat.ThreatGenerated{Amount}", ConsumableUtils.GetThreatDefinition(GameManager.Instance.modelManager, EquipmentModel.ConsumableType.BlastGrenade));
	}

	public void OnGoreClick()
	{
		SetBigCardAndHighlightVisibility(EquipmentModel.ConsumableType.Gore);
		SetCooldownAfterUse(EquipmentModel.ConsumableType.Gore);
		goreDescription.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Consumable.Gore.Description{Duration}", ConsumableUtils.GetGoreDuration(GameManager.Instance.modelManager));
	}

	private void SetBigCardAndHighlightVisibility(EquipmentModel.ConsumableType consumableType)
	{
		for (int i = 0; i < bigCards.Length; i++)
		{
			if (i == (int)(consumableType - 1))
			{
				bigCards[i].SetActive(value: true);
				consumableSelectedHighlights[i].SetActive(value: true);
			}
			else
			{
				bigCards[i].SetActive(value: false);
				consumableSelectedHighlights[i].SetActive(value: false);
			}
		}
	}

	private void SetCooldownAfterUse(EquipmentModel.ConsumableType consumableType)
	{
		cooldownAfterUseDescriptionConsumable[(int)(consumableType - 1)].text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Consumable.Menu.Stat.CooldownAfterUse{Turns}", ConsumableUtils.GetCooldownDefinition(GameManager.Instance.modelManager, consumableType));
	}

	public void EnableHowTo()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConsumablesInfoPopup, transform.parent.gameObject).Open(); //
		PlayerPrefs.SetInt("ConsumablesCampHowTo", 1);
	}

	public void EnableHowToTokenInfo()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.TokenInfoPopup, transform.parent.gameObject).Open();
	}

	public void EnableHowToSuperTokenInfo()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SuperTokenInfoPopup, transform.parent.gameObject).Open();
	}

	public void OnToolBagTabSwitched(int tabIndex)
	{
		UpdateTokenMeters();
		if (tabs.CurrentTabIndex == 0)
		{
			Helpers.GameObjectSetActive(consumableNotificationAmount, value: false);
			TWDPlayerPrefs.SetInt("NewConsumablesAcquiredAmount", 0);
		}
		else if (tabs.CurrentTabIndex == 1)
		{
			Helpers.GameObjectSetActive(tokenNotificationAmount, value: false);
			TWDPlayerPrefs.SetInt("NewSpeedUpTokenAcquiredAmount", 0);
		}
	}

	private void SetupTokenMeters()
	{
		foreach (TokenHUDMeter tokenMeter in tokenMeters)
		{
			tokenMeter.Setup();
		}
	}

	private void UpdateTokenMeters()
	{
		foreach (TokenHUDMeter tokenMeter in tokenMeters)
		{
			tokenMeter.UpdateTokenHUDMeter();
		}
	}

	private void UpdateTokenTitles(CurrencyModel currency)
	{
		if (currency != null)
		{
			if (TokenIcon != null)
			{
				TokenIcon.spriteName = HelpersGfx.GetCurrencyIconName(currency.Type);
			}
			if (TokenName != null)
			{
				TokenName.text = HelpersLocalization.GetSpeedCurrencyName(currency.Type);
			}
			if (TokenNum != null)
			{
				TokenNum.text = LocalizationManager.GetText("Currency.OwnedAmount{OwnedAmount}", currency.Value);
			}
			if (TokenDec != null)
			{
				TokenDec.text = HelpersLocalization.GetCurrencyDescription(currency.Type);
			}
		}
	}

	public void OnClickTokenInfo()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SpeedUpInfoPopup, transform.parent.gameObject).Open(); // 
	}

	private void UpdateSkillKitsNotice()
	{
		Helpers.GameObjectSetActive(skillKitsNotice, Helpers.IsSkillKitNotice());
	}

	private void UpdateTabSkillToken()
	{
		Helpers.GameObjectSetActive(tabSkillToken, Helpers.IsSystemOpenById("SystemBase.SkillBag"));
	}

	public void OnClickToolBagSkillTab()
	{
		toolBagToggleSet.SetSelectedIndex(2);
		tabs.SelectTab(2);
	}

	public void OnClickSkillBagHelp()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldTraitsSkillBagHelp, transform.parent.gameObject).Open(); //
	}
}
