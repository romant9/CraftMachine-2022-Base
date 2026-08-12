namespace TWDModel
{
	public class Migration31300 : TWDModelMigration
	{
		public Migration31300()
		{
			base.Version = "3.13.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			return true;
		}
	}
}
