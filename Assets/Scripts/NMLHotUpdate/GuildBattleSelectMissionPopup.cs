using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class GuildBattleSelectMissionPopup : HUDElement
{
	[Header("Sector Info")]
	public UILabel SectorNameLabel;

	[Header("Mission Info")]
	public GuildBattleRewardBonus BonusReward;

	public GuildBattleRewardCurrencyBonus BonusCurrencyReward;

	public UIWidget VPBonusRewardContainer;

	public UILabel LabelSectorVpAmount;

	[Header("Enemy Info")]
	public NUIScrollableList EnemyList;

	[Header("Mission Prefab")]
	public GameObject MissionIconPrefab;

	[Header("Mission PVP Prefab")]
	public GameObject MissionPvpIconPrefab;

	[Header("Searching Prefab")]
	public GameObject SearchViewPrefab;

	[Header("Enemy Info Target")]
	public GuildBattleMapEnemyButton[] EnemyInfoButtonsPerArea;

	public UILabel EnemyAmount;

	[Header("Parent Target")]
	public GameObject[] MissionGroupParent;

	[Header("Spectator Mode")]
	public GameObject SpectatorModeContainer;

	public GridMapping[] ButtonGridsMapping;

	public GameObject RewardRoot;

	public GameObject SectorRewardRoot;

	private List<GuildBattlePvpTeam> teamsInSector = new List<GuildBattlePvpTeam>();

	private List<GuildBattleMissionQueueData> missionObjects;

	private bool visualisationsRunning;

	private bool runDelayedUpdateUI;

	[SerializeField]
	private float alphaDimming = 0.2f;

	private GuildBattleMissionButton.InitState parentPopupOpenState;

	public GuildBattleMapSectorModel SectorModel => GuildWarHelper.GetCurrentMapModel()?.GetSectorModel(SectorId);

	public int SectorId { get; set; }

	public MissionQueueUI[] ButtonQueue { get; set; }

	public Dictionary<string, GuildBattleMapMissionModel> CurrentAreaEnemyMissions { get; set; }

	public override void Open()
	{
		base.Open();
		GuildBattleMapPopup guildBattleMapPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GuildBattleMapPopup) as GuildBattleMapPopup;
		parentPopupOpenState = ((guildBattleMapPopup != null) ? guildBattleMapPopup.GetInitState() : GuildBattleMissionButton.InitState.None);
		if (parentPopupOpenState == GuildBattleMissionButton.InitState.FromMap)
		{
			SaveProgressSeen();
		}
		TweenManager.PlayTweenAnchors(base.gameObject);
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();

		if (HelpersModel.IsUnlockAllSectors)
		{
			if (SectorModel.AreaMissions == null)
			{
				SectorModel.AreaMissions = new List<GuildBattleMissionQueueData>[4];
				StartGWBattle.Instance.SaveCurrentGWBattle();
				DebugTWD.Log("Generate AreaMissions for Sector " + SectorId, DebugType.Wars);
			}
			else
			{
				DebugTWD.Log("Initiate AreaMissions", DebugType.Wars);
			}

		}
		UpdatePvPEnemiesFoundSector();
		if (ButtonQueue == null)
		{
			ButtonQueue = new MissionQueueUI[4];
			ButtonQueue[0] = MissionQueueUI.AddComponent(this, 0, SectorModel.AreaMissions[0]);
			ButtonQueue[1] = MissionQueueUI.AddComponent(this, 1, SectorModel.AreaMissions[1]);
			ButtonQueue[2] = MissionQueueUI.AddComponent(this, 2, SectorModel.AreaMissions[2]);
			ButtonQueue[3] = MissionQueueUI.AddComponent(this, 3, SectorModel.AreaMissions[3]);
		}
		if (SectorModel == null || MissionGroupParent == null || SectorModel.RandomizedMissions == null || SectorModel.RandomizedMissions.Count == 0)
		{
			return;
		}
		SubscribeForGuildModelEvents();
		GuildWarHelper.GetCurrentMapModel().PVPTeamsListPerSector.TryGetValue(SectorModel.SectorId, out teamsInSector);
		StartProgressVisualisation(parentPopupOpenState);
		if (EnemyList != null && teamsInSector != null)
		{
			UpdateAreaMissionLookup();
			EnemyList.UpdateWithList(teamsInSector, "GuildBattle_Enemy_List_Button", "BadgeCardEmpty", callUpdateUI: true);
			for (int i = 0; i < EnemyList.currentItemsList.Count; i++)
			{
				GuildBattleMapEnemyButton guildBattleMapEnemyButton = EnemyList.currentItemsList[i] as GuildBattleMapEnemyButton;
				if (!(guildBattleMapEnemyButton == null))
				{
					guildBattleMapEnemyButton.ParentPopup = this;
				}
			}
			EnemyList.SortAndReset();
		}
		bool flag = GuildWarHelper.IsBattleOnGoing();
		Helpers.GameObjectSetActive(RewardRoot, flag);
		Helpers.GameObjectSetActive(SectorRewardRoot, flag);
		Helpers.GameObjectSetActive(SpectatorModeContainer, !GuildWarHelper.IsPlayerRegisteredForBattle());
		if (flag)
		{
			int num = GameManager.Instance.playerModel.GuildWarModel.CurrentBattle.GetGuildSectorBattleVictoryPoints(SectorModel.SectorId);
			RewardGuildBattleVP bonusVPRewardFromSector = GuildWarHelper.GetCurrentBattle().GetBonusVPRewardFromSector(SectorModel.SectorId);
			if (bonusVPRewardFromSector != null)
			{
				num += bonusVPRewardFromSector.Amount;
			}
			HelpersUI.SetContentToLabel(LabelSectorVpAmount, num.ToString());
			BonusReward.Model = SectorModel;
			BonusReward.UpdateUI();
			if (BonusCurrencyReward != null)
			{
				BonusCurrencyReward.Model = SectorModel;
				BonusCurrencyReward.UpdateUI();
			}
			if (SectorModel.IsCompleted())
			{
				DimRewards();
			}
		}
		HelpersUI.SetContentToLabel(SectorNameLabel, HelpersLocalization.GetGuildBattleSectorName(SectorModel));
	}

	public void UpdateAreaMissionLookup()
	{
		if (CurrentAreaEnemyMissions == null)
		{
			CurrentAreaEnemyMissions = new Dictionary<string, GuildBattleMapMissionModel>();
		}
		else
		{
			CurrentAreaEnemyMissions.Clear();
		}
		for (int i = 0; i < ButtonQueue.Length; i++)
		{
			if (ButtonQueue[i].CurrentMissionQueue() == null)
			{
				continue;
			}
			GuildBattleMapMissionModel enemyMission = ButtonQueue[i].CurrentMissionQueue().EnemyMission;
			if (enemyMission != null)
			{
				if (!CurrentAreaEnemyMissions.ContainsKey(enemyMission.Id))
				{
					CurrentAreaEnemyMissions.Add(enemyMission.Id, enemyMission);
				}
				else
				{
					CurrentAreaEnemyMissions[enemyMission.Id] = enemyMission;
				}
			}
		}
	}

	private void SubscribeForGuildModelEvents()
	{
		if (SectorModel != null)
		{
			SectorModel.Changed -= OnSectorModelChange;
			SectorModel.Changed += OnSectorModelChange;
		}
		GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
		if (currentBattle != null && currentBattle.CurrentMapModel != null)
		{
			currentBattle.CurrentMapModel.Changed -= OnMapModelChange;
			currentBattle.CurrentMapModel.Changed += OnMapModelChange;
		}
	}

	public void OnEnable()
	{
		EventManager.OnEvent -= OnEvent;
		EventManager.OnEvent += OnEvent;
		UIEvent.OnUIEvent -= OnUIEvent;
		UIEvent.OnUIEvent += OnUIEvent;
	}

	public void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		if (SectorModel != null)
		{
			SectorModel.Changed -= OnSectorModelChange;
		}
		GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
		if (currentBattle != null && currentBattle.CurrentMapModel != null)
		{
			currentBattle.CurrentMapModel.Changed -= OnMapModelChange;
		}
		EventManager.OnEvent -= OnEvent;

		if (OfflineManager.IsLoadDataManager)
		{
			StartGWBattle.Instance.RestoreGWBattle();
		}
	}

	public void OnUIEvent(string type, object parameter)
	{
		if (type == "OnGuildBattleEnemyUnlocked" || type == "OnGuildBattleEnemyCompleted")
		{
			UpdatePvPEnemiesFoundSector();
		}
	}

	private void OnEvent(EventManager.EventType eventType, object parameter)
	{
		if (eventType != EventManager.EventType.GroupModelLoaded)
		{
			return;
		}
		if (ButtonQueue != null)
		{
			for (int i = 0; i < ButtonQueue.Length; i++)
			{
				ButtonQueue[i].UpdateDataReference(SectorModel.AreaMissions[i]);
			}
			ButtonQueue = null;
		}
		UpdateUI();
	}

	public void StartProgressVisualisation(GuildBattleMissionButton.InitState initState = GuildBattleMissionButton.InitState.None)
	{
		if (ButtonQueue == null)
		{
			return;
		}
		if (visualisationsRunning)
		{
			Debug.LogWarning("Block visualisation, visualisationsRunning is still set");
			return;
		}
		visualisationsRunning = true;
		for (int i = 0; i < ButtonQueue.Length; i++)
		{
			ButtonQueue[i].complete = false;
		}
		for (int j = 0; j < ButtonQueue.Length; j++)
		{
			ButtonQueue[j].StartProgressVisualisation(VisualisationsComplete, initState);
		}
	}

	public override void Close()
	{
		base.Close();
		SaveProgressSeen();
		GuildBattleMapPopup guildBattleMapPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GuildBattleMapPopup) as GuildBattleMapPopup;
		if (guildBattleMapPopup != null)
		{
			guildBattleMapPopup.ClearToMap();
		}
		TweenManager.PlayTweenAnchors(base.gameObject, forward: false);
		UIEvent.Send("UIEventZoomOut");
		Clear();
	}

	public void Clear()
	{
		GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
		if (currentBattle != null && currentBattle.CurrentMapModel != null)
		{
			currentBattle.CurrentMapModel.Changed -= OnMapModelChange;
		}
		SectorModel.Changed -= OnSectorModelChange;
		visualisationsRunning = false;
		runDelayedUpdateUI = false;
		SectorId = -1;
		if (EnemyList != null)
		{
			EnemyList.Clear();
		}
		for (int i = 0; i < ((ButtonQueue != null) ? ButtonQueue.Length : 0); i++)
		{
			if (!(ButtonQueue[i] == null))
			{
				ButtonQueue[i].Clear();
				ButtonQueue[i] = null;
			}
		}
	}

	private void VisualisationsComplete()
	{
		bool flag = true;
		for (int i = 0; i < ButtonQueue.Length; i++)
		{
			flag = ButtonQueue[i].complete && flag;
			if (!flag)
			{
				break;
			}
		}
		if (flag)
		{
			SaveProgressSeen();
			if (EnemyList != null)
			{
				UpdateAreaMissionLookup();
				EnemyList.SortAndReset();
			}
			visualisationsRunning = false;
			if (runDelayedUpdateUI)
			{
				runDelayedUpdateUI = false;
				UpdateUI();
			}
		}
	}

	private void UpdatePvPEnemiesFoundSector()
	{
		int totalDefeatedCount = 0;
		int total = SectorModel.EnemiesDefeatedCount(out totalDefeatedCount);
		UpdateEnemiesFoundLabel(totalDefeatedCount, total);
	}

	private void UpdateEnemiesFoundLabel(int defeated, int total)
	{
		string content = $"{defeated}/{total}";
		HelpersUI.SetContentToLabel(EnemyAmount, content);
	}

	private void SaveProgressSeen()
	{
		GuildWarHelper.SaveSectorProgressionSeen(SectorModel);
		parentPopupOpenState = GuildBattleMissionButton.InitState.None;
	}

	private void DimRewards()
	{
		if (!(VPBonusRewardContainer == null) && !(BonusReward == null) && !(BonusCurrencyReward == null))
		{
			VPBonusRewardContainer.alpha = alphaDimming;
			BonusReward.transform.GetComponent<UIWidget>().alpha = alphaDimming;
			BonusCurrencyReward.GetComponent<UIWidget>().alpha = alphaDimming;
		}
	}

	private void OnSectorModelChange(TWDGroupModelChild model, string changed, object args)
	{
		if (changed == "GuildBattleAddCompletionToArea" && model is GuildBattleMapSectorModel guildBattleMapSectorModel && guildBattleMapSectorModel.SectorId == SectorModel.SectorId)
		{
			CallUpdateUIAfterModelChange();
		}
	}

	private void OnMapModelChange(TWDGroupModelChild model, string changed, object args)
	{
		if ((changed == "GuildBattleNonPvpCompletionAdded" || changed == "GuildBattlePvpCompletionAdded") && args is GuildBattleMapMissionModel guildBattleMapMissionModel && guildBattleMapMissionModel.SectorIdOwner == SectorModel.SectorId)
		{
			CallUpdateUIAfterModelChange();
		}
	}

	private void CallUpdateUIAfterModelChange()
	{
		if (visualisationsRunning)
		{
			runDelayedUpdateUI = true;
			return;
		}
		runDelayedUpdateUI = false;
		UpdateUI();
	}
}
