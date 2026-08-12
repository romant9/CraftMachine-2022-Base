using System.Collections.Generic;
using UnityEngine;

public class ZendeskManager : MonoBehaviour
{
	private const string CHANNEL_ID = "eyJzZXR0aW5nc191cmwiOiJodHRwczovL2NyaW1lY2l0eS56ZW5kZXNrLmNvbS9tb2JpbGVfc2RrX2FwaS9zZXR0aW5ncy8wMUsyNDBLOUVYMDcyRk1ZQzQ0TTAxM0YzTi5qc29uIn0=";

	[SerializeField]
	private Transform _root;

	private bool needCheckUIOpen;

	private static ZendeskManager instance;

	private string _userName = string.Empty;

	private string _userToken = string.Empty;

	private bool _isInitialized;

	private bool _isInitializing;

	private int _unreadInboxMessagesCount;

	public int UnreadMessageCount => _unreadInboxMessagesCount;

	public static ZendeskManager GetInstance()
	{
		if (instance == null)
		{
			instance = new GameObject("ZendeskManager").AddComponent<ZendeskManager>();
		}
		return instance;
	}

	private void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		instance.StartService();
	}

	public void SetUserName(string userName)
	{
		_userName = userName;
	}

	public void SetUserToken(string userToken)
	{
		_userToken = userToken;
	}

	public void StartService()
	{
	}

	public void LogUserData()
	{
	}

	public void ShowFAQs(Dictionary<string, object> metadata = null, string[] tags = null)
	{
		Application.OpenURL("https://support.thewalkingdeadnomansland.com/hc/en-us");
	}
}
