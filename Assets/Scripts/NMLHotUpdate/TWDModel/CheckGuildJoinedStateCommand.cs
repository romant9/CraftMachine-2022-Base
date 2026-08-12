using BaseModel;

namespace TWDModel
{
	public class CheckGuildJoinedStateCommand : TWDSocialModelCommand
	{
		protected override GroupCommandBase CreateGroupCommand(TWDModelManager modelManager)
		{
			return new CheckGuildJoinedStateGroupCommand();
		}
	}
}
