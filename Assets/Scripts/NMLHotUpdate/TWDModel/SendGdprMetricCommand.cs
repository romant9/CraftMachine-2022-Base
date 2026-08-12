using BaseModel;

namespace TWDModel
{
	public class SendGdprMetricCommand : ModelCommand
	{
		public enum MetricType
		{
			None = 0,
			Start_GDPR = 1,
			End_GDPR = 2,
			Open_GDPR_Link = 3
		}

		public MetricType eventType { get; set; }

		public string DialogueName { get; set; }

		public string DialogueDecision { get; set; }

		public string DialogueDeletionDate { get; set; }

		public string LinkName { get; set; }

		public SendGdprMetricCommand()
		{
		}

		public SendGdprMetricCommand(MetricType eventType)
		{
			this.eventType = eventType;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			if (tWDModelManager == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (eventType == MetricType.Start_GDPR)
			{
				tWDModelManager.Metrics.AddStartGdpr(DialogueName, DialogueDecision, DialogueDeletionDate).Send();
			}
			if (eventType == MetricType.End_GDPR)
			{
				tWDModelManager.Metrics.AddEndGdpr(DialogueName, DialogueDecision, DialogueDeletionDate).Send();
			}
			else if (eventType == MetricType.Open_GDPR_Link)
			{
				tWDModelManager.Metrics.AddOpenGdprLink(DialogueName, LinkName).Send();
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
