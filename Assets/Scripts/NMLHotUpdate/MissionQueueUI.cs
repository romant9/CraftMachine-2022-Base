using System.Collections.Generic;
using Client.Tweener;
using TWDModel;
using UnityEngine;

public class MissionQueueUI : MonoBehaviour
{
	[HideInInspector]
	public bool complete;

	private int dataIndex = -1;

	private int areaIndex = -1;

	private TweenTimeline queueTimeline;

	private List<GuildBattleMissionQueueData> queueDataList;

	private List<GuildBattleMissionButton> buttonPool;

	private GuildBattleMissionButton buttonPvp;

	private GuildBattleMissionSearchingView searchingView;

	private GuildBattleSelectMissionPopup popupRef;

	private float longestSide;

	private Callback completeCallback;

	public GuildBattleMapEnemyButton EnemyTopInfo
	{
		get
		{
			if (popupRef.EnemyInfoButtonsPerArea == null || popupRef.EnemyInfoButtonsPerArea.Length <= areaIndex)
			{
				return null;
			}
			return popupRef.EnemyInfoButtonsPerArea[areaIndex];
		}
	}

	public GameObject PveIconPrefab => popupRef.MissionIconPrefab;

	public GameObject PvpIconPrefab => popupRef.MissionPvpIconPrefab;

	public GameObject SearchingViewPrefab => popupRef.SearchViewPrefab;

	public GridMapping[] GridMapping => popupRef.ButtonGridsMapping;

	public static MissionQueueUI AddComponent(GuildBattleSelectMissionPopup popupRef, int areaIndex, List<GuildBattleMissionQueueData> data)
	{
		if (popupRef == null || data == null)
		{
			Debug.LogError("Given params contain NULL!");
			return null;
		}
		if (popupRef.MissionGroupParent.Length <= areaIndex)
		{
			Debug.LogErrorFormat("Component: {0}, Reference: {1} does not contain parent reference at Index: {2}, GameObject: {3}", "GuildBattleSelectMissionPopup", "MissionGroupParent", areaIndex.ToString(), popupRef.gameObject.name);
			return null;
		}
		MissionQueueUI missionQueueUI = Helpers.AddComponent<MissionQueueUI>(popupRef.MissionGroupParent[areaIndex]);
		missionQueueUI.ComponentAdded(popupRef, areaIndex, data);
		return missionQueueUI;
	}

	public void ComponentAdded(GuildBattleSelectMissionPopup popupRef, int areaIndex, List<GuildBattleMissionQueueData> data)
	{
		Clear();
		queueTimeline = new TweenTimeline();
		queueDataList = data;
		this.popupRef = popupRef;
		this.areaIndex = areaIndex;
	}

	public void UpdateDataReference(List<GuildBattleMissionQueueData> data)
	{
		queueDataList = data;
	}

	public GuildBattleMissionQueueData CurrentMissionQueue()
	{
		if (dataIndex >= queueDataList.Count)
		{
			return null;
		}
		return queueDataList[dataIndex];
	}

	public bool TryGetGridPositionFor(int missionCount, int index, out Vector2 position)
	{
		if (GridMapping == null || GridMapping.Length <= missionCount)
		{
			position = default(Vector2);
			return false;
		}
		if (GridMapping[missionCount].grid.Length <= index)
		{
			position = default(Vector2);
			return false;
		}
		position = GridMapping[missionCount].grid[index];
		return true;
	}

	[ContextMenu("StartProgressVisualisation")]
	public void StartProgressVisualisation(Callback completeCallback = null, GuildBattleMissionButton.InitState overrideState = GuildBattleMissionButton.InitState.None)
	{
		HideEnemyInfo(instant: true);
		dataIndex = -1;
		complete = false;
		this.completeCallback = completeCallback;
		VisualisePVEMissions(overrideState);
	}

