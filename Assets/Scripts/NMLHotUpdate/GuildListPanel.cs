using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class GuildListPanel : ScrollableListPanel<GuildModel>
{
	[SerializeField]
	private UISprite loadingSprite;

	[SerializeField]
	private UIInput searchInputField;

	[SerializeField]
	private UILabel noGuildFoundLabel;

	[SerializeField]
	private GameObject adCardPrefab;

	protected override void Awake()
	{
		base.Awake();

		if (OfflineManager.IsLoadDataManager)
		{
			DebugTWD.Log("GuildListPanel Awake", DebugType.Guild);
			if (GameManager.Instance.IsConnectedToServer)
			{
				OnSearch();
			}
			else
			{
				if (OfflineManager.IsInternetOn)
				{
					StartCoroutine(WaitForGuild());
				}
			}
		}
		else
		{
			if (!GameManager.Instance.playerModel.IsGuildMember)
			{
				OnSearch();
			}
		}
	}

	private IEnumerator WaitForGuild()
	{
		GetPlayerData.Instance.OnClickGetGuild();
		float timeout = 20f;
		float startTime = Time.realtimeSinceStartup;

		while (GetPlayerData.Instance.waitingGuild)
		{
			if (Time.realtimeSinceStartup - startTime > timeout)
			{
				DebugTWD.Log("GuildListPanel Timeout", DebugType.Guild);
				yield break;
			}
			yield return null;
		}

		if (!GameManager.Instance.IsConnectedToServer) yield break;

		OnSearch();
	}

	private void OnEnable()
	{
		if (OfflineManager.IsLoadDataManager && !GameManager.Instance.IsConnectedToServer)
		{
			return;
		}
		GuildManager guildManager = GameManager.Instance.GuildManager;
		guildManager.guildSearchFinishedEvent += OnGuildSearchFinished;
		guildManager.guildSearchFailedEvent += OnGuildSearchFailed;
	}

	private void OnDisable()
	{
		GuildManager guildManager = GameManager.Instance.GuildManager;
		guildManager.guildSearchFinishedEvent -= OnGuildSearchFinished;
		guildManager.guildSearchFailedEvent -= OnGuildSearchFailed;
	}

	public void OnGuildSearchFailed(string message)
	{
		ShowGuilds(null);
	}

	public void OnGuildSearchFinished(List<GuildModel> guildModels)
	{
		ShowGuilds(guildModels);
	}

	public void OnSearch()
	{
		DebugTWD.Log("GuildListPanel OnSearch", DebugType.Guild);

		string keyword = searchInputField.value.Trim().ToLowerInvariant();
		if (GameManager.Instance.GuildManager.SearchGuilds(keyword))
		{
			ShowGuilds(null);
			loadingSprite.gameObject.SetActive(value: true);
		}
	}

	public void ShowGuilds(List<GuildModel> guilds)
	{
		loadingSprite.gameObject.SetActive(value: false);
		noGuildFoundLabel.gameObject.SetActive(guilds != null && guilds.Count == 0);
		if (guilds == null)
		{
			ClearCards();
		}
		else
		{
			SetCards(guilds);
		}
	}

	protected override GameObject CreateCard(GuildModel item)
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (item.AdAvailableTimeSeconds > 0 && playerModel != null && playerModel.UtcTimeStamp / 1000 < item.AdExpireTimeStampSeconds && adCardPrefab != null)
		{
			GameManager.Instance.GuildManager.NotifyGuildAdViewed(item.AdIdentifierForAnalytics);
			return Helpers.InstantiateToParentAndLayer(adCardPrefab, cardsContainer);
		}
		return Helpers.InstantiateToParentAndLayer(cardPrefab, cardsContainer);
	}
}
