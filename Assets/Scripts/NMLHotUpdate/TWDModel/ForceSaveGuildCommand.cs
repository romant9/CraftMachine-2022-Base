using BaseModel;

namespace TWDModel
{
	public class ForceSaveGuildCommand : TWDSocialModelCommand
	{
		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new ForceSaveGuildGroupCommand();
		}
	}
}
