namespace TWDModel
{
	public class Migration780 : TWDModelMigration
	{
		public Migration780()
		{
			base.Version = "7.8.0";
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
