using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class SetExcludeMatchMakingTargetsCommand : ModelCommand
	{
		public List<string> ExcludedHashedIds { get; set; }

		public SetExcludeMatchMakingTargetsCommand()
		{
			ExcludedHashedIds = new List<string>();
		}

		public SetExcludeMatchMakingTargetsCommand(List<string> excludeHashedIds)
			: this()
		{
			if (excludeHashedIds != null && excludeHashedIds.Count > 0)
			{
				ExcludedHashedIds.AddRange(excludeHashedIds);
			}
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel player = (manager as TWDModelManager).Player;
			List<string> list = new List<string>();
			list.AddRange(ExcludedHashedIds);
			if (player.GuildModel != null)
			{
				foreach (GuildMemberInfo guildMember in player.GuildModel.GuildMembers)
				{
					if (guildMember != null && guildMember.MemberId != null)
					{
						list.Add(guildMember.MemberId);
					}
				}
			}
			player.ExcludedMatchMakingTargets = list.ToArray();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
