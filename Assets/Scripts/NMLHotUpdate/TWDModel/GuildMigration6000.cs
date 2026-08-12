namespace TWDModel
{
	public class GuildMigration6000 : TWDGuildMigration
	{
		public GuildMigration6000()
		{
			base.Version = "6.0.0";
		}

		public override bool Migrate(GuildModel guild, TWDModelManager manager)
		{
			guild.NextChangeNameTimeStampSeconds = guild.TimeStamp / 1000;
			return base.Migrate(guild, manager);
		}
	}
}
