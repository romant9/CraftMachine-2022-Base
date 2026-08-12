namespace TWDModel
{
	public class Migration580 : TWDModelMigration
	{
		public Migration580()
		{
			base.Version = "5.8.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
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
