using BaseModel;

namespace TWDModel
{
	public class SetMarkedForDeletionCommand : ModelCommand
	{
		public bool Marked { get; set; }

		public SetMarkedForDeletionCommand()
		{
		}

		public SetMarkedForDeletionCommand(bool marked)
		{
			Marked = marked;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			(manager as TWDModelManager).Player.SetMarkedForDeletion(Marked);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
