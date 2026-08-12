namespace TWDModel
{
	public class Migration7160 : TWDModelMigration
	{
		public Migration7160()
		{
			base.Version = "7.16.0";
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
