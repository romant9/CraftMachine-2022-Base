using BaseModel;

namespace TWDModel
{
	public class ChangeTurnAction : ModelAction
	{
		public ChangeTurnAction()
			: base(null)
		{
		}

		public override bool Execute(ModelManager manager)
		{
			return true;
		}
	}
}
