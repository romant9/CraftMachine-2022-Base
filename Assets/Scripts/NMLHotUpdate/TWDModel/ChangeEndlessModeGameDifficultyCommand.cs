using BaseModel;
using TWDModel.ContentTypes;

namespace TWDModel
{
	public class ChangeEndlessModeGameDifficultyCommand : ModelCommand
	{
		public EndlessModeGameModeType EndlessModeGameModeType { get; set; }

		public ChangeEndlessModeGameDifficultyCommand(EndlessModeGameModeType endlessModeGameModeType)
		{
			EndlessModeGameModeType = endlessModeGameModeType;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager tWDModelManager)
			{
				EndlessModeManagerModel endlessModeManager = tWDModelManager.Player.EndlessModeManager;
				if (endlessModeManager == null)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				if (EndlessModeGameModeType == EndlessModeGameModeType.Expert)
				{
					if (tWDModelManager.Player.Level < endlessModeManager.EndlessModeConfig.ExpertModeCouncilLockLevel)
					{
						return new NGModelCommandRespond(this, TWDModelResult.Error);
					}
					if (endlessModeManager.CurrentExpertModeHeroes.Count < endlessModeManager.EndlessModeConfig.ExpertModeHeroAmount)
					{
						return new NGModelCommandRespond(this, TWDModelResult.Error);
					}
				}
				endlessModeManager.EndlessModeGameModeType = EndlessModeGameModeType;
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
