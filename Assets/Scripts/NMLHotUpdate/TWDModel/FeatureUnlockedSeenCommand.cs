using BaseModel;

namespace TWDModel
{
	public class FeatureUnlockedSeenCommand : ModelCommand
	{
		public string FeatureToggleId { get; private set; }

		public FeatureUnlockedSeenCommand()
		{
		}

		public FeatureUnlockedSeenCommand(string featureToggleId)
		{
			FeatureToggleId = featureToggleId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager tWDModelManager)
			{
				tWDModelManager.Player.Blackboard.SetToggle(FeatureToggleId);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
