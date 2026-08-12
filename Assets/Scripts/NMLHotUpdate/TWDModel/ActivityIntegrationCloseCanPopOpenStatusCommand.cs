using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class ActivityIntegrationCloseCanPopOpenStatusCommand : ModelCommand
	{
		public string EventId { get; set; }

		public int? ConfigId { get; set; }

		public ActivityIntegrationCloseCanPopOpenStatusCommand()
		{
		}

		public ActivityIntegrationCloseCanPopOpenStatusCommand(string eventId)
		{
			EventId = eventId;
		}

		public ActivityIntegrationCloseCanPopOpenStatusCommand(string eventId, int? configId)
		{
			EventId = eventId;
			ConfigId = configId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (tWDModelManager?.Player?.gameEconomyData?.BroadcastDefinitions == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (tWDModelManager.Player.gameEconomyData.BroadcastDefinitions.FirstOrDefault((BroadcastDefinition x) => x.EventID == EventId) == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (tWDModelManager.Player.ActivityIntegrationManager == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			tWDModelManager.Player.ActivityIntegrationManager.CloseActivityCanPopOpenStatus(EventId, ConfigId);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
