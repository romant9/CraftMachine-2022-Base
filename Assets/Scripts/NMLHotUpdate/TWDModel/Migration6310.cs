namespace TWDModel
{
	public class Migration6310 : TWDModelMigration
	{
		public Migration6310()
		{
			base.Version = "6.31.0";
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
