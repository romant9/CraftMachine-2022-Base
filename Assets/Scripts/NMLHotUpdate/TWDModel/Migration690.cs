namespace TWDModel
{
	public class Migration690 : TWDModelMigration
	{
		public Migration690()
		{
			base.Version = "6.9.0";
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
