using TWDModel;
using UnityEngine;

public class BuildingUpgradeIndicator : BuildingIndicator
{
	public UILabel labelTime;

	[SerializeField]
	private UISprite buildingUpgrade;

	[SerializeField]
	private UISprite equipmentUpgrade;

	[SerializeField]
	private UISprite cutVegetation;

	[SerializeField]
	private UISprite walkerUpgrade;

	[SerializeField]
	private UIProgressBar progressBar;

	private long originalUpgradeTime;

	protected UpgradeType type;

	private int previousTimeSeconds = -1;

	public void SetType(UpgradeType upgradeType)
	{
		type = upgradeType;
	}

	protected virtual void Start()
	{
		DisableAllSprites();
		switch (type)
		{
		case UpgradeType.Building:
			buildingUpgrade.enabled = true;
			originalUpgradeTime = base.Building.Model.OriginalUpgradeTimer;
			break;
		case UpgradeType.EquipmentUpgrading:
			equipmentUpgrade.enabled = true;
			if (((WorkshopBuildingModel)base.Building.Model).UpgradingModel is EquipmentItemModel equipmentItemModel)
			{
				originalUpgradeTime = equipmentItemModel.TimedActionModel.OriginalActionTime;
			}
			break;
		case UpgradeType.UpgradeSurvivor:
		{
			SurvivorModel upgradingSurvivor = ((TrainingGroundBuildingModel)base.Building.Model).UpgradingSurvivor;
			if (upgradingSurvivor != null)
			{
				originalUpgradeTime = upgradingSurvivor.TimedActionModel.OriginalActionTime;
			}
			break;
		}
		case UpgradeType.UpgradeWalker:
		{
			walkerUpgrade.enabled = true;
			OutpostWalkerModel upgradingWalker = ((CageBuildingModel)base.Building.Model).UpgradingWalker;
			if (upgradingWalker != null)
			{
				originalUpgradeTime = upgradingWalker.TimedActionModel.OriginalActionTime;
			}
			break;
		}
		case UpgradeType.CutVegetation:
			originalUpgradeTime = ((VegetationModel)base.Building.Model).CutTimedActionModel.OriginalActionTime;
			if (cutVegetation != null)
			{
				cutVegetation.enabled = true;
			}
			break;
		}
	}

	protected virtual void LateUpdate()
	{
		long num = 0L;
		switch (type)
		{
		case UpgradeType.Building:
			num = base.Building.Model.UpgradeTimer;
			break;
		case UpgradeType.EquipmentUpgrading:
			num = ((!(((WorkshopBuildingModel)base.Building.Model).UpgradingModel is EquipmentItemModel equipmentItemModel)) ? 0 : equipmentItemModel.TimedActionModel.MillisecondsTillCompletion);
			break;
		case UpgradeType.UpgradeSurvivor:
			num = ((TrainingGroundBuildingModel)base.Building.Model).UpgradingSurvivor?.TimedActionModel.MillisecondsTillCompletion ?? 0;
			break;
		case UpgradeType.UpgradeWalker:
			num = ((CageBuildingModel)base.Building.Model).UpgradingWalker?.TimedActionModel.MillisecondsTillCompletion ?? 0;
			break;
		case UpgradeType.CutVegetation:
			num = ((VegetationModel)base.Building.Model).CutTimedActionModel.MillisecondsTillCompletion;
			break;
		}
		if (num > 0)
		{
			int num2 = Helpers.ConvertToSecondsNoZero(num);
			if (num2 != previousTimeSeconds)
			{
				previousTimeSeconds = num2;
				labelTime.text = Helpers.FormatTime(num2 * 1000);
			}
			if (progressBar != null)
			{
				progressBar.value = (float)num / (float)originalUpgradeTime;
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void DisableAllSprites()
	{
		if (equipmentUpgrade != null)
		{
			equipmentUpgrade.enabled = false;
		}
		if ((bool)buildingUpgrade)
		{
			buildingUpgrade.enabled = false;
		}
		if ((bool)cutVegetation)
		{
			cutVegetation.enabled = false;
		}
		if ((bool)walkerUpgrade)
		{
			walkerUpgrade.enabled = false;
		}
	}
}
