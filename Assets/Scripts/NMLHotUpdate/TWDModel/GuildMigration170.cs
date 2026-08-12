namespace TWDModel
{
	public class GuildMigration170 : TWDGuildMigration
	{
		public GuildMigration170()
		{
			base.Version = "1.7.0";
		}

		public override bool Migrate(GuildModel guild, TWDModelManager manager)
		{
			for (int i = 0; i < guild.GuildMembers.Count; i++)
			{
				GuildMemberInfo guildMemberInfo = guild.GuildMembers[i];
				if (guildMemberInfo.TotalChallengeStarsAtChallengeStart == 0)
				{
					guildMemberInfo.TotalChallengeStarsAtChallengeStart = guildMemberInfo.TotalChallengeStars - guildMemberInfo.CurrentChallengeStars;
				}
			}
			return base.Migrate(guild, manager);
		}
	}
}
