using BaseModel;

namespace TWDModel
{
	public class OutpostTutorialProgressCommand : ModelCommand
	{
		public OutpostTutorialState StateToSet { get; private set; }

		public OutpostTutorialProgressCommand()
		{
		}

		public OutpostTutorialProgressCommand(OutpostTutorialState stateToSet)
		{
			StateToSet = stateToSet;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			PlayerModel playerModel = (PlayerModel)manager.GetPlayer();
			if (playerModel != null)
			{
				OutpostTutorialStateForAnalytics stateToSet = (OutpostTutorialStateForAnalytics)StateToSet;
				tWDModelManager.Metrics.AddStart().AddOutpostTutorial(stateToSet).Send();
				playerModel.OutpostTutorialState = StateToSet;
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
