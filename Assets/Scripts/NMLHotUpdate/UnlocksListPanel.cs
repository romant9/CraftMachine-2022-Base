using System.Collections.Generic;
using TWDModel;

public class UnlocksListPanel : ScrollableListPanel<UnlockItem>
{
	public void SetBuildingUnlocks(BuildingModel buildingModelUnlocker)
	{
		if (buildingModelUnlocker.TypeName != "Council" || !buildingModelUnlocker.CanUpgrade)
		{
			NGUITools.SetActiveChildren(base.gameObject, state: false);
			return;
		}
		NGUITools.SetActiveChildren(base.gameObject, state: true);
		List<UnlockItem> list = new List<UnlockItem>();
		int councilLevel = GameManager.Instance.playerModel.Camp.GetBuildingDependencyLevel() + 1;
		GameEconomyData gameEconomyData = GameManager.Instance.playerModel.gameEconomyData;
		BuildingType[] buildingTypes = gameEconomyData.BuildingTypes;
		foreach (BuildingType buildingType in buildingTypes)
		{
			if (!(buildingType.Name == "Cage") && !(buildingType.Name == "Outpost") && gameEconomyData.GetAdditionalBuildingAmountAtCouncilLevel(councilLevel, buildingType.Name) > 0)
			{
				UnlockItem unlockItem = new UnlockItem();
				unlockItem.BuildingType = buildingType;
				unlockItem.Level = 1;
				list.Add(unlockItem);
			}
		}
		SetCards(list);
	}

	public void SetCampUnlocks(CampType campType, CampSubtype campSubtype)
	{
	}
}
