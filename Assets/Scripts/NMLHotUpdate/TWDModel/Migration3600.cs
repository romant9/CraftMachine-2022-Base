namespace TWDModel
{
	public class Migration3600 : TWDModelMigration
	{
		public Migration3600()
		{
			base.Version = "3.6.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			return true;
		}
	}
}
