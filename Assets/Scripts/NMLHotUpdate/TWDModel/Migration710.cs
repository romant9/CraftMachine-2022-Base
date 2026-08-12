namespace TWDModel
{
	public class Migration710 : TWDModelMigration
	{
		public Migration710()
		{
			base.Version = "7.1.0";
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
