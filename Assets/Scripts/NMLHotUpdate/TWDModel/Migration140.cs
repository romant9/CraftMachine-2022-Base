namespace TWDModel
{
	public class Migration140 : TWDModelMigration
	{
		public Migration140()
		{
			base.Version = "1.4.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			return true;
		}
	}
}
