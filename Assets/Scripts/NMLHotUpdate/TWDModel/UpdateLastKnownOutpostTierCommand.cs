using BaseModel;

namespace TWDModel
{
	public class UpdateLastKnownOutpostTierCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel playerModel = ((manager is TWDModelManager tWDModelManager) ? tWDModelManager.Player : null);
			if (playerModel != null)
			{
				OutpostSeason outpostSeasonById = playerModel.gameEconomyData.GetOutpostSeasonById(playerModel.CurrentOutpostSeasonId);
				if (outpostSeasonById != null)
				{
					OutpostTier outpostInfluenceTier = playerModel.gameEconomyData.GetOutpostInfluenceTier(playerModel.RankingScore, outpostSeasonById.TierSetId);
					if (outpostInfluenceTier != null)
					{
						playerModel.LastKnownOutpostTierId = outpostInfluenceTier.Id;
					}
				}
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
