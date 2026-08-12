using BaseModel;

namespace TWDModel
{
	public class TWDGroupCommand : GroupCommandBase
	{
		protected void SaveGroupModel(ModelManager manager)
		{
			if (manager is TWDModelManager tWDModelManager)
			{
				tWDModelManager.Player.GuildModel.SetMemberLastActiveDate(SenderId, tWDModelManager.Player.UtcTimeStamp);
				if (tWDModelManager.ServerService != null && SenderId == manager.GetPlayer().HashedId)
				{
					tWDModelManager.ServerService.SaveGroupModel(GroupId);
				}
			}
		}
	}
}
