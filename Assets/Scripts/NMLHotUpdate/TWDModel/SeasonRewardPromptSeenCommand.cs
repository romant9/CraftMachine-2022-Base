using BaseModel;

namespace TWDModel
{
	public class SeasonRewardPromptSeenCommand : ModelCommand
	{
		public string ActorId { get; set; }

		public SeasonRewardPromptSeenCommand()
		{
		}

		public SeasonRewardPromptSeenCommand(ActorDefinition actorDefinition)
		{
			if (actorDefinition != null)
			{
				ActorId = actorDefinition.ID;
			}
			else
			{
				ActorId = "";
			}
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			TWDModelResult result = TWDModelResult.Error;
			if (!string.IsNullOrEmpty(ActorId))
			{
				tWDModelManager.Blackboard.IncreaseCounter(BlackboardModel.GetPromptedUnlocksPerActorKey(ActorId));
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
