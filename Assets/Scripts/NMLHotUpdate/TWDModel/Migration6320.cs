namespace TWDModel
{
	public class Migration6320 : TWDModelMigration
	{
		public Migration6320()
		{
			base.Version = "6.32.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (false)
			{
				MigrationUtils.DeleteCombatModel(player);
			}
			return false;
		}
	}
}
