using BaseModel;
using TWDModel;
using UnityEngine;

public class SurvivorUpgradePopup : HUDElement
{
	[SerializeField]
	private Transform survivorLocation;

	[Header("Survivor Info")]
	[SerializeField]
	private SurvivorCard survivorCard;

	[SerializeField]
	private UILabel survivorClassLabel;

	[SerializeField]
	private UILabel survivorRarityLabel;

	[Header("Equipment")]
	[SerializeField]
	private GameObject equipmentPrefab;

	[SerializeField]
	private GameObject weaponPosition;

	[SerializeField]
	private GameObject armorPosition;

	[SerializeField]
	private GameObject fakeCloseButton;

	private EquipmentButton weaponCard;

	private EquipmentButton armorCard;

	private Popup_LevelUp_Base popupLevelUpBase;

	public bool ShowNextLevel { get; set; }

	public bool IsAcceptingSurvivor { get; set; }

	public SurvivorModel Survivor { get; private set; }

	private void Awake()
	{
		popupLevelUpBase = GetComponent<Popup_LevelUp_Base>();
	}

	public override void OpenForModel(ModelObject model)
	{
		base.OpenForModel(model);
		UIEvent.OnUIEvent += OnUIEvent;
		AllowNormalClosing(active: true);
		Survivor = model as SurvivorModel;
		survivorCard.Item = Survivor;
		defaultPopup.SetActionButton(available: true, LocalizationManager.GetText("Popup.SurvivorLevelUp.Button.Demote"), OnDemote);
		popupLevelUpBase.ShowNextLevel = ShowNextLevel;
		popupLevelUpBase.Init(Survivor.SurvivorRarityLevel);
		UpgradePathData upgradePathData = new UpgradePathData();
		upgradePathData.StartLevel = Survivor.StartingLevel;
		upgradePathData.CurrentLevel = Survivor.Level;
		upgradePathData.MaxLevel = Survivor.MaxUpgradeLevel;
		upgradePathData.Survivor = Survivor;
		popupLevelUpBase.InitUpgradePath(upgradePathData);
		if (weaponCard == null)
		{
			weaponCard = Helpers.InstantiateToParent(equipmentPrefab, weaponPosition).GetComponent<EquipmentButton>();
		}
		if (armorCard == null)
		{
			armorCard = Helpers.InstantiateToParent(equipmentPrefab, armorPosition).GetComponent<EquipmentButton>();
		}
		weaponCard.SetupWeapon(Survivor);
		armorCard.SetupArmor(Survivor);
		UpdateUI();
	}

	public override void Close()
	{
		base.Close();
		IsAcceptingSurvivor = false;
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnDestroy()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (!IsAcceptingSurvivor && type == "SurvivorCardEquipmentClicked")
		{
			EquipmentButton equipmentButton = parameter as EquipmentButton;
			if (equipmentButton.GetEquipment() != null && survivorCard != null)
			{
				base.gameObject.GetComponent<EquipmentSelectionContainerView>().OpenForSurvivorCard(survivorCard, equipmentButton);
			}
			UpdateUI();
		}
	}

	public void AllowNormalClosing(bool active)
	{
		fakeCloseButton.SetActive(active);
		defaultPopup.AllowNormalClosing(active);
	}

