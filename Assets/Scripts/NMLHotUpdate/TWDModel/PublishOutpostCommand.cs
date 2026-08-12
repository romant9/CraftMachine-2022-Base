using BaseModel;

namespace TWDModel
{
	public class PublishOutpostCommand : ModelCommand
	{
		public static TWDModelResult PublishOutpost(TWDModelManager twdManager, bool sendAnalytics = false)
		{
			TWDModelResult result = TWDModelResult.Error;
			PlayerModel player = twdManager.Player;
			OutpostModel outpostModel = player.OutpostModel;
			if (twdManager != null && outpostModel.StoredLevelModel != null)
			{
				RunLocationModel runLocationModel = null;
				runLocationModel = player.GetOutpostTemplate(player.OutpostModel.StoredLevelModel.BaseRunLocationID);
				if (runLocationModel != null)
				{
					if (outpostModel.OutpostRunLocation == null && sendAnalytics)
					{
						OutpostTutorialStateForAnalytics analyticsState = OutpostTutorialStateForAnalytics.FirstOutpostPublished;
						twdManager.Metrics.AddStart().AddOutpostTutorial(analyticsState).Send();
					}
					outpostModel.OutpostRunLocation = outpostModel.StoredLevelModel.GenerateOutpost(runLocationModel);
					outpostModel.PublishedLevelDataVersion = twdManager.GameEconomyData.ConfigData.OutpostLevelDataVersion;
					if (outpostModel.OutpostRunLocation != null)
					{
						if (player.CurrentOutpostSeasonId == -1)
						{
							player.UpdateOutpostSeason();
						}
						twdManager.UpdateOutpostLeaderboardEntry();
						if (sendAnalytics)
						{
							twdManager.Metrics.AddEnd().AddEdit().AddPvpDefender(twdManager.Player)
								.Send();
						}
						result = TWDModelResult.OK;
					}
				}
			}
			return result;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = PublishOutpost(manager as TWDModelManager, sendAnalytics: true);
			return new NGModelCommandRespond(this, result);
		}
	}
}
