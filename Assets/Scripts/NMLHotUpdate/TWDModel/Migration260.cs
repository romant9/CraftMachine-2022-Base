using BaseModel;

namespace TWDModel
{
	public class Migration260 : TWDModelMigration
	{
		public Migration260()
		{
			base.Version = "2.6.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.MerleToken, CurrencyType.GovernorToken);
			FixChargeEquipmentLevel(player.Equipment.MeleeWeapons);
			FixChargeEquipmentLevel(player.Equipment.RangeWeapons);
			if (string.IsNullOrEmpty(player.Country))
			{
				player.Country = "US";
			}
			if (player.Combat != null && player.Combat.OutpostCombat != null && string.IsNullOrEmpty(player.Combat.OutpostCombat.DefenderCountry))
			{
				player.Combat.OutpostCombat.DefenderCountry = "US";
			}
			FixPendingIAPMissingProduct(player, manager);
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in manager.GameEconomyData.MissionSpawnPointData.MissionSpawnPointGroups)
			{
				player.MapContainerModel.SpawnMissionGroup(missionSpawnPointGroup);
			}
			player.MapContainerModel.SpawnSeasonEpisodes();
			if (player.Blackboard.IsToggleOn("Toggle.ToggleUpdateInfoPopupShown"))
			{
				player.Blackboard.ClearToggle("Toggle.ToggleUpdateInfoPopupShown");
			}
			return true;
		}

		private void FixPendingIAPMissingProduct(PlayerModel player, TWDModelManager manager)
		{
			if (player.PendingIAPs == null)
			{
				return;
			}
			for (int i = 0; i < player.PendingIAPs.Count; i++)
			{
				PendingPurchaseInfo pendingPurchaseInfo = player.PendingIAPs[i];
				if (pendingPurchaseInfo != null && pendingPurchaseInfo.Transaction != null)
				{
					SubmitReceiptCommand.VerifyBundleId(pendingPurchaseInfo, manager);
					SubmitReceiptCommand.FillProduct(pendingPurchaseInfo, pendingPurchaseInfo.Product, manager);
				}
			}
		}

		private void FixChargeEquipmentLevel(ModelList<EquipmentItemModel> equipmentList)
		{
			for (int i = 0; i < (equipmentList?.Count ?? 0); i++)
			{
				EquipmentItemModel equipmentItemModel = equipmentList[i];
				if (equipmentItemModel != null && !equipmentItemModel.IsChargeEquipment && equipmentItemModel.ChargeEquipment != null)
				{
					int level = equipmentItemModel.Level;
					EquipmentItemModel chargeEquipment = equipmentItemModel.ChargeEquipment;
					if (chargeEquipment != null && chargeEquipment.Level != level)
					{
						chargeEquipment.Level = level;
					}
				}
			}
		}
	}
}
