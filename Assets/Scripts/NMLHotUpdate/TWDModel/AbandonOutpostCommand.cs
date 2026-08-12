using BaseModel;

namespace TWDModel
{
	public class AbandonOutpostCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager tWDModelManager)
			{
				tWDModelManager.Player.OutpostModel.Reset();
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
