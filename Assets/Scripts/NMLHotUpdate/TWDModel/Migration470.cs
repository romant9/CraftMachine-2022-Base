using System.Collections.Generic;

namespace TWDModel
{
	public class Migration470 : TWDModelMigration
	{
		public Migration470()
		{
			base.Version = "4.7.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.AssassinCarolToken, CurrencyType.HwachaToken, CurrencyType.CarolsCookiesToken);
			EndlessModeManagerModel endlessModeManager = player.EndlessModeManager;
			if (endlessModeManager != null)
			{
				endlessModeManager.CurrentExpertModeHeroes = new List<EndlessModeExpertModeHeroDefinition>();
			}
			return true;
		}
	}
}
