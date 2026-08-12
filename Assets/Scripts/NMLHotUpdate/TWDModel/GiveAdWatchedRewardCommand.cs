using BaseModel;
using BaseModel.ContentTypes;

namespace TWDModel
{
	public class GiveAdWatchedRewardCommand : ModelCommand
	{
		public AdUsage AdUsage { get; set; }

		public string BlackMarketHeroId { get; set; }

		public AdProvider AdProvider { get; set; }

		public GiveAdWatchedRewardCommand()
		{
		}

		public GiveAdWatchedRewardCommand(AdUsage adUsage)
		{
			AdUsage = adUsage;
		}

		public GiveAdWatchedRewardCommand(AdUsage adUsage, BuildingModel buildingModel, AdProvider adProvider)
			: base(buildingModel)
		{
			AdUsage = adUsage;
			AdProvider = adProvider;
		}

		public GiveAdWatchedRewardCommand(AdUsage adUsage, string blackMarketHeroId, AdProvider adProvider)
		{
			AdUsage = adUsage;
			BlackMarketHeroId = blackMarketHeroId;
			AdProvider = adProvider;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (!(manager.GetPlayer() is PlayerModel playerModel))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (!playerModel.IsVideoAdRewardAvailable(AdUsage))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (AdUsage == AdUsage.BuildUpgradeSpeedUp)
			{
				if (!playerModel.PendingVideoAdRewardInBuildingMenu)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				TWDModelResult tWDModelResult = ((BuildingModel)manager.GetModel(base.ModelId)).AdSpeedUpUpgrade();
				if (tWDModelResult == TWDModelResult.OK)
				{
					if (playerModel.VideoAdsServedBuildingMenuScreen == 0)
					{
						playerModel.VideoAdRewardBuildingMenuScreen = playerModel.LifeTime;
					}
					playerModel.PendingVideoAdRewardInBuildingMenu = false;
					playerModel.VideoAdsServedBuildingMenuScreen++;
				}
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			if (AdUsage == AdUsage.RefreshBlackMarketSlot)
			{
				if (!playerModel.PendingVideoAdRewardInBlackMarketScreen)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				if (playerModel.BlackMarket.RefreshHero(BlackMarketHeroId))
				{
					if (playerModel.VideoAdsServedBlackMarketScreen == 0)
					{
						playerModel.VideoAdRewardBlackMarketScreen = playerModel.LifeTime;
					}
					playerModel.PendingVideoAdRewardInBlackMarketScreen = false;
					playerModel.VideoAdsServedBlackMarketScreen++;
					return new NGModelCommandRespond(this, TWDModelResult.OK);
				}
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
