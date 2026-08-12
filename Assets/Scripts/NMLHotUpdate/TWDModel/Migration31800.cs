namespace TWDModel
{
	public class Migration31800 : TWDModelMigration
	{
		public Migration31800()
		{
			base.Version = "3.18.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			return true;
		}
	}
}
