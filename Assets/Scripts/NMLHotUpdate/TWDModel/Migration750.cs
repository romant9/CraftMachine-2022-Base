namespace TWDModel
{
	public class Migration750 : TWDModelMigration
	{
		public Migration750()
		{
			base.Version = "7.5.0";
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
