namespace TWDModel
{
	public class Migration6290 : TWDModelMigration
	{
		public Migration6290()
		{
			base.Version = "6.29.0";
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
