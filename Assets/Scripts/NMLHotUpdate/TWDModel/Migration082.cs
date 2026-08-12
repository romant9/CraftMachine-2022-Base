namespace TWDModel
{
	public class Migration082 : TWDModelMigration
	{
		public Migration082()
		{
			base.Version = "0.8.2";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (!player.Tutorial.HasCompletedPart("Phone"))
			{
				throw new MigrationResetRequiredException();
			}
			return true;
		}
	}
}
