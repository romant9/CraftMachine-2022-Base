namespace TWDModel
{
	public class Migration540 : TWDModelMigration
	{
		public Migration540()
		{
			base.Version = "5.4.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.BruiserRositaToken);
			player.BeginnerBattlePassInfo.State = BeginnerBattlePassState.NotStarted;
			if (player.CouncilLevel >= manager.GameEconomyData.BattlePassConfig.CouncilLockLevel)
			{
				player.BeginnerBattlePassInfo.State = BeginnerBattlePassState.Skipped;
			}
			player.SetGdprAction("PrivacyPolicyChanged", new TimestampedActionResult
			{
				Accepted = false,
				Timestamp = 0L,
				ActionTaken = false
			});
			return true;
		}
	}
}