	private void VisualisePVEMissions(GuildBattleMissionButton.InitState overrideState = GuildBattleMissionButton.InitState.None)
	{
		if (queueTimeline != null)
		{
			queueTimeline.Clear();
		}
		dataIndex++;
		GuildBattleMissionQueueData guildBattleMissionQueueData = CurrentMissionQueue();
		if (guildBattleMissionQueueData == null)
		{
			dataIndex--;
			AllVisualisationDone();
			return;
		}
		GuildBattleProgressSnapshot currentCompletionSnapshot = GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.CurrentCompletionSnapshot;
		if (guildBattleMissionQueueData.IsCompleteAndSeen(currentCompletionSnapshot) && !guildBattleMissionQueueData.Last)
		{
			VisualisePVEMissions(overrideState);
			return;
		}
		InstantiateButtons(guildBattleMissionQueueData.Count - 1);
		if (guildBattleMissionQueueData.IsCompleteAndSeen(currentCompletionSnapshot) && guildBattleMissionQueueData.Last)
		{
			VisualiseSearchForNewEnemy();
			return;
		}
		Vector3 localPosition = default(Vector3);
		Vector2 position = Helpers.staticVector3One;
		for (int i = 0; i < guildBattleMissionQueueData.Count; i++)
		{
			GuildBattleMapMissionModel guildBattleMapMissionModel = guildBattleMissionQueueData[i];
			if (!TryGetGridPositionFor(guildBattleMissionQueueData.Count, i, out position))
			{
				continue;
			}
			if (guildBattleMapMissionModel.Type == GuildBattleMapMissionModel.MissionType.PVP)
			{
				localPosition.x = position.x * longestSide;
				localPosition.y = position.y * longestSide;
				localPosition.z = 0f;
				if (buttonPvp != null)
				{
					buttonPvp.transform.localPosition = localPosition;
				}
				continue;
			}
			guildBattleMapMissionModel.MissionPositionWithinArea = i;
			localPosition.x = position.x * longestSide;
			localPosition.y = position.y * longestSide;
			localPosition.z = 0f;
			buttonPool[i].Index = i + 1;
			buttonPool[i].transform.localPosition = localPosition;
			buttonPool[i].Model = guildBattleMapMissionModel;
			buttonPool[i].EnemyInQueueUnlocked = guildBattleMissionQueueData.PvPEnemyUnlocked;
			buttonPool[i].PvpButton = false;
			if (overrideState != GuildBattleMissionButton.InitState.None)
			{
				buttonPool[i].initState = overrideState;
				buttonPvp.initState = overrideState;
				searchingView.initState = overrideState;
			}
			if (!buttonPool[i].ResetToSavedState(guildBattleMissionQueueData, currentCompletionSnapshot))
			{
				buttonPool[i].initState = GuildBattleMissionButton.InitState.IsOpen;
				Helpers.GameObjectSetActive(buttonPool[i], value: false);
				continue;
			}
			buttonPool[i].UpdateUI();
			buttonPool[i].ClearTimeline();
			buttonPool[i].QueueStartDelay();
			buttonPool[i].QueueOpenTween();
			buttonPool[i].QueueCompleteTween(currentCompletionSnapshot);
			buttonPool[i].QueueEnemyFoundTween(guildBattleMissionQueueData, currentCompletionSnapshot);
			buttonPool[i].initState = GuildBattleMissionButton.InitState.IsOpen;
			queueTimeline.Add(buttonPool[i].ButtonTimeline);
			Helpers.GameObjectSetActive(buttonPool[i], value: true);
		}
		queueTimeline.OnComplete(VisualisePVPMission);
		queueTimeline.Play();
	}

	private void VisualisePVPMission()
	{
		GuildBattleMissionQueueData guildBattleMissionQueueData = CurrentMissionQueue();
		GuildBattleProgressSnapshot currentCompletionSnapshot = GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.CurrentCompletionSnapshot;
		if (!guildBattleMissionQueueData.Last && guildBattleMissionQueueData.IsCompleteAndSeen(currentCompletionSnapshot))
		{
			VisualisePVEMissions();
			return;
		}
		buttonPvp.PvpButton = true;
		buttonPvp.Model = guildBattleMissionQueueData.EnemyMission;
		Helpers.GameObjectSetActive(buttonPvp, value: true);
		if (guildBattleMissionQueueData.PvPEnemyUnlocked)
		{
			SendPvpUnlockedEvent();
			queueTimeline.Clear();
			buttonPvp.EnemyInQueueUnlocked = true;
			buttonPvp.ResetToSavedState(guildBattleMissionQueueData, currentCompletionSnapshot);
			buttonPvp.UpdateUI();
			buttonPvp.ClearTimeline();
			buttonPvp.QueueOpenTween();
			buttonPvp.QueueEnemyFoundTween(guildBattleMissionQueueData, currentCompletionSnapshot);
			buttonPvp.QueueCompleteTween(currentCompletionSnapshot);
			buttonPvp.initState = GuildBattleMissionButton.InitState.IsOpen;
			queueTimeline.Add(buttonPvp.ButtonTimeline);
			if (guildBattleMissionQueueData.IsComplete)
			{
				queueTimeline.OnComplete(VisualiseHideAll);
			}
			else
			{
				queueTimeline.OnComplete(AllVisualisationDone);
			}
			queueTimeline.Play();
		}
		else
		{
			buttonPvp.UpdateUI();
			AllVisualisationDone();
		}
	}

