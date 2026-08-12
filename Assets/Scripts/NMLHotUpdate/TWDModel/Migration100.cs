using System.Collections.Generic;

namespace TWDModel
{
	public class Migration100 : TWDModelMigration
	{
		public Migration100()
		{
			base.Version = "1.0.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			if (!player.Tutorial.StaticTutorialComplete)
			{
				throw new MigrationResetRequiredException();
			}
			if (player.SurvivorContainer != null)
			{
				foreach (SurvivorModel survivor in player.SurvivorContainer.Survivors)
				{
					survivor.CombatCleanup();
				}
			}
			if (player.LootManager != null)
			{
				if (player.LootManager.LootKeysSources == null)
				{
					player.LootManager.LootKeysSources = new List<LootKeySource>();
				}
				if (player.LootManager.DropCummulativeProbabilities == null)
				{
					player.LootManager.DropCummulativeProbabilities = new List<LootCummulativeProbabilityEntry>();
				}
			}
			return true;
		}
	}
}
