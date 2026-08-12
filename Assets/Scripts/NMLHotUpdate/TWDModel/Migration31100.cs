namespace TWDModel
{
	public class Migration31100 : TWDModelMigration
	{
		public Migration31100()
		{
			base.Version = "3.11.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			return true;
		}
	}
}
