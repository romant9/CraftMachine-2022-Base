namespace TWDModel
{
	public class Migration6300 : TWDModelMigration
	{
		public Migration6300()
		{
			base.Version = "6.30.0";
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
