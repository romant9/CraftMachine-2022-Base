namespace TWDModel
{
	public class Migration530 : TWDModelMigration
	{
		public Migration530()
		{
			base.Version = "5.3.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			return true;
		}
	}
}
