using System;
using BaseModel;
using TWDModel;
using UnityEngine;

public class NewBieQuestsPopup : HUDElement
{
	[SerializeField]
	private UITabs tabs;

	[SerializeField]
	private UILabel timerLabel;

	[Header("Tab Content Panels")]
	[SerializeField]
	private NewBieQuestListPanel newBieQuestListPanel;

	[SerializeField]
	private Transform questChestTooltipLocation;

	[SerializeField]
	private GameObject[] lockIcons;

	[SerializeField]
	private GameObject timerContent;

	private TimeSpan lastTimeUntilRefresh = TimeSpan.MaxValue;

	public static void OpenQuestsPopup()
	{
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NewBieQuestsPopup);
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
	}

	public override void Update()
	{
	}

	private void UpdateTimer(TimeSpan timeUntilRefresh)
	{
	}

	private void OnDailyQuestsChanged(ModelObject model, string changed, object args)
	{
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

	public override void Open()
	{
		base.Open();
		tabs.OnNewTabSelectedEvent -= OnTabSelected;
		tabs.OnNewTabSelectedEvent += OnTabSelected;
		int unlockDay = GameManager.Instance.modelManager.Player.NewbieSenvenQuest.UnlockDay;
		for (int i = 0; i < lockIcons.Length; i++)
		{
			bool active = true;
			if (unlockDay > i)
			{
				active = false;
			}
			lockIcons[i].SetActive(active);
		}
	}

	public override void Close()
	{
		base.Close();
		tabs.OnNewTabSelectedEvent -= OnTabSelected;
	}

	private void OnTabSelected(int tabindex)
	{
		tabs.GetContent(tabindex).SetActive(value: false);
		tabs.GetContent(0).SetActive(value: true);
		if (newBieQuestListPanel != null)
		{
			newBieQuestListPanel.Init(tabindex);
		}
		UpdateTime(tabindex + 1);
	}

	private void UpdateTime(int day)
	{
		NewbieSevenQuestModel newbieSenvenQuest = GameManager.Instance.modelManager.Player.NewbieSenvenQuest;
		if (day <= newbieSenvenQuest.UnlockDay)
		{
			timerContent.SetActive(value: false);
			return;
		}
		timerContent.SetActive(value: true);
		long dayUnlockLeftTime = newbieSenvenQuest.GetDayUnlockLeftTime(day);
		if (dayUnlockLeftTime >= 0)
		{
			int num = (int)(dayUnlockLeftTime / 1000);
			int num2 = num / 86400;
			int num3 = num - num2 * 24 * 60 * 60;
			int num4 = num3 / 3600;
			int num5 = (num3 - num4 * 60 * 60) / 60;
			if (num2 > 0)
			{
				timerLabel.text = LocalizationManager.GetText("Popup.NewbieSevenQuest.RefreshTime", num2, num4);
			}
			else
			{
				timerLabel.text = LocalizationManager.GetText("Popup.NewbieSevenQuest.RefreshTimeHourMinute", num4, num5);
			}
		}
		else
		{
			timerContent.SetActive(value: false);
		}
	}
}
