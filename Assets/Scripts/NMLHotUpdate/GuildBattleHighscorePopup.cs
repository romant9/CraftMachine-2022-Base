using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class GuildBattleHighscorePopup : HUDElement
{
	[SerializeField]
	private UIButtonToggleSet toggleMenu;

	[SerializeField]
	private UILabel filterDropDownLabel;

	[SerializeField]
	private UIPopupList filterDropDown;

	[SerializeField]
	private NUIScrollableList listComponent;

	[SerializeField]
	private string guildBoardItem = "GuildBattleGuildScoreItem";

	[SerializeField]
	private string playerBoardItem = "GuildBattleGuildScoreItem";

	[SerializeField]
	private GameObject GlobalBoardExtraParent;

	[SerializeField]
	private GameObject MembersBoardExtraParent;

	protected List<GuildBattleGuildLeaderboardDataProvider> providers;

	protected EventDelegate typeDropDownCallback;

	private string cachedGuildId = "";

	private int cachedSelectedIndex = -1;

	private void OnEnable()
	{
		InitializeProviders();
		if (typeDropDownCallback == null)
		{
			typeDropDownCallback = new EventDelegate(OnDropDownChange);
		}
		if (toggleMenu != null)
		{
			toggleMenu.SetSelectedIndex(0);
			toggleMenu.SetChangeCallback(UpdateFilterDropDown);
			if (toggleMenu.GetUIButtonToggleList.Length > 1 && toggleMenu.GetUIButtonToggleList[1] != null)
			{
				string text = (GameManager.Instance.playerModel.IsGuildMember ? GameManager.Instance.playerModel.GuildModel.Name : "");
				toggleMenu.GetUIButtonToggleList[1].SetContentToLabelOne(LocalizationManager.GetText("GvG.Highscores.Tab.TopMembers{GuildName}", text));
			}
		}
	}

	private void UpdateFilterDropDown(UIButtonExtended button)
	{
		if (toggleMenu == null || filterDropDown == null || listComponent == null || string.IsNullOrEmpty(cachedGuildId))
		{
			return;
		}
		int selectedIndex = toggleMenu.GetSelectedIndex();
		if (cachedSelectedIndex != selectedIndex)
		{
			cachedSelectedIndex = selectedIndex;
			listComponent.Clear();
		}
		int currentSeasonDefinitionId = GuildWarHelper.GetCurrentSeasonDefinitionId();
		int currentWarDefinitionId = GuildWarHelper.GetCurrentWarDefinitionId();
		filterDropDown.Clear();
		switch (selectedIndex)
		{
		case 0:
			AddFilterButton(Leaderboards.GvgGuildGlobalVpWarTotalPrefix, Leaderboards.GetLeaderboardNameGuildGlobalWar(currentWarDefinitionId));
			AddFilterButton(Leaderboards.GvgGuildGlobalVpSeasonTotalPrefix, Leaderboards.GetLeaderboardNameGuildGlobalSeason(currentSeasonDefinitionId));
			AddFilterButton(Leaderboards.GvgGuildGlobalVpAllTimeTotal, Leaderboards.GvgGuildGlobalVpAllTimeTotal);
			break;
		case 1:
			AddFilterButton(Leaderboards.GvgGuildMembersVpWarTotalPrefix, Leaderboards.GetLeaderboardNameGuildMembersWar(currentWarDefinitionId, cachedGuildId));
			AddFilterButton(Leaderboards.GvgGuildMembersVpSeasonTotalPrefix, Leaderboards.GetLeaderboardNameGuildMembersSeason(currentSeasonDefinitionId, cachedGuildId));
			AddFilterButton(Leaderboards.GvgGuildMembersVpAllTimeTotalPrefix, Leaderboards.GetLeaderboardNameGuildMembersAlltime(cachedGuildId));
			break;
		}
		Helpers.GameObjectSetActive(GlobalBoardExtraParent, selectedIndex == 0);
		Helpers.GameObjectSetActive(MembersBoardExtraParent, selectedIndex == 1);
		if (filterDropDown != null && !filterDropDown.onChange.Contains(typeDropDownCallback))
		{
			filterDropDown.onChange.Add(typeDropDownCallback);
		}
		if (filterDropDown.items.Count > 0)
		{
			if (string.IsNullOrEmpty(filterDropDown.value))
			{
				filterDropDown.Set(filterDropDown.items[0]);
			}
			else
			{
				filterDropDown.TriggerCallbacks();
			}
		}
	}

	private void AddFilterButton(string text, object data)
	{
		if (!(filterDropDown == null) && GameManager.Instance.gameEconomyData.GetFeature(text).Enabled)
		{
			filterDropDown.AddItem(HelpersLocalization.GetLeaderboardName(text), data);
		}
	}

	private void OnDisable()
	{
		if (providers != null)
		{
			for (int i = 0; i < providers.Count; i++)
			{
				providers[i].OnDataReceived -= OnDataReceived;
				providers[i].Clear();
			}
			providers.Clear();
		}
		if (filterDropDown != null)
		{
			filterDropDown.Clear();
			filterDropDown.onChange.Remove(typeDropDownCallback);
		}
		if (toggleMenu != null)
		{
			toggleMenu.Clear();
		}
	}

	private void OnDropDownChange()
	{
		if (filterDropDown == null || filterDropDown.data == null)
		{
			return;
		}
		HelpersUI.SetContentToLabel(filterDropDownLabel, filterDropDown.value);
		for (int i = 0; i < providers.Count; i++)
		{
			if (providers[i] != null)
			{
				string text = filterDropDown.data.ToString();
				if (providers[i].GetLeaderboardName() == text)
				{
					providers[i].RequestData();
					break;
				}
			}
		}
	}

	private void InitializeProviders()
	{
		if (providers == null)
		{
			providers = new List<GuildBattleGuildLeaderboardDataProvider>();
		}
		int currentSeasonDefinitionId = GuildWarHelper.GetCurrentSeasonDefinitionId();
		int currentWarDefinitionId = GuildWarHelper.GetCurrentWarDefinitionId();
		if (!(GameManager.Instance == null) && GameManager.Instance.playerModel.IsGuildMember)
		{
			cachedGuildId = GameManager.Instance.playerModel.GuildId;
			SetCachedDataProvider(Leaderboards.GvgGuildGlobalVpAllTimeTotal);
			providers.Add(new GuildBattleGuildLeaderboardDataProvider(Leaderboards.GetLeaderboardNameGuildMembersAlltime(cachedGuildId), GameManager.Instance.playerModel.GuildModel, OnDataReceived));
			if (currentSeasonDefinitionId > -1)
			{
				SetCachedDataProvider(Leaderboards.GetLeaderboardNameGuildGlobalSeason(currentSeasonDefinitionId));
				providers.Add(new GuildBattleGuildLeaderboardDataProvider(Leaderboards.GetLeaderboardNameGuildMembersSeason(currentSeasonDefinitionId, cachedGuildId), GameManager.Instance.playerModel.GuildModel, OnDataReceived));
			}
			if (currentWarDefinitionId > -1)
			{
				SetCachedDataProvider(Leaderboards.GetLeaderboardNameGuildGlobalWar(currentWarDefinitionId));
				providers.Add(new GuildBattleGuildLeaderboardDataProvider(Leaderboards.GetLeaderboardNameGuildMembersWar(currentWarDefinitionId, cachedGuildId), GameManager.Instance.playerModel.GuildModel, OnDataReceived));
			}
		}
	}

	private void SetCachedDataProvider(string leaderboardName)
	{
		GuildBattleGuildLeaderboardDataProvider guildBattleGuildLeaderboardDataProvider = GameManager.Instance.CachedLeaderboardsManager.GetLeaderBoard(leaderboardName) as GuildBattleGuildLeaderboardDataProvider;
		if (guildBattleGuildLeaderboardDataProvider == null)
		{
			guildBattleGuildLeaderboardDataProvider = new GuildBattleGuildLeaderboardDataProvider(leaderboardName, null, null, "100", 300, cached: true);
			GameManager.Instance.CachedLeaderboardsManager.AddLeaderboard(leaderboardName, guildBattleGuildLeaderboardDataProvider);
		}
		guildBattleGuildLeaderboardDataProvider.OnDataReceived += OnDataReceived;
		providers.Add(guildBattleGuildLeaderboardDataProvider);
	}

	private void OnDataReceived(ScoreDataProvider scoreDataProvider, List<ScoreDataEntry> listScoreDataEntries)
	{
		if (!(listComponent == null) && scoreDataProvider is GuildBattleGuildLeaderboardDataProvider guildBattleGuildLeaderboardDataProvider)
		{
			string text = (guildBattleGuildLeaderboardDataProvider.IsGuildBoard() ? playerBoardItem : guildBoardItem);
			listComponent.UpdateWithList(listScoreDataEntries, text, text);
			StartCoroutine(SortAndResetScrollPosition());
		}
	}

	private IEnumerator SortAndResetScrollPosition()
	{
		listComponent.SortAndRepositionItems();
		yield return null;
		listComponent.ResetScrollPosition();
	}
}
