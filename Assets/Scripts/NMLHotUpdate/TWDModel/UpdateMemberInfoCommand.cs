using BaseModel;

namespace TWDModel
{
	public class UpdateMemberInfoCommand : TWDSocialModelCommand
	{
		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new UpdateMemberInfoGroupCommand
			{
				NewLevel = manager.Player.Level,
				NewName = manager.Player.Name,
				NewEmblem = manager.Player.PlayerEmblem
			};
		}
	}
}
