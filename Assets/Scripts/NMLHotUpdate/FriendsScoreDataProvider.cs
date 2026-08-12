using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;

public class FriendsScoreDataProvider : PlayerScoreDataProvider
{
	protected override bool RequestInternal()
	{
		string friends = GameManager.Instance.FriendListManager.GetFriends();
		if (string.IsNullOrEmpty(friends))
		{
			Debug.LogError("No friends list");
			return false;
		}
		if (GameManager.Instance.IsConnectedToServer)
		{
			SignalRClient.Instance.RequestCommand("GetHighScoresBySocialIds", friends, "100", OnHighScoreListFriends, null, waitForResponse: true);
			return true;
		}
		return false;
	}

	protected void OnHighScoreListFriends(string response)
	{
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(response))
		{
			Debug.LogError("GetHighScoresFriends failed");
			SignalRClient.Instance.ClearError();
			NotifyDataReceived(null);
		}
		else
		{
			ScoreEntry[] entries = GameManager.Instance.jsonSerializer.DeserializeObject<ScoreEntry[]>(response);
			List<ScoreDataEntry> list = new List<ScoreDataEntry>();
			list = PlayerScoreDataEntry.ParseLeaderboardData(entries, GameManager.Instance.jsonSerializer);
			NotifyDataReceived(list);
		}
	}
}
