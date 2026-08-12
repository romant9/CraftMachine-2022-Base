using BaseModel;

namespace TWDModel
{
	public class CheatForceExpireGuildAdGroupCommand : TWDGroupCommand
	{
		public long TimeLeftMs { get; set; }

		public override GroupCommandBase Execute(ModelManager manager)
		{
			if (((GuildModel)manager.GetGroupModel(GroupId)).DEBUG_cheatExpireAd(TimeLeftMs) == TWDModelResult.OK)
			{
				SaveGroupModel(manager);
			}
			return this;
		}
	}
}
