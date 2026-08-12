using BaseModel;

namespace TWDModel
{
	public class ForceGenerateEndlessExpertActorsCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			EndlessModeManagerModel endlessModeManagerModel = (manager as TWDModelManager)?.Player?.EndlessModeManager;
			if (endlessModeManagerModel?.GetActiveEndlessMode != null && !endlessModeManagerModel.IsLockedByCouncilLevel && !endlessModeManagerModel.AreEndlessActorsValidAndGenerated)
			{
				endlessModeManagerModel.ForceRegenerateExpertActors();
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
