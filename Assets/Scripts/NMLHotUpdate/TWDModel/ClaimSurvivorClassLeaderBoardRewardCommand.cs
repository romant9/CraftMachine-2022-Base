using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ClaimSurvivorClassLeaderBoardRewardCommand : ModelCommand
	{
		[JsonIgnore]
		public Rewards Rewards;

		public List<SurvivorClassLeaderboardInfo> SurvivorClassLeaderboardInfos;

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (SurvivorClassLeaderboardInfos == null || SurvivorClassLeaderboardInfos.Count == 0)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (!tWDModelManager.GameEconomyData.ConfigData.EndlessExpertClassLeaderboardSwitch)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			EndlessModeManagerModel endlessModeManager = tWDModelManager.Player.EndlessModeManager;
			if (endlessModeManager == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (!endlessModeManager.DoWeHaveSurvivorClassRewardsUnclaimed())
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			EndlessModeCalendarDefinition currentEndlessModeCalendarDefinition = endlessModeManager.CurrentEndlessModeCalendarDefinition;
			if (currentEndlessModeCalendarDefinition == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			int num = currentEndlessModeCalendarDefinition.Identifier - 1;
			string endlessLeaderboardRewardsSetForDefinitionId = tWDModelManager.GameEconomyData.GetEndlessLeaderboardRewardsSetForDefinitionId(num);
			if (endlessLeaderboardRewardsSetForDefinitionId == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (tWDModelManager.ServerService == null)
			{
				TWDModelResult result = endlessModeManager.GiveExpertLeaderSurvivorClassLeaderBoardRewards(out Rewards, SurvivorClassLeaderboardInfos, endlessLeaderboardRewardsSetForDefinitionId, num);
				return new NGModelCommandRespond(this, result);
			}
			string hashedId = tWDModelManager.Player.HashedId;
			foreach (SurvivorClassLeaderboardInfo survivorClassLeaderboardInfo in SurvivorClassLeaderboardInfos)
			{
				string endlessModeLeaderboardNameByClass = Leaderboards.GetEndlessModeLeaderboardNameByClass(num, survivorClassLeaderboardInfo.SurvivorClass);
				LeaderboardPosition leaderboardPosition = tWDModelManager.ServerService.GetLeaderboardPosition(endlessModeLeaderboardNameByClass, hashedId);
				if (leaderboardPosition == null)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				leaderboardPosition.Position++;
				if (leaderboardPosition.Position != survivorClassLeaderboardInfo.LeaderBoardPosition || survivorClassLeaderboardInfo.LeaderBoardEntryCount != leaderboardPosition.LeaderboardCount)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
			}
			TWDModelResult result2 = endlessModeManager.GiveExpertLeaderSurvivorClassLeaderBoardRewards(out Rewards, SurvivorClassLeaderboardInfos, endlessLeaderboardRewardsSetForDefinitionId, num);
			return new NGModelCommandRespond(this, result2);
		}
	}
}
