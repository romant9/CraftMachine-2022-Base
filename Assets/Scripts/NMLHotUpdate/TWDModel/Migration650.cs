namespace TWDModel
{
	public class Migration650 : TWDModelMigration
	{
		public Migration650()
		{
			base.Version = "6.5.0";
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
