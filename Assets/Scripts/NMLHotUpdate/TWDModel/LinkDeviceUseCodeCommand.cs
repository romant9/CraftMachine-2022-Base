using BaseModel;

namespace TWDModel
{
	public class LinkDeviceUseCodeCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
