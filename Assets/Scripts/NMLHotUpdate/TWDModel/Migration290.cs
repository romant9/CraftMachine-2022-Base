namespace TWDModel
{
	public class Migration290 : TWDModelMigration
	{
		public Migration290()
		{
			base.Version = "2.9.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			if (player.Blackboard != null && player.Blackboard.IsToggleOn("Toggle.ToggleUpdateInfoPopupShown"))
			{
				player.Blackboard.ClearToggle("Toggle.ToggleUpdateInfoPopupShown");
			}
			player.DailyQuestManager = new DailyQuestModel();
			player.DailyQuestManager.SetManager(manager);
			player.DailyQuestManager.Initialize();
			return true;
		}
	}
}
