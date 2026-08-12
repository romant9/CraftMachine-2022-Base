using BaseModel;

namespace TWDModel
{
	public class SendMetricCommand : ModelCommand
	{
		public enum MetricType
		{
			None = 0,
			BundleOpened = 1,
			StartEditOutpost = 2,
			GuildAdViewed = 3,
			ShopViewEnd = 4,
			GvGRetryScreenViewed = 5
		}

		public MetricType eventType { get; set; }

		public string IdParameter { get; set; }

		public Metrics.BundleSource BundleSource { get; set; }

		public string BundleIds { get; set; }

		public int ViewTimeInSeconds { get; set; }

		public int ShopTabIndex { get; set; }

		public SendMetricCommand()
		{
		}

		public SendMetricCommand(MetricType eventType)
		{
			this.eventType = eventType;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			if (eventType == MetricType.ShopViewEnd)
			{
				tWDModelManager.Metrics.AddEnd().AddShopVisit(BundleIds, BundleSource, ViewTimeInSeconds, ShopTabIndex).Send();
			}
			if (eventType == MetricType.StartEditOutpost)
			{
				tWDModelManager.Metrics.AddStart().AddEdit().AddPvpDefender(tWDModelManager.Player)
					.Send();
			}
			else if (eventType == MetricType.GuildAdViewed)
			{
				tWDModelManager.Metrics.AddView().AddGuild(tWDModelManager.Player.GuildModel).AddGuildAd(IdParameter)
					.Send();
			}
			else if (eventType == MetricType.GvGRetryScreenViewed)
			{
				tWDModelManager.Metrics.AddView().AddGvG().AddGvGBattle()
					.AddRetryScreen()
					.Send();
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
