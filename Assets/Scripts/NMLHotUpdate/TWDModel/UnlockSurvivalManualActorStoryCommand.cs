using BaseModel;

namespace TWDModel
{
	public class UnlockSurvivalManualActorStoryCommand : ConsumeCurrencyCommand
	{
		public new int ModelId { get; set; }

		public string StoryActorId { get; set; }

		public int MemoryID { get; set; }

		public UnlockSurvivalManualActorStoryCommand()
		{
		}

		public UnlockSurvivalManualActorStoryCommand(int modleId, string storyActorId, int memoryId)
		{
			ModelId = modleId;
			StoryActorId = storyActorId;
			MemoryID = memoryId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = manager.GetModel<SurvivalManualModel>(ModelId).UnlockSurvivalManualActorStory(StoryActorId, MemoryID);
			return new NGModelCommandRespond(this, result);
		}
	}
}
