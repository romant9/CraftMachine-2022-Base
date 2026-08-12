using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class Migration080 : TWDModelMigration
	{
		public Migration080()
		{
			base.Version = "0.8.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			foreach (SurvivorModel survivor in player.SurvivorContainer.Survivors)
			{
				List<TraitEntry> list = new List<TraitEntry>();
				foreach (TraitEntry trait in survivor.GetTraits())
				{
					string traitIdentifier = trait.TraitIdentifier;
					if (manager.GameEconomyData.GetTraitDefinition(traitIdentifier) == null)
					{
						UpgradeTraitsData upgradeTraitsData = FindUpgradeTraitsData(survivor, traitIdentifier);
						if (upgradeTraitsData == null)
						{
							continue;
						}
						trait.TraitIdentifier = trait.TraitIdentifier + "." + Enum.GetName(typeof(TraitBucketsDefinition.BucketType), upgradeTraitsData.BucketType);
						if (manager.GameEconomyData.GetTraitDefinition(trait.TraitIdentifier) == null)
						{
							manager.Debug.LogWarning("Failed to find trait " + trait.TraitIdentifier);
							continue;
						}
					}
					list.Add(trait);
				}
				survivor.ReplaceTraits(list);
			}
			foreach (EquipmentItemModel model in player.Equipment.MeleeWeapons.Models)
			{
				MigrateItemTraits(model, manager);
			}
			foreach (EquipmentItemModel model2 in player.Equipment.RangeWeapons.Models)
			{
				MigrateItemTraits(model2, manager);
			}
			Dictionary<SurvivorModel, EquipmentItemModel> dictionary = new Dictionary<SurvivorModel, EquipmentItemModel>();
			ModelList<EquipmentItemModel> modelList = new ModelList<EquipmentItemModel>();
			foreach (EquipmentItemModel model3 in player.Equipment.Armors.Models)
			{
				SurvivorModel survivorModel2 = model3.Owner as SurvivorModel;
				SurvivorClass survivorClass = survivorModel2?.SurvivorClass ?? SurvivorClass.Scout;
				EquipmentItemModel equipmentItemModel = manager.Player.Equipment.GenerateRandomEquipment(EquipmentCategory.Armor, model3.Level, (int)model3.Rarity, useSpecialization: false, Faction.Survivor, survivorClass);
				if (equipmentItemModel != null)
				{
					if (survivorModel2 != null)
					{
						dictionary.Add(survivorModel2, equipmentItemModel);
					}
					modelList.Add(equipmentItemModel);
				}
			}
			player.Equipment.ReplaceEquipmentList(EquipmentCategory.Armor, modelList);
			foreach (SurvivorModel survivor2 in player.SurvivorContainer.Survivors)
			{
				if (dictionary.ContainsKey(survivor2))
				{
					survivor2.MigrateToNewArmor(dictionary[survivor2]);
				}
			}
			player.Camp.CampDefenseModel.Walkers.Clear();
			return base.Migrate(player, manager);
		}

		private void MigrateItemTraits(EquipmentItemModel item, TWDModelManager manager)
		{
			List<UpgradeTraitsData> list = new List<UpgradeTraitsData>();
			foreach (UpgradeTraitsData upgradeTrait in item.UpgradeTraits)
			{
				if (upgradeTrait.BucketType == TraitBucketsDefinition.BucketType.Tactical)
				{
					list.Add(upgradeTrait);
					continue;
				}
				upgradeTrait.Identifier = upgradeTrait.Identifier + "." + Enum.GetName(typeof(TraitBucketsDefinition.BucketType), upgradeTrait.BucketType);
				if (manager.GameEconomyData.GetTraitDefinition(upgradeTrait.Identifier) == null)
				{
					manager.Debug.LogWarning("Failed to find trait " + upgradeTrait.Identifier);
				}
				else
				{
					list.Add(upgradeTrait);
				}
			}
			item.UpgradeTraits = list;
		}

		private EquipmentItemModel CreateNewArmor()
		{
			return null;
		}

		private UpgradeTraitsData FindUpgradeTraitsData(SurvivorModel survivor, string traitId)
		{
			for (int i = 0; i < survivor.UpgradeTraits.Count; i++)
			{
				UpgradeTraitsData upgradeTraitsData = survivor.UpgradeTraits[i];
				if (traitId == upgradeTraitsData.Identifier)
				{
					return upgradeTraitsData;
				}
			}
			return null;
		}
	}
}
