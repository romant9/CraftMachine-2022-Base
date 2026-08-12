using BaseModel;

namespace TWDModel
{
	public class SendIDFAStatusCommand : ModelCommand
	{
		public string Status { get; private set; }

		public SendIDFAStatusCommand()
		{
		}

		public SendIDFAStatusCommand(string status)
		{
			Status = status;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			((TWDModelManager)manager).Metrics.AddIDFAStatus(Status).Send();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
