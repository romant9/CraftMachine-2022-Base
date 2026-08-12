using BaseModel;

namespace TWDModel
{
	public class SetPlayerPlatformCommand : ModelCommand
	{
		public string Platform;

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			manager.GetPlayer();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
