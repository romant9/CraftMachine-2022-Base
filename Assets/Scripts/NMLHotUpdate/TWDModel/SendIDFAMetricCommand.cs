using BaseModel;

namespace TWDModel
{
	public class SendIDFAMetricCommand : ModelCommand
	{
		public string Action { get; private set; }

		public int Position { get; private set; }

		public bool IsNativePopup { get; private set; }

		public SendIDFAMetricCommand()
		{
		}

		public SendIDFAMetricCommand(string action, int position, bool isNativePopup = false)
		{
			Action = action;
			Position = position;
			IsNativePopup = isNativePopup;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			((TWDModelManager)manager).Metrics.AddIDFAPopupMetric(IsNativePopup, Position, Action).Send();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
