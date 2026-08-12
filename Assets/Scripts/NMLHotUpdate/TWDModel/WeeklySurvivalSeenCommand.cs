using BaseModel;

namespace TWDModel
{
	public class WeeklySurvivalSeenCommand : ModelCommand
	{
		public bool MarkSurvivalStartedAsSeen { get; set; }

		public bool MarkSurvivalEndedAsSeen { get; set; }

		public int NumberCompletedSeen { get; set; }

		public int ResetCounterSeen { get; set; }

		public WeeklySurvivalSeenCommand()
		{
		}

		public WeeklySurvivalSeenCommand(WeeklySurvivalModel weeklySurvivalModel)
			: base(weeklySurvivalModel)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager { Player: not null } tWDModelManager)
			{
				WeeklySurvivalModel model = tWDModelManager.GetModel<WeeklySurvivalModel>(base.ModelId);
				if (model != null)
				{
					if (MarkSurvivalStartedAsSeen)
					{
						model.MarkSurvivalStartedAsSeen();
					}
					if (MarkSurvivalEndedAsSeen)
					{
						model.MarkSurvivalEndedAsSeen();
					}
					if (NumberCompletedSeen > 0)
					{
						model.LastSeenNumberCompleted = NumberCompletedSeen;
					}
					if (ResetCounterSeen > 0)
					{
						model.LastSeenResetCount = ResetCounterSeen;
					}
				}
				else
				{
					tWDModelManager.Debug.Log("WeeklySurvivalSeenCommand: WeeklySurvivalModel is NULL!");
				}
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
