using BaseModel;

namespace TWDModel
{
	public class Migration630 : TWDModelMigration
	{
		public Migration630()
		{
			base.Version = "6.3.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.BuildingToken10min) == null)
			{
				MigrationUtils.DeleteCombatModel(player);
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.BuildingToken10min, CurrencyType.BuildingToken1h, CurrencyType.BuildingToken6h, CurrencyType.BuildingToken12h, CurrencyType.BuildingToken24h, CurrencyType.TrainingToken20min, CurrencyType.TrainingToken1h, CurrencyType.TrainingToken3h, CurrencyType.TrainingToken8h, CurrencyType.TrainingToken16h, CurrencyType.EquipmentToken20min, CurrencyType.EquipmentToken1h, CurrencyType.EquipmentToken3h, CurrencyType.EquipmentToken7h, CurrencyType.EquipmentToken14h, CurrencyType.HealingToken10min, CurrencyType.HealingToken1h, CurrencyType.HealingToken2h, CurrencyType.HealingToken4h);
				flag = true;
			}
			if (player.Equipment != null && player.Equipment.Armors != null)
			{
				foreach (EquipmentItemModel armor in player.Equipment.Armors)
				{
					if (!(armor.EquipmentDefinitionIdentifier == "Armor_Assault_LeatherJacketB_bp10"))
					{
						continue;
					}
					foreach (UpgradeTraitsData upgradeTrait in armor.UpgradeTraits)
					{
						if (upgradeTrait.Identifier == "Equipment.Tactical.Level1")
						{
							upgradeTrait.Identifier = "Equipment.ArmorTactical.Level1";
							flag = true;
						}
						if (upgradeTrait.Identifier == "Equipment.Tactical.Level0")
						{
							upgradeTrait.Identifier = "Equipment.ArmorTactical.Level0";
							flag = true;
						}
						if (upgradeTrait.Identifier == "Equipment.Tactical.Level2")
						{
							upgradeTrait.Identifier = "Equipment.ArmorTactical.Level2";
							flag = true;
						}
					}
				}
				if (flag)
				{
					MigrationUtils.DeleteCombatModel(player);
				}
			}
			if (player.Equipment != null && player.Equipment.BounsModes == null)
			{
				player.Equipment.MigrateBounsForOldPlayers();
				flag = true;
			}
			if (player.BundleManager.ShareRewardEntrys == null)
			{
				player.BundleManager.ShareRewardEntrys = new ModelList<LootEntry>();
				player.BundleManager.ShareRewardEntrys.SetManager(manager);
				player.BundleManager.ShareRewardEntrys.Initialize();
				flag = true;
			}
			if (player.ShareManagerModel == null)
			{
				player.ShareManagerModel = new ShareManagerModel();
				player.ShareManagerModel.SetManager(manager);
				player.ShareManagerModel.Initialize();
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.MagnaToken) == null)
			{
				MigrationUtils.DeleteCombatModel(player);
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.MagnaToken);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.BounsItem) == null)
			{
				MigrationUtils.DeleteCombatModel(player);
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.BounsItem);
				flag = true;
			}
			return flag;
		}
	}
}
