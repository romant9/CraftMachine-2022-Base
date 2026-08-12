using BaseModel;

namespace TWDModel
{
	public class DisbandGuildGroupCommand : TWDGroupCommand
	{
		public override GroupCommandBase Execute(ModelManager manager)
		{
			return this;
		}
	}
}
