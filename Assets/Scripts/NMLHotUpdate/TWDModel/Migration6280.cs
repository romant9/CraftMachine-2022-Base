namespace TWDModel
{
	public class Migration6280 : TWDModelMigration
	{
		public Migration6280()
		{
			base.Version = "6.28.0";
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
