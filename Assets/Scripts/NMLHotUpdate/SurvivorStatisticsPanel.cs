using System.Collections.Generic;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class SurvivorStatisticsPanel : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Survivior Class Icon")]
	private UISprite uiClassIcon;

	[SerializeField]
	[Tooltip("Survivior Level Text Label")]
	private UILabel labelTextLevel;

	[SerializeField]
	[Tooltip("Survivior Level Label")]
	private UILabel labelLevel;

	[SerializeField]
	[Tooltip("has better weapon thingy")]
	private GameObject betterWeapons;

	[SerializeField]
	[Tooltip("has better armor thingy")]
	private GameObject betterArmor;

	[SerializeField]
	[Tooltip("Survivior Damage Prefab")]
	private SurvivorDamageHealthPanel damagePanel;

	[SerializeField]
	[Tooltip("Survivior Health Prefab")]
	private SurvivorDamageHealthPanel healthPanel;

	[SerializeField]
	[Tooltip("Survivior Class Label")]
	private UILabel labelClass;

	[SerializeField]
	private SurvivorStatusPanel statusPanel;

	[Header("Featured Hero Elements")]
	[SerializeField]
	private GameObject featuredHeroContainer;

	[SerializeField]
	private UILabel featuredHeroDamageBonusLabel;

	[SerializeField]
	private UILabel featuredHeroHealthBonusLabel;

	[Header("Equipment Card")]
	[SerializeField]
	private GameObject equipmentPrefab;

	[SerializeField]
	private GameObject weaponPosition;

	[SerializeField]
	private GameObject armorPosition;

	private SurvivorModel survivorModel;

	private EquipmentButton weaponCard;

	private EquipmentButton armorCard;

	public bool IsLimited { get; set; }

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "EquipmentStartUpgrade":
		case "EquipmentInstantUpgraded":
		case "EquipmentUpgraded":
		case "OnEquipmentUpdated":
		case "BounsEquip":
		case "BreakThroughed":
		case "EquipmentRemodelSelectioned":
			if (weaponCard != null && weaponCard.GetOwningSurvivor() != null)
			{
				SetInfo(weaponCard.GetOwningSurvivor());
			}
			break;
		}
		if (type == "OnClickedShare")
		{
			statusPanel.gameObject.SetActive(value: false);
		}
	}

	public static bool CheckForWeaponUpgrades(SurvivorModel survivor)
	{
		List<EquipmentItemModel> list = new List<EquipmentItemModel>();
		list.AddRange(GameManager.Instance.playerModel.Equipment.MeleeWeapons);
		list.AddRange(GameManager.Instance.playerModel.Equipment.RangeWeapons);
		int num = 0;
		int count = survivor.EquipmentItems.Count;
		for (int i = 0; i < count; i++)
		{
			EquipmentItemModel equipmentItemModel = survivor.EquipmentItems[i];
			if (equipmentItemModel.IsWeaponEquipment && equipmentItemModel.GetDamageForLevel(equipmentItemModel.Level) > num)
			{
				num = equipmentItemModel.GetDamageForLevel(equipmentItemModel.Level);
			}
		}
		bool flag = false;
		foreach (EquipmentItemModel item in list)
		{
			flag = item.StartingLevel > survivor.Level;
			if (item.Definition.CanBeEquippedBySurvivorClass(survivor.SurvivorClass) && item.CanBeManipulated() && item.GetOwnerForClient() == null && item.IsWeaponEquipment && !flag && item.GetDamageForLevel(item.Level) > num)
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckForArmorUpgrades(SurvivorModel survivor)
	{
		List<EquipmentItemModel> list = new List<EquipmentItemModel>();
		list.AddRange(GameManager.Instance.playerModel.Equipment.Armors);
		int num = 0;
		int count = survivor.EquipmentItems.Count;
		for (int i = 0; i < count; i++)
		{
			EquipmentItemModel equipmentItemModel = survivor.EquipmentItems[i];
			if (equipmentItemModel.Definition != null && equipmentItemModel.Definition.Category == EquipmentCategory.Armor && equipmentItemModel.GetDefenseForLevel(equipmentItemModel.Level) > num)
			{
				num = equipmentItemModel.GetDefenseForLevel(equipmentItemModel.Level);
			}
		}
		bool flag = false;
		foreach (EquipmentItemModel item in list)
		{
			flag = item.StartingLevel > survivor.Level;
			if (item.Definition.CanBeEquippedBySurvivorClass(survivor.SurvivorClass) && item.CanBeManipulated() && item.GetOwnerForClient() == null && !flag && item.Definition != null && item.Definition.Category == EquipmentCategory.Armor && item.GetDefenseForLevel(item.Level) > num)
			{
				return true;
			}
		}
		return false;
	}

	public void SetInfo(SurvivorModel model, bool allowWeaponEquipmentHint = true, bool showEquipmentLockedState = true)
	{
		if (statusPanel != null)
		{
			statusPanel.SetInfo(model);
		}
		if (betterWeapons != null)
		{
			betterWeapons.SetActive(allowWeaponEquipmentHint && CheckForWeaponUpgrades(model));
		}
		if (betterArmor != null)
		{
			betterArmor.SetActive(allowWeaponEquipmentHint && CheckForArmorUpgrades(model));
		}
		if (labelTextLevel != null)
		{
			labelTextLevel.text = LocalizationManager.GetText("Popup.SurvivorInfoPopup.Level");
		}
		if (uiClassIcon != null)
		{
			HelpersUI.SetSprite(uiClassIcon, HelpersGfx.GetSurvivorClassIconName(model));
		}
		if (labelLevel != null && model != null)
		{
			labelLevel.text = model.Level.ToString();
		}
		float num = model.GetDamageForPreferredWeapon();
		float num2 = model.GetHitpoints();
		SurvivalManualManager survivalManualManager = GameManager.Instance.playerModel.SurvivalManualManager;
		if (survivalManualManager != null)
		{
			num = num / 100f * (float)(100 + survivalManualManager.GetPrivateAttackRatioClient(model) + survivalManualManager.GetAttributeAttackRatioClient()) + (float)survivalManualManager.GetAttackClinet(model);
			num2 = num2 / 100f * (float)(100 + survivalManualManager.GetPrivateHpRatioClient(model) + survivalManualManager.GetAttributeHpRatioClient()) + (float)survivalManualManager.GetHPClinet(model);
		}
		bool value = false;
		int num3 = 0;
		int num4 = 0;
		FeaturedHeroDefinition featuredDefinition = model.FeaturedDefinition;
		if (featuredDefinition != null)
		{
			value = !IsLimited;
			num3 = (int)(num2 * ((float)featuredDefinition.HealthBoostMultiplier / 100f));
			num4 = (int)(num * ((float)featuredDefinition.DamageBoostMultiplier / 100f));
		}
		if (damagePanel != null && model != null)
		{
			string amount = model.GetCommonDamage().ToString();
			string baseAmount = model.GetDamageForPreferredWeapon(addEquipmentValue: false).ToString() ?? "";
			damagePanel.setInfo(LocalizationManager.GetText("Statistic.Damage"), amount, LocalizationManager.GetText("Popup.SurvivorInfoPopup.Base"), baseAmount, model.GetWeaponEquipment());
		}
		if (healthPanel != null && model != null)
		{
			string amount2 = model.GetCommonHealth().ToString();
			string baseAmount2 = model.GetHitpointsForLevel(model.Level, addEquipmentValue: false).ToString() ?? "";
			healthPanel.setInfo(LocalizationManager.GetText("Statistic.Health"), amount2, LocalizationManager.GetText("Popup.SurvivorInfoPopup.Base"), baseAmount2, model.GetEquipmentOfCategory(EquipmentCategory.Armor));
		}
		HelpersUI.SetContentToLabel(featuredHeroHealthBonusLabel, "+" + num3, num3 > 0);
		HelpersUI.SetContentToLabel(featuredHeroDamageBonusLabel, "+" + num4, num4 > 0);
		Helpers.GameObjectSetActive(featuredHeroContainer, value);
		if (IsLoadDataManager)
		{
			var go = featuredHeroContainer.transform.GetChild(2).gameObject;
			DebugTWD.Log("Скрываем объект: " + go.name);
			//Helpers.GameObjectSetActive(go, false);
		}

		if ((bool)labelClass)
		{
			labelClass.text = HelpersLocalization.GetSurvivorClassName(model.SurvivorClass) + " " + HelpersLocalization.GetRarityLevel(model.SurvivorRarityLevel);
			labelClass.gradientTop = GameManager.Instance.GetRarityColorData(model.SurvivorRarityLevel).GradientColorTop;
			labelClass.gradientBottom = GameManager.Instance.GetRarityColorData(model.SurvivorRarityLevel).GradientColorBottom;
		}
		survivorModel = model;
		if (weaponCard == null)
		{
			weaponCard = Helpers.InstantiateToParentAndLayer(equipmentPrefab, weaponPosition).GetComponent<EquipmentButton>();
		}
		if (armorCard == null)
		{
			armorCard = Helpers.InstantiateToParentAndLayer(equipmentPrefab, armorPosition).GetComponent<EquipmentButton>();
		}
		if (weaponCard != null && armorCard != null)
		{
			weaponCard.SetupWeapon(model, useStatAddedToSurvivor: false, "SurvivorCardEquipmentClicked", showEquipmentLockedState);
			armorCard.SetupArmor(model, useStatAddedToSurvivor: false, "SurvivorCardEquipmentClicked", showEquipmentLockedState);
			if (GameManager.Instance.playerModel.SurvivorContainer.ContainsSurvivor(model))
			{
				weaponCard.AllowUpgradeIndicator(value: true);
				armorCard.AllowUpgradeIndicator(value: true);
			}
			else if (IsLoadDataManager && !DataManager.Instance.SurvivorManagementPopUp.gameObject.activeSelf)
			{
				weaponCard.IsForProtectors = true;
				armorCard.IsForProtectors = true;
			}
		}
	}

	public void HideFeaturedHeroContainer()
	{
		Helpers.GameObjectSetActive(featuredHeroContainer, value: false);
	}

	public void OpenWeaponInfoPopup()
	{
		OpenEquipmentInfoPopup(weaponCard);
	}

	public void OpenArmorInfoPopup()
	{
		OpenEquipmentInfoPopup(armorCard);
	}

	private void OpenEquipmentInfoPopup(EquipmentButton equipmentButton)
	{
		EquipmentItemModel equipment = equipmentButton.GetEquipment();
		if (equipment != null)
		{
			bool flag = GameManager.Instance.playerModel.SurvivorContainer.Survivors.Contains(survivorModel);
			EquipmentUpgradePopup equipmentUpgradePopup = ((!flag) ? Helpers.OpenEquipmentUpgradePopupPreview(equipment.Definition, equipment.RarityLevel) : Helpers.OpenEquipmentUpgradePopup(equipmentButton.GetEquipment()));
			if (!(equipmentUpgradePopup == null))
			{
				equipmentUpgradePopup.ShowNextLevel = flag;
				equipmentUpgradePopup.EnableOwnCloseArea(!flag);
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
			}
		}
	}


	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	#endregion

	#region mycode
	public int GetStrengthValue(SurvivorModel model)
	{
		int damageForPreferredWeapon = model.GetDamageForPreferredWeapon();
		int num2 = 0;
		FeaturedHeroDefinition featuredDefinition = model.FeaturedDefinition;
		if (featuredDefinition != null)
		{
			num2 = (int)((float)damageForPreferredWeapon * ((float)featuredDefinition.DamageBoostMultiplier / 100f));
		}
		return damageForPreferredWeapon + num2;
	}
	#endregion
}
