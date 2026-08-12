using BaseModel;

namespace TWDModel
{
	public class UpdateMemberInfoGroupCommand : TWDGroupCommand
	{
		public int NewLevel = -1;

		public string NewName;

		public PlayerEmblem NewEmblem;

		public override GroupCommandBase Execute(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			bool flag = false;
			GuildMemberInfo memberInfo = guildModel.GetMemberInfo(SenderId);
			if (memberInfo == null)
			{
				((TWDModelManager)manager).Debug.LogWarning("Member not found in group. GroupId: " + GroupId + " - SenderId: " + SenderId);
			}
			else
			{
				if (NewLevel != -1)
				{
					memberInfo.PlayerLevel = NewLevel;
					flag = true;
				}
				if (NewName != memberInfo.Name)
				{
					guildModel.AddPlayerNameChangedNotification(memberInfo.MemberId, NewName, memberInfo.Name);
					memberInfo.Name = NewName;
					flag = true;
				}
				if (NewEmblem != null)
				{
					memberInfo.PlayerEmblem = NewEmblem;
					flag = true;
				}
				if (flag)
				{
					guildModel.NotifyChange("GuildModified");
					SaveGroupModel(manager);
				}
			}
			return this;
		}
	}
}
