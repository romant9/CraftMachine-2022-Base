using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class SupportsGiveConsumablesLoadQueueMessage : SupportLoadQueueMessage
	{
		public List<SupportGiveConsumablesEntry> Consumables { get; set; }

		public override bool Execute(TWDModelManager manager)
		{
			foreach (SupportGiveConsumablesEntry consumable in Consumables)
			{
				int addValue = consumable.AddValue;
				if (addValue > 0)
				{
					EquipmentDefinition equipmentDefinition = Array.Find(manager.GameEconomyData.EquipmentDefinitions, (EquipmentDefinition x) => x.ID == ConsumableUtils.ConsumableTypeToId(consumable.ConsumableType));
					EquipmentItemModel equipmentItemModel = manager.Player.Equipment.GenerateAndInitializeEquipmentFromDefinition(equipmentDefinition.ID, 1, 1);
					for (int num = 0; num < addValue; num++)
					{
						if (equipmentItemModel != null)
						{
							manager.Player.Equipment.AddEquipment(equipmentItemModel, EquipmentSource.Support);
						}
						else
						{
							manager.Debug.LogError("Could not add consumable, equipment not found " + consumable.ConsumableType);
						}
						manager.Metrics.AddFind().AddEquipment(equipmentItemModel).AddSupport(base.SupportGivenTimestamp, base.SupportEntityGUID)
							.Send();
					}
					continue;
				}
				int num2 = Math.Min(manager.Player.Equipment.GetConsumablesOfType(consumable.ConsumableType).Count, Math.Abs(consumable.AddValue));
				EquipmentItemModel equipmentItemModel2 = manager.Player.Equipment.Consumables.Models.Find((EquipmentItemModel x) => x.Definition.ID == ConsumableUtils.ConsumableTypeToId(consumable.ConsumableType));
				for (int num3 = 0; num3 < num2; num3++)
				{
					if (equipmentItemModel2 != null)
					{
						manager.Player.Equipment.Consumables.Remove(equipmentItemModel2);
						foreach (CombatBackup combatBackup in manager.Player.CombatBackups)
						{
							combatBackup.Consumables.Remove(equipmentItemModel2);
						}
					}
					else
					{
						manager.Debug.LogError("Could not add consumable, equipment not found " + consumable.ConsumableType);
					}
					manager.Metrics.AddRemove().AddEquipment(equipmentItemModel2).AddSupport(base.SupportGivenTimestamp, base.SupportEntityGUID)
						.Send();
				}
			}
			return true;
		}
	}
}
