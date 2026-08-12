using BaseModel;

namespace TWDModel
{
	public class CompleteQuestCommand : ModelCommand
	{
		public CompleteQuestCommand()
		{
		}

		public CompleteQuestCommand(StoryTellerModel storyTellerModel)
			: base(storyTellerModel)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = ((!((StoryTellerModel)manager.GetModel(base.ModelId)).ClaimQuestCompleted()) ? TWDModelResult.Error : TWDModelResult.OK);
			return new NGModelCommandRespond(this, result);
		}
	}
}
