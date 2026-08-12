using System.Collections.Generic;

namespace TWDModel
{
	public class Migration2110 : TWDModelMigration
	{
		public Migration2110()
		{
			base.Version = "2.11.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			if (player.Blackboard != null && player.Blackboard.IsToggleOn("Toggle.ToggleUpdateInfoPopupShown"))
			{
				player.Blackboard.ClearToggle("Toggle.ToggleUpdateInfoPopupShown");
			}
			player.CampaignModel = new CampaignModel();
			player.CampaignModel.SetManager(manager);
			player.CampaignModel.Initialize();
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.CampaignToken);
			player.GdprActions = new Dictionary<string, TimestampedActionResult>();
			return true;
		}
	}
}
