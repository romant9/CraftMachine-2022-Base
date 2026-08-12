namespace TWDModel
{
	public class Migration6260 : TWDModelMigration
	{
		public Migration6260()
		{
			base.Version = "6.26.0";
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
