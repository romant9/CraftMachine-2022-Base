public interface IFriendListManager
{
	event GetFriendsReady OnFriendListReady;

	string GetFriends();

	void UpdateFriends();
}
