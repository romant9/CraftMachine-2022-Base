using TWDModel;
using UnityEngine;

public class SurvivorUpgradeDoneIndicator : BuildingIndicator
{
	[SerializeField]
	[Tooltip("Panel showing that we upgraded a survivor.")]
	private GameObject upgradedSurvivorPanel;

	[SerializeField]
	private UILabel messageLabel;

	[SerializeField]
	private UISprite icon;

	protected void Start()
	{
		Reset();
	}

	public void Reset()
	{
		if (upgradedSurvivorPanel != null)
		{
			upgradedSurvivorPanel.SetActive(value: false);
			ModelUpgraderBuildingModel modelUpgraderBuildingModel = base.Building.Model as ModelUpgraderBuildingModel;
			upgradedSurvivorPanel.SetActive(modelUpgraderBuildingModel.UpgradedUnseenModel != null);
		}
		if (messageLabel != null)
		{
			if (base.Building is WorkshopView)
			{
				messageLabel.text = LocalizationManager.GetText("Indicator.EquipmentUpgraded");
			}
			else if (base.Building is TrainingGroundView)
			{
				messageLabel.text = LocalizationManager.GetText("Indicator.SurvivorUpgraded");
			}
			else if (base.Building is CageView)
			{
				messageLabel.text = LocalizationManager.GetText("Indicator.WalkerUpgraded");
			}
		}
		if (icon != null)
		{
			if (base.Building is WorkshopView)
			{
				icon.spriteName = "Ui_Icon_Weaponupgrading";
			}
			else if (base.Building is TrainingGroundView)
			{
				icon.spriteName = "Ui_Icon_Newsurvivor";
			}
		}
	}

	public void Destroy()
	{
		if (base.gameObject != null)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void OnClickUpgradedSurvivor()
	{
		if (!(base.Building != null) || base.Building.Model == null || !(base.Building.Model is ModelUpgraderBuildingModel))
		{
			return;
		}
		if (base.Building.Model is ModelUpgraderBuildingModel { UpgradedUnseenModel: not null, UpgradedUnseenModel: var upgradedUnseenModel } modelUpgraderBuildingModel)
		{
			Helpers.ExecuteCommand(new UpgradedModelViewedCommand(modelUpgraderBuildingModel));
			if (base.Building is WorkshopView)
			{
				EquipmentItemModel model = upgradedUnseenModel as EquipmentItemModel;
				EquipmentUpgradePopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
				obj.ShowNextLevel = false;
				obj.ShowThisLevelUnlocks = true;
				obj.OpenForModel(model);
				EventManager.NotifyClick("Workshop");
			}
			else if (base.Building is TrainingGroundView)
			{
				if (!TutorialView.Instance.Allow("TrainingGround"))
				{
					return;
				}
				SurvivorInfoPopup obj2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup;
				obj2.currentStateMachineState = SurvivorInfoStateBase.States.SurvivorTrainDone;
				obj2.OpenForModel(upgradedUnseenModel as SurvivorModel);
				EventManager.NotifyClick("TrainingGround");
			}
			else if (base.Building is CageView)
			{
				if (!TutorialView.Instance.Allow("Cage"))
				{
					return;
				}
				WalkerInfoPopup obj3 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.PopupWalkerInfo) as WalkerInfoPopup;
				obj3.currentState = WalkerInfoPopup.WalkerInfoPopupStates.ShowUpgradeFromCamp;
				obj3.OpenForModel(upgradedUnseenModel as OutpostWalkerModel);
				EventManager.NotifyClick("Cage");
			}
		}
		Object.Destroy(base.gameObject);
	}
}
