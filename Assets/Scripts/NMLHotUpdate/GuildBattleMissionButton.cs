using System;
using System.Collections;
using Client.Tweener;
using TWDModel;
using UnityEngine;

[RequireComponent(typeof(UIButtonWithLabelAndIcon))]
public class GuildBattleMissionButton : MonoBehaviourExtended
{
	public enum InitState
	{
		None = 0,
		NewQueue = 1,
		ReturnFromCombat = 2,
		IsOpen = 3,
		FromMap = 4
	}

	[NonSerialized]
	public InitState initState;

	[NonSerialized]
	public TweenTimeline ButtonTimeline = new TweenTimeline();

	[NonSerialized]
	public int Index = -1;

	[Header("General")]
	public UILabel ButtonIndexLabel;

	[Header("Delays")]
	public float DelayFromCombat = 2f;

	public float DelayFromMap;

	[Header("Misc")]
	[SerializeField]
	private GuildBattleActivityIndicator activityIndicator;

	[Header("Tweens Generic")]
	public int TweenGroupOpen = 3;

	public int TweenGroupClose = 4;

	public int TweenGroupAreaComplete = 9;

	[Header("Tween Missions Main States")]
	[Tooltip("Forced to end state if already seen")]
	public int TweenNonCompleted = 5;

	[Tooltip("Forced to end state if already seen")]
	public int TweenCompleted = 6;

	[Header("Tween Transitions")]
	public int TweenEnemyFound = 7;

	[Header("PVP Mission")]
	[SerializeField]
	private UILabel enemyPlayerNameLabel;

	[SerializeField]
	private PlayerEmblemIcon playerEmblemIcon;

	[SerializeField]
	private UISprite missionObjectiveIcon;

	[Header("VP Reward Prefab")]
	[SerializeField]
	protected GameObject collectAnimVP;

	private GuildBattleParticipantInfo pvpPlayer;

	private UIButtonWithLabelAndIcon button;

	public GuildBattleMapMissionModel Model { get; set; }

	public bool PvpButton { get; set; }

	public bool EnemyInQueueUnlocked { get; set; }

	public UIButtonWithLabelAndIcon Button
	{
		get
		{
			if (button == null)
			{
				button = GetComponent<UIButtonWithLabelAndIcon>();
			}
			return button;
		}
	}

	public void ClearTimeline()
	{
		if (ButtonTimeline == null)
		{
			ButtonTimeline = new TweenTimeline();
		}
		else
		{
			ButtonTimeline.Clear();
		}
	}

	public void SetModel(GuildBattleMapMissionModel model)
	{
		Model = model;
	}

	private bool IsCompletetionSeen(GuildBattleProgressSnapshot savedState)
	{
		return savedState.IsMissionCompletionSeen(Model);
	}

	public bool ResetToSavedState(GuildBattleMissionQueueData currentQueue, GuildBattleProgressSnapshot savedState)
	{
		if (Model == null || savedState == null || currentQueue == null)
		{
			return false;
		}
		int num = (IsCompletetionSeen(savedState) ? TweenCompleted : TweenNonCompleted);
		num = ((!PvpButton && EnemyInQueueUnlocked) ? TweenCompleted : num);
		TweenManager.PlayTweenGroup(base.gameObject, num, forward: true, null, resetToEnd: true);
		return true;
	}

	public void QueueStartDelay()
	{
		float num = 0f;
		if (initState == InitState.ReturnFromCombat)
		{
			num = DelayFromCombat;
		}
		else if (initState == InitState.None)
		{
			num = DelayFromMap;
		}
		if (num > 0f)
		{
			ButtonTimeline.Queue(TweenObjects.Wait(base.transform, num));
		}
	}

	public void QueueOpenTween()
	{
		if (initState != InitState.IsOpen)
		{
			TweenManager.PlayTweenGroup(base.gameObject, TweenGroupClose, forward: true, null, resetToEnd: true);
			ButtonTimeline.Queue(TweenObjects.Group(base.transform, TweenGroupOpen));
		}
	}

	public void QueueCompleteTween(GuildBattleProgressSnapshot savedState)
	{
		if (Model != null && savedState != null)
		{
			bool flag = (PvpButton ? Model.IsCompleted() : Model.IsMissionPveComplete());
			if (!IsCompletetionSeen(savedState) && flag && !EnemyInQueueUnlocked)
			{
				ButtonTimeline.Queue(TweenObjects.Group(base.transform, TweenCompleted));
			}
		}
	}

	public void QueueEnemyFoundTween(GuildBattleMissionQueueData currentQueue, GuildBattleProgressSnapshot savedState)
	{
		if (currentQueue.PvPEnemyUnlocked && savedState.IsMissionEnemySeen(currentQueue.EnemyMission) && Model.IsEnemyUnlocked() && savedState.IsMissionEnemySeen(Model))
		{
			ButtonTimeline.Queue(TweenObjects.Group(base.transform, TweenEnemyFound));
		}
	}

