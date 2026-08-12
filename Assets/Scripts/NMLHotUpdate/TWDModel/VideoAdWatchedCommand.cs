using BaseModel;
using BaseModel.ContentTypes;

namespace TWDModel
{
	public class VideoAdWatchedCommand : ModelCommand
	{
		public AdUsage Usage;

		public VideoAdWatchedCommand()
		{
		}

		public VideoAdWatchedCommand(PlayerModel player)
			: base(player)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel model = manager.GetModel<PlayerModel>(base.ModelId);
			if (!model.IsVideoAdRewardAvailable(Usage))
			{
				(manager as TWDModelManager)?.Debug.LogWarning("Player is watching videos above the limit");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (Usage == AdUsage.CinemaReward)
			{
				model.PendingVideoAdReward = true;
			}
			if (Usage == AdUsage.CombatRewardKey)
			{
				model.PendingVideoAdRewardInRewardScreen = true;
				model.Combat.VideoAdsServedInRewardScreen++;
			}
			if (Usage == AdUsage.BuildUpgradeSpeedUp)
			{
				model.PendingVideoAdRewardInBuildingMenu = true;
			}
			if (Usage == AdUsage.RefreshBlackMarketSlot)
			{
				model.PendingVideoAdRewardInBlackMarketScreen = true;
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
