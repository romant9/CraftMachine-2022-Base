using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;
using TwdCustomMod;

public class MissionHubPopup : HUDElement
{
	[SerializeField]
	private MissionHubListBase TopContentParent;

	[SerializeField]
	private MissionHubListBase BottomContentParent;

	[SerializeField]
	private GameObject[] AllPanels;

	private List<MissionHubContent> _missionHubContents;

	public static MissionHubPopup OpenPopup()
	{
		MissionHubPopup missionHubPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MissionHubPopup, GetParent(HUDManager.Instance.UIContainerTopCameras)) as MissionHubPopup;
		if (missionHubPopup != null)
		{
			missionHubPopup.Open();
		}
		return missionHubPopup;
	}

	[ContextMenu("Open")]
	public override void Open()
	{
		base.Open();
		if (GameManager.Instance.gameEconomyData.GetFeature("UpdateSeasonTrials").Enabled && GameManager.Instance.playerModel.MapContainerModel.DoesSeasonTrialsNeedUpdate())
		{
			Helpers.ExecuteCommand(new UpdateSeasonTrialsCommand());
			UIEvent.Send("OnSeasonTrialsUpdated");
		}
		Helpers.GameObjectSetActive(TopContentParent, value: false);
		Helpers.GameObjectSetActive(BottomContentParent, value: false);
		InitAllPanels();
		ShowOutpostUnlockedSuggestion();
		StartCoroutine(CenterScrollableMapToNormalizedMapPosition());

		if (OfflineManager.IsLoadDataManager && OfflineManager.IsFixModelShaders)
		{
			var fxEffects = this.GetComponentsInChildren<UISprite>().Where(x=>x.name == "Fx");
			if (fxEffects != null)
			{
				foreach (var fx in fxEffects)
				{
					fx.gameObject.SetActive(false);
				}
			}
		}
	}

	private void ShowOutpostUnlockedSuggestion()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		bool show = playerModel.OutpostTutorialState != OutpostTutorialState.None && playerModel.OutpostTutorialState != OutpostTutorialState.Done;
		if (CampView.Instance != null && CampView.Instance.Model != null && TutorialView.Instance != null)
		{
			TutorialView.Instance.ShowButtonSuggest("OutpostButton", show);
		}
	}

	[ContextMenu("Close")]
	public override void OnClickClose()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			DebugTWD.Log("OnClickClose MissionHubPopup", DebugType.Wars);
			if (ResidencePopup.Instance != null)
			{
				ResidencePopup.Instance.gameObject.SetActive(true);
			}
			HelpersModel.IsUnlockPVP = false;
		}

		base.OnClickClose();
	}

	public override void Close()
	{
		Clear();
		base.Close();
	}

	public void Clear()
	{
		if (TopContentParent != null)
		{
			TopContentParent.Clear();
		}
		if (BottomContentParent != null)
		{
			BottomContentParent.Clear();
		}
	}

	private void OnEnable()
	{
		if (IsLoadDataManager)
		{
			if (BattleOngoingToggle) BattleOngoingToggle.value = StartGWBattle.Instance.OverrideHours > 0;
			if (AllPvpEnabledToggle) AllPvpEnabledToggle.value = HelpersModel.IsUnlockAllSectors;
		}
		UIEvent.OnUIEvent += OnUIEvent;
		EventManager.OnEvent += OnEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		EventManager.OnEvent -= OnEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
	}

	private void OnEvent(EventManager.EventType eventtype, object parameter)
	{
		if (eventtype == EventManager.EventType.TutorialPartOver)
		{
			if (TutorialView.Instance != null && TutorialView.Instance.RunningButNotSuggesting)
			{
				BottomContentParent.SetScrollViewEnabled(enabled: false);
			}
			else
			{
				BottomContentParent.SetScrollViewEnabled(enabled: true);
			}
		}
	}

	private void InitAllPanels()
	{
		List<MissionHubContent> list = (_missionHubContents = GetSortedContentList());
		if (list != null)
		{
			Vector3 cellSizeTop = Vector3.zero;
			Vector3 vector = Vector3.zero;
			MissionHubPanelBase missionHubPanelBase = null;
			if (!(TopContentParent != null) || !(BottomContentParent != null))
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				GameObject gameObject = null;
				if (list[i] == null)
				{
					continue;
				}
				gameObject = FindPanelWithName(list[i].PrefabName);
				if (list[i].Placement == MissionHubContent.ListPlacement.Top && !OverrideTopPanel(list[i].Id, ref cellSizeTop))
				{
					missionHubPanelBase = TopContentParent.InstantiatePanel(gameObject, list[i], this);
					if (missionHubPanelBase != null)
					{
						cellSizeTop = missionHubPanelBase.GetLocalSize();
					}
				}
				else if (list[i].Placement == MissionHubContent.ListPlacement.Bottom)
				{
					missionHubPanelBase = BottomContentParent.InstantiatePanel(gameObject, list[i], this);
					if (missionHubPanelBase != null)
					{
						vector = missionHubPanelBase.GetLocalSize();
					}
				}
			}
			BottomContentParent.SetGridSize(vector.x, vector.y);
			BottomContentParent.UpdateUI();
			BottomContentParent.RepositionNow();
			TopContentParent.SetGridSize(cellSizeTop.x, cellSizeTop.y);
			TopContentParent.RepositionNow();
			TopContentParent.UpdateUI();
			Helpers.GameObjectSetActive(TopContentParent, value: true);
			Helpers.GameObjectSetActive(BottomContentParent, value: true);
			if (TutorialView.Instance != null && TutorialView.Instance.RunningButNotSuggesting)
			{
				BottomContentParent.SetScrollViewEnabled(enabled: false);
			}
			else
			{
				BottomContentParent.SetScrollViewEnabled(enabled: true);
			}
			CheckForTutorialSuggestions();
		}
		else
		{
			DebugLogError("Can not InitAllPanels, MissioHubContent was NULL!");
		}
	}

	private bool OverrideTopPanel(int id, ref Vector3 cellSizeTop)
	{
		if (!GameManager.Instance.gameEconomyData.GetFeature("MissionHubBackgroundOverride").Enabled)
		{
			return false;
		}
		MissionHubContent missionHubContent = CheckForDynamicMissionHubContent(id);
		if (missionHubContent != null)
		{
			GameObject obj = FindPanelWithName(missionHubContent.PrefabName);
			MissionHubPanelBase missionHubPanelBase = TopContentParent.InstantiatePanel(obj, missionHubContent, this);
			if (missionHubPanelBase != null)
			{
				cellSizeTop = missionHubPanelBase.GetLocalSize();
			}
			return true;
		}
		return false;
	}

	private MissionHubContent CheckForDynamicMissionHubContent(int id)
	{
		if (GuildWarHelper.ShowWarIsOnOnMissionHub())
		{
			return new MissionHubContent
			{
				Id = id,
				Placement = MissionHubContent.ListPlacement.Top,
				PrefabName = GameManager.Instance.gameEconomyData.GuildWarConfig.GvGMissionHubTopContentPrefab
			};
		}
		return null;
	}

	private void CheckForTutorialSuggestions()
	{
		_ = GameManager.Instance.playerModel.OutpostTutorialState;
		_ = 1;
	}

	private IEnumerator ScrollToTheEnd()
	{
		yield return null;
		HelpersUI.ScrollToTheEndHorizontal(BottomContentParent.gameObject.GetComponent<UIPanel>());
	}

	private GameObject FindPanelWithName(string prefabName)
	{
		for (int i = 0; i < AllPanels.Length; i++)
		{
			if (AllPanels[i].name == prefabName)
			{
				return AllPanels[i];
			}
		}
		return null;
	}

	private List<MissionHubContent> GetSortedContentList()
	{
		List<MissionHubContent> list = new List<MissionHubContent>();
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		if (Helpers.IsSeasonMapAllCompleted())
		{
			list = GetSortedContentList_MissionEnterOrder(gameEconomyData.ConfigData.MissionEnterOrderEndSeason);
			AppendWorldBossMissionHubContent(list);
			return list;
		}
		if (Helpers.IsStoryMapAllCompleted())
		{
			list = GetSortedContentList_MissionEnterOrder(gameEconomyData.ConfigData.MissionEnterOrderEndStory);
			AppendWorldBossMissionHubContent(list);
			return list;
		}
		MissionEnterOrder missionEnterOrder = GameManager.Instance.gameEconomyData.MissionEnterOrder.FirstOrDefault((MissionEnterOrder t) => GameManager.Instance.playerModel.CouncilLevel >= t.minimal() && GameManager.Instance.playerModel.CouncilLevel <= t.max());
		if (missionEnterOrder == null)
		{
			return null;
		}
		list = GetSortedContentList_MissionEnterOrder(missionEnterOrder.SortInt);
		AppendWorldBossMissionHubContent(list);
		return list;
	}

	private void AppendWorldBossMissionHubContent(List<MissionHubContent> contentList)
	{
		if (contentList == null || !ShouldShowWorldBossInMissionHub())
		{
			return;
		}
		for (int i = 0; i < contentList.Count; i++)
		{
			if (contentList[i] != null && contentList[i].PrefabName == "Mission_Hub_Small_WorldBoss")
			{
				return;
			}
		}
		MissionHubContent item = new MissionHubContent
		{
			Id = 9,
			PrefabName = "Mission_Hub_Small_WorldBoss",
			Placement = MissionHubContent.ListPlacement.Bottom,
			SortInt = 3
		};
		int num = contentList.FindIndex((MissionHubContent c) => c != null && c.Id == 7);
		if (num >= 0)
		{
			contentList.Insert(num + 1, item);
		}
		else
		{
			contentList.Add(item);
		}
	}

	private static bool ShouldShowWorldBossInMissionHub()
	{
		PlayerModel playerModel = GameManager.Instance?.playerModel;
		GameEconomyData gameEconomyData = GameManager.Instance?.gameEconomyData;
		if (playerModel == null || gameEconomyData == null)
		{
			return false;
		}
		ConfigData configData = gameEconomyData.ConfigData;
		if (configData == null || !configData.WorldBoss)
		{
			return false;
		}
		SystemOpen systemOpenById = gameEconomyData.GetSystemOpenById("SystemBase.WorldBoss");
		if (systemOpenById != null && playerModel.CouncilLevel < systemOpenById.ShowCampLv)
		{
			return false;
		}
		return true;
	}

	private List<MissionHubContent> GetSortedContentList_MissionEnterOrder(List<int> sortInts)
	{
		_ = GameManager.Instance.playerModel;
		MissionHubContent[] missionHubContentList = GameManager.Instance.gameEconomyData.MissionHubContentList;
		if (missionHubContentList == null || missionHubContentList.Length == 0 || sortInts == null || sortInts.Count <= 0)
		{
			return null;
		}
		List<MissionHubContent> list = new List<MissionHubContent>();
		for (int i = 0; i < missionHubContentList.Length; i++)
		{
			if (missionHubContentList[i].Placement == MissionHubContent.ListPlacement.Top)
			{
				list.Add(missionHubContentList[i]);
			}
		}
		foreach (int sortInt in sortInts)
		{
			MissionHubContent missionHubContent = missionHubContentList.FirstOrDefault((MissionHubContent t) => t != null && t.Id == sortInt);
			if (missionHubContent != null && missionHubContent.Placement == MissionHubContent.ListPlacement.Bottom)
			{
				list.Add(missionHubContent);
			}
		}
		return list;
	}

	private IEnumerator CenterScrollableMapToNormalizedMapPosition()
	{
		float normalizedPos = GetNormalizedPos();
		if (normalizedPos <= 0f || !TutorialView.Instance.Running)
		{
			yield break;
		}
		yield return null;
		try
		{
			UIPanel component = BottomContentParent.GetComponent<UIPanel>();
			if (component == null)
			{
				yield break;
			}
			UIScrollView component2 = BottomContentParent.GetComponent<UIScrollView>();
			if (!(component2 == null))
			{
				float num = component2.customBoundsForRestrict.extents.x * 2f;
				float num2 = 0f;
				if (num > 0f)
				{
					num2 = component.width / num;
				}
				float value = 0.5f + (normalizedPos - 0.5f) * (1f + num2);
				value = UtilsMath.Clamp(value, 0f, 1f);
				component.ResetAndUpdateAnchors();
				component2.SetDragAmount(value, 0f, updateScrollbars: false);
				component2.RestrictWithinBounds(instant: true);
				component2.UpdateScrollbars();
			}
		}
		catch (Exception arg)
		{
			Debug.LogError($"CenterScrollableMapToNormalizedMapPosition Error:{arg}");
		}
	}

	private float GetNormalizedPos()
	{
		float result = 0f;
		try
		{
			TutorialModel tutorial = GameManager.Instance.playerModel.Tutorial;
			if (tutorial == null)
			{
				return result;
			}
			List<string> getCurrentActions = tutorial.GetCurrentActions;
			if (getCurrentActions == null)
			{
				return result;
			}
			foreach (string item in getCurrentActions)
			{
				string[] array = item.Split(',');
				if (array[0] == "WaitClickButton")
				{
					string text = array[1];
					result = ((!string.IsNullOrEmpty(text) && !text.Equals("Hub") && !text.Equals("MissionHub")) ? GetNormalizedPosBySelectHero(text) : 0f);
				}
			}
		}
		catch (Exception arg)
		{
			Debug.LogError($"GetNormalizedPos Error:{arg}");
		}
		return result;
	}

	private float GetNormalizedPosBySelectHero(string panelName)
	{
		if (_missionHubContents == null)
		{
			return 0f;
		}
		int count = _missionHubContents.Count;
		if (count <= 1)
		{
			return 0f;
		}
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			if (_missionHubContents[i].PrefabName.Contains(panelName))
			{
				num = i;
			}
		}
		return (float)num * 1f / (float)(count - 1);
	}


	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	public UIToggle BattleOngoingToggle;
	public UIToggle AllPvpEnabledToggle;
	public UILabel BattleTimeLabel;
	#endregion

	#region mycode
	public void SwitchBattleOngoing(UIToggle tg)
	{
		if (StartGWBattle.Instance == null) return;
		StartGWBattle.Instance.OverrideHours = tg.value ? 1 : 0;

		long overrideTime = GameManager.Instance.playerModel.UtcTimeStamp;

		StartGWBattle.Instance.OverrideTime = MyTools.LongToDate(overrideTime).ToLocalTime().ToString(UserPrefsKeys.TimeFormat);
		BattleTimeLabel.text = StartGWBattle.Instance.OverrideTime;
	}

	public void SwitchAllPvpEnabled(UIToggle tg)
	{
		if (StartGWBattle.Instance == null) return;
		HelpersModel.IsUnlockAllSectors = tg.value;
	}
	#endregion
}
