using BaseModel;

namespace TWDModel
{
	public class SetMemberGvGInfoGroupCommand : TWDGroupCommand
	{
		public string MemberId { get; private set; }

		public int TotalVPPoints { get; private set; }

		public SetMemberGvGInfoGroupCommand(string memberId, int totalVPPoints)
		{
			MemberId = memberId;
			TotalVPPoints = totalVPPoints;
		}

		public override GroupCommandBase Execute(ModelManager manager)
		{
			if (((GuildModel)manager.GetGroupModel(GroupId)).UpdateTotalVp(MemberId, TotalVPPoints) == TWDModelResult.OK)
			{
				SaveGroupModel(manager);
			}
			return this;
		}
	}
}
