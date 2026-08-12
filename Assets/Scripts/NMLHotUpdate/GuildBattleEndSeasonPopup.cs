using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class GuildBattleEndSeasonPopup : HUDElement
{
	[SerializeField]
	private GameObject page1Container;

	[SerializeField]
	private GameObject page2Container;

	[SerializeField]
	private GameObject closePopupArea;

	[SerializeField]
	private UIButtonExtended closePopupAreaBtn;

	[Header("Page 1")]
	[SerializeField]
	private UIButtonExtended nextButton;

	[SerializeField]
	private UILabel battleWonLabel;

	[SerializeField]
	private UILabel battleLostLabel;

	[Header("Page 2")]
	[SerializeField]
	private UIButtonExtended continueButton;

	[SerializeField]
	private UILabel resetTimerLabel;

	[SerializeField]
	private UILabel rewardPointsAmountLabel;

	[SerializeField]
	private GuildBattleHighscoresPlayerEntry[] topPlayersEntry;

	private float refreshTimer;

	private int refreshRate = 1;

	private ScoreDataProvider scoreProvider;

	private bool onSecondPage;

	public static bool CanShow()
	{
		bool num = GuildWarHelper.IsSeasonOngoing();
		bool flag = GuildWarHelper.HasSeenGvGSeasonEnd();
		bool flag2 = GuildWarHelper.GetGvGSeasonModelPlayer().IsCurrentSeasonEnded();
		bool flag3 = GuildWarHelper.IsLockedByCouncilLevelOrTutorial();
		if (!num && flag2 && !flag)
		{
			return !flag3;
		}
		return false;
	}

	public override void Open()
	{
		base.Open();
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.GuildBattleMapPopup);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.GuildBattleOverviewPopup);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.StartWarBannerPopup);
		if (continueButton != null)
		{
			continueButton.SetClickCallback(OnContinueButtonClicked);
		}
		if (nextButton != null)
		{
			nextButton.SetClickCallback(OnNextButtonClicked);
		}
		if (closePopupAreaBtn != null)
		{
			closePopupAreaBtn.SetClickCallback(OnContinueButtonClicked);
			closePopupAreaBtn.SetClickCallback(OnNextButtonClicked);
		}
		int currentSeasonDefinitionId = GuildWarHelper.GetCurrentSeasonDefinitionId();
		string guildId = GameManager.Instance.playerModel.GuildId;
		if (currentSeasonDefinitionId > -1 && !string.IsNullOrEmpty(guildId))
		{
			scoreProvider = new GuildBattleGuildLeaderboardDataProvider(Leaderboards.GetLeaderboardNameGuildMembersSeason(currentSeasonDefinitionId, guildId), GameManager.Instance.playerModel.GuildModel, OnDataReceived);
			scoreProvider.RequestData(forceFetch: true);
		}
		Helpers.GameObjectSetActive(page1Container, value: true);
		Helpers.GameObjectSetActive(page2Container, value: false);
		UpdateUI();
		UpdateDynamicUI();
	}

	private void OnDataReceived(ScoreDataProvider scoreDataProvider, List<ScoreDataEntry> listScoreDataEntries)
	{
		if (listScoreDataEntries == null || !(scoreDataProvider is GuildBattleGuildLeaderboardDataProvider))
		{
			return;
		}
		for (int i = 0; i < topPlayersEntry.Length; i++)
		{
			GuildBattleHighscoresPlayerEntry guildBattleHighscoresPlayerEntry = topPlayersEntry[i];
			if (i < listScoreDataEntries.Count)
			{
				guildBattleHighscoresPlayerEntry.SetPlayerData(listScoreDataEntries[i] as GuildBattlePlayersScoreDataEntry);
			}
			else
			{
				guildBattleHighscoresPlayerEntry.SetPlayerData(null);
			}
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (guildModel != null)
		{
			HelpersUI.SetContentToLabel(battleWonLabel, guildModel.CurrentSeasonVictories.ToString());
			HelpersUI.SetContentToLabel(battleLostLabel, guildModel.CurrentSeasonDefeats.ToString());
		}
		HelpersUI.SetContentToLabel(rewardPointsAmountLabel, GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.GuildBattleRP).ToString());
		HelpersUI.SetContentToLabel(resetTimerLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.GuildShop.SeasonResetIn{parameter}", GuildWarHelper.GetFormatedTimeLeftToNextSeason()));
	}

	private void UpdateDynamicUI()
	{
		HelpersUI.SetContentToLabel(resetTimerLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.GuildShop.SeasonResetIn{parameter}", GuildWarHelper.GetFormatedTimeLeftToNextSeason()));
	}

	public override void Update()
	{
		base.Update();
		refreshTimer -= Time.deltaTime;
		if (refreshTimer < 0f)
		{
			UpdateDynamicUI();
			refreshTimer = refreshRate;
		}
	}

	private void OnNextButtonClicked(UIButtonExtended button)
	{
		Helpers.GameObjectSetActive(page1Container, value: false);
		Helpers.GameObjectSetActive(page2Container, value: true);
		onSecondPage = true;
	}

	private void OnContinueButtonClicked(UIButtonExtended button)
	{
		Close();
	}

	public override void Close()
	{
		if (onSecondPage)
		{
			TweenManager.PlayTweenGroup(closePopupArea, 2);
			GuildWarHelper.SetHasSeenGvGSeasonEndFlag();
			base.Close();
		}
	}



	#region myparams
	private GuildBattleMapView viewInstance;
	#endregion

	#region mycode
	public void CloseMapView()
	{
		if (OfflineManager.IsLoadDataManager)
		{
			viewInstance = FindAnyObjectByType<GuildBattleMapView>();
			if (viewInstance != null)
			{
				viewInstance.Clear();
				Object.Destroy(viewInstance.gameObject);
			}
		}
	}
	#endregion
}
