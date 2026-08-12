namespace TWDModel
{
	public class Migration6100 : TWDModelMigration
	{
		public Migration6100()
		{
			base.Version = "6.10.0";
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
