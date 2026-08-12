namespace TWDModel
{
	public class Migration31500 : TWDModelMigration
	{
		public Migration31500()
		{
			base.Version = "3.15.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			return true;
		}
	}
}
