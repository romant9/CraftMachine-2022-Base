using System.Collections.Generic;

namespace TWDModel
{
	public class GuildMigration250 : TWDGuildMigration
	{
		public GuildMigration250()
		{
			base.Version = "2.5.0";
		}

		public override bool Migrate(GuildModel guild, TWDModelManager manager)
		{
			List<string> currentChallengeParticipants = guild.CurrentChallengeParticipants;
			for (int i = 0; i < (currentChallengeParticipants?.Count ?? 0); i++)
			{
				string memberId = currentChallengeParticipants[i];
				GuildMemberInfo memberInfo = guild.GetMemberInfo(memberId);
				if (memberInfo != null)
				{
					guild.CurrentChallengeMemberInfos[memberInfo.MemberId] = memberInfo.CurrentChallengeStars;
				}
			}
			return base.Migrate(guild, manager);
		}
	}
}
