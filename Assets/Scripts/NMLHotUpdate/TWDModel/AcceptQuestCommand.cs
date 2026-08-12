using BaseModel;

namespace TWDModel
{
	public class AcceptQuestCommand : ModelCommand
	{
		public AcceptQuestCommand()
		{
		}

		public AcceptQuestCommand(StoryTellerModel storyTellerModel)
			: base(storyTellerModel)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = ((!((StoryTellerModel)manager.GetModel(base.ModelId)).AcceptQuest()) ? TWDModelResult.Error : TWDModelResult.OK);
			return new NGModelCommandRespond(this, result);
		}
	}
}