	public void HideUpgradeSurvivorSpecifics()
	{
		defaultPopup.HideAllPayButtons();
		defaultPopup.ShowActionButton(show: false);
		defaultPopup.ShowLockedPanel(null);
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		int maxNumberOfUpgrades = Survivor.manager.Player.gameEconomyData.GetMaxNumberOfUpgrades(TWDModel.UpgradeType.SurvivorUpgrade);
		survivorCard.UpdateUI();
		if (Survivor != null)
		{
			if (weaponCard != null)
			{
				weaponCard.SetupWeapon(Survivor, useStatAddedToSurvivor: true);
			}
			if (armorCard != null)
			{
				armorCard.SetupArmor(Survivor, useStatAddedToSurvivor: true);
			}
		}
		if (Survivor.CanUpgrade)
		{
			defaultPopup.SetInstantPayButton(Survivor.GetUpgradeCashier(instantUpgrade: true, addInitialSurvivorPoints: true));
			defaultPopup.SetPayButton(LocalizationManager.GetText("Popup.SurvivorLevelUp.Button.LevelUp"), Survivor.GetUpgradeCashier(instantUpgrade: false), Survivor.UpgradeTime);
			defaultPopup.SetInstantPayButtonClickCallback(OnUpgradeSurvivorInstant);
			defaultPopup.SetPayButtonClickCallback(OnUpgradeSurvivor);
		}
		else
		{
			defaultPopup.HideInstantPayButton();
			defaultPopup.HidePayButton();
			defaultPopup.HideCannotPayButton();
		}
		survivorClassLabel.text = LocalizationManager.GetText("Popup.SurvivorLevelUp.Class{Name}", HelpersLocalization.GetSurvivorClassName(Survivor.SurvivorClass));
		survivorRarityLabel.text = LocalizationManager.GetText("Popup.SurvivorLevelUp.Rarity{Name}", HelpersLocalization.GetRarityLevel(Survivor.SurvivorRarityLevel));
		int startingLevel = Survivor.StartingLevel;
		int getTotalUpgrades = Survivor.GetTotalUpgrades;
		int damageForPreferredWeapon = Survivor.GetDamageForPreferredWeapon(addEquipmentValue: false);
		int damageForPreferredWeaponForLevel = Survivor.GetDamageForPreferredWeaponForLevel(Survivor.MaxUpgradeLevel, addEquipmentValue: false);
		popupLevelUpBase.SetDamagePanel(damageForPreferredWeapon, damageForPreferredWeaponForLevel, Survivor.Level, startingLevel, getTotalUpgrades, maxNumberOfUpgrades);
		int hitpointsForLevel = Survivor.GetHitpointsForLevel(Survivor.Level, addEquipmentValue: false);
		int hitpointsForLevel2 = Survivor.GetHitpointsForLevel(Survivor.MaxUpgradeLevel, addEquipmentValue: false);
		popupLevelUpBase.SetHealthPanel(hitpointsForLevel, hitpointsForLevel2, Survivor.Level, startingLevel, getTotalUpgrades, maxNumberOfUpgrades);
		if (IsAcceptingSurvivor)
		{
			defaultPopup.ShowPayButtons();
			defaultPopup.HideInstantPayButton();
			defaultPopup.HidePayButton();
			defaultPopup.HideCannotPayButton();
		}
		else
		{
			SurvivorUpgradeDefinition nextUpgradeDefinition = Survivor.NextUpgradeDefinition;
			TrainingGroundBuildingModel trainingGroundBuildingModel = GameManager.Instance.playerModel.Camp.GetBuilding("TrainingGround") as TrainingGroundBuildingModel;
			bool flag = trainingGroundBuildingModel?.IsUpgrading ?? false;
			bool flag2 = trainingGroundBuildingModel != null && (trainingGroundBuildingModel.UpgradingSurvivor != null || trainingGroundBuildingModel.UpgradedUnseenModel != null);
			bool canUpgrade = Survivor.CanUpgrade;
			bool num = !flag2 && !flag && canUpgrade && !IsAcceptingSurvivor;
			if (ShowNextLevel && nextUpgradeDefinition != null)
			{
				popupLevelUpBase.SetNextDamageValue(Survivor.GetDamageForPreferredWeaponForLevel(Survivor.Level + 1, addEquipmentValue: false) - damageForPreferredWeapon);
				popupLevelUpBase.SetNextHealthValue(Survivor.GetHitpointsForLevel(Survivor.Level + 1, addEquipmentValue: false) - hitpointsForLevel);
			}
			if (num)
			{
				defaultPopup.ShowLockedPanel(null);
				defaultPopup.ShowPayButtons();
			}
			else
			{
				string text = null;
				if (!canUpgrade)
				{
					if (Survivor.HasReachedMaxLevel)
					{
						text = LocalizationManager.GetText("Popup.UpgradeSurvivor.TrainingComplete");
						popupLevelUpBase.HideNextDamage();
						popupLevelUpBase.HideNextHealth();
					}
					else
					{
						text = LocalizationManager.GetText("Popup.UpgradeSurvivor.TrainingGroundLevelRequired{Level}", nextUpgradeDefinition.TrainingGroundLevel);
					}
				}
				else if (flag2)
				{
					text = LocalizationManager.GetText("Popup.UpgradeSurvivor.SurvivorUpgrading");
				}
				else if (flag)
				{
					text = LocalizationManager.GetText("Popup.UpgradeSurvivor.TrainingGoundsUpgrading");
				}
				defaultPopup.HideAllPayButtons();
				if (text != null)
				{
					defaultPopup.ShowLockedPanel(text);
				}
			}
		}
		defaultPopup.SetActionButton(!Survivor.IsUpgrading(), LocalizationManager.GetText("Popup.SurvivorLevelUp.Button.Demote"), OnDemote);
		popupLevelUpBase.UpdateUpgradePath(Survivor.Level);
	}

