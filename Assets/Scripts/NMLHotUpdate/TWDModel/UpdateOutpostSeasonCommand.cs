using BaseModel;

namespace TWDModel
{
	public class UpdateOutpostSeasonCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			TWDModelResult result = TWDModelResult.Error;
			if (tWDModelManager.Player != null)
			{
				tWDModelManager.Player.UpdateOutpostSeason();
				tWDModelManager.Player.OutpostSeasonChanged = false;
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
