using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class GuildWarConfig
	{
		public int MatchmakingVersion;

		public int GuildWarUnlockAtCouncilLevel;

		public string GuildWarUnlockAtAfterTutorialPartId;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long GuildBattleDurationMilliseconds;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long MatchmakingLockdownDurationInMilliseconds;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long MatchmakingLockdownBufferClientSide;

		public int NumChars;

		public FixedPoint PinkLevelEq;

		public FixedPoint HeroLevelEq;

		public int MaxLevelWt;

		public int AlmostMaxLevelWt;

		public int MinBaseLevelOffset;

		public int NumHeroes;

		public string PVPSurvivorRangeColumn0String;

		public string PVPSurvivorRangeColumn1String;

		public string PVPSurvivorRangeColumn2String;

		public int BattlePassRefreshAmount;

		public int MinPlayersToStartBattle;

		public int MaxPlayerCountInBattle;

		public int MaxPlayerCountPerWar;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long BattleLeaderboardsCacheDurationInMilliseconds;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long SeasonResetPopupCooldownInMilliseconds;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int NotificationDelayInSeconds;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public int UniversalNotificationDelayInMilliseconds;

		public int GuildBattleMinimumTier;

		public bool GuildShopHighestTierFirst;

		public string GvGMissionHubTopContentPrefab;

		public int KeysPerBattle;

		public int MaxAmountOfRetries;

		public string RetryCostsString;

		[JsonIgnore]
		public int[] RetryCosts;

		public FixedPoint RetryMissionPenalty;

		[JsonIgnore]
		public List<Tuple<int, int>> PVPSurvivorRangeColumns;

		public FixedPoint SeasonResetPercentage;

		public int MaxSeasonStartVictoryPoints;

		public int GuildWarRegistrationLimit;

		public int GetRetryCost(int numOfRetries)
		{
			return RetryCosts[Math.Min(RetryCosts.Length - 1, numOfRetries)];
		}
	}
}
