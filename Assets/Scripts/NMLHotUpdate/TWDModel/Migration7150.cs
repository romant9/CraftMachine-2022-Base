namespace TWDModel
{
	public class Migration7150 : TWDModelMigration
	{
		public Migration7150()
		{
			base.Version = "7.15.0";
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
