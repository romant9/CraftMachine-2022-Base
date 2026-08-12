namespace TWDModel
{
	public class Migration31700 : TWDModelMigration
	{
		public Migration31700()
		{
			base.Version = "3.17.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			return true;
		}
	}
}
