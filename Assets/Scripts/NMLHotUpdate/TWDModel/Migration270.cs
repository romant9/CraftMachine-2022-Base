using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class Migration270 : TWDModelMigration
	{
		public Migration270()
		{
			base.Version = "2.7.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			player.SurvivorContainer.SurvivalCharacters = new SurvivalCharacterContainerModel();
			player.SurvivorContainer.SurvivalCharacters.SetManager(manager);
			player.SurvivorContainer.SurvivalCharacters.Initialize();
			player.SurvivorContainer.SavedSurvivalModeCombatTeam = new List<SurvivorModel>();
			player.SurvivorContainer.SavedCombatTeam = new List<SurvivorModel>();
			player.SurvivorContainer.StoreCombatTeam(SurvivorContainerModel.SurvivorType.Combat);
			player.SavedSurvivalMissionData = new SurvivalSavedMissionModel();
			player.SavedSurvivalMissionData.SetManager(manager);
			player.SavedSurvivalMissionData.Initialize();
			player.WeeklySurvival = new WeeklySurvivalModel();
			player.WeeklySurvival.SetManager(manager);
			player.WeeklySurvival.Initialize();
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			string[] array = new string[5] { "Metal", "Badge", "Cloth", "Chemicals", "Food" };
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					CurrencyModel currencyModel = new CurrencyModel(player.GetComponentCurrencyType(array[i], j));
					currencyModel.SetManager(manager);
					currencyModel.AddMultiplier = 1L;
					player.Currencies.Add(currencyModel);
				}
			}
			player.UpdateCurrenciesCapacity();
			RunSurvivorBadgeMigration(player);
			if (player.Equipment != null)
			{
				player.Equipment.MigrateBadgesForOldPlayers();
			}
			return true;
		}

		public static bool RunSurvivorBadgeMigration(PlayerModel player)
		{
			bool flag = false;
			if (player != null)
			{
				ModelList<SurvivorModel> survivors = player.SurvivorContainer.Survivors;
				for (int i = 0; i < (survivors?.Count ?? 0); i++)
				{
					SurvivorModel survivorModel = survivors[i];
					if (survivorModel != null)
					{
						flag |= survivorModel.CreateBadgeContainerForOldPlayers();
					}
				}
				if (player.PhoneCall != null)
				{
					flag |= MigrateGeneratedSurvivorsToBadges(player.PhoneCall.LootsList);
				}
				if (player.LootManager != null)
				{
					flag |= MigrateGeneratedSurvivorsToBadges(player.LootManager.Loots);
				}
				if (player.Combat != null && player.Combat.ExtraSurvivors != null)
				{
					for (int j = 0; j < player.Combat.ExtraSurvivors.Count; j++)
					{
						SurvivorModel survivorModel = ((player.Combat.ExtraSurvivors[j] != null) ? (player.Combat.ExtraSurvivors[j] as SurvivorModel) : null);
						if (survivorModel != null)
						{
							flag |= survivorModel.CreateBadgeContainerForOldPlayers();
						}
					}
				}
				if (player.SurvivorContainer.StoryTeller != null)
				{
					flag |= player.SurvivorContainer.StoryTeller.CreateBadgeContainerForOldPlayers();
				}
				if (player.SurvivorContainer.StoryTeller2 != null)
				{
					flag |= player.SurvivorContainer.StoryTeller2.CreateBadgeContainerForOldPlayers();
				}
			}
			return flag;
		}

		private static bool MigrateGeneratedSurvivorsToBadges(ModelList<LootEntry> lootsList)
		{
			bool flag = false;
			if (lootsList != null)
			{
				for (int i = 0; i < lootsList.Count; i++)
				{
					if (lootsList[i] != null && lootsList[i].GeneratedSurvivor != null)
					{
						flag |= lootsList[i].GeneratedSurvivor.CreateBadgeContainerForOldPlayers();
					}
				}
			}
			return flag;
		}
	}
}
