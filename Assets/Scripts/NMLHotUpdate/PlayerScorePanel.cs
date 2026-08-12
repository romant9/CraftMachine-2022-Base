using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class PlayerScorePanel : ScrollableListPanel<ScoreDataEntry>
{
	[SerializeField]
	protected UISprite loadingSprite;

	[SerializeField]
	protected UIButtonToggleSet tabs;

	[SerializeField]
	[Tooltip("The label of the tab for local leaderboard")]
	protected UILabel localActiveTabLabel;

	[SerializeField]
	[Tooltip("The label of the tab for local leaderboard")]
	protected UILabel localInactiveTabLabel;

	[SerializeField]
	protected GameObject notInAGuildTextContainer;

	protected List<ScoreDataProvider> providers;

	protected List<GuildPlayerListCardBase.GuildPlayerListCardType> cardTypes;

	protected bool updateCardsOnEnable;

	protected GuildPlayerListCardBase.GuildPlayerListCardType currentProviderCardType;

	protected virtual void OnEnable()
	{
		if (tabs != null)
		{
			UIButtonToggleSet uIButtonToggleSet = tabs;
			uIButtonToggleSet.OnChangeDelegate = (UIButtonToggleSet.OnTabsChangeDelegate)Delegate.Combine(uIButtonToggleSet.OnChangeDelegate, new UIButtonToggleSet.OnTabsChangeDelegate(OnNewTabSelected));
		}
		providers = new List<ScoreDataProvider>();
		cardTypes = new List<GuildPlayerListCardBase.GuildPlayerListCardType>();
		InitializeProviders();
		for (int i = 0; i < providers.Count; i++)
		{
			providers[i].OnDataReceived += OnDataReceived;
		}
		SetLocalTabsTextLabel();
		if (updateCardsOnEnable)
		{
			PositionCards();
			updateCardsOnEnable = false;
		}
	}

	protected virtual void SetLocalTabsTextLabel()
	{
		if (localInactiveTabLabel != null)
		{
			string text = GameManager.GetCountryCode().ToUpper();
			localInactiveTabLabel.text = LocalizationManager.GetText("Popup.Social.HighScore.Local") + "(" + text + ")";
			localActiveTabLabel.text = localInactiveTabLabel.text;
		}
	}

	protected virtual void InitializeProviders()
	{
		providers.Add(new GuildMemberScoreDataProvider());
		providers.Add(new PlayerLeaderboardScoreDataProvider(Leaderboards.ChallengeStarsGlobal));
		providers.Add(new PlayerLeaderboardScoreDataProvider(Leaderboards.ChallengeStarsCountryPrefix + GameManager.GetCountryCode()));
		providers.Add(new FriendsScoreDataProvider());
		cardTypes.Add(GuildPlayerListCardBase.GuildPlayerListCardType.GuildPlayerList);
		cardTypes.Add(GuildPlayerListCardBase.GuildPlayerListCardType.PlayerList);
		cardTypes.Add(GuildPlayerListCardBase.GuildPlayerListCardType.PlayerList);
		cardTypes.Add(GuildPlayerListCardBase.GuildPlayerListCardType.FriendList);
	}

	protected virtual void OnDisable()
	{
		if (tabs != null)
		{
			UIButtonToggleSet uIButtonToggleSet = tabs;
			uIButtonToggleSet.OnChangeDelegate = (UIButtonToggleSet.OnTabsChangeDelegate)Delegate.Remove(uIButtonToggleSet.OnChangeDelegate, new UIButtonToggleSet.OnTabsChangeDelegate(OnNewTabSelected));
		}
		for (int i = 0; i < providers.Count; i++)
		{
			providers[i].OnDataReceived -= OnDataReceived;
		}
	}

	protected virtual void OnNewTabSelected(UIButtonExtended button)
	{
		if (button == null)
		{
			return;
		}
		int num = int.Parse(button.id);
		loadingSprite.gameObject.SetActive(value: true);
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_tab_change");
		}
		foreach (UIListCard<ScoreDataEntry> card in cards)
		{
			card.GetComponent<CacheableObject>().Destroy();
		}
		providers[num].RequestData();
		if (notInAGuildTextContainer != null)
		{
			notInAGuildTextContainer.SetActive(num == 0);
		}
	}

	protected virtual void OnDataReceived(ScoreDataProvider provider, List<ScoreDataEntry> data)
	{
		int num = providers.IndexOf(provider);
		if (num == -1 || data == null)
		{
			ClearCards();
			loadingSprite.gameObject.SetActive(value: false);
			return;
		}
		if (tabs.GetSelectedIndex() != num)
		{
			tabs.SetSelectedIndex(num);
		}
		loadingSprite.gameObject.SetActive(value: false);
		int num2 = 100;
		if (data.Count > num2)
		{
			data.RemoveRange(num2, data.Count - num2 - 1);
		}
		currentProviderCardType = cardTypes[num];
		SetCards(data);
		if (!base.gameObject.activeInHierarchy)
		{
			updateCardsOnEnable = true;
		}
		for (int i = 0; i < cards.Count; i++)
		{
			((GuildPlayerListCard)cards[i]).SetRank(cards.Count - i);
		}
	}

	protected override void SetCard(UIListCard<ScoreDataEntry> card)
	{
		base.SetCard(card);
		((GuildPlayerListCard)card).Type = currentProviderCardType;
	}
}
