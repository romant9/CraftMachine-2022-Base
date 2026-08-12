using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class EndlessModeConfig
	{
		public int MaxPasses;

		public int SubscriptionMaxPasses;

		public int PassRefresh;

		public int PassesGivenPerRefresh;

		public int SubscriptionPassesGivenPerRefresh;

		public int AttemptsToSumForFinalScore;

		public int AttemptsToSumForFinalScoreNormal;

		public int AttemptsToSumForFinalScoreExpert;

		public int EndlessMaxDifficultyBase;

		public FixedPoint MaximumBaseScoreMultiplier;

		public int MinBaseLevelOffset;

		public int MaxLevelOffset;

		public int MaxWalkerCount;

		public FixedPoint StartingScoreMultiplier;

		public string ScoreMultiplierDecreaseRate;

		public int StartingWaveTurnCount;

		public int CouncilLockLevel;

		public int MissionBaseCost;

		public int MissionTicketCost;

		public int DailyGoldAttemptCount;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public int DelayUntilRewardsAreClaimed;

		public bool IsUsingMaxLevelDifference;

		public int MaxLevelDifferenceBetweenWalkers;

		public string EndlessRefreshDays;

		public int ExpertModeHeroAmount;

		public int ExpertModeStartingOffset;

		public FixedPoint ExpertModeTotalScoreMultiplier;

		public int ExpertModeCouncilLockLevel;

		public bool MaxoutRetry;

		public bool MaxoutRetryPass;

		public bool MaxoutRetryGold;

		public int MaxEndlessPassExpertToken;

		public int EndlessExpertPassesGivenPerRefresh;

		public int DailyGoldExpertAttemptCount;

		public int SubscriptionExpertPassesGivenPerRefresh;

		public int SubscriptionMaxExpertPasses;

		public int MissionTicketCostExpert;

		public int NormalModeStartingLevel;

		public int ExpertModeStartingLevel;

		public int WarningTextSpacesAmount;

		public List<DayOfWeek> GetValidRefreshDays()
		{
			string[] array = EndlessRefreshDays.Split(';');
			List<DayOfWeek> list = new List<DayOfWeek>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				DayOfWeek typeEnum = GameEconomyData.GetTypeEnum<DayOfWeek>(array2[i]);
				list.Add(typeEnum);
			}
			return list;
		}
	}
}
