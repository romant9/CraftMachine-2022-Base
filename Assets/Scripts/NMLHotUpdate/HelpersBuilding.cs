using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class HelpersBuilding
{
	public static BuildingModel GetBuildingModel(GameObject gameObject)
	{
		BuildingView component = gameObject.GetComponent<BuildingView>();
		Transform transform = null;
		if (component == null && (transform = gameObject.transform.parent) != null)
		{
			return GetBuildingModel(transform.gameObject);
		}
		return component.Model;
	}

	public static BuildingView GetBuildingView(GameObject gameObject)
	{
		BuildingView component = gameObject.GetComponent<BuildingView>();
		Transform transform = null;
		if (component == null && (transform = gameObject.transform.parent) != null)
		{
			return GetBuildingView(transform.gameObject);
		}
		return component;
	}

	public static string GetBuildingIconName(BuildingType buildingType)
	{
		return "Icon_Building_" + buildingType.Name;
	}

	public static string GetLocalizedBuildingLevel(BuildingModel building)
	{
		int num = 1;
		if (building != null)
		{
			num = Mathf.Max(1, building.Level);
		}
		return LocalizationManager.GetText("Statistic.Level{Level}", num);
	}

	public static int GetMaxStartingLevelForSurvivor(bool max)
	{
		BuildingModel building = GameManager.Instance.playerModel.Camp.GetBuilding("RadioTent");
		int radioTentLevel = 0;
		if (building != null)
		{
			radioTentLevel = building.Level;
		}
		return GetMaxStartingLevelForSurvivor(radioTentLevel, max);
	}

	public static int GetMaxStartingLevelForSurvivor(int radioTentLevel, bool max = true)
	{
		int num = ((!max) ? 999 : 0);
		DropType[] array = new DropType[3]
		{
			DropType.Regular,
			DropType.Silver,
			DropType.Gold
		};
		for (int i = 0; i < array.Length; i++)
		{
			List<int> startingLevelForRarity = GameManager.Instance.gameEconomyData.GetDropStartingLevelDefinition(array[i], DropRewardType.Survivor, radioTentLevel).GetStartingLevelForRarity(0);
			int num2 = startingLevelForRarity[0];
			if (!max)
			{
				if (num2 < num)
				{
					num = num2;
				}
				continue;
			}
			int num3 = ((startingLevelForRarity.Count > 1) ? startingLevelForRarity[1] : num2);
			if (num3 > num)
			{
				num = num3;
			}
		}
		return num;
	}
}
