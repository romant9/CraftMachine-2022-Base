using System.Collections.Generic;
using NextGames.Sdk.AssetBundleManager;
using TWDModel;
using UnityEngine;

public class PlayerHubActivityPanel : MonoBehaviour
{
	[SerializeField]
	private PlayerHubActivityList activityList;

	[SerializeField]
	private GameObject emptyPanel;

	[SerializeField]
	private UITexture texture;

	[SerializeField]
	private UITexture bgTexture;

	[SerializeField]
	private UIButton leftButton;

	[SerializeField]
	private UILabel leftButtonLabel;

	[SerializeField]
	private UIButton rightButton;

	[SerializeField]
	private UILabel rightButtonLabel;

	[SerializeField]
	private UILabel timeLabel;

	[SerializeField]
	private GameObject characterPanel;

	[SerializeField]
	private GameObject bonusPanel;

	[SerializeField]
	private GameObject weaponPanel;

	[SerializeField]
	private GameObject armorPanel;

	[SerializeField]
	private GameObject textPanel;

	[Header("CharacterPanel")]
	[SerializeField]
	private UILabel heroLabel;

	[SerializeField]
	private UISprite heroProfessionSprite;

	[SerializeField]
	private UISprite traitSprite;

	[SerializeField]
	private UILabel traitTitleLabel;

	[SerializeField]
	private UILabel traitDesLabel;

	[SerializeField]
	private UIScrollBar characterScrollBar;

	[Header("BonusPanel")]
	[SerializeField]
	private UILabel bonusHeroLabel;

	[SerializeField]
	private UISprite bonusHeroProfessionSprite;

	[SerializeField]
	private UISprite bonusSprite;

	[SerializeField]
	private UILabel bonusTitleLabel;

	[SerializeField]
	private UILabel bonusDesLabel;

	[SerializeField]
	private BounsPortrait ownerPortrait;

	[SerializeField]
	private GameObject partnerParent;

	[SerializeField]
	private UIScrollBar bonusScrollBar;

	[SerializeField]
	private UILabel bonusLevelLabel;

	[Header("WeaponPanel")]
	[SerializeField]
	private UISprite weaponTypeSprite;

	[SerializeField]
	private UILabel weaponLabel;

	[SerializeField]
	private UILabel weaponRangeLabel;

	[SerializeField]
	private UILabel weaponRadiusLabel;

	[SerializeField]
	private UILabel weaponDesLabel;

	[SerializeField]
	private UIScrollBar weaponScrollBar;

	[Header("ArmorPanel")]
	[SerializeField]
	private UISprite armorTypeSprite;

	[SerializeField]
	private UILabel armorLabel;

	[SerializeField]
	private UILabel armorDesLabel;

	[SerializeField]
	private UIScrollBar armorScrollBar;

	[Header("TextPanel")]
	[SerializeField]
	private UILabel textTitleLabel;

	[SerializeField]
	private UILabel textLabel;

	[SerializeField]
	private UIScrollBar textScrollBar;

	private GameObject[] _panels;

	private List<GameObject> _portraitList = new List<GameObject>();

	private Vector2 texturePos = new Vector2(-71f, -26f);

