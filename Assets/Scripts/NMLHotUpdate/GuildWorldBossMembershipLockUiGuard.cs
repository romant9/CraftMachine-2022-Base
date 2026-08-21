using System;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public static class GuildWorldBossMembershipLockUiGuard
{
	private const string TitleKey = "Popup.WorldBossGuildMembershipLocked.Title";

	private const string MessageKey = "Popup.WorldBossGuildMembershipLocked.Message";

	public static void ExecuteIfAllowed(Action action)
	{
		PlayerModel playerModel = GameManager.Instance?.playerModel;
		if (action == null || playerModel == null || string.IsNullOrEmpty(playerModel.GuildId))
		{
			action?.Invoke();
			return;
		}
		WorldBossCycleDefinition worldBossCycleDefinition = playerModel.WorldBossModelManager?.GetCurrentCycle() ?? playerModel.WorldBossModelManager?.GetNextCycle();
		if (worldBossCycleDefinition == null)
		{
			action();
			return;
		}
		if (SignalRClient.Instance == null || GameManager.Instance.jsonSerializer == null)
		{
			ShowCheckFailed();
			return;
		}
		string expectedGuildId = playerModel.GuildId;
		int expectedSeasonId = worldBossCycleDefinition.Season;
		int expectedCycleId = worldBossCycleDefinition.Cycle;
		long expectedCycleEndUtcMs = worldBossCycleDefinition.EndTimeMilliseconds;
		WorldBossGetSnapshotRequest value = new WorldBossGetSnapshotRequest
		{
			GroupId = expectedGuildId,
			SeasonId = expectedSeasonId,
			CycleId = expectedCycleId
		};
		string arg = GameManager.Instance.jsonSerializer.Serialize(value);
		SignalRClient.Instance.RequestCommand("WorldBossBaseSnapshot", arg, delegate(string responseJson)
		{
			HandleSnapshot(expectedGuildId, expectedSeasonId, expectedCycleId, expectedCycleEndUtcMs, action, responseJson);
		}, waitForResponse: true);
	}

	private static void HandleSnapshot(string expectedGuildId, int expectedSeasonId, int expectedCycleId, long expectedCycleEndUtcMs, Action action, string responseJson)
	{
		PlayerModel playerModel = GameManager.Instance?.playerModel;
		WorldBossCycleDefinition worldBossCycleDefinition = playerModel?.WorldBossModelManager?.GetCurrentCycle() ?? playerModel?.WorldBossModelManager?.GetNextCycle();
		if (playerModel == null || playerModel.GuildId != expectedGuildId || worldBossCycleDefinition == null || worldBossCycleDefinition.Season != expectedSeasonId || worldBossCycleDefinition.Cycle != expectedCycleId)
		{
			ShowCheckFailed();
			return;
		}
		if (SignalRClient.Instance == null || SignalRClient.Instance.HasError || string.IsNullOrEmpty(responseJson))
		{
			SignalRClient.Instance?.ClearError();
			ShowCheckFailed();
			return;
		}
		WorldBossGuildBaseSnapshot snapshot;
		try
		{
			snapshot = GameManager.Instance.jsonSerializer.Deserialize<WorldBossGuildBaseSnapshot>(responseJson);
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError("WorldBoss membership lock snapshot deserialization failed: " + ex.Message);
			ShowCheckFailed();
			return;
		}
		switch (GuildWorldBossMembershipLockPolicy.Evaluate(snapshot, expectedGuildId, expectedSeasonId, expectedCycleId, playerModel.UtcTimeStamp, expectedCycleEndUtcMs))
		{
		case GuildWorldBossMembershipLockDecision.Locked:
			AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.WorldBossGuildMembershipLocked.Title"), LocalizationManager.GetText("Popup.WorldBossGuildMembershipLocked.Message"), LocalizationManager.GetText("Button.Ok"));
			break;
		case GuildWorldBossMembershipLockDecision.InvalidSnapshot:
			ShowCheckFailed();
			break;
		default:
			action();
			break;
		}
	}

	private static void ShowCheckFailed()
	{
		HUDNotification.Error(LocalizationManager.GetText("Error.ErrorGeneric"));
	}
}
