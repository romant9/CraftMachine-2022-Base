using System;
using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;
using TWDModel;

public class LeaderboardPositionProvider
{
	private LeaderboardPosition endlessModeLeaderboardPosition;

	private long lastLeaderboardUpdateUTC;

	private readonly int leaderboardId;

	private readonly HashSet<Action<LeaderboardPosition>> callbacks = new HashSet<Action<LeaderboardPosition>>();

	public LeaderboardPositionProvider(int leaderboardId)
	{
		this.leaderboardId = leaderboardId;
	}

	public void GetLeaderboardPosition(Action<LeaderboardPosition> callback)
	{
		if (GameManager.Instance.playerModel.UtcTimeStamp - lastLeaderboardUpdateUTC > EndlessModeHelpers.LeaderboardCacheTime)
		{
			RequestLeaderboardPosition(callback);
		}
		else
		{
			callback?.Invoke(endlessModeLeaderboardPosition);
		}
	}

	private void RequestLeaderboardPosition(Action<LeaderboardPosition> callback)
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		string endlessModeLeaderboardName = Leaderboards.GetEndlessModeLeaderboardName(leaderboardId);
		string hashedId = playerModel.HashedId;
		lastLeaderboardUpdateUTC = playerModel.UtcTimeStamp;
		if (GameManager.Instance.IsConnectedToServer)
		{
			callbacks.Add(callback);
			if (callbacks.Count == 1)
			{
				SignalRClient.Instance.RequestCommand("GetLeaderboardPosition", endlessModeLeaderboardName, hashedId, OnGetCurrentLeaderBoardRanking, null, waitForResponse: true);
			}
		}
	}

	private void OnGetCurrentLeaderBoardRanking(string result)
	{
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(result))
		{
			Debug.LogError("OnGetCurrentLeaderBoardRanking failed");
			InvokeAllCallbacks();
			SignalRClient.Instance.ClearError();
			return;
		}
		LeaderboardPosition leaderboardPosition = GameManager.Instance.jsonSerializer.DeserializeObject<LeaderboardPosition>(result);
		if (leaderboardPosition != null)
		{
			leaderboardPosition.Position++;
			endlessModeLeaderboardPosition = leaderboardPosition;
		}
		InvokeAllCallbacks();
	}

	private void InvokeAllCallbacks()
	{
		foreach (Action<LeaderboardPosition> callback in callbacks)
		{
			callback?.Invoke(endlessModeLeaderboardPosition);
		}
		callbacks.Clear();
	}
}