	private void OnEnable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
		UIEvent.OnUIEvent += OnUiEvent;
		_panels = new GameObject[5] { characterPanel, bonusPanel, weaponPanel, armorPanel, textPanel };
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "PlayerHubActivitySelectedEvent")
		{
			if (!(parameter is ActiveInformationDefinition model))
			{
				SetEmptyPanel();
				return;
			}
			PlayerHubActivityCard playerHubActivityCard = (PlayerHubActivityCard)activityList.GetCard(model);
			RefreshUI(playerHubActivityCard.Item);
		}
	}

	private void RefreshUI(ActiveInformationDefinition info)
	{
		Helpers.GameObjectSetActive(emptyPanel.gameObject, value: false);
		PlayerHubActivityType type = (PlayerHubActivityType)info.Type;
		SetActivePanel(type);
		Helpers.GameObjectSetActive(timeLabel.gameObject, value: false);
		bgTexture.alpha = 0.4f;
		texture.mainTexture = null;
		texture.transform.localPosition = new Vector3(texturePos.x, texturePos.y + info.RotateOffset, 0f);
		texture.transform.localScale = new Vector3(info.SizeOffset, info.SizeOffset, info.SizeOffset);
		texture.material = null;
		bgTexture.mainTexture = null;
		Material material = AssetBundleManager.Instance.LoadAsset<Material>(info.Image, "uimaterials");
		if (material != null && bgTexture != null)
		{
			bgTexture.material = material;
		}
		object[] arguments = new object[2] { info.Open, info.End };
		HelpersUI.SetContentToLabel(timeLabel, LocalizationManager.GetText("System.ActiveInfo.Timer", arguments));
		if (string.IsNullOrEmpty(info.Button1Name))
		{
			Helpers.GameObjectSetActive(leftButton.gameObject, value: false);
		}
		else
		{
			Helpers.GameObjectSetActive(leftButton.gameObject, value: true);
			HelpersUI.SetContentToLabel(leftButtonLabel, LocalizationManager.GetText(info.Button1Name));
		}
		if (string.IsNullOrEmpty(info.Button2Name))
		{
			Helpers.GameObjectSetActive(rightButton.gameObject, value: false);
		}
		else if (IsEpicMacAndBundleShop(info.Button2))
		{
			Helpers.GameObjectSetActive(rightButton.gameObject, value: false);
		}
		else
		{
			Helpers.GameObjectSetActive(rightButton.gameObject, value: true);
			HelpersUI.SetContentToLabel(rightButtonLabel, LocalizationManager.GetText(info.Button2Name));
		}
		if (info.Button1 == "3001")
		{
			leftButton.normalSprite = "Ui_Regular_Button_Yellow_Bg";
			leftButton.pressedSprite = "Ui_Regular_Button_Yellow_Pressed_Bg";
		}
		else
		{
			leftButton.normalSprite = "Ui_Regular_Button_Bg";
			leftButton.pressedSprite = "Ui_Regular_Button_Bg_Pressed";
		}
		if (info.Button2 == "3001")
		{
			rightButton.normalSprite = "Ui_Regular_Button_Yellow_Bg";
			rightButton.pressedSprite = "Ui_Regular_Button_Yellow_Pressed_Bg";
		}
		else
		{
			rightButton.normalSprite = "Ui_Regular_Button_Bg";
			rightButton.pressedSprite = "Ui_Regular_Button_Bg_Pressed";
		}
		switch (type)
		{
		case PlayerHubActivityType.Character:
		{
			Helpers.GameObjectSetActive(texture.gameObject, value: true);
			ActorDefinition actorDefinition2 = GameManager.Instance.gameEconomyData.GetActorDefinition(info.FunctionValue);
			HelpersGfx.SetSeasonHeroMaterial(texture, actorDefinition2.Image);
			heroLabel.text = actorDefinition2.Name;
			heroProfessionSprite.spriteName = HelpersGfx.GetSurvivorClassIconName(actorDefinition2.Class, 4);
			string traitIdentifier = actorDefinition2.UpgradeTraits[0] + ".Level9";
			TraitDefinition traitDefinition3 = GameManager.Instance.gameEconomyData.GetTraitDefinition(traitIdentifier);
			traitSprite.spriteName = HelpersGfx.GetSurvivorTraitIconName(traitDefinition3);
			HelpersUI.SetContentToLabel(traitTitleLabel, HelpersLocalization.GetTraitName(traitDefinition3));
			HelpersUI.SetContentToLabel(traitDesLabel, HelpersLocalization.GetTraitDescription(traitDefinition3));
			characterScrollBar.value = 0f;
			break;
		}
		case PlayerHubActivityType.Bonus:
		{
			Helpers.GameObjectSetActive(texture.gameObject, value: true);
			if (int.TryParse(info.FunctionValue, out var result))
			{
				BounsInfoDefinition bounsInfo = GameManager.Instance.gameEconomyData.GetBounsInfo(result);
				ActorDefinition actorDefinition = GameManager.Instance.gameEconomyData.GetActorDefinition(bounsInfo.Owner);
				HelpersGfx.SetSeasonHeroMaterial(texture, actorDefinition.Image);
				int bounsMaxLevel = GameManager.Instance.gameEconomyData.GetBounsMaxLevel(bounsInfo.ItemID);
				BounsLevelDefinition bounsLevelDefinition = GameManager.Instance.gameEconomyData.GetBounsLevelDefinition(bounsInfo.ItemID, bounsMaxLevel);
				TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(bounsLevelDefinition.TraitsLevel);
				TraitDefinition traitDefinition2 = GameManager.Instance.gameEconomyData.GetTraitDefinition(bounsLevelDefinition.QualityLevel);
				HelpersUI.SetContentToLabel(bonusLevelLabel, LocalizationManager.GetText("ActiveInformation_KeyWord_HeirloomLv", bounsMaxLevel));
				HelpersUI.SetContentToLabel(bonusDesLabel, HelpersLocalization.GetTraitDescription(traitDefinition) + "\n" + HelpersLocalization.GetTraitDescription(traitDefinition2));
				bonusSprite.spriteName = bounsInfo.VisualOverride;
				bonusHeroProfessionSprite.spriteName = HelpersGfx.GetSurvivorClassIconName(actorDefinition.Class, 4);
				bonusHeroLabel.text = actorDefinition.Name;
				bonusTitleLabel.text = LocalizationManager.GetText(bounsInfo.Name);
				SetPortrait(bounsInfo);
				bonusScrollBar.value = 0f;
			}
			break;
		}
		case PlayerHubActivityType.Weapon:
		{
			bgTexture.alpha = 0.7f;
			Helpers.GameObjectSetActive(texture.gameObject, value: true);
			EquipmentDefinition equipmentDefinition2 = GameManager.Instance.gameEconomyData.GetEquipmentDefinition(info.FunctionValue);
			AbilityDefinition abilityDefinition = GameManager.Instance.gameEconomyData.GetAbilityDefinition(equipmentDefinition2.AbilityIdentifier);
			texture.mainTexture = HelpersGfx.GetEquipmentIconTextureFromID(equipmentDefinition2.ID);
			weaponTypeSprite.spriteName = HelpersGfx.GetSurvivorEventIconName(equipmentDefinition2.SurvivorClass.ToString());
			HelpersUI.SetContentToLabel(weaponLabel, HelpersLocalization.GetEquipmentName(equipmentDefinition2.ID));
			HelpersUI.SetContentToLabel(weaponRangeLabel, LocalizationManager.GetText("AbilityRange.Base.Desc", abilityDefinition.AbilityRange.ToString()));
			HelpersUI.SetContentToLabel(weaponRadiusLabel, HelpersLocalization.GetWeaponAreaDesc(abilityDefinition));
			HelpersUI.SetContentToLabel(weaponDesLabel, LocalizationManager.GetText(abilityDefinition.SpecialDescriptionKey));
			weaponScrollBar.value = 0f;
			break;
		}
		case PlayerHubActivityType.Armor:
		{
			bgTexture.alpha = 0.7f;
			Helpers.GameObjectSetActive(texture.gameObject, value: true);
			EquipmentDefinition equipmentDefinition = GameManager.Instance.gameEconomyData.GetEquipmentDefinition(info.FunctionValue);
			texture.mainTexture = HelpersGfx.GetEquipmentIconTextureFromID(equipmentDefinition.ID);
			armorTypeSprite.spriteName = HelpersGfx.GetSurvivorEventIconName(equipmentDefinition.SurvivorClass.ToString());
			HelpersUI.SetContentToLabel(armorLabel, HelpersLocalization.GetEquipmentName(equipmentDefinition.ID));
			HelpersUI.SetContentToLabel(armorDesLabel, LocalizationManager.GetText(equipmentDefinition.SpecialTrait));
			armorScrollBar.value = 0f;
			break;
		}
		case PlayerHubActivityType.Text:
		{
			bgTexture.alpha = 0.59f;
			Helpers.GameObjectSetActive(texture.gameObject, value: false);
			Helpers.GameObjectSetActive(timeLabel.gameObject, value: false);
			string[] array = info.FunctionValue.Split(";");
			HelpersUI.SetContentToLabel(textTitleLabel, LocalizationManager.GetText(array[0]));
			HelpersUI.SetContentToLabel(textLabel, LocalizationManager.GetText(array[1]));
			textScrollBar.value = 0f;
			break;
		}
		}
	}

	private void SetEmptyPanel()
	{
		bgTexture.mainTexture = null;
		Material material = AssetBundleManager.Instance.LoadAsset<Material>("Ui_ActiveInfor_InfoBg", "uimaterials");
		if (material != null && bgTexture != null)
		{
			bgTexture.material = material;
			bgTexture.alpha = 0.59f;
		}
		GameObject[] panels = _panels;
		for (int i = 0; i < panels.Length; i++)
		{
			Helpers.GameObjectSetActive(panels[i], value: false);
		}
		Helpers.GameObjectSetActive(emptyPanel.gameObject, value: true);
		Helpers.GameObjectSetActive(texture.gameObject, value: false);
		Helpers.GameObjectSetActive(timeLabel.gameObject, value: false);
		Helpers.GameObjectSetActive(leftButton.gameObject, value: false);
		Helpers.GameObjectSetActive(rightButton.gameObject, value: false);
	}

	private void SetActivePanel(PlayerHubActivityType type)
	{
		for (int i = 1; i < 6; i++)
		{
			if (type == (PlayerHubActivityType)i)
			{
				Helpers.GameObjectSetActive(_panels[i - 1], value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(_panels[i - 1], value: false);
			}
		}
	}

	private void SetPortrait(BounsInfoDefinition bounsDefinition)
	{
		if (ownerPortrait == null || partnerParent == null)
		{
			return;
		}
		ClearPortraitList();
		ownerPortrait.Init(bounsDefinition.Owner);
		string partner = bounsDefinition.Partner;
		if (string.IsNullOrEmpty(partner))
		{
			return;
		}
		string[] array = partner.Split(';');
		for (int i = 0; i < array.Length; i++)
		{
			string heroId = array[i];
			GameObject gameObject = Helpers.InstantiateToParent(ownerPortrait.gameObject, partnerParent);
			_portraitList.Add(gameObject);
			if (gameObject != null)
			{
				BounsPortrait component = gameObject.GetComponent<BounsPortrait>();
				if (component != null)
				{
					component.Init(heroId);
					component.transform.localPosition = new Vector3((float)i * 60f, 0f, 0f);
				}
			}
		}
	}

	public void OnLeftButtonClicked()
	{
		if (!string.IsNullOrEmpty(activityList.selectedCard.Item.Button1))
		{
			JumpPanel(activityList.selectedCard.Item.Button1);
			ActiveInformationDefinition item = activityList.selectedCard.Item;
			SingularityMonoBehaviour<SDKManager>.Instance.ActivityJump(item.Button1, item.Type, item.FunctionValue);
		}
	}

	public void OnRightButtonClicked()
	{
		if (!string.IsNullOrEmpty(activityList.selectedCard.Item.Button2))
		{
			JumpPanel(activityList.selectedCard.Item.Button2);
			ActiveInformationDefinition item = activityList.selectedCard.Item;
			SingularityMonoBehaviour<SDKManager>.Instance.ActivityJump(item.Button2, item.Type, item.FunctionValue);
		}
	}

	public void OnShowEquipButtonClicked()
	{
		if (activityList.selectedCard.Item.Type == 3 || activityList.selectedCard.Item.Type == 4)
		{
			EquipmentUpgradePopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
			obj.ShowNextLevel = false;
			obj.OpenForPreview(activityList.selectedCard.Item.FunctionValue, 5);
		}
	}

	private void JumpPanel(string jumpID)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.PlayerHubPopup);
		switch (jumpID)
		{
		case "1001":
		{
			CampHUD.HandleClickTrainingGround();
			SurvivorManagementPopUp obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampTrainingGrounds) as SurvivorManagementPopUp;
			GameObject filterButton = obj.transform.FindInChildren("Hero").gameObject;
			obj.survivorClassFilter.OnFilterClicked(filterButton);
			obj.survivorClassFilter.GetUITabs().SelectTab(1);
			string id = null;
			if (activityList.selectedCard.Item.Type == 1)
			{
				id = activityList.selectedCard.Item.FunctionValue;
			}
			else if (activityList.selectedCard.Item.Type == 2)
			{
				int.TryParse(activityList.selectedCard.Item.FunctionValue, out var result);
				id = GameManager.Instance.gameEconomyData.GetBounsInfo(result).Owner;
			}
			obj.survivorClassFilter.SurvivorList.ClickCardFromActorDefinitionID(id);
			break;
		}
		case "1002":
			NewPhonePopup.OpenRadiophoneFeaturePopup();
			break;
		case "2001":
			NewPhonePopup.OpenRadiophoneFeaturePopup();
			(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NewRadioPopup) as NewPhonePopup).OnClickWeapon();
			break;
		case "3001":
			if (TutorialView.Allowed(EventManager.EventTypeClick.Shop.ToString()))
			{
				EventManager.NotifyClick(EventManager.EventTypeClick.Shop);
				ShopPopupHelper.OpenWithIndex(0);
			}
			break;
		}
	}

	private void ClearPortraitList()
	{
		foreach (GameObject portrait in _portraitList)
		{
			if (portrait != null)
			{
				Object.Destroy(portrait);
			}
		}
		_portraitList.Clear();
	}

	private bool IsEpicMacAndBundleShop(string button2)
	{
		_ = button2 == "3001";
		return false;
	}
}
