using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ModifyMemberRoleGroupCommand : TWDGroupCommand
	{
		public string MemberId;

		public GuildMemberRole NewRole;

		[JsonIgnore]
		public bool IsPromotion;

		public ModifyMemberRoleGroupCommand()
		{
		}

		public ModifyMemberRoleGroupCommand(string memberId)
		{
			MemberId = memberId;
		}

		public override GroupCommandBase Execute(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			if (guildModel != null)
			{
				guildModel.SetMemberRole(MemberId, SenderId, NewRole, ref IsPromotion);
				SaveGroupModel(manager);
			}
			return this;
		}
	}
}
