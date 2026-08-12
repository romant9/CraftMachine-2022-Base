using BaseModel;
using BaseModel.ContentTypes;

namespace TWDModel
{
	public class CancelVideoAdRewardCommand : ModelCommand
	{
		public AdUsage AdUsage { get; set; }

		public CancelVideoAdRewardCommand()
		{
		}

		public CancelVideoAdRewardCommand(PlayerModel player, AdUsage adUsage)
			: base(player)
		{
			AdUsage = adUsage;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel model = manager.GetModel<PlayerModel>(base.ModelId);
			if (AdUsage == AdUsage.CinemaReward)
			{
				model.PendingVideoAdReward = false;
			}
			if (AdUsage == AdUsage.CombatRewardKey)
			{
				model.PendingVideoAdRewardInRewardScreen = false;
				if (model.Combat.VideoAdsServedInRewardScreen > 0)
				{
					model.Combat.VideoAdsServedInRewardScreen--;
				}
			}
			if (AdUsage == AdUsage.BuildUpgradeSpeedUp)
			{
				model.PendingVideoAdRewardInBuildingMenu = false;
			}
			if (AdUsage == AdUsage.RefreshBlackMarketSlot)
			{
				model.PendingVideoAdRewardInBlackMarketScreen = false;
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
