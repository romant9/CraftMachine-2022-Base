using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ClaimLeaderBoardRewardCommand : ModelCommand
	{
		[JsonIgnore]
		public Rewards Rewards;

		public long LeaderBoardPosition { get; set; }

		public long LeaderBoardEntryCount { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			EndlessModeManagerModel endlessModeManager = tWDModelManager.Player.EndlessModeManager;
			if (endlessModeManager == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (!endlessModeManager.DoWeHaveRewardsUnclaimed())
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			EndlessModeCalendarDefinition currentEndlessModeCalendarDefinition = endlessModeManager.CurrentEndlessModeCalendarDefinition;
			if (currentEndlessModeCalendarDefinition == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			int num = currentEndlessModeCalendarDefinition.Identifier - 1;
			string text = tWDModelManager.GameEconomyData.GetEndlessLeaderboardRewardsSetForDefinitionId(num);
			if (tWDModelManager.Player.EndlessModeManager.EndlessModeZoneModel.FeatureEnabled)
			{
				int zoneIdById = tWDModelManager.Player.EndlessModeManager.EndlessModeZoneModel.GetZoneIdById(tWDModelManager.Player.EndlessModeManager.Id);
				text = tWDModelManager.GameEconomyData.GetEndlessLeaderboardRewardsSetForDefinitionIdWithZoneId(num, zoneIdById);
			}
			if (text == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (tWDModelManager.ServerService == null)
			{
				TWDModelResult tWDModelResult = endlessModeManager.GiveLeaderBoardRewards(out Rewards, LeaderBoardPosition, LeaderBoardEntryCount, text, num);
				if (tWDModelResult == TWDModelResult.OK)
				{
					tWDModelManager.Player.EndlessModeManager.EndlessRewardsClaimedLog.Add(num);
				}
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			string hashedId = tWDModelManager.Player.HashedId;
			string leaderboard = Leaderboards.GetEndlessModeLeaderboardName(num);
			if (tWDModelManager.Player.EndlessModeManager.EndlessModeZoneModel.FeatureEnabled)
			{
				int zoneIdById2 = tWDModelManager.Player.EndlessModeManager.EndlessModeZoneModel.GetZoneIdById(tWDModelManager.Player.EndlessModeManager.Id);
				leaderboard = Leaderboards.GetEndlessModeLeaderboardNameWithZoneId(num, zoneIdById2);
			}
			LeaderboardPosition leaderboardPosition = tWDModelManager.ServerService.GetLeaderboardPosition(leaderboard, hashedId);
			if (leaderboardPosition == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			leaderboardPosition.Position++;
			if (leaderboardPosition.Position != LeaderBoardPosition || LeaderBoardEntryCount != leaderboardPosition.LeaderboardCount)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			TWDModelResult tWDModelResult2 = endlessModeManager.GiveLeaderBoardRewards(out Rewards, LeaderBoardPosition, LeaderBoardEntryCount, text, num);
			if (tWDModelResult2 == TWDModelResult.OK)
			{
				tWDModelManager.Player.EndlessModeManager.EndlessRewardsClaimedLog.Add(num);
			}
			return new NGModelCommandRespond(this, tWDModelResult2);
		}
	}
}
