namespace TWDModel
{
	public class Migration6140 : TWDModelMigration
	{
		public Migration6140()
		{
			base.Version = "6.14.0";
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
