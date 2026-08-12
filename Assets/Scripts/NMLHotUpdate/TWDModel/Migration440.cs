namespace TWDModel
{
	public class Migration440 : TWDModelMigration
	{
		public Migration440()
		{
			base.Version = "4.4.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			return true;
		}
	}
}
