namespace TWDModel
{
	public class Migration590 : TWDModelMigration
	{
		public Migration590()
		{
			base.Version = "5.9.0";
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
