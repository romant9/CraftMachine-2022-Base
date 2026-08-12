namespace TWDModel
{
	public class GuildMigration200 : TWDGuildMigration
	{
		public GuildMigration200()
		{
			base.Version = "2.0.0";
		}

		public override bool Migrate(GuildModel guild, TWDModelManager manager)
		{
			if (guild.JoinType == GuildJoinType.Open)
			{
				guild.JoinType = GuildJoinType.Invite;
			}
			else if (guild.JoinType == GuildJoinType.Invite)
			{
				guild.JoinType = GuildJoinType.Closed;
			}
			return base.Migrate(guild, manager);
		}
	}
}
