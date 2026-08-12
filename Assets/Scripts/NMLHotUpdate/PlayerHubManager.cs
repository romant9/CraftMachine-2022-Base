using System;
using System.Collections.Generic;
using BaseModel.ContentTypes;
using Client.Connectivity;
using TWD.Externals;
using TWDModel;
using UnityEngine;

public class PlayerHubManager
{
	private Dictionary<string, int> promoShownSessionCount = new Dictionary<string, int>();

	public static string ActivityRedDotKey = "ActivityRedDot_";

	public int ActivityRedDotNum;

	public List<PlayerHubNewsItem> News { get; set; }

	public List<PlayerHubNewsItem> NewsWithPlacement { get; set; }

	public List<PlayerHubNewsItem> AllNews { get; set; }

	public PlayerHubManager()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
		UIEvent.OnUIEvent += OnUiEvent;
		News = new List<PlayerHubNewsItem>();
		NewsWithPlacement = new List<PlayerHubNewsItem>();
	}

	public void UpdateInfo()
	{
		LoadCmsData();
		LoadActivityRedDotData();
	}

	public void LoadCmsData()
	{
		if (SignalRClient.Instance != null && GameManager.Instance.IsConnectedToServer)
		{
			ContentManager.Instance.LoadContent(typeof(NewsItem).Name + "/" + SingularityMonoBehaviour<LocalizationManager>.Instance.CurrentLanguage, OnNewsContent);
		}
	}

	public void LoadActivityRedDotData()
	{
		if (!(SignalRClient.Instance != null) || !GameManager.Instance.IsConnectedToServer)
		{
			return;
		}
		List<ActiveInformationDefinition> list = new List<ActiveInformationDefinition>();
		ActivityRedDotNum = 0;
		ActiveInformationDefinition[] activeInformationDefinitions = GameManager.Instance.gameEconomyData.ActiveInformationDefinitions;
		foreach (ActiveInformationDefinition activeInformationDefinition in activeInformationDefinitions)
		{
			long utcTimeStamp = GameManager.Instance.playerModel.UtcTimeStamp;
			if (utcTimeStamp >= activeInformationDefinition.ShowTimeMilliseconds && utcTimeStamp <= activeInformationDefinition.EndTimeMilliseconds && Helpers.IsInSpenderTier(activeInformationDefinition.SpenderTiers))
			{
				list.Add(activeInformationDefinition);
			}
		}
		foreach (ActiveInformationDefinition item in list)
		{
			if (TWDPlayerPrefs.GetInt(ActivityRedDotKey + item.ID, 1) == 1)
			{
				ActivityRedDotNum++;
			}
		}
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "NewsBecameUnvalid")
		{
			UpdateActiveNews();
		}
	}

	private void OnNewsContent(string transactionId, bool loaded)
	{
		if (!loaded)
		{
			return;
		}
		string content = ContentManager.Instance.GetContent(transactionId);
		if (!string.IsNullOrEmpty(content))
		{
			AllNews = GameManager.Instance.jsonSerializer.Deserialize<List<PlayerHubNewsItem>>(content);
			if (AllNews == null)
			{
				Debug.LogWarning("Invalid news item info received");
			}
			else
			{
				UpdateActiveNews();
			}
		}
	}

	public void UpdateActiveNews()
	{
		bool enabled = GameManager.Instance.gameEconomyData.GetFeature("PromoLoader").Enabled;
		if (News == null || NewsWithPlacement == null || AllNews == null)
		{
			return;
		}
		News.Clear();
		NewsWithPlacement.Clear();
		for (int i = 0; i < AllNews.Count; i++)
		{
			ValidateIncomingNews(AllNews[i]);
			if (enabled && AllNews[i].HasAttribute(PlayerHubNewsItem.AttributeTag.PlacementId))
			{
				NewsWithPlacement.Add(AllNews[i]);
			}
			else
			{
				News.Add(AllNews[i]);
			}
		}
		UIEvent.Send("NewsUpdated");
	}

	public int GetUnreadNewsNumber()
	{
		if (News == null)
		{
			return 0;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel.NewsLetterItemsRead == null)
		{
			return News.Count;
		}
		int num = 0;
		for (int i = 0; i < News.Count; i++)
		{
			if (!playerModel.NewsLetterItemsRead.Contains(News[i].EntryId))
			{
				num++;
			}
		}
		return num;
	}

	[Obsolete]
	public void OpenIfNeeded()
	{
		if (News == null)
		{
			return;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		for (int i = 0; i < News.Count; i++)
		{
			if (!playerModel.NewsLetterItemsRead.Contains(News[i].EntryId))
			{
				OpenNewsletter();
				break;
			}
		}
	}

	public void OpenNewsletter()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.PlayerHubPopup).Open();
		PlayerModel playerModel = GameManager.Instance.playerModel;
		List<string> list = new List<string>();
		if (News != null)
		{
			UpdateActiveNews();
			foreach (PlayerHubNewsItem item in News)
			{
				if (!playerModel.NewsLetterItemsRead.Contains(item.EntryId))
				{
					item.HasBeenRead = false;
					list.Add(item.EntryId);
				}
				else
				{
					item.HasBeenRead = true;
				}
			}
			if (list.Count > 0)
			{
				List<string> list2 = new List<string>();
				foreach (string item2 in playerModel.NewsLetterItemsRead)
				{
					bool flag = false;
					foreach (PlayerHubNewsItem item3 in News)
					{
						if (item3.EntryId == item2)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						list2.Add(item2);
					}
				}
				Helpers.ExecuteCommand(new ReadNewsletterItemCommand
				{
					ItemsReadId = list,
					OldItemsId = list2
				});
			}
		}
		Helpers.ExecuteCommand(new PlayerHubCommand
		{
			EventName = "player_hub_open"
		});
	}

	public void GetItemsWithAttribute(PlayerHubNewsItem.AttributeTag tag, string attributeValue, ref List<PlayerHubNewsItem> returnList)
	{
		returnList = new List<PlayerHubNewsItem>();
		for (int i = 0; i < NewsWithPlacement.Count; i++)
		{
			if (NewsWithPlacement[i] != null && NewsWithPlacement[i].GetAttributeValue(tag) == attributeValue && DeepLinkNavigation.IsDeepLinkAccessable(NewsWithPlacement[i]))
			{
				returnList.Add(NewsWithPlacement[i]);
			}
		}
	}

	public PlayerHubNewsItem GetArticleWithId(string id)
	{
		for (int i = 0; i < News.Count; i++)
		{
			if (News[i] != null && News[i].EntryId == id)
			{
				return News[i];
			}
		}
		return null;
	}

	public int GetShownCount(PlayerHubNewsItem item)
	{
		int value = 0;
		if (promoShownSessionCount != null && item != null)
		{
			promoShownSessionCount.TryGetValue(item.EntryId, out value);
		}
		else
		{
			Debug.LogError("PlayerHubManager::GetShownCount, Could not retrive ShownCount. Data was NULL");
			value = -1;
		}
		return value;
	}

	public void SaveItemShown(PlayerHubNewsItem item)
	{
		if (promoShownSessionCount != null && item != null)
		{
			int value = 0;
			if (promoShownSessionCount.TryGetValue(item.EntryId, out value))
			{
				promoShownSessionCount[item.EntryId] = Mathf.Min(value + 1, 1000000);
			}
			else if (promoShownSessionCount.Count < 500)
			{
				promoShownSessionCount.Add(item.EntryId, 1);
			}
		}
	}

	private static void ValidateIncomingNews(PlayerHubNewsItem newsItem)
	{
		if (newsItem != null && !(GameManager.Instance == null) && GameManager.Instance.gameEconomyData != null && GameManager.Instance.playerModel != null && !(newsItem.NavigationLink != "HERO_PREVIEW"))
		{
			string attributeValue = newsItem.GetAttributeValue(PlayerHubNewsItem.AttributeTag.EntryId);
			ActorDefinition actorDefinition = (string.IsNullOrEmpty(attributeValue) ? null : GameManager.Instance.gameEconomyData.GetActorDefinition(attributeValue));
			if (actorDefinition != null && actorDefinition.ID.ToLower().Contains("hero") && GameManager.Instance.playerModel.SurvivorContainer.HasHero(actorDefinition.ID))
			{
				newsItem.NavigationLink = "OPEN_RADIO_TENT";
			}
		}
	}
}