	public void OnUpgradeSurvivorInstant()
	{
		if (Survivor.CanUpgrade)
		{
			ConsumeCurrencyCommandUtils.Execute(new UpgradeSurvivorCommand(Survivor)
			{
				Instant = true,
				Cashier = Survivor.GetUpgradeCashier(instantUpgrade: true, addInitialSurvivorPoints: true)
			}, InstantUpgradeCallback);
		}
	}

	public void InstantUpgradeCallback(TWDModelResult result)
	{
		if (result != TWDModelResult.Cancelled)
		{
			Helpers.ExecuteCommand(new UpgradedModelViewedCommand(Survivor.manager.Player.Camp.GetBuilding("TrainingGround") as TrainingGroundBuildingModel));
			UpdateUI();
			UIEvent.Send("OnSurvivorInstantUpgraded", Survivor);
		}
	}

	public void OnUpgradeSurvivor()
	{
		if (Survivor.CanUpgrade)
		{
			ConsumeCurrencyCommandUtils.Execute(new UpgradeSurvivorCommand(Survivor)
			{
				Instant = false,
				Cashier = Survivor.GetUpgradeCashier(instantUpgrade: false)
			});
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_accept");
		UIEvent.Send("OnSurvivorUpgradeStarted", Survivor);
		Close();
	}

	public void OnDemote()
	{
		if (GameManager.Instance.playerModel.SurvivorContainer.Survivors.Count <= 3)
		{
			AlertPopup.ShowPopupGetText("Generic.Info", "Popup.DemoteConfirmation.NotEnoughSurvivor", "Button.Ok", null);
			return;
		}
		if (GameManager.Instance.playerModel.SurvivorContainer.IsOutpostDefending(Survivor))
		{
			AlertPopup.ShowPopupGetText("Generic.Info", "Popup.DemoteConfirmation.CannotDemoteOutpostDefender", "Button.Ok", null);
			return;
		}
		ConfirmationPopup confirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
		confirmationPopup.SetContent(LocalizationManager.GetText("Popup.DemoteConfirmation.Title{Name}", Survivor.Name), LocalizationManager.GetText("Popup.DemoteConfirmation.Message{Name}", Survivor.Name));
		confirmationPopup.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
		confirmationPopup.SetCurrencies(Survivor.GetDemoteCashier());
		confirmationPopup.SetCallbacks(OnDemoteConfirmed);
		confirmationPopup.Open();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	private void OnDemoteConfirmed()
	{
		Cashier demoteCashier = Survivor.GetDemoteCashier();
		if (CampView.Instance != null && CampView.Instance.BuildingsHud != null)
		{
			CampView.Instance.BuildingsHud.CreateCollectAnim(demoteCashier);
		}
		Helpers.ExecuteCommand(new DemoteSurvivorCommand(Survivor));
		UIEvent.Send("SurvivorDeleted", Survivor);
		SurvivorInfoPopup.HandleSurvivorUpgradeViewed(Survivor);
		Close();
	}
}
