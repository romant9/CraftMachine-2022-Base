using System.Collections.Generic;

namespace BaseModel
{
	public interface IServerService : IServerServiceV1
	{
		void SaveLeaderboardEntry(string leaderboard, LeaderboardEntry entry);

		bool TrySaveLeaderboardEntry(string leaderboard, LeaderboardEntry entry);

		List<LeaderboardEntry> GetLeaderboard(string leaderboard, string count);

		void AddPlayerQueueMessage(string hashedId, QueueMessageKind messageKind, string payload);

		PlayerHostingState GetPlayerHostingState(string playerHashedId);

		string GetPlayerJsonByPlayerId(string playerId);

		void FreeVisit();

		bool SaveMatchMakingInfo(string hashedId, Dictionary<MatchMakingValue, object> values);

		NotificationHubSendPushResponse SendRemotePush(NotificationHubSendPushRequest notificationHubSendPushRequest);

		void CancelRemotePush(List<string> notificationIds);

		bool GvgJoinBattle(IGvgBattleEntry entry);

		bool GvgLeaveBattle(string groupId, long matchmakingEpochMsec);

		IGvgBattleOpponentMatchmakingEntry GvgGetBattleOpponent(string groupId, long matchmakingEpochMsec);

		bool GvgHasJoinedBattle(string groupId, long matchmakingEpochMsec);

		WorldBossOperationResult WorldBossSignUpCycle(WorldBossSignUpCycleOperationRequest request);

		WorldBossOperationResult WorldBossSelectDifficulty(WorldBossSelectDifficultyOperationRequest request);

		WorldBossOperationResult WorldBossAttackBoss(WorldBossAttackBossOperationRequest request);

		WorldBossOperationResult WorldBossSettleBoss(WorldBossSettleBossOperationRequest request);

		WorldBossOperationResult WorldBossAttackCell(WorldBossAttackCellOperationRequest request);

		WorldBossOperationResult WorldBossOccupyEmptyCell(WorldBossOccupyEmptyCellOperationRequest request);

		WorldBossOperationResult WorldBossSettleCell(WorldBossSettleCellOperationRequest request);

		WorldBossOperationResult WorldBossWithdrawCell(WorldBossWithdrawCellOperationRequest request);

		WorldBossOperationResult WorldBossInstantReturn(WorldBossInstantReturnOperationRequest request);

		WorldBossClaimSettlementResult WorldBossClaimSettlementReward(WorldBossClaimSettlementRewardOperationRequest request);

		WorldBossOperationResult WorldBossUpdateGuildName(WorldBossUpdateGuildNameOperationRequest request);

		LeaderboardPosition GetLeaderboardPosition(string leaderboard, string entryId);

		void DebugGvgMatch(long matchmakingEpochMsec);

		void NotifyGuildBattleHighscoresChanged(GuildBattleHighscoresChangedNotification notification, List<string> targetPlayerHashedIds);

		void SendFeiShuHook(string text);

		BuyBundleResultInfoList VerifiedWebshopBuyBundleResultInfoList(BuyBundleResultInfoList waitVerifyBuyBundleResultInfoList);

		void ChangeWebshopPaySucModelsStateDeliverySuccess(BuyBundleResultInfoList deliverySuccessBuyBundleResultInfoList);

		void ChangeWebshopPaySucModelsStateNotFound(BuyBundleResultInfoList notFoundBuyBundleResultInfoList);
	}
}
