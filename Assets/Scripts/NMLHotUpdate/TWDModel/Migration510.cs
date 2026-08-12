using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class Migration510 : TWDModelMigration
	{
		public Migration510()
		{
			base.Version = "5.1.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.HunterHershelToken, CurrencyType.TyreeseToken, CurrencyType.WalkerMikeToken);
			foreach (SurvivorModel survivor in player.SurvivorContainer.Survivors)
			{
				foreach (HeroSkinDefinition item in manager.GameEconomyData.HeroSkinDefinitions.Where((HeroSkinDefinition x) => x.HeroID == survivor.ActorDefinitionID && x.AvailableOnHeroPurchased))
				{
					player.SurvivorContainer.AddHeroSkin(item.ID);
				}
			}
			if (player != null && player.BundleManager != null && !string.IsNullOrEmpty(player.BundleManager.ViewHeroSkin))
			{
				if (player.BundleManager.PendingViewHeroSkins == null)
				{
					player.BundleManager.PendingViewHeroSkins = new List<string>();
				}
				player.BundleManager.PendingViewHeroSkins.Add(player.BundleManager.ViewHeroSkin);
				player.BundleManager.ViewHeroSkin = null;
			}
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in manager.GameEconomyData.MissionSpawnPointData.MissionSpawnPointGroups)
			{
				player.MapContainerModel.SpawnMissionGroup(missionSpawnPointGroup);
			}
			player.MapContainerModel.SpawnSeasonEpisodes();
			return true;
		}
	}
}
