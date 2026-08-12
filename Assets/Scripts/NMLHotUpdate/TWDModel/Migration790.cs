namespace TWDModel
{
	public class Migration790 : TWDModelMigration
	{
		public Migration790()
		{
			base.Version = "7.9.0";
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
