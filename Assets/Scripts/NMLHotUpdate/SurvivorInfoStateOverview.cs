using TWDModel;

public class SurvivorInfoStateOverview : SurvivorInfoStateBase
{
	private MedicTentModel medicTentModelCached;

	public override void Init()
	{
		base.Init();
		CurrentState = States.SurvivorOverview;
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		TrainingGroundBuildingModel trainingGroundBuildingModel = GameManager.Instance.playerModel.Camp.GetBuilding("TrainingGround") as TrainingGroundBuildingModel;
		bool flag = trainingGroundBuildingModel?.IsUpgrading ?? false;
		bool flag2 = trainingGroundBuildingModel != null && (trainingGroundBuildingModel.UpgradingSurvivor != null || trainingGroundBuildingModel.UpgradedUnseenModel != null);
		bool flag3 = base.SurvivorModel.CanUpgradeSurvivorRarity();
		bool flag4 = base.SurvivorModel.CanUpgradeTraitRarity();
		bool flag5 = base.SurvivorModel.GetUpgradeTraitCashier().CanAfford();
		bool flag6 = base.SurvivorModel.InjuryType != InjuryType.None;
		bool flag7 = !flag && !flag2;
		bool isEnabled = flag3 || flag4;
		bool flag8 = TeamSelectionPopup.GetSurvivorsForType(SurvivorContainerModel.SurvivorType.Outpost).Contains(base.SurvivorModel);
		if (base.OpenTrainButton != null && base.SpeedUpPayButton != null)
		{
			string text = "";
			if (base.SurvivorModel.IsUpgrading())
			{
				base.SpeedUpPayButton.UpdateUI(base.SurvivorModel.TimedActionModel.GetSpeedUpCashier(), LocalizationManager.GetText("Popup.MedicTent.Button.SpeedupOneSurvivor"));
				Helpers.GameObjectSetActive(base.SpeedUpPayButton, value: true);
			}
			else if (flag6 && base.MedicTent != null)
			{
				base.SpeedUpPayButton.UpdateUI(base.MedicTent.GetFinishOneCashier(base.SurvivorModel), LocalizationManager.GetText("Popup.MedicTent.Button.SpeedupOneSurvivor"));
				Helpers.GameObjectSetActive(base.SpeedUpPayButton, value: true);
			}
			else
			{
				GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
				Helpers.GameObjectSetActive(base.OpenTrainButton, value: true);
				base.OpenTrainButton.isEnabled = base.SurvivorModel.CanUpgrade && flag7 && !flag6 && (!flag8 || gameEconomyData.GetFeature("CanTrainOutpostSurvivor").Enabled);
				base.OpenTrainButton.IsVisuallyDisabled = !base.OpenTrainButton.isEnabled;
			}
			bool value = (base.OpenTrainButton.gameObject.activeSelf && !base.OpenTrainButton.isEnabled) || base.SpeedUpPayButton.gameObject.activeSelf;
			if (Helpers.GameObjectSetActive(base.TrainingLockedParent, value))
			{
				SurvivorUpgradeDefinition nextUpgradeDefinition = base.SurvivorModel.NextUpgradeDefinition;
				if (flag2 && !flag6 && !base.SurvivorModel.IsUpgrading())
				{
					text = LocalizationManager.GetText("Popup.UpgradeSurvivor.SurvivorUpgrading");
				}
				if (flag && !flag6)
				{
					text = LocalizationManager.GetText("Popup.UpgradeSurvivor.TrainingGoundsUpgrading");
				}
				else if (!base.SurvivorModel.CanUpgrade)
				{
					if (base.SurvivorModel.HasReachedMaxLevel)
					{
						text = LocalizationManager.GetText("Popup.UpgradeSurvivor.TrainingComplete");
					}
					else if (nextUpgradeDefinition != null && !flag6 && !base.SurvivorModel.IsUpgrading())
					{
						text = LocalizationManager.GetText("Popup.UpgradeSurvivor.TrainingGroundLevelRequired{Level}", nextUpgradeDefinition.TrainingGroundLevel);
					}
				}
				else if (flag8)
				{
					text = LocalizationManager.GetText("Popup.UpgradeSurvivor.SurvivorAssignedToOutpost");
				}
			}
			if (Helpers.GameObjectSetActive(base.TrainingLockedParent, text != ""))
			{
				HelpersUI.SetContentToLabel(base.TrainingLockedUILabel, text);
			}
		}
		if (base.PromoteButton != null && base.UpgradeButton != null)
		{
			CurrencyType survivorTraitUpgradeCurrencyType = SurvivorModel.GetSurvivorTraitUpgradeCurrencyType(base.SurvivorModel);
			int totalCost = base.SurvivorModel.GetUpgradeTraitCashier().GetTotalCost(survivorTraitUpgradeCurrencyType);
			int value2 = GameManager.Instance.playerModel.GetCurrency(survivorTraitUpgradeCurrencyType).Value;
			if (flag3)
			{
				Helpers.GameObjectSetActive(base.PromoteButton, value: true);
				base.PromoteButton.SetContentToLabelOne(LocalizationManager.GetText("Popup.SurvivorInfo.Button.Promote"));
				base.PromoteButton.isEnabled = isEnabled;
				base.PromoteButton.IsVisuallyDisabled = !flag5;
				base.PromoteButton.SetContentToIconOne(HelpersGfx.GetCurrencyIconName(survivorTraitUpgradeCurrencyType));
				base.PromoteButton.SetContentToLabelTwo(value2 + "/" + totalCost);
			}
			else
			{
				Helpers.GameObjectSetActive(base.UpgradeButton, value: true);
				base.UpgradeButton.SetContentToLabelOne(LocalizationManager.GetText(flag4 ? "Popup.SurvivorInfo.Button.Upgrade" : "Popup.SurvivorInfoPopup.Traits.FullyUpgraded"));
				base.UpgradeButton.isEnabled = isEnabled;
				base.UpgradeButton.IsVisuallyDisabled = !flag5;
				base.UpgradeButton.SetContentToIconOne(HelpersGfx.GetCurrencyIconName(survivorTraitUpgradeCurrencyType));
				base.UpgradeButton.SetContentToLabelTwo(flag4 ? (value2 + "/" + totalCost) : value2.ToString());
			}
		}
		UpdateAndShowExtraButtons();
	}

	public override void Enter()
	{
		base.Enter();
		PlayAnchorTween(base.SurvivorStatistics, TweenAnchorId.Show);
		PlayAnchorTween(base.SurvivorRightSidePanel, TweenAnchorId.Show);
		base.SurvivorRightSidePanel.SetSelectedIndex(0);
	}

	public override bool AllowAddToHistory()
	{
		return true;
	}
}
