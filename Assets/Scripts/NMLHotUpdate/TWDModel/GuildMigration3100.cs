namespace TWDModel
{
	public class GuildMigration3100 : TWDGuildMigration
	{
		public GuildMigration3100()
		{
			base.Version = "3.10.0";
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
