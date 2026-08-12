namespace TWDModel
{
	public class Migration611 : TWDModelMigration
	{
		public Migration611()
		{
			base.Version = "6.1.1";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			return true;
		}
	}
}
