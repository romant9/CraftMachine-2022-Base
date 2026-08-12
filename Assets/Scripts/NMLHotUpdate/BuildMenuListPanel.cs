using System.Collections.Generic;
using TWDModel;

public class BuildMenuListPanel : ScrollableListPanel<BuildingConstructionData>
{
	public int NumberBuildableBuildings { get; set; }

	protected override void SetCard(UIListCard<BuildingConstructionData> card)
	{
		if (card is BuildingCard)
		{
			((BuildingCard)card).EnableToggle();
			BuildingConstructionData item = card.Item;
			if (item != null && (item.BuildingType == "Cage" || item.BuildingType == "Outpost"))
			{
				(card as BuildingCard).Highlight = true;
			}
		}
	}

	public void SetupCardsByFiltering(BuildingCategory buildingCategory)
	{
		List<BuildingConstructionData> buildableBuildings = CampView.Instance.BuildableBuildings.GetBuildableBuildings(buildingCategory);
		SetCards(buildableBuildings);
	}

	protected override void Sort()
	{
		cards.Sort((UIListCard<BuildingConstructionData> a, UIListCard<BuildingConstructionData> b) => CompareCard(a, b));
	}

	private int CompareCard(UIListCard<BuildingConstructionData> cardA, UIListCard<BuildingConstructionData> cardB)
	{
		if (cardA is BuildingCard && cardB is BuildingCard)
		{
			BuildingConstructionData item = cardA.Item;
			BuildingConstructionData item2 = cardB.Item;
			int requiredCouncilLevel = item.RequiredCouncilLevel;
			int requiredCouncilLevel2 = item2.RequiredCouncilLevel;
			bool flag = item.BuildingType == "Cage" || item.BuildingType == "Outpost";
			bool flag2 = item2.BuildingType == "Cage" || item2.BuildingType == "Outpost";
			if (flag && !flag2)
			{
				return -1;
			}
			if (flag2 && !flag)
			{
				return 1;
			}
			if (requiredCouncilLevel < requiredCouncilLevel2)
			{
				return -1;
			}
			if (requiredCouncilLevel > requiredCouncilLevel2)
			{
				return 1;
			}
			Cashier buildingUpgradeCashier = GameManager.Instance.playerModel.Camp.GetBuildingUpgradeCashier(item.BuildingType, 1, instantUpgrade: false, addSpeedUpCashier: false);
			Cashier buildingUpgradeCashier2 = GameManager.Instance.playerModel.Camp.GetBuildingUpgradeCashier(item2.BuildingType, 1, instantUpgrade: false, addSpeedUpCashier: false);
			int num = buildingUpgradeCashier?.GetTotalCost(CurrencyType.Supplies) ?? 0;
			int num2 = buildingUpgradeCashier2?.GetTotalCost(CurrencyType.Supplies) ?? 0;
			if (num < num2)
			{
				return -1;
			}
			if (num > num2)
			{
				return 1;
			}
			return 0;
		}
		return 0;
	}

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
		if (!(type == "OnNewSurvivorSelected"))
		{
			_ = type == "SurvivorDeleted";
		}
	}
}
