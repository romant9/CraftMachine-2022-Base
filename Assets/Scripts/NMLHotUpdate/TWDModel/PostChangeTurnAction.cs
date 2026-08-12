using BaseModel;

namespace TWDModel
{
	public class PostChangeTurnAction : ModelAction
	{
		public PostChangeTurnAction()
			: base(null)
		{
		}

		public override bool Execute(ModelManager manager)
		{
			return true;
		}
	}
}