	private void VisualiseHideAll()
	{
		SendPvpCompleteEvent();
		queueTimeline.Clear();
		Vector2 position = Helpers.staticVector3One;
		GuildBattleMissionQueueData guildBattleMissionQueueData = CurrentMissionQueue();
		for (int i = 0; i < buttonPool.Count; i++)
		{
			if (TryGetGridPositionFor(guildBattleMissionQueueData.Count, i, out position))
			{
				Helpers.GameObjectSetActive(buttonPool[i], value: true);
				buttonPool[i].ClearTimeline();
				buttonPool[i].QueueButtonClose();
				queueTimeline.Add(buttonPool[i].ButtonTimeline);
			}
		}
		Helpers.GameObjectSetActive(buttonPvp, value: true);
		buttonPvp.ClearTimeline();
		buttonPvp.QueueButtonClose();
		queueTimeline.Add(buttonPvp.ButtonTimeline);
		queueTimeline.OnComplete(VisualiseSearchForNewEnemy);
		queueTimeline.Play();
	}

	private void VisualiseSearchForNewEnemy()
	{
		if (Helpers.GameObjectSetActive(searchingView, value: true))
		{
			GuildBattleMissionQueueData queueData = CurrentMissionQueue();
			searchingView.UpdateUI(queueData);
			searchingView.ClearTimeline();
			searchingView.QueueOpenTween();
			searchingView.QueueCloseTween(queueData);
			searchingView.initState = GuildBattleMissionButton.InitState.IsOpen;
			queueTimeline.Clear();
			queueTimeline.Add(searchingView.ButtonTimeline);
			queueTimeline.OnComplete(PvPButtonTweensComplete);
			queueTimeline.Play();
		}
		else
		{
			PvPButtonTweensComplete();
		}
	}

	private void SendPvpUnlockedEvent()
	{
		GuildBattleMissionQueueData guildBattleMissionQueueData = CurrentMissionQueue();
		GuildBattleProgressSnapshot currentCompletionSnapshot = GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.CurrentCompletionSnapshot;
		if (guildBattleMissionQueueData != null && !currentCompletionSnapshot.IsMissionEnemySeen(guildBattleMissionQueueData.EnemyMission))
		{
			GuildBattlePvpTeam pvpTeamForMission = GameManager.Instance.playerModel.GuildWarModel.CurrentBattle.CurrentMapModel.GetPvpTeamForMission(guildBattleMissionQueueData.EnemyMission.Id);
			UIEvent.Send("OnGuildBattleEnemyUnlocked", pvpTeamForMission);
		}
	}

	private void SendPvpCompleteEvent()
	{
		GuildBattleMissionQueueData guildBattleMissionQueueData = CurrentMissionQueue();
		GuildBattleProgressSnapshot currentCompletionSnapshot = GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.CurrentCompletionSnapshot;
		if (guildBattleMissionQueueData != null && guildBattleMissionQueueData.EnemyMission.IsPvpComplete() && !currentCompletionSnapshot.IsMissionCompletionSeen(guildBattleMissionQueueData.EnemyMission))
		{
			GuildBattlePvpTeam pvpTeamForMission = GameManager.Instance.playerModel.GuildWarModel.CurrentBattle.CurrentMapModel.GetPvpTeamForMission(guildBattleMissionQueueData.EnemyMission.Id);
			UIEvent.Send("OnGuildBattleEnemyCompleted", pvpTeamForMission);
		}
	}

