namespace TWDModel
{
	public class Migration660 : TWDModelMigration
	{
		public Migration660()
		{
			base.Version = "6.6.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			bool flag = false;
			if (player.Currencies.Find((CurrencyModel x) => x.Type == CurrencyType.JadisToken) == null)
			{
				MigrationUtils.AddNewCurrency(player, manager, CurrencyType.JadisToken);
				flag = true;
			}
			if (flag)
			{
				MigrationUtils.DeleteCombatModel(player);
			}
			if (player.WeeklyChallenge != null)
			{
				player.WeeklyChallenge.UpdateChallengePlayerLeaderboards();
				flag = true;
			}
			if (player.EndlessModeManager != null)
			{
				player.EndlessModeManager.PendingLeaderBoardUpdate = true;
				flag = true;
			}
			return flag;
		}
	}
}
