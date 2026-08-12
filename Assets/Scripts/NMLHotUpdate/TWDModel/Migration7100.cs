namespace TWDModel
{
	public class Migration7100 : TWDModelMigration
	{
		public Migration7100()
		{
			base.Version = "7.10.0";
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
