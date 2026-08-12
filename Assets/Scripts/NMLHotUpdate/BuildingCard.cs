using TWDModel;
using UnityEngine;

public class BuildingCard : UIListCard<BuildingConstructionData>
{
	[SerializeField]
	private UISprite background;

	[SerializeField]
	private GameObject payButton;

	[SerializeField]
	private UILabel buildingNameLabel;

	[SerializeField]
	private UILabel messageLabel;

	[SerializeField]
	private UILabel priceLabel;

	[SerializeField]
	private UISprite buildingIcon;

	[SerializeField]
	private UIButton buildButton;

	[SerializeField]
	private Color unavailableColorTint;

	[SerializeField]
	private TutorialArrowParent tutorialArrow;

	private Cashier buildingUpgradeCashier;

	public bool Highlight { get; set; }

	public bool IsAvailable()
	{
		BuildingConstructionData item = base.Item;
		if (item != null)
		{
			int level = GameManager.Instance.playerModel.Camp.GetBuilding("Council").Level;
			bool num = string.IsNullOrEmpty(item.RequiredBuilding) || GameManager.Instance.playerModel.Camp.GetBuildingLevel(item.RequiredBuilding) > 0;
			bool flag = item.RequiredCouncilLevel <= level;
			return num && flag;
		}
		return false;
	}

	public void OnCreateBuildingClicked()
	{
		if (!IsAvailable() || !TutorialView.Allowed(base.Item.BuildingType))
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/invalid_action");
			return;
		}
		BuildingConstructionData item = base.Item;
		if (item != null)
		{
			UIEvent.Send("OnBuildingConstructionRequested", item);
		}
	}

	public override int GetSortValue()
	{
		if (IsAvailable())
		{
			if (buildingUpgradeCashier != null)
			{
				return -buildingUpgradeCashier.GetTotalCost(CurrencyType.Supplies);
			}
			return 1;
		}
		return -99999;
	}

	public override void UpdateUI()
	{
		BuildingConstructionData item = base.Item;
		if (item == null)
		{
			return;
		}
		tutorialArrow.Id = item.BuildingType;
		int level = GameManager.Instance.playerModel.Camp.GetBuilding("Council").Level;
		bool flag = string.IsNullOrEmpty(item.RequiredBuilding) || GameManager.Instance.playerModel.Camp.GetBuildingLevel(item.RequiredBuilding) > 0;
		bool flag2 = item.RequiredCouncilLevel <= level;
		bool flag3 = flag2 && flag;
		if (buildingIcon != null)
		{
			buildingIcon.spriteName = HelpersGfx.GetBuildingIconName(item.BuildingType);
			if (!flag3)
			{
				buildingIcon.color = unavailableColorTint;
			}
		}
		if (background != null && !flag3)
		{
			background.color = unavailableColorTint;
		}
		string text = "";
		if (flag3)
		{
			text = LocalizationManager.GetText("BuildingCard.Description." + item.BuildingType);
		}
		else if (!flag2)
		{
			text = LocalizationManager.GetText("BuildingCard.RequiredLevelDescription", item.RequiredCouncilLevel);
		}
		else if (!flag)
		{
			text = LocalizationManager.GetText("BuildingCard.RequiredBuildingDescription", HelpersLocalization.GetBuildingName(item.RequiredBuilding));
		}
		if (messageLabel != null)
		{
			messageLabel.text = text;
		}
		if (buildingNameLabel != null)
		{
			buildingNameLabel.text = LocalizationManager.GetText("Building.Name." + item.BuildingType);
		}
		if (payButton != null)
		{
			payButton.SetActive(flag3);
			buildingUpgradeCashier = GameManager.Instance.playerModel.Camp.GetBuildingUpgradeCashier(item.BuildingType, 1, instantUpgrade: false, addSpeedUpCashier: false);
			payButton.GetComponent<PayButton>().UpdateUI(buildingUpgradeCashier, "");
		}
		EffectSparkle component = background.GetComponent<EffectSparkle>();
		if (component != null)
		{
			component.enabled = Highlight;
		}
	}
}
