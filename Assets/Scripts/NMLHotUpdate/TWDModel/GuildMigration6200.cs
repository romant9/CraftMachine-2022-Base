namespace TWDModel
{
	public class GuildMigration6200 : TWDGuildMigration
	{
		public GuildMigration6200()
		{
			base.Version = "6.2.0";
		}

		public override bool Migrate(GuildModel guild, TWDModelManager manager)
		{
			if (guild.GvGSeasonModel != null && guild.GvGSeasonModel.SeasonDefinitionId == 31)
			{
				guild.GvGSeasonModel.SeasonDefinitionId = 30;
				guild.GvGSeasonModel.CurrentSeasonStats.CurrentVictoryPoints *= 4;
				guild.GvGSeasonModel.GuildWarModel.WarDefinitionId = 100;
			}
			return base.Migrate(guild, manager);
		}
	}
}
