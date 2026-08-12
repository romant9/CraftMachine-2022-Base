using BaseModel;

namespace TWDModel
{
	public class OpenGuildGiftCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
