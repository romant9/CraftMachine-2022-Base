using System;
using System.Collections.Generic;
using BaseModel;
using TWDModel;

public class RewardEquipment : RandomizedReward, IReward
{
	public string EquipmentId { get; set; }

	public int RarityLevel { get; set; }

	public int StartingLevel { get; set; }

	public int StartingLevelOffset { get; set; }

	public string Quality { get; set; }

	public EquipmentSource EquipmentSource { get; set; }

	public int Amount { get; set; }

	public EquipmentItemModel GivenEquipment { get; private set; }

	public RewardType Type => RewardType.Equipment;

	public EquipmentDefinition EquipmentDefinition(TWDModelManager manager)
	{
		return manager.GameEconomyData.GetEquipmentDefinition(EquipmentId);
	}

	public bool IsConsumableReward(TWDModelManager manager)
	{
		EquipmentDefinition equipmentDefinition = EquipmentDefinition(manager);
		if (equipmentDefinition == null)
		{
			return false;
		}
		return equipmentDefinition.Category == EquipmentCategory.Utility;
	}

	public object Give(TWDModelManager manager, object[] param = null)
	{
		ModelRandom random = GetRandom(param);
		int num = StartingLevel;
		EquipmentDefinition equipmentDefinition = manager.GameEconomyData.GetEquipmentDefinition(EquipmentId);
		if (num <= 0 && equipmentDefinition != null)
		{
			int num2 = manager.Player.SurvivorContainer.GetHighestLevelOfSurvivorClass(equipmentDefinition.SurvivorClass);
			if (num2 == 0)
			{
				num2 = manager.Player.SurvivorContainer.GetHighestLevelSurvivor();
			}
			num = num2 + StartingLevelOffset;
			int maximumEquipmentLevel = manager.Player.gameEconomyData.GetMaximumEquipmentLevel();
			if (maximumEquipmentLevel > 0)
			{
				num = Math.Min(num, maximumEquipmentLevel);
			}
		}
		EquipmentItemModel equipmentItemModel = manager.Player.Equipment.GenerateAndInitializeEquipmentFromDefinition(EquipmentId, RarityLevel, num, random);
		if (equipmentItemModel == null)
		{
			manager.Debug.LogError("Could not get equipment for '" + EquipmentId + "', cannot give reward!");
			return null;
		}
		if (!OfflineManager.IsNoAddRewards)
		{
			if (equipmentItemModel.Definition.Category == EquipmentCategory.Utility)
			{
				for (int i = 0; i < Amount; i++)
				{
					manager.Player.Equipment.AddEquipment(equipmentItemModel, (EquipmentSource == EquipmentSource.Unknown) ? EquipmentSource.Bundle : EquipmentSource);
				}
				manager.Player.NotifyChange("ConsumableAcquired");
			}
			else
			{
				manager.Player.Equipment.AddEquipment(equipmentItemModel, (EquipmentSource == EquipmentSource.Unknown) ? EquipmentSource.Bundle : EquipmentSource);
			}
		}
		GivenEquipment = equipmentItemModel;
		return equipmentItemModel;
	}

	public void PreviewUpgradeTraitsDataForEquipment(TWDModelManager manager, out List<UpgradeTraitsData> upgradeTraitsDataList)
	{
		upgradeTraitsDataList = new List<UpgradeTraitsData>();
		EquipmentDefinition equipmentDefinition = EquipmentDefinition(manager);
		if (manager == null || equipmentDefinition == null || equipmentDefinition.TraitsOverride == null || equipmentDefinition.TraitsOverride.Count <= 0)
		{
			return;
		}
		Dictionary<int, TraitBucketsDefinition> levelsThatUnlockATrait = manager.GameEconomyData.GetLevelsThatUnlockATrait(RarityLevel, TWDModel.UpgradeType.EquipmentUpgrade, StartingLevel, replaceTacticalWithLowLevel: false);
		int num = 0;
		foreach (KeyValuePair<int, TraitBucketsDefinition> item in levelsThatUnlockATrait)
		{
			if (num >= equipmentDefinition.TraitsOverride.Count)
			{
				continue;
			}
			string traitIdentifier = equipmentDefinition.TraitsOverride[num];
			if (item.Value.IsTactical)
			{
				traitIdentifier = "ChargeEquipment";
			}
			else
			{
				num++;
			}
			UpgradeTraitsData upgradeTraitsData = new UpgradeTraitsData();
			TraitDefinition traitDefinition = manager.GameEconomyData.GetTraitDefinition(traitIdentifier);
			if (traitDefinition != null)
			{
				upgradeTraitsData.Identifier = traitDefinition.Identifier;
				upgradeTraitsData.UnlockingLevel = item.Key;
				if (EquipmentItemModel.IsApocalypticTrait(traitDefinition.Identifier))
				{
					upgradeTraitsData.RarityLevel = 5;
				}
				else
				{
					upgradeTraitsData.RarityLevel = item.Value.RarityLevel;
				}
				upgradeTraitsData.RarityLevel = item.Value.RarityLevel;
				upgradeTraitsData.IsLocked = item.Value.IsLocked;
				upgradeTraitsData.IsTactical = item.Value.IsTactical;
				upgradeTraitsDataList.Add(upgradeTraitsData);
			}
		}
	}
}
