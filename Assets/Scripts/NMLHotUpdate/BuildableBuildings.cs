using System.Collections.Generic;
using TWDModel;

public class BuildableBuildings
{
	private Dictionary<BuildingCategory, List<BuildingConstructionData>> buildableBuildings = new Dictionary<BuildingCategory, List<BuildingConstructionData>>();

	private int numberBuildableBuildings;

	private int oldCouncilLevel;

	private int oldNumberBuildingsInCamp;

	private OutpostTutorialState oldOutpostTutorialState;

	private bool outpostTutorialStateSet;

	public int NumberBuildableBuildings
	{
		get
		{
			Update();
			return numberBuildableBuildings;
		}
	}

	public void Update()
	{
		int level = GameManager.Instance.playerModel.Camp.GetBuilding("Council").Level;
		int num = CampView.Instance.CampViewBuildings.Buildings.Count;
		OutpostTutorialState outpostTutorialState = GameManager.Instance.playerModel.OutpostTutorialState;
		foreach (BuildingView building in CampView.Instance.CampViewBuildings.Buildings)
		{
			if (building != null && building.Model == null)
			{
				num--;
			}
		}
		if (level != oldCouncilLevel || num != oldNumberBuildingsInCamp || !outpostTutorialStateSet || outpostTutorialState != oldOutpostTutorialState)
		{
			oldCouncilLevel = level;
			oldNumberBuildingsInCamp = num;
			oldOutpostTutorialState = outpostTutorialState;
			outpostTutorialStateSet = true;
			numberBuildableBuildings = 0;
			for (int i = 0; i < 4; i++)
			{
				CreateBuildableList((BuildingCategory)i);
			}
		}
	}

	public List<BuildingConstructionData> GetBuildableBuildings(BuildingCategory buildingCategory)
	{
		return buildableBuildings[buildingCategory];
	}

	private void CreateBuildableList(BuildingCategory buildingCategory)
	{
		int level = GameManager.Instance.playerModel.Camp.GetBuilding("Council").Level;
		List<string> availableBuildingsToBuild = GameManager.Instance.playerModel.Camp.GetAvailableBuildingsToBuild();
		List<BuildingConstructionData> list = new List<BuildingConstructionData>();
		Dictionary<string, BuildingConstructionData> dictionary = new Dictionary<string, BuildingConstructionData>();
		bool flag = GameManager.Instance.gameEconomyData.ConfigData.OutpostEnabled && GameManager.Instance.playerModel.OutpostTutorialState >= OutpostTutorialState.WaitingForBuildings;
		foreach (string item in availableBuildingsToBuild)
		{
			if ((item == "Outpost" || item == "Cage") && !flag)
			{
				continue;
			}
			BuildingType buildingType = GameManager.Instance.playerModel.gameEconomyData.GetBuildingType(item);
			if (buildingType != null && buildingType.Category == buildingCategory)
			{
				if (dictionary.ContainsKey(item))
				{
					dictionary[item].Amount++;
				}
				else
				{
					BuildingConstructionData buildingConstructionData = new BuildingConstructionData();
					buildingConstructionData.RequiredCouncilLevel = level;
					buildingConstructionData.RequiredBuilding = buildingType.RequiredBuilding;
					buildingConstructionData.BuildingType = item;
					buildingConstructionData.Amount = 1;
					dictionary.Add(item, buildingConstructionData);
					list.Add(buildingConstructionData);
				}
				if (buildingCategory != BuildingCategory.BuffBuilding)
				{
					numberBuildableBuildings++;
				}
			}
		}
		int totalBuildingsAmountsEntries = GameManager.Instance.playerModel.gameEconomyData.GetTotalBuildingsAmountsEntries();
		for (int i = level + 1; i < totalBuildingsAmountsEntries; i++)
		{
			BuildingType[] buildingTypes = GameManager.Instance.gameEconomyData.BuildingTypes;
			foreach (BuildingType buildingType2 in buildingTypes)
			{
				if (((!(buildingType2.Name == "Outpost") && !(buildingType2.Name == "Cage")) || flag) && buildingType2.Category == buildingCategory && GameManager.Instance.gameEconomyData.GetAdditionalBuildingAmountAtCouncilLevel(i, buildingType2.Name) > 0 && !dictionary.ContainsKey(buildingType2.Name))
				{
					BuildingConstructionData buildingConstructionData2 = new BuildingConstructionData();
					buildingConstructionData2.RequiredCouncilLevel = i;
					buildingConstructionData2.BuildingType = buildingType2.Name;
					dictionary.Add(buildingType2.Name, buildingConstructionData2);
					list.Add(buildingConstructionData2);
				}
			}
		}
		buildableBuildings[buildingCategory] = list;
	}
}
