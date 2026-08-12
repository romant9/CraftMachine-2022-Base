using System.Collections.Generic;
using BaseModel;
using BaseModel.ContentTypes;
using TWDModel;
using UnityEngine;

public class BuildingMenu : HUDElement
{
	[SerializeField]
	private float buttonSpacing;

	[SerializeField]
	private UIButton buttonCollectCurrency;

	[SerializeField]
	private UISprite buttonCollectCurrencySprite;

	[SerializeField]
	private UIButton buttonSpeedUp;

	[SerializeField]
	private UIButton buttonSpeedAdUp;

	[SerializeField]
	private UIButton buttonUpgrade;

	[SerializeField]
	private UIButton buttonSurvivorView;

	[SerializeField]
	private UIButton buttonWorkshopView;

	[SerializeField]
	private UIButton buttonResidenceView;

	[SerializeField]
	private UIButton buttonGraveyardView;

	[SerializeField]
	private UIButton buttonRadioTentView;

	[SerializeField]
	private UIButton buttonMedicTent;

	[SerializeField]
	private UIButton buttonCage;

	[SerializeField]
	private UIButton buttonOutpost;

	[SerializeField]
	private UIButton buttonInfo;

	[SerializeField]
	private UIButton buttonCancelUpgrade;

	[SerializeField]
	private UIButton buttonConfirmConstruction;

	[SerializeField]
	private UIButton buttonCancelConstruction;

	[SerializeField]
	private UIButton buttonCutVegetation;

	[SerializeField]
	private UILabel topicName;

	[SerializeField]
	private UILabel topicLevel;

	[SerializeField]
	[Tooltip("The game object containing the level background & the level text.")]
	private GameObject levelGameObject;

	[SerializeField]
	private GameObject speedUpTokenContainer;

	[SerializeField]
	private GameObject speedUpDiamondContainer;

	[SerializeField]
	private UILabel speedUpDiamondAmountLabel;

	[SerializeField]
	private UILabel speedUpTokenAmountLabel;

	[SerializeField]
	private UILabel speedUpDiamondsOnlyAmountLabel;

	[SerializeField]
	private UISprite speedUpTokenIcon;

	[SerializeField]
	private UILabel speedUpRadioTentDiamondAmountLabel;

	[SerializeField]
	private BoxCollider speedUpButtonCollider;

	[Header("Box Collider Dimensions")]
	[Tooltip("In case using Speedup tokens are possible")]
	[SerializeField]
	private Vector3 speedUpInteractionCenter;

	[SerializeField]
	private Vector3 speedUpInteractionSize;

	[SerializeField]
	[Tooltip("In case using Speedup tokens are impossible")]
	private Vector3 noSpeedUpInteractionCenter;

	[SerializeField]
	private Vector3 noSpeedUpInteractionSize;

	private BuildingModel building;

	private bool isMovingBuilding;

	public BuildingView BuildingView { get; set; }

	public override void Open()
	{
		base.Open();
		SetBuilding(GetModel<BuildingModel>());
		SetupButtons();
	}

	public override void Close()
	{
		base.Close();
		SetBuilding(null);
		isMovingBuilding = false;
	}

	public void SetBuilding(BuildingModel newBuilding)
	{
		isMovingBuilding = false;
		if (building != null)
		{
			building.Changed -= OnModelChange;
		}
		building = newBuilding;
		if (building != null)
		{
			building.Changed += OnModelChange;
			SetupButtons();
			UpdateGUI();
		}
	}

	public void SetMovingBuildingButtons()
	{
		base.Open();
		isMovingBuilding = true;
		SetupButtons();
		UpdateGUI();
	}

	protected void OnModelChange(ModelObject m, string changed, object args)
	{
		if (building != null)
		{
			if (changed == "UpgradingItemReady")
			{
				Close();
				CampView.Instance.CampViewBuildings.UnselectBuilding();
			}
			else if (changed == "RemoveBuilding" && m == building)
			{
				Close();
			}
			else
			{
				SetupButtons();
				UpdateGUI();
			}
		}
	}

