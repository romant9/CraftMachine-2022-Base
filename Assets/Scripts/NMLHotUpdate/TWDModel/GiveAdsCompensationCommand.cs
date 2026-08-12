using BaseModel;

namespace TWDModel
{
	public class GiveAdsCompensationCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager tWDModelManager)
			{
				if (tWDModelManager.Player.Blackboard.IsToggleOn("Toggle.ToggleAdsCompensationReceived"))
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				Rewards adsCompensationRewards = tWDModelManager.GameEconomyData.GetAdsCompensationRewards();
				if (adsCompensationRewards == null)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				adsCompensationRewards.Give(tWDModelManager);
				tWDModelManager.Player.Blackboard.SetToggle("Toggle.ToggleAdsCompensationReceived");
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
