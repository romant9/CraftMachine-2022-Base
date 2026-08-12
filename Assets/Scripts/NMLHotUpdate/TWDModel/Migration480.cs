namespace TWDModel
{
	public class Migration480 : TWDModelMigration
	{
		public Migration480()
		{
			base.Version = "4.8.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			return true;
		}
	}
}