	protected void UpdateGUI()
	{
		string text = "";
		if (building != null)
		{
			text = building.TypeName;
		}
		else if (BuildingView != null)
		{
			text = BuildingView.BuildingType;
		}
		topicName.text = LocalizationManager.GetText("Building.Name." + text);
		topicLevel.text = HelpersBuilding.GetLocalizedBuildingLevel(building);
		if (building == null)
		{
			return;
		}
		levelGameObject.SetActive(!(building is BuffBuildingModel) && !(building is VegetationModel));
		if (!building.NeedSpeedUpButton())
		{
			return;
		}
		Cashier cashier = null;
		Cashier cashier2 = null;
		CurrencyType currencyType = CurrencyType.None;
		GetColliderSizeForTokens(isTokenEnabled: true);
		if (building.IsUpgrading)
		{
			cashier = building.GetSpeedUpUpgradeCashier();
			cashier2 = building.GetSpeedUpUpgradeCashierWithTokens();
			currencyType = CurrencyType.BuildingTokenBP;
			Helpers.GameObjectSetActive(speedUpTokenContainer, value: true);
			Helpers.GameObjectSetActive(speedUpDiamondContainer, value: false);
		}
		else if (building is WorkshopBuildingModel && ((WorkshopBuildingModel)building).UpgradingEquipment != null)
		{
			cashier = ((WorkshopBuildingModel)building).UpgradingEquipment.TimedActionModel.GetSpeedUpCashier();
			cashier2 = ((WorkshopBuildingModel)building).UpgradingEquipment.TimedActionModel.GetSpeedUpCashierWithTokens(CurrencyType.EquipmentTokenBP);
			currencyType = CurrencyType.EquipmentTokenBP;
			Helpers.GameObjectSetActive(speedUpTokenContainer, value: true);
			Helpers.GameObjectSetActive(speedUpDiamondContainer, value: false);
		}
		else if (building is TrainingGroundBuildingModel && ((TrainingGroundBuildingModel)building).UpgradingSurvivor != null)
		{
			cashier = ((TrainingGroundBuildingModel)building).UpgradingSurvivor.TimedActionModel.GetSpeedUpCashier();
			cashier2 = ((TrainingGroundBuildingModel)building).UpgradingSurvivor.TimedActionModel.GetSpeedUpCashierWithTokens(CurrencyType.TrainingTokenBP);
			currencyType = CurrencyType.TrainingTokenBP;
			Helpers.GameObjectSetActive(speedUpTokenContainer, value: true);
			Helpers.GameObjectSetActive(speedUpDiamondContainer, value: false);
		}
		else if (building is CageBuildingModel && ((CageBuildingModel)building).UpgradingWalker != null)
		{
			cashier = ((CageBuildingModel)building).UpgradingWalker.TimedActionModel.GetSpeedUpCashier();
			Helpers.GameObjectSetActive(speedUpTokenContainer, value: false);
			Helpers.GameObjectSetActive(speedUpDiamondContainer, value: true);
			GetColliderSizeForTokens(isTokenEnabled: false);
		}
		if (cashier != null)
		{
			int totalCost = cashier.GetTotalCost(CurrencyType.Diamonds);
			if (totalCost == 0)
			{
				speedUpDiamondAmountLabel.text = LocalizationManager.GetText("Generic.Free");
				speedUpDiamondsOnlyAmountLabel.text = LocalizationManager.GetText("Generic.Free");
			}
			else
			{
				speedUpDiamondAmountLabel.text = totalCost.ToString();
				speedUpDiamondsOnlyAmountLabel.text = totalCost.ToString();
			}
		}
		if (cashier2 != null && currencyType != CurrencyType.None)
		{
			speedUpTokenIcon.spriteName = HelpersGfx.GetCurrencyIconName(HelpersGfx.GetSPCurrencyType_N(currencyType));
		}
	}

