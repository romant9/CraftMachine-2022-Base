using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class BuildingInfoStatistics : MonoBehaviour
{
	[Tooltip("The gameObject that will contain all of the statistics")]
	[SerializeField]
	private GameObject statisticContainer;

	[Tooltip("The prefab for a statistic")]
	[SerializeField]
	private GameObject statisticPrefab;

	[Tooltip("Space between each statistic. Added on top of the statistic panel collider height.")]
	[SerializeField]
	private float spaceBetweenStatistics;

	private List<StatisticPanel> statisticObjects = new List<StatisticPanel>();

	private BuildingModel buildingModel;

	private BuildingUpgradeLevel buildingData;

	private BuildingUpgradeLevel buildingNextLevelData;

	public void CreateStatistics(BuildingModel buildingModel, bool showUpgrade)
	{
		Reset();
		int num = 0;
		Vector3 localPosition = default(Vector3);
		List<Statistic> buildingStatistics = GetBuildingStatistics(buildingModel, showUpgrade);
		if (buildingStatistics != null)
		{
			for (int i = 0; i < buildingStatistics.Count; i++)
			{
				Statistic statistic = buildingStatistics[i];
				GameObject obj = Helpers.InstantiateToParent(statisticPrefab, statisticContainer);
				float num2 = obj.GetComponent<BoxCollider>().size.y + spaceBetweenStatistics;
				localPosition.y -= num2 / 2f;
				obj.transform.localPosition = localPosition;
				StatisticPanel component = obj.GetComponent<StatisticPanel>();
				component.SetStatistic(statistic.Type, statistic.Icon, statistic.Value, statistic.GainValue);
				statisticObjects.Add(component);
				localPosition.y -= num2 / 2f;
				num++;
			}
		}
	}

	private void OnDisable()
	{
		Reset();
	}

	private void Reset()
	{
		statisticContainer.RemoveAllChildren();
		statisticObjects.Clear();
	}

	private List<Statistic> GetBuildingStatistics(BuildingModel buildingModel, bool showUpgrade)
	{
		this.buildingModel = buildingModel;
		buildingData = buildingModel.GetCurrentUpgradeLevel();
		buildingNextLevelData = null;
		if (showUpgrade)
		{
			buildingNextLevelData = buildingModel.GetNextUpgradeLevel();
		}
		List<Statistic> list = new List<Statistic>();
		if (buildingData == null)
		{
			return list;
		}
		for (int i = 0; i < (int)CurrencyType.Count; i++)
		{
			list = AddStatisticStorageCapacity(list, (CurrencyType)i);
		}
		for (int j = 0; j < (int)CurrencyType.Count; j++)
		{
			list = AddStatisticProduction(list, (CurrencyType)j);
		}
		if (buildingModel.TypeName == "MedicTent")
		{
			int gainValue = 0;
			int medicInjuryTimeBonus = buildingData.MedicInjuryTimeBonus;
			if (buildingNextLevelData != null)
			{
				gainValue = buildingNextLevelData.MedicInjuryTimeBonus - medicInjuryTimeBonus;
			}
			list.Add(new Statistic
			{
				Type = "HealingTime",
				Icon = "Ui_Icon_Survivor_Healing",
				Value = medicInjuryTimeBonus,
				GainValue = gainValue
			});
			gainValue = 0;
			medicInjuryTimeBonus = buildingData.MedicSlotsAmount;
			if (buildingNextLevelData != null)
			{
				gainValue = buildingNextLevelData.MedicSlotsAmount - medicInjuryTimeBonus;
			}
			list.Add(new Statistic
			{
				Type = "HealingSlots",
				Icon = null,
				Value = medicInjuryTimeBonus,
				GainValue = gainValue
			});
		}
		if (!(buildingModel.TypeName == "MissionCar"))
		{
			if (buildingModel is TrainingGroundBuildingModel)
			{
				TrainingGroundBuildingModel trainingGroundBuildingModel = buildingModel as TrainingGroundBuildingModel;
				int num = trainingGroundBuildingModel.Level + 1;
				int num2 = Math.Min(trainingGroundBuildingModel.Level + 1, trainingGroundBuildingModel.MaxUpgradeLevel) + 1;
				int gainValue2 = 0;
				if (buildingNextLevelData != null)
				{
					gainValue2 = num2 - num;
				}
				list.Add(new Statistic
				{
					Type = "StatisticTypeTrainingGround",
					Icon = "",
					Value = num,
					GainValue = gainValue2
				});
			}
			else if (buildingModel is WorkshopBuildingModel)
			{
				WorkshopBuildingModel obj = buildingModel as WorkshopBuildingModel;
				int maxEquipmentLevel = obj.GetMaxEquipmentLevel(obj.Level);
				int maxEquipmentLevel2 = obj.GetMaxEquipmentLevel(obj.Level + 1);
				int gainValue3 = 0;
				if (buildingNextLevelData != null)
				{
					gainValue3 = maxEquipmentLevel2 - maxEquipmentLevel;
				}
				list.Add(new Statistic
				{
					Type = "StatisticTypeWorkshop",
					Icon = "",
					Value = maxEquipmentLevel,
					GainValue = gainValue3
				});
			}
			else if (buildingModel.TypeName == "RadioTent")
			{
				int maxStartingLevelForSurvivor = HelpersBuilding.GetMaxStartingLevelForSurvivor(buildingModel.Level);
				int maxStartingLevelForSurvivor2 = HelpersBuilding.GetMaxStartingLevelForSurvivor(buildingModel.Level + 1);
				int gainValue4 = 0;
				if (buildingNextLevelData != null)
				{
					gainValue4 = maxStartingLevelForSurvivor2 - maxStartingLevelForSurvivor;
				}
				list.Add(new Statistic
				{
					Type = "StatisticTypeRadioTent",
					Icon = "",
					Value = maxStartingLevelForSurvivor,
					GainValue = gainValue4
				});
			}
			else if (buildingModel.TypeName == "Residence")
			{
				int gainValue5 = 0;
				if (buildingNextLevelData != null)
				{
					gainValue5 = 1;
				}
				list.Add(new Statistic
				{
					Type = "StatisticTypeCraftsman",
					Icon = "",
					Value = buildingModel.Level,
					GainValue = gainValue5
				});
			}
		}
		if (buildingNextLevelData != null && buildingNextLevelData.AwardedXp > 0)
		{
			list.Add(new Statistic
			{
				Type = "BuildingPoints",
				Icon = "Ui_Icon_Resource_Building_Points",
				GainValue = buildingNextLevelData.AwardedXp,
				Value = 0
			});
		}
		if (buildingData.BuffEffectType != BuffEffectType.None)
		{
			TraitDefinition traitDefinition = ((BuffBuildingModel)buildingModel).TraitDefinition;
			if (traitDefinition != null)
			{
				list.Add(new Statistic
				{
					Type = traitDefinition.Identifier,
					Value = traitDefinition.GetParameter<int>(0)
				});
			}
			else
			{
				Debug.LogError("Could not get buff building trait for [" + buildingModel.TypeName + "]");
			}
		}
		if (showUpgrade)
		{
			_ = buildingNextLevelData;
		}
		return list;
	}

	private List<Statistic> AddStatisticStorageCapacity(List<Statistic> statistics, CurrencyType currencyType)
	{
		int gainValue = 0;
		int capacity = buildingData.GetCapacity(currencyType);
		if (capacity > 0)
		{
			if (buildingNextLevelData != null)
			{
				gainValue = buildingNextLevelData.GetCapacity(currencyType) - capacity;
			}
			statistics.Add(new Statistic
			{
				Type = "StorageCapacity",
				Icon = HelpersGfx.GetCurrencyIconName(currencyType),
				Value = capacity,
				GainValue = gainValue
			});
		}
		return statistics;
	}

	private List<Statistic> AddStatisticProduction(List<Statistic> statistics, CurrencyType currencyType)
	{
		int gainValue = 0;
		int gainValue2 = 0;
		int buildingUpgradeRate = GameManager.Instance.playerModel.ActivityManager.GetBuildingUpgradeRate(buildingData);
		if (buildingModel.BuildingType.ProductionType == currencyType && buildingUpgradeRate > 0)
		{
			if (buildingNextLevelData != null)
			{
				gainValue = GameManager.Instance.playerModel.ActivityManager.GetBuildingUpgradeRate(buildingNextLevelData) - buildingUpgradeRate;
				gainValue2 = buildingNextLevelData.ProductionCapacity - buildingData.ProductionCapacity;
			}
			string type = "Production";
			statistics.Add(new Statistic
			{
				Type = type,
				Icon = HelpersGfx.GetCurrencyIconName(currencyType),
				Value = buildingUpgradeRate,
				GainValue = gainValue
			});
			statistics.Add(new Statistic
			{
				Type = "MaxProduction",
				Icon = HelpersGfx.GetCurrencyIconName(currencyType),
				Value = buildingData.ProductionCapacity,
				GainValue = gainValue2
			});
		}
		return statistics;
	}
}
