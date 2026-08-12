using BaseModel;
using System;
using System.Collections;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class SocialPopupGuild : HUDElement
{
	public const int GuildInfoTabIndex = 0;

	public const int GuildWarTabIndex = 2;

	public const int GuildChallengeTabIndex = 1;

	public const int GuildChatTabIndex = 3;

	public const int GuildShopTabIndex = 4;

	[SerializeField]
	private UIButtonToggleSet tabButtons;

	[SerializeField]
	private UIButton guildWarButton;

	[SerializeField]
	private UILabel guildWarTimer;

	[SerializeField]
	private GameObject battleOngoingIndicator;

	[SerializeField]
	private UIButton challengeButton;

	[SerializeField]
	private UILabel challengeTimerLabel;

	[SerializeField]
	private UIButton chatButton;

	[SerializeField]
	private UIButton guildShopButton;

	[SerializeField]
	private GameObject guildShopNewIndicator;

	[SerializeField]
	private GameObject guildTabNewIndicator;

	[SerializeField]
	private GameObject indicatorResetAlert;

	[SerializeField]
	private ThingsToDoIndicator chatMessagesNotifications;

	[SerializeField]
	private int timerRefreshRate = 1;

	private float refreshTimer;

	[HideInInspector]
	private int defaultStartIndex;

	private UIToggleMenu ToggleSet;

	private bool isGuildMember;

	public int SelectedTab => tabButtons.GetSelectedIndex();

	public void OpenForTab(int tabIndex)
	{
		if (base.IsOpen)
		{
			if (!ToggleSet) ToggleSet = GetComponentInChildren<UIToggleMenu>();

			ToggleSet.OpenContentByIndex(tabIndex);
			return;
		}
		defaultStartIndex = tabIndex;
		Open();
	}

	public override void Open()
	{
		if (IsLoadDataManager && GameManager.Instance.guildModel == null)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager && GameManager.Instance.guildModel == null)");
			return;
		}

		base.Open();
		UIEvent.OnUIEvent += OnUIEvent;
		tabButtons.SetInitialToggle(Mathf.Clamp(defaultStartIndex, 0, tabButtons.GetUIButtonToggleList.Length - 1));
		ToggleSet = base.gameObject.GetComponentInChildren<UIToggleMenu>();
		if (ToggleSet != null)
		{
			UIToggleMenu toggleSet = ToggleSet;
			toggleSet.OnChangeDelegate = (UIButtonToggleSet.OnTabsChangeDelegate)Delegate.Remove(toggleSet.OnChangeDelegate, new UIButtonToggleSet.OnTabsChangeDelegate(OnNewTabSelected));
			UIToggleMenu toggleSet2 = ToggleSet;
			toggleSet2.OnChangeDelegate = (UIButtonToggleSet.OnTabsChangeDelegate)Delegate.Combine(toggleSet2.OnChangeDelegate, new UIButtonToggleSet.OnTabsChangeDelegate(OnNewTabSelected));
		}
		Setup();
	}

	public override void OnClickClose()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			ResidencePopup.Instance.OpenAtTabIndex(0);
			return;
		}
		base.OnClickClose();
	}

	public override void Close()
	{
		if (!(this == null) && !(base.gameObject == null))
		{
			base.Close();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/social_close");
		}
	}

	private void OnEnable()
	{
		if (IsLoadDataManager)
		{
			if (OfflineManager.Instance.Player != null)
			{
				DebugTWD.LogMycode("if (IsLoadDataManager && OfflineManager.Instance.Player != null)");
				GWTeamUtils.Instance.SwitchCustomGuild.gameObject.SetActive(DataManager.Instance.ProGuild);
			}
			return;
		}
		if (GameManager.Instance != null)
		{
			GameManager.Instance.playerModel.Changed += OnPlayerChanged;
		}
		if (ToggleSet != null)
		{
			UIToggleMenu toggleSet = ToggleSet;
			toggleSet.OnChangeDelegate = (UIButtonToggleSet.OnTabsChangeDelegate)Delegate.Remove(toggleSet.OnChangeDelegate, new UIButtonToggleSet.OnTabsChangeDelegate(OnNewTabSelected));
			UIToggleMenu toggleSet2 = ToggleSet;
			toggleSet2.OnChangeDelegate = (UIButtonToggleSet.OnTabsChangeDelegate)Delegate.Combine(toggleSet2.OnChangeDelegate, new UIButtonToggleSet.OnTabsChangeDelegate(OnNewTabSelected));
		}
		if (GameManager.Instance.guildModel != null)
		{
			GameManager.Instance.guildModel.Changed -= OnGuildChanged;
			GameManager.Instance.guildModel.Changed += OnGuildChanged;
		}
	}

	private void OnDisable()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager && (OfflineManager.Instance.Player != null && ToggleSet != null)");
			if (OfflineManager.Instance.Player != null && ToggleSet != null)
			{
				UIToggleMenu toggleSet = ToggleSet;
				toggleSet.OnChangeDelegate = (UIButtonToggleSet.OnTabsChangeDelegate)Delegate.Remove(toggleSet.OnChangeDelegate, new UIButtonToggleSet.OnTabsChangeDelegate(OnNewTabSelected));
			}
			return;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		GuildModel guildModel = playerModel.GuildModel;
		playerModel.Changed -= OnPlayerChanged;
		if (guildModel != null)
		{
			guildModel.Changed -= OnGuildChanged;
		}
		if (ToggleSet != null)
		{
			UIToggleMenu toggleSet = ToggleSet;
			toggleSet.OnChangeDelegate = (UIButtonToggleSet.OnTabsChangeDelegate)Delegate.Remove(toggleSet.OnChangeDelegate, new UIButtonToggleSet.OnTabsChangeDelegate(OnNewTabSelected));
		}
		if (GameManager.Instance.guildModel != null)
		{
			GameManager.Instance.guildModel.Changed -= OnGuildChanged;
		}
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void Setup()
	{
		isGuildMember = GameManager.Instance.playerModel.IsGuildMember;
		SetupTabButtons();
		UpdateNotifications();
	}

	private void SetupTabButtons()
	{
		guildWarButton.isEnabled = isGuildMember;
		challengeButton.isEnabled = isGuildMember;
		chatButton.isEnabled = isGuildMember;
		guildShopButton.isEnabled = isGuildMember;
		bool flag = GuildWarHelper.CheckForGuildShopResetWarning();
		Helpers.GameObjectSetActive(indicatorResetAlert, flag);
		Helpers.GameObjectSetActive(guildShopNewIndicator, !flag && GameManager.Instance.playerModel.GuildShopModel.HasNewItems());
		Helpers.GameObjectSetActive(guildTabNewIndicator, isGuildMember && GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.FreeGuildGiftPerk) > 0);
	}

	private void OnPlayerChanged(ModelObject modelObject, string changed, object args)
	{
		if (changed == "guildChanged")
		{
			if (GameManager.Instance.guildModel != null)
			{
				GameManager.Instance.guildModel.Changed -= OnGuildChanged;
				GameManager.Instance.guildModel.Changed += OnGuildChanged;
			}
			else
			{
				Setup();
				ToggleSet.OpenContentByIndex(0);
			}
		}
	}

	private void OnGuildChanged(GroupModelBase groupModelBase, string changed, object args)
	{
		if (changed == "GuildCreated")
		{
			Setup();
		}
		else if (changed == "MemberRemoved")
		{
			Setup();
		}
		if (changed == "MessageAdded" || changed == "MessagesTruncated")
		{
			UIEvent.Send("SocialChatNewMessage");
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (IsLoadDataManager && GameManager.Instance.guildModel == null)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager && GameManager.Instance.guildModel == null)");
			return;
		}
		switch (type)
		{
		case "SocialChatRead":
		case "SocialChatNewMessage":
			UpdateNotifications();
			break;
		case "SocialGuildJoined":
			Setup();
			break;
		}
		if (parameter is MissionHubPopup && type == "OnPopUpOpen")
		{
			Close();
		}
	}

	public override void Update()
	{
		base.Update();
		refreshTimer -= Time.deltaTime;
		if (refreshTimer <= 0f)
		{
			RefreshTimers();
			refreshTimer = timerRefreshRate;
		}
	}

	private void RefreshTimers()
	{
		long timeLeftToNextWarForNonGuildMember = GuildWarHelper.GetTimeLeftToNextWarForNonGuildMember();
		if (timeLeftToNextWarForNonGuildMember == 0L)
		{
			HelpersUI.SetContentToLabel(guildWarTimer, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.Guild.WarEndingIn{Parameter}", GuildWarHelper.GetFormatedTimeLeftToCurrentWarEnd(isGuildMember: false)));
		}
		else if (timeLeftToNextWarForNonGuildMember > 0)
		{
			HelpersUI.SetContentToLabel(guildWarTimer, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.Guild.WarStartingIn{Parameter}", Helpers.FormatTime(timeLeftToNextWarForNonGuildMember)));
		}
		else
		{
			HelpersUI.SetContentToLabel(guildWarTimer, "");
		}
		Helpers.GameObjectSetActive(battleOngoingIndicator, GuildWarHelper.IsBattleOnGoing());
		if (WeeklyChallengeHelper.IsChallengeOngoing())
		{
			HelpersUI.SetContentToLabel(challengeTimerLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Map.WeeklyChallenge.EndsIn{Time}", WeeklyChallengeHelper.GetFormatedTimeLeftToCurrentChallengeEnd()));
		}
		else
		{
			HelpersUI.SetContentToLabel(challengeTimerLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Map.WeeklyChallenge.StartsIn{Time}", WeeklyChallengeHelper.GetFormatedTimeToNextChallengeStart()));
		}
	}

	private void UpdateNotifications()
	{
		if (chatMessagesNotifications == null)
		{
			return;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel.IsGuildMember)
		{
			GuildModel guildModel = GameManager.Instance.guildModel;
			int number = 0;
			if (guildModel != null)
			{
				number = guildModel.GetUnreadChatAmount(playerModel.HashedId, playerModel.LastReadChatTime);
			}
			chatMessagesNotifications.SetNumber(number);
		}
		else
		{
			chatMessagesNotifications.SetNumber(0);
		}
	}

	private void OnNewTabSelected(UIButtonExtended button)
	{
		if (SelectedTab == 4)
		{
			Helpers.GameObjectSetActive(guildShopNewIndicator, value: false);
		}
		if (SelectedTab == 0)
		{
			Helpers.GameObjectSetActive(guildTabNewIndicator, value: false);
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_tab_change");
		UIEvent.Send("GuildTabChanged");
	}




	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	public static SocialPopupGuild Instance { get; private set; }
	#endregion

	#region mycode
	private void Awake()
	{
		if (IsLoadDataManager)
		{
			if (Instance != null)
			{
				Debug.LogError("Multiple SocialPopupGuild!");
				return;
			}
			Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	public IEnumerator WaitForData()
	{
		float startTime = Time.realtimeSinceStartup;
		while (!GWTeamUtils.Instance.IsGuildLoaded)
		{
			if (Time.realtimeSinceStartup - startTime > 20f)
			{
				DebugTWD.LogWarning("Can't load player");
				yield break;
			}
			yield return null;
		}
		OpenForTab(0);
	}
	#endregion
}
