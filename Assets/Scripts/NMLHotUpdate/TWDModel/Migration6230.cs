namespace TWDModel
{
	public class Migration6230 : TWDModelMigration
	{
		public Migration6230()
		{
			base.Version = "6.23.0";
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
