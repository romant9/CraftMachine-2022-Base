namespace TWDModel
{
	public class Migration720 : TWDModelMigration
	{
		public Migration720()
		{
			base.Version = "7.2.0";
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
