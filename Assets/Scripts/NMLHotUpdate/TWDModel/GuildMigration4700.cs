namespace TWDModel
{
	public class GuildMigration4700 : TWDGuildMigration
	{
		public GuildMigration4700()
		{
			base.Version = "4.7.0";
		}

		public override bool Migrate(GuildModel guild, TWDModelManager manager)
		{
			if (guild.GvGSeasonModel != null && guild.GvGSeasonModel.GuildWarModel != null)
			{
				guild.GuildWarModel.CurrentBattle = new GuildBattleModel();
			}
			return base.Migrate(guild, manager);
		}
	}
}
