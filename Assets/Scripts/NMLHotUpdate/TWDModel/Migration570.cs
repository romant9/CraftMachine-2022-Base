namespace TWDModel
{
	public class Migration570 : TWDModelMigration
	{
		public Migration570()
		{
			base.Version = "5.7.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			return true;
		}
	}
}
