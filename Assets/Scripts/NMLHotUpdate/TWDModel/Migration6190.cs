namespace TWDModel
{
	public class Migration6190 : TWDModelMigration
	{
		public Migration6190()
		{
			base.Version = "6.19.0";
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
