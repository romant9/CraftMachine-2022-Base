namespace TWDModel
{
	public class Migration6120 : TWDModelMigration
	{
		public Migration6120()
		{
			base.Version = "6.12.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (true)
			{
				MigrationUtils.DeleteCombatModel(player);
			}
			return true;
		}
	}
}
