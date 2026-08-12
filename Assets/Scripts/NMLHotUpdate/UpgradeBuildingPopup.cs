using System.Collections;
using TWDModel;
using UnityEngine;

[RequireComponent(typeof(BuildingInfoStatistics))]
public class UpgradeBuildingPopup : HUDElement
{
	[SerializeField]
	[Tooltip("If we use of the big pop up how much do we need to add in y-coordinate?")]
	private float bigPopupOffsetY;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel buildingNameLabel;

	[SerializeField]
	private UITexture buildingImage;

	[SerializeField]
	private UnlocksListPanel unlocksListPanel;

	private BuildingModel building;

	private bool useBigPopup;

	public override void Open()
	{
		base.Open();
		building = GetModel<BuildingModel>();
		useBigPopup = building.TypeName == "Council" && building.CanUpgrade;
		defaultPopup.SetPayButtonClickCallback(OnUpgradeBuilding);
		defaultPopup.SetInstantPayButtonClickCallback(OnUpgradeBuildingInstant);
		defaultPopup.SetInstantPayWithTokensButtonClickCallback(OnUpgradeBuildingInstantWithTokens);
		UpdateUI();
		GetComponent<BuildingInfoStatistics>().CreateStatistics(building, showUpgrade: true);
	}

	public override void Close()
	{
		base.Close();
		buildingImage.mainTexture = null;
		BuildingPhotoManager.Instance.RemoveAll();
	}

	public override void UpdateUI()
	{
		if (!building.BuildingType.DisableUpgrade)
		{
			string buildingName = HelpersLocalization.GetBuildingName(building);
			int h = (useBigPopup ? DefaultPopup.DefaultHeightBig : DefaultPopup.DefaultHeightSmall);
			defaultPopup.SetSize(DefaultPopUpWidth, h);
			Vector3 localPosition = base.transform.localPosition;
			localPosition.y = (useBigPopup ? bigPopupOffsetY : 0f);
			base.transform.localPosition = localPosition;
			string labelText = null;
			if (building.Level == 0)
			{
				titleLabel.text = LocalizationManager.GetText("Popup.UpgradeBuilding.RepairTitle{BuildingType}", buildingName);
				labelText = LocalizationManager.GetText("Popup.UpgradeBuilding.Button.Repair");
			}
			else if (!building.CanUpgrade)
			{
				titleLabel.text = LocalizationManager.GetText("Popup.UpgradeBuilding.MaxLevelReachedTitle{BuildingType}", buildingName);
			}
			else
			{
				titleLabel.text = LocalizationManager.GetText("Popup.UpgradeBuilding.UpgradeTitle{BuildingType}{Level}", buildingName, building.Level + 1);
				labelText = LocalizationManager.GetText("Popup.UpgradeBuilding.Button.Upgrade", buildingName);
			}
			if (building.CanUpgrade)
			{
				int buildingUpgradeTime = GameManager.Instance.playerModel.ActivityManager.GetBuildingUpgradeTime(building.GetNextUpgradeLevel());
				defaultPopup.SetInstantPayWithTokensButton(building.GetInstantUpgradeCashierWithTokens());
				defaultPopup.SetInstantPayButton(building.GetUpgradeCashier(instantUpgrade: true));
				defaultPopup.SetPayButton(labelText, building.GetUpgradeCashier(instantUpgrade: false, addSpeedUpCashier: false), buildingUpgradeTime);
			}
			buildingNameLabel.text = HelpersLocalization.GetBuildingName(building);
			BuildingView buildingView = CampView.Instance.CampViewBuildings.FindBuildingView(building);
			buildingImage.mainTexture = BuildingPhotoManager.Instance.GetBuildingPhoto(buildingView.BuildingType, buildingView.Model.MaxUpgradeLevel);
			CheckDependencyLevel();
			unlocksListPanel.SetBuildingUnlocks(building);
			if (building.CanUpgrade)
			{
				StartCoroutine(ShowButtons());
			}
		}
	}

	private IEnumerator ShowButtons()
	{
		yield return null;
		defaultPopup.HideAllPayButtons();
		defaultPopup.ShowPayButtons();
	}

	private void CheckDependencyLevel()
	{
		if (!building.CanUpgrade)
		{
			if (!building.HasRequiredBuilding)
			{
				defaultPopup.ShowLockedPanel(LocalizationManager.GetText("Popup.UpgradeBuilding.RequiredBuildingMissing{Building}", HelpersLocalization.GetBuildingName(building.BuildingType.RequiredBuilding)));
			}
			else if (!building.HasDepencyLevelToUpgrade)
			{
				defaultPopup.ShowLockedPanel(LocalizationManager.GetText("Popup.UpgradeBuilding.CouncilLevelRequiredMessage{Level}", building.DependencyLevelRequiredToUpgrade));
			}
			else if (!building.HasPlayerLevelToUpgrade)
			{
				defaultPopup.ShowLockedPanel(LocalizationManager.GetText("Popup.UpgradeBuilding.PlayerLevelRequiredMessage{Level}", building.GetNextUpgradeLevel().PlayerLevelRequired));
			}
			else if (building.HasReachedCouncilMaxForcedLevel)
			{
				defaultPopup.ShowLockedPanel(LocalizationManager.GetText("Generic.ComingSoon"));
			}
			else
			{
				defaultPopup.ShowLockedPanel(LocalizationManager.GetText("Popup.UpgradeBuilding.MaxLevelReachedMessage"));
			}
		}
		else
		{
			defaultPopup.ShowPayButtons();
		}
	}

	public void OnUpgradeBuildingInstant()
	{
		if (TutorialView.Allowed("BuyInstant"))
		{
			ConsumeCurrencyCommandUtils.Execute(new UpgradeBuildingCommand(building)
			{
				Instant = true,
				Cashier = building.GetUpgradeCashier(instantUpgrade: true)
			}, InstantBuildCallback);
		}
	}

	public void OnUpgradeBuildingInstantWithTokens()
	{
		if (TutorialView.Allowed("BuyInstant"))
		{
			ConsumeCurrencyCommandUtils.Execute(new UpgradeBuildingCommand(building)
			{
				Instant = true,
				Cashier = building.GetInstantUpgradeCashierWithTokens()
			}, InstantBuildCallback);
		}
	}

	private void InstantBuildCallback(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			Close();
		}
	}

	public void OnUpgradeBuilding()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/building_upgrade");
		ConsumeCurrencyCommandUtils.Execute(new UpgradeBuildingCommand(building)
		{
			Instant = false,
			Cashier = building.GetUpgradeCashier(instantUpgrade: false)
		});
		EventManager.NotifyClick("Upgrade");
		Close();
	}
}
