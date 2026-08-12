using System;
using System.Collections;
using BaseModel;
using TWDModel;
using UnityEngine;

public class QuestsPopup : HUDElement
{
	[SerializeField]
	private GameObject GoogleAchievementParent;

	[SerializeField]
	private UITabs tabs;

	[SerializeField]
	private UILabel timerLabel;

	[Header("Tab Content Panels")]
	[SerializeField]
	private DailyQuestListPanel2 dailyQuestListPanel;

	[SerializeField]
	private AchievementListPanel achievementListPanel;

	[SerializeField]
	private Transform questChestTooltipLocation;

	private TimeSpan lastTimeUntilRefresh = TimeSpan.MaxValue;

	public static void OpenQuestsPopup()
	{
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.QuestsPopup);
		hUDElement.Open();
		if (hUDElement.IsOpen)
		{
			ShopPopup shopPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ShopPopup, null, createIfNotExist: false) as ShopPopup;
			if (shopPopup != null)
			{
				shopPopup.Close();
			}
		}
	}

	public override void Start()
	{
		base.Start();
		UpdateGoogleAchievementButton();
	}

	public override void Update()
	{
		DateTime utcTime = GameManager.Instance.playerModel.UtcTime;
		utcTime = new DateTime(utcTime.Year, utcTime.Month, utcTime.Day, utcTime.Hour, utcTime.Minute, 0);
		TimeSpan timeSpan = GameManager.Instance.playerModel.DailyQuestManager.NextQuestRefreshTimeUtc - utcTime;
		if (timeSpan < lastTimeUntilRefresh)
		{
			UpdateTimer(timeSpan);
		}
	}

	private void UpdateTimer(TimeSpan timeUntilRefresh)
	{
		lastTimeUntilRefresh = timeUntilRefresh;
		if (timerLabel != null)
		{
			int num = 24 * timeUntilRefresh.Days + timeUntilRefresh.Hours;
			timerLabel.text = LocalizationManager.GetText("DailyQuest.NewQuestsTimer.Text{Hours}{Minutes}", num, timeUntilRefresh.Minutes);
		}
	}

	private void OnDailyQuestsChanged(ModelObject model, string changed, object args)
	{
		DateTime utcTime = GameManager.Instance.playerModel.UtcTime;
		utcTime = new DateTime(utcTime.Year, utcTime.Month, utcTime.Day, utcTime.Hour, utcTime.Minute, 0);
		TimeSpan timeUntilRefresh = GameManager.Instance.playerModel.DailyQuestManager.NextQuestRefreshTimeUtc - utcTime;
		UpdateTimer(timeUntilRefresh);
	}

	public void OnQuestChestClicked()
	{
		DailyQuestModel dailyQuestManager = GameManager.Instance.playerModel.DailyQuestManager;
		if (dailyQuestManager != null)
		{
			DailyQuestChestDefinition currentQuestChestDefinition = dailyQuestManager.CurrentQuestChestDefinition;
			if (currentQuestChestDefinition != null && !(questChestTooltipLocation == null))
			{
				TooltipManager.OpenTextBoxWithText(questChestTooltipLocation.gameObject, LocalizationManager.GetText($"DailyQuest.QuestChestInfo.{currentQuestChestDefinition.Tag.ToString()}"));
			}
		}
	}

	public void OnGooglePlayAchievements()
	{
		GameManager.Instance.GameCenterManager.OpenSystemDefaultAchievementsUI();
	}

	public override void Open()
	{
		base.Open();
		if (!GameManager.Instance.modelManager.Player.DailyQuestManager.QuestsInitialized)
		{
			Helpers.ExecuteCommand(new TryInitializeDailyQuestsCommand());
		}
		GameManager.Instance.playerModel.DailyQuestManager.Changed -= OnDailyQuestsChanged;
		GameManager.Instance.playerModel.DailyQuestManager.Changed += OnDailyQuestsChanged;
		tabs.OnNewTabSelectedEvent -= OnTabSelected;
		tabs.OnNewTabSelectedEvent += OnTabSelected;
	}

	public override void Close()
	{
		base.Close();
		GameManager.Instance.playerModel.DailyQuestManager.Changed -= OnDailyQuestsChanged;
		tabs.OnNewTabSelectedEvent -= OnTabSelected;
	}

	protected IEnumerator OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			yield return null;
			yield return null;
			UpdateGoogleAchievementButton();
		}
	}

	private void UpdateGoogleAchievementButton()
	{
		Helpers.GameObjectSetActive(GoogleAchievementParent, value: false);
	}

	public void SelectTabAchievements()
	{
		tabs.SelectTab(0);
	}

	public void SelectTabDailyQuests()
	{
		tabs.SelectTab(1);
	}

	private void OnTabSelected(int tabindex)
	{
		if (!(tabs.GetContent(tabindex) != null))
		{
			return;
		}
		switch (tabindex)
		{
		case 0:
			if (dailyQuestListPanel != null)
			{
				dailyQuestListPanel.Init();
			}
			break;
		case 2:
			if (achievementListPanel != null)
			{
				achievementListPanel.Init();
			}
			break;
		}
	}
}
