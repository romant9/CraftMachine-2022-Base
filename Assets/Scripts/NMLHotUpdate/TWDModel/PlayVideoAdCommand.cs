using BaseModel;
using BaseModel.ContentTypes;

namespace TWDModel
{
	public class PlayVideoAdCommand : ModelCommand
	{
		public AdProvider Provider { get; set; }

		public int AdsWatched { get; set; }

		public AdUsage Usage { get; set; }

		public PlayVideoAdCommand()
		{
		}

		public PlayVideoAdCommand(PlayerModel player)
			: base(player)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (!tWDModelManager.Player.IsVideoAdRewardAvailable(Usage))
			{
				tWDModelManager.Debug.LogWarning("Player is watching videos above the limit");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (tWDModelManager.Analytics != null)
			{
				if (Usage == AdUsage.CombatRewardKey)
				{
					tWDModelManager.Metrics.AddStart().AddVideoAd(Provider, AdStatus.OK).AddMission()
						.Send();
				}
				if (Usage == AdUsage.CinemaReward)
				{
					tWDModelManager.Metrics.AddStart().AddVideoAd(Provider, AdStatus.OK).AddCinema()
						.Send();
				}
				if (Usage == AdUsage.BuildUpgradeSpeedUp)
				{
					BuildingModel buildingModel = tWDModelManager.CampModel.IsBuildingUpgradeInProgress();
					if (buildingModel != null)
					{
						tWDModelManager.Metrics.AddStart().AddVideoAd(Provider, AdStatus.OK).AddUpgrade()
							.AddBuilding(buildingModel)
							.Send();
					}
				}
				if (Usage == AdUsage.RefreshBlackMarketSlot)
				{
					tWDModelManager.Metrics.AddStart().AddVideoAd(Provider, AdStatus.OK).AddBlackMarketRefresh()
						.Send();
				}
				if (Usage == AdUsage.Unknown)
				{
					tWDModelManager.Debug.LogWarning($"Ad information missing from provider {Provider} AdsWatched={AdsWatched}");
				}
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
