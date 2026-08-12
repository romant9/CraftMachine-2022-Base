using BaseModel;

namespace TWDModel
{
	public class ForceSaveGuildGroupCommand : TWDGroupCommand
	{
		public override GroupCommandBase Execute(ModelManager manager)
		{
			SaveGroupModel(manager);
			return this;
		}
	}
}
