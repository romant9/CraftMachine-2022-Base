namespace TWDModel
{
	public class Migration120 : TWDModelMigration
	{
		public Migration120()
		{
			base.Version = "1.2.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			if (!player.Tutorial.StaticTutorialComplete)
			{
				throw new MigrationResetRequiredException();
			}
			return true;
		}
	}
}
