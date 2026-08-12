namespace TWDModel
{
	public class Migration550 : TWDModelMigration
	{
		public Migration550()
		{
			base.Version = "5.5.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			return true;
		}
	}
}
