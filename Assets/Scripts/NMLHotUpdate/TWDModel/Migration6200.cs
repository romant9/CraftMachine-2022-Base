namespace TWDModel
{
	public class Migration6200 : TWDModelMigration
	{
		public Migration6200()
		{
			base.Version = "6.20.0";
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
