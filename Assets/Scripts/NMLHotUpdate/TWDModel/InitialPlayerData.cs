using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	[Serializable]
	public class InitialPlayerData
	{
		public const int RandomSeed = 599672118;

		public static void CreatePlayerModel(TWDModelManager manager)
		{
			ModelRandom modelRandom = new ModelRandom();
			PlayerModel player = manager.Player;
			player.SetManager(manager);
			player.Initialize();
			if (string.IsNullOrEmpty(player.Country))
			{
				player.Country = "US";
			}
			player.SetName("");
			player.GetCurrency(CurrencyType.Diamonds).SetValue(manager.Player.gameEconomyData.ConfigData.InitialDiamonds);
			player.GetCurrency(CurrencyType.Supplies).SetCapacity(player.GetCapacity(CurrencyType.Supplies));
			player.GetCurrency(CurrencyType.Gas).SetCapacity(player.GetCapacity(CurrencyType.Gas));
			player.GetCurrency(CurrencyType.SurvivalPoints).SetCapacity(player.GetCapacity(CurrencyType.SurvivalPoints));
			player.GetCurrency(CurrencyType.Inhabitants).SetCapacity(player.GetCapacity(CurrencyType.Inhabitants));
			player.SurvivorContainer.UnlockSurvivorClass(SurvivorClass.Scout);
			player.SurvivorContainer.UnlockSurvivorClass(SurvivorClass.Bruiser);
			player.SurvivorContainer.UnlockSurvivorClass(SurvivorClass.None);
			List<string> initialSurvivors = manager.Player.gameEconomyData.ConfigData.InitialSurvivors;
			for (int i = 0; i < initialSurvivors.Count; i++)
			{
				if (!player.SurvivorContainer.CanAddSurvivor())
				{
					break;
				}
				SurvivorModel survivorModel = player.SurvivorContainer.SetupInitialSurvivor(initialSurvivors[i], modelRandom);
				player.SurvivorContainer.AddSurvivor(survivorModel);
				List<EquipmentSetupData> initialEquipmentsData = survivorModel.Definition.InitialEquipmentsData;
				if (initialEquipmentsData != null && initialEquipmentsData.Count > 0)
				{
					foreach (EquipmentSetupData item in initialEquipmentsData)
					{
						int startingLevel = 1;
						if (item.MinTier > 0 && item.MaxTier > 0)
						{
							startingLevel = modelRandom.GetRandomInRange(item.MinTier, item.MaxTier);
						}
						EquipmentItemModel equipmentItemModel = player.Equipment.GenerateAndInitializeEquipmentFromDefinition(item.ID, item.RarityLevel, startingLevel, modelRandom);
						player.Equipment.AddEquipment(equipmentItemModel, EquipmentSource.Survivor);
						survivorModel.Equip(equipmentItemModel);
					}
				}
				player.SurvivorContainer.AddSurvivorToCombat(survivorModel);
			}
			player.SurvivorContainer.StoreCombatTeam(SurvivorContainerModel.SurvivorType.Combat);
			player.CampMover.PlayerCamp = player.Camp;
			player.CampMover.Move(0, 0, player);
		}
	}
}
