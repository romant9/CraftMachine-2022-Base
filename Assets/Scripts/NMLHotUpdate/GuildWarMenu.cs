using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class GuildWarMenu : UIToggleContent
{
	[SerializeField]
	private GameObject notMemberContainer;

	[SerializeField]
	private GameObject memberContainer;

	[Header("Member View")]
	[SerializeField]
	private UILabel tierNumberLabel;

	[SerializeField]
	private UILabel battlesWonLabel;

	[SerializeField]
	private UILabel battleLostLabel;

	[SerializeField]
	private UIButton getGasButton;

	[Header("Battle Log")]
	[SerializeField]
	private UILabel emptyLogInfoLabel;

	[SerializeField]
	private UIButtonToggleSet warButtons;

	[SerializeField]
	private UILabel[] warButtonsLabels;

	[SerializeField]
	private NUIScrollableList battleListScroll;

	[SerializeField]
	private GameObject battleEntryPrefab;

	[SerializeField]
	private GameObject callForBattlePrefab;

	private int selectedIndex = -1;

	private UIToggleContent UIToggleContentRef;

	private void OnEnable()
	{
		Setup();
		UIEvent.OnUIEvent += OnUiEvent;
		if (GameManager.Instance != null && (OfflineManager.IsLoadDataManager ? GameManager.Instance.guildModel : GameManager.Instance.playerModel.GuildModel) != null)
		{
			(OfflineManager.IsLoadDataManager ? GameManager.Instance.guildModel : GameManager.Instance.playerModel.GuildModel).Changed += OnGuildChanged;
		}
		UIToggleContentRef = GetComponent<UIToggleContent>();
		if (warButtons != null)
		{
			warButtons.SetChangeCallback(OnNewWarSelected);
		}
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
		if (GameManager.Instance != null && (OfflineManager.IsLoadDataManager ? GameManager.Instance.guildModel : GameManager.Instance.playerModel.GuildModel) != null)
		{
			(OfflineManager.IsLoadDataManager ? GameManager.Instance.guildModel : GameManager.Instance.playerModel.GuildModel).Changed -= OnGuildChanged;
		}
		UIToggleContentRef = null;
	}

	private void OnGuildChanged(GroupModelBase model, string changed, object args)
	{
		Setup();
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "SocialGuildPlayerChanged")
		{
			Setup();
		}
	}

	private void Setup()
	{
		if (!(GameManager.Instance == null))
		{
			if (!GameManager.Instance.playerModel.IsGuildMember)
			{
				if (OfflineManager.IsLoadDataManager)
				{
					SetupMemberView();
					return;
				}
				SetupNotMemberView();
			}
			else
			{
				SetupMemberView();
			}
		}
	}

	private void SetupMemberView()
	{
		Helpers.GameObjectSetActive(notMemberContainer, value: false);
		Helpers.GameObjectSetActive(memberContainer, value: true);
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (guildModel != null)
		{
			HelpersUI.SetContentToLabel(tierNumberLabel, guildModel.GuildBattleTier.ToString(), setActive: false);
			HelpersUI.SetContentToLabel(battlesWonLabel, guildModel.CurrentSeasonVictories.ToString());
			HelpersUI.SetContentToLabel(battleLostLabel, guildModel.CurrentSeasonDefeats.ToString());
		}
		SetupWarButtons();
		if (getGasButton != null)
		{
			getGasButton.isEnabled = !GuildWarHelper.IsLockedByCouncilLevel();
		}
	}

	private void SetupWarButtons()
	{
		List<GuildWarDefinition> list = GameManager.Instance.playerModel.gameEconomyData.FindGuildWarDefinitionInSeason(GuildWarHelper.GetGvGSeasonModel().SeasonDefinitionId);
		long utcTimeStamp = GameManager.Instance.playerModel.UtcTimeStamp;
		int num = list?.Count ?? 0;
		selectedIndex = -1;
		for (int i = 0; i < Mathf.Min(num, 4); i++)
		{
			if (list[i].IsOpen(utcTimeStamp))
			{
				warButtons.GetUIButtonToggleList[i].isEnabled = true;
				HelpersUI.SetContentToLabel(warButtonsLabels[i], SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.Guild.WarOngoing"));
				selectedIndex = i;
			}
			else if (list[i].EndTimeMilliseconds < utcTimeStamp)
			{
				warButtons.GetUIButtonToggleList[i].isEnabled = true;
				HelpersUI.SetContentToLabel(warButtonsLabels[i], SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.Guild.WarEnded"));
				selectedIndex = i;
			}
			else
			{
				warButtons.GetUIButtonToggleList[i].isEnabled = false;
				long milliSeconds = list[i].StartTimeMilliseconds - utcTimeStamp;
				HelpersUI.SetContentToLabel(warButtonsLabels[i], SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.Guild.WarStartingIn{Parameter}", Helpers.FormatTime(milliSeconds)));
			}
		}
		if (num < warButtons.GetUIButtonToggleList.Length)
		{
			for (int j = num; j < warButtons.GetUIButtonToggleList.Length; j++)
			{
				Helpers.GameObjectSetActive(warButtonsLabels[j], value: false);
				warButtons.GetUIButtonToggleList[j].isEnabled = false;
			}
		}
		if (selectedIndex >= 0)
		{
			warButtons.SetSelectedIndex(selectedIndex);
		}
	}

	private void FillBattleLogForSelectedWarIndex()
	{
		if (battleListScroll != null && selectedIndex != -1)
		{
			List<GvGSeasonModel.GuildBattleLogEntry> list = new List<GvGSeasonModel.GuildBattleLogEntry>();
			GuildWarDefinition guildWarDefinition = GameManager.Instance.gameEconomyData.FindGuildWarDefinitionInSeason(GuildWarHelper.GetGvGSeasonModel().SeasonDefinitionId)[selectedIndex];
			List<GvGSeasonModel.GuildBattleLogEntry> battleLogForWar = GuildWarHelper.GetBattleLogForWar(guildWarDefinition.Identifier);
			if (battleLogForWar != null)
			{
				list.AddRange(battleLogForWar);
			}
			GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
			long utcTimeStamp = GameManager.Instance.playerModel.UtcTimeStamp;
			bool flag = currentBattle.IsOngoing(utcTimeStamp) && currentBattle.WarId == guildWarDefinition.Identifier;
			if ((GuildWarHelper.IsWarOngoing() && GuildWarHelper.GetCurrentWarDefinitionId() == guildWarDefinition.Identifier) || flag)
			{
				GvGSeasonModel.GuildBattleLogEntry guildBattleLogEntry = new GvGSeasonModel.GuildBattleLogEntry();
				guildBattleLogEntry.Result = 0;
				list.Add(guildBattleLogEntry);
			}
			if (list.Count > 0)
			{
				list.Reverse();
				Helpers.GameObjectSetActive(emptyLogInfoLabel, value: false);
				UpdateListWithData(list, resetScrollPosition: false);
			}
			else
			{
				battleListScroll.Clear();
				Helpers.GameObjectSetActive(emptyLogInfoLabel, value: true);
			}
		}
	}

	private void UpdateListWithData<T>(List<T> data, bool resetScrollPosition) where T : class
	{
		if (battleListScroll != null)
		{
			Vector2 scrollPosition = battleListScroll.GetScrollPosition();
			battleListScroll.Clear();
			NUIListItem<T> nUIListItem = null;
			for (int i = 0; i < data.Count; i++)
			{
				if (data[i] != null)
				{
					nUIListItem = ((!(data[i] as GvGSeasonModel.GuildBattleLogEntry).Ended) ? (battleListScroll.InstantiateAdd(callForBattlePrefab) as NUIListItem<T>) : (battleListScroll.InstantiateAdd(battleEntryPrefab) as NUIListItem<T>));
					if (nUIListItem != null)
					{
						nUIListItem.SetData(data[i]);
						nUIListItem.UpdateUI();
					}
				}
			}
			if (resetScrollPosition)
			{
				battleListScroll.SortAndReset();
			}
			else
			{
				battleListScroll.SortAndRepositionItems();
				battleListScroll.SetScrollPosition(scrollPosition);
			}
			nUIListItem = null;
		}
		else
		{
			Debug.LogError("Guild Popup-War: No Prefab Reference to a NUIScrollableList defined!");
		}
	}

	private void SetupNotMemberView()
	{
		Helpers.GameObjectSetActive(notMemberContainer, value: true);
		Helpers.GameObjectSetActive(memberContainer, value: false);
	}

	private void OnNewWarSelected(UIButtonExtended toggle)
	{
		selectedIndex = warButtons.GetSelectedIndex();
		if (warButtons.GetUIButtonToggleList[selectedIndex].isEnabled)
		{
			FillBattleLogForSelectedWarIndex();
		}
	}
}
