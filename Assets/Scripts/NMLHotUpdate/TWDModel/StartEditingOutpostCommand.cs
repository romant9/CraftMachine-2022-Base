using BaseModel;

namespace TWDModel
{
	public class StartEditingOutpostCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager { Player: var player } tWDModelManager && player.OutpostModel != null && player.OutpostModel.InitializeEditModel())
			{
				tWDModelManager.Metrics.AddStart().AddEdit().AddPvpDefender(tWDModelManager.Player)
					.Send();
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