	private void SetupButtons()
	{
		Helpers.GameObjectSetActive(buttonCollectCurrency, value: false);
		Helpers.GameObjectSetActive(buttonInfo, value: false);
		Helpers.GameObjectSetActive(buttonSpeedUp, value: false);
		Helpers.GameObjectSetActive(buttonSpeedAdUp, value: false);
		Helpers.GameObjectSetActive(buttonUpgrade, value: false);
		Helpers.GameObjectSetActive(buttonSurvivorView, value: false);
		Helpers.GameObjectSetActive(buttonWorkshopView, value: false);
		Helpers.GameObjectSetActive(buttonResidenceView, value: false);
		Helpers.GameObjectSetActive(buttonGraveyardView, value: false);
		Helpers.GameObjectSetActive(buttonRadioTentView, value: false);
		Helpers.GameObjectSetActive(buttonMedicTent, value: false);
		Helpers.GameObjectSetActive(buttonCage, value: false);
		Helpers.GameObjectSetActive(buttonOutpost, value: false);
		Helpers.GameObjectSetActive(buttonCancelUpgrade, value: false);
		Helpers.GameObjectSetActive(buttonCancelConstruction, value: false);
		Helpers.GameObjectSetActive(buttonConfirmConstruction, value: false);
		Helpers.GameObjectSetActive(buttonCutVegetation, value: false);
		if (TutorialView.Instance == null || (building == null && !isMovingBuilding))
		{
			return;
		}
		List<UIButton> list = new List<UIButton>();
		if (isMovingBuilding)
		{
			list.Add(buttonCancelConstruction);
			list.Add(buttonConfirmConstruction);
		}
		else
		{
			bool flag = building.NeedSpeedUpButton();
			if (flag)
			{
				if (!TutorialView.Instance.Running)
				{
					list.Add(buttonCancelUpgrade);
					if (GameManager.Instance.gameEconomyData.ConfigData.AdsBuildingSpeedUpEnabled)
					{
						bool num = building.IsUpgradingEquipment() || building.IsUpgradingWalker() || building.IsUpgradingSurvivor();
						bool flag2 = SingularityMonoBehaviour<VideoAdManager>.Instance.IsVideoReadyForServe(AdUsage.BuildUpgradeSpeedUp);
						_ = !num && !building.SpeedUpByAd && flag2;
					}
				}
				list.Add(buttonSpeedUp);
			}
			else
			{
				list.Add(buttonInfo);
				if (!building.BuildingType.DisableUpgrade)
				{
					list.Add(buttonUpgrade);
				}
				if (building.Producer != null)
				{
					list.Add(buttonCollectCurrency);
					HelpersUI.SetSprite(buttonCollectCurrencySprite, HelpersGfx.GetCurrencyIconName(building.Producer.CurrencyType));
				}
			}
			if (building.BuildingRepaired)
			{
				if (building.BuildingType.Name == "Graveyard" && !flag)
				{
					list.Add(buttonGraveyardView);
				}
				if (building.BuildingType.Name == "RadioTent" && !flag)
				{
					if (!TutorialView.Instance.Running || TutorialView.Instance.Model.HasCompletedPart("Phone"))
					{
						list.Add(buttonRadioTentView);
					}
				}
				else if (building.BuildingType.Name == "Workshop")
				{
					list.Add(buttonWorkshopView);
				}
				else if (building.BuildingType.Name == "MedicTent")
				{
					list.Add(buttonMedicTent);
				}
				else if (building.BuildingType.Name == "TrainingGround")
				{
					if (!TutorialView.Instance.Running || !flag)
					{
						list.Add(buttonSurvivorView);
					}
				}
				else if (building.BuildingType.Name == "Cage")
				{
					list.Add(buttonCage);
				}
				else if (building.BuildingType.Name == "Outpost")
				{
					list.Add(buttonOutpost);
				}
				else if (building.BuildingType.Name == "Residence")
				{
					list.Add(buttonResidenceView);
				}
				else if (building is VegetationModel)
				{
					list.Remove(buttonInfo);
					VegetationModel vegetationModel = (VegetationModel)building;
					int councilLevel = GameManager.Instance.playerModel.Camp.GetCouncilLevel();
					if (!vegetationModel.IsBeingCut && vegetationModel.CanBeCutAt(councilLevel))
					{
						list.Add(buttonCutVegetation);
					}
				}
			}
		}
		float num2 = -0.5f * buttonSpacing * (float)(list.Count - 1);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null && list[i].gameObject != null)
			{
				Helpers.GameObjectSetActive(list[i], value: true);
				list[i].gameObject.transform.localPosition = new Vector3(num2, 0f, -1f);
				num2 += buttonSpacing;
			}
		}
	}

	public void OnUpgradeBuilding()
	{
		EventManager.NotifyClick("BuildingMenuUpgrade");
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampUpgradeBuildingPopup).OpenForModel(building);
		Close();
	}

	public void OnInfo()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampBuildingInfoPopup).OpenForModel(building);
		Close();
	}

	public void OnSpeedUpBuilding()
	{
		if (building.IsUpgrading)
		{
			ConsumeCurrencyCommandUtils.Execute(new FinishBuildingCommand(building)
			{
				Cashier = building.GetSpeedUpUpgradeCashier()
			});
		}
		else if (building is WorkshopBuildingModel)
		{
			WorkshopBuildingModel workshopBuildingModel = building as WorkshopBuildingModel;
			if (workshopBuildingModel.UpgradingModel != null)
			{
				EquipmentItemModel equipmentItemModel = workshopBuildingModel.UpgradingModel as EquipmentItemModel;
				ConsumeCurrencyCommandUtils.Execute(new SpeedUpUpgradeEquipmentCommand(equipmentItemModel)
				{
					Cashier = equipmentItemModel.TimedActionModel.GetSpeedUpCashier()
				});
			}
		}
		else if (building is TrainingGroundBuildingModel)
		{
			SurvivorModel upgradingSurvivor = ((TrainingGroundBuildingModel)building).UpgradingSurvivor;
			if (upgradingSurvivor != null)
			{
				ConsumeCurrencyCommandUtils.Execute(new SpeedUpUpgradeSurvivorCommand(upgradingSurvivor)
				{
					Cashier = upgradingSurvivor.TimedActionModel.GetSpeedUpCashier()
				});
			}
		}
		else if (building is CageBuildingModel)
		{
			OutpostWalkerModel upgradingWalker = ((CageBuildingModel)building).UpgradingWalker;
			if (upgradingWalker != null)
			{
				ConsumeCurrencyCommandUtils.Execute(new SpeedUpUpgradeOutpostWalkerCommand(upgradingWalker)
				{
					Cashier = upgradingWalker.TimedActionModel.GetSpeedUpCashier()
				});
			}
		}
		EventManager.NotifyClick("BuildingMenuSpeedUp");
		Close();
	}

	public void OnCancelUpgrade()
	{
		string text = null;
		if (building.IsUpgrading)
		{
			text = HelpersLocalization.GetBuildingName(building);
		}
		else if (building is WorkshopBuildingModel)
		{
			WorkshopBuildingModel workshopBuildingModel = building as WorkshopBuildingModel;
			if (workshopBuildingModel.UpgradingModel != null)
			{
				text = HelpersLocalization.GetEquipmentName(workshopBuildingModel.UpgradingModel as EquipmentItemModel);
			}
		}
		else if (building is TrainingGroundBuildingModel)
		{
			SurvivorModel upgradingSurvivor = ((TrainingGroundBuildingModel)building).UpgradingSurvivor;
			if (upgradingSurvivor != null)
			{
				text = upgradingSurvivor.Name;
			}
		}
		else if (building is CageBuildingModel)
		{
			OutpostWalkerModel upgradingWalker = ((CageBuildingModel)building).UpgradingWalker;
			if (upgradingWalker != null)
			{
				text = string.Format("{0} {1}", LocalizationManager.GetText("Walker.Class." + upgradingWalker.Id), LocalizationManager.GetText("Walker"));
			}
		}
		if (text != null)
		{
			ConfirmationPopup confirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
			confirmationPopup.SetContent(LocalizationManager.GetText("Popup.CancelUpgrade.Title"), LocalizationManager.GetText("Popup.CancelUpgrade.Description{ThingUpgraded}{Percentage}", text, GameManager.Instance.gameEconomyData.ConfigData.CancelUpgradeRefundPercentage));
			confirmationPopup.SetOkButtonLabel(LocalizationManager.GetText("Popup.CancelUpgrade.Button.Stop"));
			confirmationPopup.SetCancelButtonLabel(LocalizationManager.GetText("Button.Cancel"));
			confirmationPopup.SetCallbacks(OnCancelUpgradeConfirmed);
			confirmationPopup.Open();
		}
	}

	private void GetColliderSizeForTokens(bool isTokenEnabled)
	{
		speedUpButtonCollider.center = (isTokenEnabled ? speedUpInteractionCenter : noSpeedUpInteractionCenter);
		speedUpButtonCollider.size = (isTokenEnabled ? speedUpInteractionSize : noSpeedUpInteractionSize);
	}

	public void OnCancelUpgradeConfirmed()
	{
		if (building != null)
		{
			Helpers.ExecuteCommand(new CancelUpgradeCommand(building));
			EventManager.NotifyEvent(EventManager.EventType.CampVisualizationChanged);
		}
	}

	public void OnCollectBuilding()
	{
		TWDModelResult result = TWDModelResult.Error;
		if (BuildingView.Model.CanCollect)
		{
			result = BuildingView.Collect();
		}
		CollectIndicator.ShowCollectError(BuildingView.Model.Producer, result);
	}

	public void OnGraveyardView()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampGraveyardPopup).Open();
		Close();
	}

	public void OnWorkshopView()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampWorkshopPopup).Open();
		Close();
	}

	public void OnTrainingGroundView()
	{
		CampHUD.HandleClickTrainingGround();
		Close();
	}

	public void OnResidenceView()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampResidencePopup).Open();
	}

	public void OnConfirmConstruction()
	{
		UIEvent.Send("OnBuildingMoveConfirmed", building);
	}

	public void OnCancelConstruction()
	{
		UIEvent.Send("OnBuildingMoveCancelled", building);
	}

	public void OnMedicTent()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampMedicTent).Open();
		Close();
	}

	public void OnRadioTent()
	{
		Close();
		NewPhonePopup.OpenRadiophoneFeaturePopup();
	}

	public void OnCutVegetation()
	{
		string buildingName = HelpersLocalization.GetBuildingName(building);
		VegetationModel vegetationModel = building as VegetationModel;
		int councilLevel = GameManager.Instance.playerModel.Camp.GetCouncilLevel();
		if (vegetationModel == null || vegetationModel.CanBeCutAt(councilLevel))
		{
			ConfirmationPopup confirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
			confirmationPopup.SetContent(LocalizationManager.GetText("Popup.CutVegetation.Title{VegetationName}", buildingName), LocalizationManager.GetText("Popup.CutVegetation.Message{VegetationName}", buildingName));
			confirmationPopup.SetCurrencies(vegetationModel.GetCutCashier);
			confirmationPopup.SetCallbacks(OnCutVegetationConfirmed);
			confirmationPopup.Open();
		}
	}

	private void OnCutVegetationConfirmed()
	{
		VegetationModel vegetationModel = building as VegetationModel;
		ConsumeCurrencyCommandUtils.Execute(new CutVegetationCommand(vegetationModel)
		{
			Cashier = vegetationModel.GetCutCashier
		});
	}

	public void OnClickCage()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.PopupCage).Open();
	}

	public void OnClickOutpost()
	{
		CampHUD.TryOpenOutpostTutorial(CampHUD.OpenOutpostPopupAfterChecks);
	}

	private void OnNameSubmitComplete(UIType popupType)
	{
		if (popupType != UIType.None)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(popupType).Open();
		}
	}
}
