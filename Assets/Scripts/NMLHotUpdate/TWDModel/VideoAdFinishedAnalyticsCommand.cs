using BaseModel;
using BaseModel.ContentTypes;

namespace TWDModel
{
	public class VideoAdFinishedAnalyticsCommand : ModelCommand
	{
		public AdProvider Provider;

		public AdStatus Status;

		public AdUsage Usage;

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (Usage == AdUsage.CinemaReward)
			{
				tWDModelManager?.Metrics.AddEnd().AddVideoAd(Provider, Status).AddCinema()
					.Send();
			}
			if (Usage == AdUsage.CombatRewardKey)
			{
				tWDModelManager?.Metrics.AddEnd().AddVideoAd(Provider, Status).AddMission()
					.Send();
			}
			if (Usage == AdUsage.BuildUpgradeSpeedUp)
			{
				BuildingModel buildingModel = tWDModelManager?.CampModel.IsBuildingUpgradeInProgress();
				if (buildingModel != null)
				{
					tWDModelManager.Metrics.AddEnd().AddVideoAd(Provider, Status).AddUpgrade()
						.AddBuilding(buildingModel)
						.Send();
				}
			}
			if (Usage == AdUsage.RefreshBlackMarketSlot)
			{
				tWDModelManager?.Metrics.AddEnd().AddVideoAd(Provider, Status).AddBlackMarketRefresh()
					.Send();
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
