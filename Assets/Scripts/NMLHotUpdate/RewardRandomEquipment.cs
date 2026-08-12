using BaseModel;
using TWDModel;

public class RewardRandomEquipment : RandomizedReward, IReward
{
	public EquipmentCategory Category { get; set; }

	public int StartingLevelOffset { get; set; }

	public SurvivorClass SurvivorClass { get; set; }

	public int RarityLevel { get; set; }

	public EquipmentSource EquipmentSource { get; set; }

	public EquipmentItemModel GivenEquipment { get; private set; }

	public RewardType Type => RewardType.RandomEquipment;

	public object Give(TWDModelManager manager, object[] param = null)
	{
		ModelRandom random = GetRandom(param);
		int levelOut;
		EquipmentDefinition randomEquipmentDefinition = GetRandomEquipmentDefinition(manager, random, out levelOut);
		EquipmentItemModel equipmentItemModel = manager.Player.Equipment.GenerateAndInitializeEquipmentFromDefinition(randomEquipmentDefinition.ID, RarityLevel, levelOut, random);
		if (equipmentItemModel != null)
		{
			manager.Player.Equipment.AddEquipment(equipmentItemModel, (EquipmentSource == EquipmentSource.Unknown) ? EquipmentSource.Bundle : EquipmentSource);
		}
		GivenEquipment = equipmentItemModel;
		return equipmentItemModel;
	}

	public EquipmentDefinition GetRandomEquipmentDefinition(TWDModelManager manager, ModelRandom random, out int levelOut)
	{
		levelOut = manager.Player.LootManager.GetEquipmentStartingLevel(StartingLevelOffset, SurvivorClass);
		return manager.Player.Equipment.GetRandomEquipment(Category, levelOut, RarityLevel, useSpecialization: false, Faction.Survivor, SurvivorClass, null, random);
	}
}
