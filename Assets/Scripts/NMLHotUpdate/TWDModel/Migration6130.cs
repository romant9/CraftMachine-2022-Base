using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class Migration6130 : TWDModelMigration
	{
		public Migration6130()
		{
			base.Version = "6.13.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.ApocalypticEquipToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.ApocalypticEquipToken);
				CurrencyModel currency = player.GetCurrency(CurrencyType.ApocalypticEquipToken);
				int num = 0;
				if (player.EquipTokenContainer == null)
				{
					player.EquipTokenContainer = new EquipTokenContainerModel();
					player.EquipTokenContainer.SetManager(manager);
					player.EquipTokenContainer.Initialize();
				}
				if (player.EquipTokenContainer.EquipTokenItems == null)
				{
					player.EquipTokenContainer.EquipTokenItems = new ModelList<EquipTokenItemModel>();
					player.EquipTokenContainer.EquipTokenItems.SetManager(manager);
					player.EquipTokenContainer.EquipTokenItems.Initialize();
				}
				foreach (EquipTokenItemModel equipTokenItem in player.EquipTokenContainer.EquipTokenItems)
				{
					num += equipTokenItem.OwnedTokensAmount;
					player.EquipTokenContainer.AddEquipToken(equipTokenItem.EquipTokenId, -equipTokenItem.OwnedTokensAmount);
				}
				currency.SetValue(num);
				foreach (string item in manager.GameEconomyData.ConfigData.bufatuzhi)
				{
					player.EquipTokenContainer.AddEquipToken(item, 1, isMigrate: true);
				}
				flag = true;
			}
			if (player.WeeklyChallenge.AppendDifficultyEffect == null)
			{
				player.WeeklyChallenge.AppendDifficultyEffect = new List<IncrementalDifficultyEffectDefinition>();
				player.WeeklyChallenge.AppendDifficultyEffect.AddRange(manager.GameEconomyData.GetDifficultyEffects(IncrementalDifficultyMissionType.ThreatMission, 20));
				player.WeeklyChallenge.RerollApocalypseBuffCount = 0;
				player.WeeklyChallenge.PendingSelectApocalypseBuffs = new List<WeeklyChallengeApocalypseBuff>();
				player.WeeklyChallenge.weeklyChallengeApocalypseBuffs = new List<WeeklyChallengeApocalypseBuff>();
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.EquipTraitsRemodelToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.EquipTraitsRemodelToken);
				flag = true;
			}
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.ProtectorDarylToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.ProtectorDarylToken);
				flag = true;
			}
			if (player.SurvivorContainer.SurvivalCharacters.SurvivalModelShieldStates == null)
			{
				player.SurvivorContainer.SurvivalCharacters.SurvivalModelShieldStates = new List<SurvivalCharacterShieldStateModel>();
				for (int num2 = 0; num2 < player.SurvivorContainer.Survivors.Count; num2++)
				{
					player.SurvivorContainer.SurvivalCharacters.SurvivalModelShieldStates.Add(new SurvivalCharacterShieldStateModel
					{
						MaxShieldPoints = 0,
						ShieldPoints = 0
					});
				}
				flag = true;
			}
			if (flag)
			{
				MigrationUtils.DeleteCombatModel(player);
			}
			return flag;
		}
	}
}
