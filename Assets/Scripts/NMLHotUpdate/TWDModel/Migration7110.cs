namespace TWDModel
{
	public class Migration7110 : TWDModelMigration
	{
		public Migration7110()
		{
			base.Version = "7.11.0";
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
