using System.Collections.Generic;

namespace TWDModel
{
	public class Migration560 : TWDModelMigration
	{
		public Migration560()
		{
			base.Version = "5.6.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.ConnieToken);
			if (player.SurvivorContainer.HeroSkinsOwned == null)
			{
				player.SurvivorContainer.HeroSkinsOwned = new List<string>();
			}
			return true;
		}
	}
}
