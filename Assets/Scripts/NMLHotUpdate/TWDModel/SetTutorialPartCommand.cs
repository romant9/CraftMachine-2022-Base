using BaseModel;

namespace TWDModel
{
	public class SetTutorialPartCommand : ModelCommand
	{
		public string PartId;

		public SetTutorialPartCommand()
		{
		}

		public SetTutorialPartCommand(TutorialModel tutorial)
			: base(tutorial)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			manager.GetModel<TutorialModel>(base.ModelId).SetPart(PartId);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
