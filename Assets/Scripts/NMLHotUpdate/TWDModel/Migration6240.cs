namespace TWDModel
{
	public class Migration6240 : TWDModelMigration
	{
		public Migration6240()
		{
			base.Version = "6.24.0";
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