	public void QueueButtonClose()
	{
		ButtonTimeline.Queue(TweenObjects.Group(base.transform, TweenGroupClose));
	}

	public void QueueAreaCompleteTween(GuildBattleMissionQueueData currentQueue)
	{
		if (currentQueue.IsComplete && currentQueue.Last)
		{
			ButtonTimeline.Queue(TweenObjects.Group(base.transform, TweenGroupAreaComplete));
		}
	}

	private void OnEnable()
	{
		if (Model != null)
		{
			GuildBattleMapPopup guildBattleMapPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GuildBattleMapPopup) as GuildBattleMapPopup;
			if (((guildBattleMapPopup != null) ? guildBattleMapPopup.GetInitState() : InitState.None) == InitState.ReturnFromCombat)
			{
				CheckIfPlayerJustCompletedMission();
			}
			SetMissionObjectiveIcons();
		}
	}

	private static SurvivalMissionConfig GetGeneratedSurvivalMissionConfigs(GuildBattleMapMissionModel Model)
	{
		SurvivalMissionConfig result = null;
		TWDModel.Tuple<int, int> tuple = null;
		string text = null;
		text = Model.SectorModelOwner.MissionConfigPoolName;
		tuple = new TWDModel.Tuple<int, int>(Model.MissionConfigIndexObjective, Model.MissionConfigIndexEnemies);
		if (tuple != null && text != null)
		{
			result = GuildBattleMapMissionModel.GenerateSurvivalMissionConfig(text, tuple, GameManager.Instance.gameEconomyData);
		}
		return result;
	}

	private void SetMissionObjectiveIcons()
	{
		if (missionObjectiveIcon == null)
		{
			return;
		}
		SurvivalMissionConfig survivalMissionConfig = null;
		survivalMissionConfig = ((Model.GeneratedSurvivalMissionConfig != null) ? Model.GeneratedSurvivalMissionConfig : GetGeneratedSurvivalMissionConfigs(Model));
		if (survivalMissionConfig != null)
		{
			if (survivalMissionConfig.ThreatStart > 0)
			{
				missionObjectiveIcon.spriteName = "Ui_Mission_ThreatStart";
			}
			else if (survivalMissionConfig.SpawnerCount > 0)
			{
				missionObjectiveIcon.spriteName = "Ui_Mission_PileSpawns";
			}
			else if (survivalMissionConfig.ObjectiveType == SurvivalMissionConfig.SurvivalObjectiveType.KillAllWalkers)
			{
				missionObjectiveIcon.spriteName = "Ui_Mission_KillAll";
			}
		}
	}

	public void UpdateUI()
	{
		if (Model == null)
		{
			return;
		}
		GuildBattleParticipantInfo guildBattleParticipantInfo = (PvpButton ? GetPvPPlayer() : null);
		if (guildBattleParticipantInfo != null)
		{
			if (Model.Type == GuildBattleMapMissionModel.MissionType.PVP)
			{
				HelpersUI.SetContentToLabel(enemyPlayerNameLabel, guildBattleParticipantInfo.Name);
				if (playerEmblemIcon != null)
				{
					playerEmblemIcon.SetEmblem(guildBattleParticipantInfo.PlayerEmblem);
				}
			}
			else
			{
				Debug.LogWarningFormat("PVP Enemy found for NON pvp. Sector:{0} Mission{1}", Model.SectorIdOwner, Model.Id);
			}
		}
		SubscribeForGuildModelEvents();
		UpdateMissionActivity();
		if (Button != null)
		{
			if (Model.IsCompleted() && !HelpersModel.IsUnlockAllSectors)
			{
				Button.Clear();
			}
			else
			{
				Button.SetClickCallback(OnClickMissionSelect);
			}
		}
		HelpersUI.SetContentToLabel(ButtonIndexLabel, "#" + Index, Index > -1);
	}

	protected void CheckIfPlayerJustCompletedMission(GuildBattleMapMissionModel missionModel = null)
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		GuildBattleMapMissionModel guildBattleMapMissionModel = ((missionModel == null) ? Model : missionModel);
		if (playerModel.GuildBattlePlayer.AttackTargetMission.AttackMissionId != null && playerModel.MissionStatistics.LastCombatResult == ECombatResult.Successful && guildBattleMapMissionModel.Id == playerModel.GuildBattlePlayer.AttackTargetMission.AttackMissionId && GuildWarHelper.IsBattleOnGoing())
		{
			bool isPvPCombat = playerModel.GuildBattlePlayer.AttackTargetMission.IsPvPCombat;
			bool num = playerModel.GuildBattlePlayer.CurrentMissionRetriedAttempts > 0;
			if (num && HelpersModel.IsUnlockAllSectors) num = false;
			playerModel.GuildBattlePlayer.AttackTargetMission.AttackMissionId = null;
			int num2 = GuildWarHelper.GetCurrentBattle().GetGuildBattleMissionVictoryPoints(guildBattleMapMissionModel.SectorIdOwner, isPvPCombat, guildBattleMapMissionModel.AreaIndex);
			if (num)
			{
				int num3 = (int)FixedPoint.Round(num2 * (GameManager.Instance.gameEconomyData.GuildWarConfig.RetryMissionPenalty + 0.0001));
				num2 -= num3;
			}
			StartCoroutine(DelayVPAnimation(num2));
		}
	}

	private IEnumerator DelayVPAnimation(int rewardMissionAmount)
	{
		yield return new WaitForSeconds(DelayFromCombat);
		SingularityMonoBehaviour<GuildWarManager>.Instance.PlayVPRewardAnimation(rewardMissionAmount, collectAnimVP, base.transform, null, useCameraOffsetPos: false);
	}

	private void SubscribeForGuildModelEvents()
	{
		GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
		if (currentBattle != null && currentBattle.CurrentMapModel != null)
		{
			currentBattle.Changed -= OnBattleModelChange;
			currentBattle.Changed += OnBattleModelChange;
		}
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			guildWarModel.Changed -= OnBattleModelChange;
			guildWarModel.Changed += OnBattleModelChange;
		}
	}

	private void OnBattleModelChange(TWDGroupModelChild model, string changed, object args)
	{
		if (changed == "GuildBattleLiveDataUpdated")
		{
			UpdateUI();
		}
		else if (changed == "GuildBattleEnded")
		{
			UpdateMissionActivity();
		}
	}

	private void UpdateMissionActivity()
	{
		if (activityIndicator != null)
		{
			GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
			if (currentBattle != null)
			{
				activityIndicator.MissionActivityIndicatorCheck(currentBattle, Model, PvpButton);
			}
		}
	}

	public GuildBattleParticipantInfo GetPvPPlayer()
	{
		if (Model == null || !Model.IsEnemyUnlocked())
		{
			return null;
		}
		if (pvpPlayer == null)
		{
			PlayerModel playerModel = GameManager.Instance.playerModel;
			GuildBattlePvpTeam pvpTeamForMission = playerModel.GuildWarModel.CurrentBattle.CurrentMapModel.GetPvpTeamForMission(Model.Id);
			if (pvpTeamForMission != null)
			{
				pvpPlayer = playerModel.GuildWarModel.CurrentBattle.GetCurrentGuildBattlePlayerInfo(pvpTeamForMission);
			}
		}
		return pvpPlayer;
	}

	public override void Clear()
	{
		base.Clear();
		GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
		if (currentBattle != null && currentBattle.CurrentMapModel != null)
		{
			currentBattle.Changed -= OnBattleModelChange;
		}
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			guildWarModel.Changed -= OnBattleModelChange;
		}
		if (button != null)
		{
			button.Clear();
		}
		Model = null;
		pvpPlayer = null;
		initState = InitState.None;
		Index = -1;
		EnemyInQueueUnlocked = false;
	}

	private void OnClickMissionSelect(UIButtonExtended button)
	{
		if (Model == null)
		{
			return;
		}
		if (GuildWarHelper.IsBattleOngoingAndPlayerRegistered())
		{
			if (HelpersModel.IsUnlockPVP)
			{
				OpenTeamSelection();
				return;
			}

			if (GameManager.Instance.GuildManager.GuildOffline || GameManager.Instance.GuildManager.IsBusy)
			{
				AlertPopup.ShowPopupGetText("Popup.Alert.NotAvailableTitle", "Popup.Alert.NotAvailableMessage", "Button.Ok", null);
				return;
			}
			if (PvpButton && !EnemyInQueueUnlocked)
			{
				AlertPopup.ShowPopupGetText("Generic.Info", "GvG.Popup.CompletePvEFirst", "Button.Ok", null);
				return;
			}
			GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
			activityIndicator.CheckIfMissionIsOccupied(currentBattle, Model, PvpButton, OpenTeamSelection);
		}
		else
		{
			if (HelpersModel.IsUnlockPVP)
			{
				OpenTeamSelection();
				return;
			}
			GuildWarHelper.ShowNotAvailableAlertPopup();
		}
	}

	private void OpenTeamSelection()
	{
		if (!Model.IsCompleted() && (!EnemyInQueueUnlocked || PvpButton) || HelpersModel.IsUnlockPVP)
		{
			var parent = OfflineManager.IsLoadDataManager ? HUDManager.Instance.UIContainerTopCameras : null;
			TeamSelectionPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MapTeamSelection, parent) as TeamSelectionPopup;
			obj.SurvivorType = SurvivorContainerModel.SurvivorType.CombatGuildBattle;
			obj.OpenForModel(Model);
			EventManager.NotifyClick("SelectTeam");
		}
	}
}
