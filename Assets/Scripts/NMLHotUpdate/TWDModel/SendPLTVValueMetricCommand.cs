using BaseModel;

namespace TWDModel
{
	public class SendPLTVValueMetricCommand : ModelCommand
	{
		public int PLTVValue { get; private set; }

		public SendPLTVValueMetricCommand()
		{
		}

		public SendPLTVValueMetricCommand(int value)
		{
			PLTVValue = value;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			((TWDModelManager)manager).Metrics.AddPLTVValue(PLTVValue).Send();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
