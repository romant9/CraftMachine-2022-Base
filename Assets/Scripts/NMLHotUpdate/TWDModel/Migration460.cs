namespace TWDModel
{
	public class Migration460 : TWDModelMigration
	{
		public Migration460()
		{
			base.Version = "4.6.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			return true;
		}
	}
}