	private void PvPButtonTweensComplete()
	{
		VisualisePVEMissions(GuildBattleMissionButton.InitState.NewQueue);
	}

	private void AllVisualisationDone()
	{
		ShowEnemyInfo();
		SendPvpCompleteEvent();
		complete = true;
		if (completeCallback != null)
		{
			completeCallback();
		}
	}

	public void Clear()
	{
		for (int i = 0; i < ((buttonPool != null) ? buttonPool.Count : 0); i++)
		{
			if (buttonPool[i] != null)
			{
				buttonPool[i].Clear();
				Helpers.DestroyOrCache(buttonPool[i]);
			}
		}
		buttonPool = null;
		if (buttonPvp != null)
		{
			buttonPvp.Clear();
			Helpers.DestroyOrCache(buttonPvp);
			buttonPvp = null;
		}
		if (searchingView != null)
		{
			Helpers.DestroyOrCache(searchingView);
			searchingView = null;
		}
		areaIndex = -1;
		longestSide = 0f;
	}

	private void HideEnemyInfo(bool instant = false)
	{
		GuildBattleMapEnemyButton enemyTopInfo = EnemyTopInfo;
		if (!(enemyTopInfo == null))
		{
			if (instant)
			{
				Helpers.GameObjectSetActive(enemyTopInfo, value: false);
			}
			else
			{
				enemyTopInfo.Hide();
			}
		}
	}

	private void ShowEnemyInfo()
	{
		GuildBattleMapEnemyButton enemyTopInfo = EnemyTopInfo;
		if (!(enemyTopInfo == null) && GuildWarHelper.GetCurrentMapModel() != null)
		{
			GuildBattleMissionQueueData guildBattleMissionQueueData = CurrentMissionQueue();
			GuildBattlePvpTeam pvpTeamForMission = GuildWarHelper.GetCurrentMapModel().GetPvpTeamForMission(guildBattleMissionQueueData.EnemyMission.Id);
			enemyTopInfo.SetData(pvpTeamForMission);
			enemyTopInfo.UpdateWithOverride(enemyFoundOverride: true);
			enemyTopInfo.Show();
			Helpers.GameObjectSetActive(enemyTopInfo, value: true);
		}
	}

	private void InstantiateButtons(int missionCount)
	{
		if (searchingView == null)
		{
			searchingView = Helpers.InstantiateWithComponent<GuildBattleMissionSearchingView>(SearchingViewPrefab, base.gameObject);
		}
		Helpers.GameObjectSetActive(searchingView, value: false);
		if (buttonPvp == null)
		{
			buttonPvp = Helpers.InstantiateWithComponent<GuildBattleMissionButton>(PvpIconPrefab, base.gameObject);
		}
		Helpers.GameObjectSetActive(buttonPvp, value: false);
		if (buttonPool == null)
		{
			buttonPool = new List<GuildBattleMissionButton>();
		}
		int num = 0;
		if (missionCount > buttonPool.Count)
		{
			num = missionCount - buttonPool.Count;
		}
		BoxCollider boxCollider = null;
		for (int i = 0; i < num; i++)
		{
			GuildBattleMissionButton guildBattleMissionButton = Helpers.InstantiateToList(PveIconPrefab, base.gameObject, buttonPool);
			Helpers.GameObjectSetActive(guildBattleMissionButton, value: false);
			boxCollider = ((guildBattleMissionButton != null) ? guildBattleMissionButton.GetComponent<BoxCollider>() : null);
			if (boxCollider != null)
			{
				longestSide = Mathf.Max(boxCollider.size.x, boxCollider.size.y);
			}
		}
		for (int j = 0; j < buttonPool.Count; j++)
		{
			Helpers.GameObjectSetActive(buttonPool[j], value: false);
		}
	}

	private void Update()
	{
		if (searchingView != null && searchingView.isActiveAndEnabled && buttonPool != null)
		{
			for (int i = 0; i < buttonPool.Count; i++)
			{
				Helpers.GameObjectSetActive(buttonPool[i], value: false);
			}
			Helpers.GameObjectSetActive(buttonPvp, value: false);
		}
	}
}
