namespace TWDModel
{
	public class Migration670 : TWDModelMigration
	{
		public Migration670()
		{
			base.Version = "6.7.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (true)
			{
				MigrationUtils.DeleteCombatModel(player);
			}
			return true;
		}
	}
}
