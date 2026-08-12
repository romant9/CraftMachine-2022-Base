namespace TWDModel
{
	public class Migration7180 : TWDModelMigration
	{
		public Migration7180()
		{
			base.Version = "7.18.0";
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
