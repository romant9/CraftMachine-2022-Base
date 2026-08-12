using System.Collections;
using System.Collections.Generic;
using System.Text;

public class FriendListManager : IFriendListManager
{
	private static string LIST_SEPARATOR = ",";

	private string storedFriends;

	private GameCenterManager gameCenterManager;

	public event GetFriendsReady OnFriendListReady;

	public FriendListManager(GameCenterManager gameCenterManager)
	{
		this.gameCenterManager = gameCenterManager;
	}

	public string GetFriends()
	{
		return storedFriends;
	}

	public void UpdateFriends()
	{
		GameManager.Instance.StartCoroutine(UpdateFriendsInternal());
	}

	private IEnumerator UpdateFriendsInternal()
	{
		string ids = "";
		if (!GameManager.Instance.IsConnectedToServer)
		{
			yield break;
		}
		if (gameCenterManager.Authenticated)
		{
			gameCenterManager.GetFriendListAuto();
			while (!gameCenterManager.friendsLoadReady)
			{
				yield return null;
			}
		}
		storedFriends = ids;
		if (this.OnFriendListReady != null)
		{
			this.OnFriendListReady(ids);
		}
	}

	private string FriendListToBackend(List<Friend> friends, string prefix)
	{
		if (friends != null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < (friends?.Count ?? 0); i++)
			{
				stringBuilder.Append(prefix);
				stringBuilder.Append(friends[i].id);
				if (i < friends.Count - 1)
				{
					stringBuilder.Append(LIST_SEPARATOR);
				}
			}
			return stringBuilder.ToString();
		}
		return "";
	}
}
