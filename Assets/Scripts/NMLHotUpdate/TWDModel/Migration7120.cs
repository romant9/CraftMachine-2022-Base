namespace TWDModel
{
	public class Migration7120 : TWDModelMigration
	{
		public Migration7120()
		{
			base.Version = "7.12.0";
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
