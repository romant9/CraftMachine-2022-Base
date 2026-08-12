using BaseModel;

namespace TWDModel
{
	public class UpdateLiveDataGroupCommand : TWDGroupCommand
	{
		public long Timestamp;

		public string UniqueMissionId { get; set; }

		public int Attacks { get; set; }

		public override GroupCommandBase Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			GuildModel guildModel = (GuildModel)tWDModelManager.GetGroupModel(GroupId);
			if (guildModel == null)
			{
				manager.GvGLogError("UpdateLiveDataGroupCommand: No Guild found with GroupId: " + GroupId);
				return this;
			}
			bool flag = false;
			if (guildModel.GuildWarModel == null)
			{
				manager.GvGLog("UpdateLiveDataGroupCommand: guild war null");
				return this;
			}
			if (guildModel.GuildWarModel.CurrentBattle.IsOngoing(Timestamp))
			{
				flag = guildModel.GuildWarModel.CurrentBattle.UpdateLiveData(UniqueMissionId, SenderId);
				if (flag)
				{
					manager.GvGLog("UpdateLiveDataGroupCommand: Live data updated");
				}
				if (UniqueMissionId != null)
				{
					guildModel.GuildWarModel.CurrentBattle.UpdateMemberAttackAttempts(SenderId, Attacks);
					flag = true;
				}
			}
			if (flag)
			{
				SaveGroupModel(tWDModelManager);
			}
			return this;
		}
	}
}
