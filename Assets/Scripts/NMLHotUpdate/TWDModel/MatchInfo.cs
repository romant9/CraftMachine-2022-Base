using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class MatchInfo
	{
		public OutpostLevelModel OutpostLevelModel { get; set; }

		public string OutpostTierId { get; set; }

		public int DefendingOutpostLevel { get; set; }

		public int RankingScore { get; set; }

		public int DefendingOutpostPower { get; set; }

		public int DefendingOutpostWalkerPower { get; set; }

		public int DefendingPlayerLevel { get; set; }

		public long UtcTime { get; set; }

		public long TradeGoodsAmount { get; set; }

		public long TradeGoodsCapacity { get; set; }

		[JsonIgnore]
		public bool IsFake { get; set; }

		public List<SurvivorClass> DefendingSurvivorClasses { get; set; }

		public List<int> DefendingSurvivorLevels { get; set; }

		public List<string> DefendingSurvivorNames { get; set; }

		public List<int> DefendingSurvivorRarityLevels { get; set; }

		public MatchInfo()
		{
			DefendingSurvivorClasses = new List<SurvivorClass>();
			DefendingSurvivorLevels = new List<int>();
			DefendingSurvivorNames = new List<string>();
			DefendingSurvivorRarityLevels = new List<int>();
		}

		public MatchInfo(OutpostLevelModel outpostLevelModel, int rankingScore, long tradeGoodsAmount, long tradeGoodsCapacity, string tierId)
			: this()
		{
			OutpostLevelModel = outpostLevelModel;
			RankingScore = rankingScore;
			TradeGoodsAmount = tradeGoodsAmount;
			TradeGoodsCapacity = tradeGoodsCapacity;
			OutpostTierId = tierId;
		}

		public static MatchInfo CreateMatchInfo(IMessageSerializer serializer, string matchInfoJson)
		{
			return serializer.DeserializeObject<MatchInfo>(matchInfoJson);
		}

		public string GetJson(IMessageSerializer serializer)
		{
			return serializer.SerializeObject(this);
		}

		public int GetRankingScoreGain(PlayerModel attackPlayerModel)
		{
			return UtilsMath.Max(attackPlayerModel.GetRankingScoreChange(attackPlayerModel.RankingScore, RankingScore) * attackPlayerModel.TierAttackWinMultiplier / 100, attackPlayerModel.gameEconomyData.ConfigData.OutpostMinimumInfluenceReward);
		}

		public int GetRankingScoreLoss(PlayerModel attackPlayerModel)
		{
			return attackPlayerModel.GetRankingScoreChange(RankingScore, attackPlayerModel.RankingScore) * attackPlayerModel.manager.GameEconomyData.ConfigData.OutpostDefendersWonInfluencePercentage / 100 * attackPlayerModel.TierAttackLossMultiplier / 100;
		}
	}
}
