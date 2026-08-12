namespace TWDModel
{
	public class Migration760 : TWDModelMigration
	{
		public Migration760()
		{
			base.Version = "7.6.0";
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
