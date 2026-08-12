using TWDModel;
using UnityEngine;

public class GuildBattleMapEnemyButton : NUIListItem<GuildBattlePvpTeam>
{
	public UILabel NameLabel;

	public GameObject ParentNotFound;

	public GameObject ParentFound;

	public GameObject ParentCompleted;

	[SerializeField]
	private PlayerEmblemIcon playerEmblemIcon;

	[SerializeField]
	private UILabel difficultyLabel;

	[Header("Tween Groups")]
	public int TweenGroupShow = 3;

	public int TweenGroupHide = 4;

	private UIButtonExtended buttonRef;

	private GuildBattleParticipantInfo pvpPlayer;

	private bool enemyCompleted;

	private string participantsTooltipText;

	private GuildBattleMapMissionModel missionModelRef;

	public GuildBattleSelectMissionPopup ParentPopup { get; set; }

	private GuildBattleMapMissionModel missionModel
	{
		get
		{
			if (GetData() == null)
			{
				return null;
			}
			if (missionModelRef == null)
			{
				PlayerModel playerModel = GameManager.Instance.playerModel;
				missionModelRef = playerModel.GuildWarModel.CurrentBattle.CurrentMapModel.GetMissionModel(GetData().MissionId);
			}
			return missionModelRef;
		}
	}

	public UIButtonExtended Button
	{
		get
		{
			if (buttonRef == null)
			{
				buttonRef = GetComponent<UIButtonExtended>();
			}
			return buttonRef;
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		UpdateWithOverride(enemyFoundOverride: true);
	}

	public void UpdateWithOverride(bool enemyFoundOverride = false, bool enemyCompletedOverride = false)
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		GuildBattleProgressSnapshot currentCompletionSnapshot = playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.CurrentCompletionSnapshot;
		bool flag = true;
		enemyCompleted = missionModel != null && currentCompletionSnapshot.IsMissionCompletionSeen(missionModel);
		enemyCompleted |= enemyCompletedOverride;
		if (pvpPlayer == null && flag)
		{
			pvpPlayer = playerModel.GuildWarModel.CurrentBattle.GetCurrentGuildBattlePlayerInfo(GetData());
		}
		if (pvpPlayer != null)
		{
			HelpersUI.SetContentToLabel(NameLabel, GameManager.Instance.GetFilteredText(pvpPlayer.Name));
			if (playerEmblemIcon != null)
			{
				playerEmblemIcon.SetEmblem(pvpPlayer.PlayerEmblem);
			}
			if (Button != null)
			{
				Button.SetClickCallback(OnClickCallback);
			}
		}
		Helpers.GameObjectSetActive(ParentNotFound, !flag);
		Helpers.GameObjectSetActive(ParentFound, flag);
		Helpers.GameObjectSetActive(ParentCompleted, enemyCompleted);
		if (enemyCompleted)
		{
			participantsTooltipText = HelpersLocalization.GetParticipantsTooltipText(missionModel);
		}
		else
		{
			participantsTooltipText = string.Empty;
		}
		if (missionModel != null && missionModel.IsEnemyUnlocked() && !missionModel.IsCompleted())
		{
			HelpersUI.SetContentToLabel(difficultyLabel, LocalizationManager.GetText("GvG.DefeatEnemy"));
		}
		else if (enemyCompleted)
		{
			HelpersUI.SetContentToLabel(difficultyLabel, LocalizationManager.GetText("GvG.MissionSelectPvpEliminated"));
		}
		else if (missionModel != null)
		{
			HelpersUI.SetContentToLabel(difficultyLabel, LocalizationManager.GetText("Popup.TeamSelection.MissionLocked[MinSurvivorLevel]", missionModel.MissionDifficultyLevel));
		}
	}

	public override int GetSortValue()
	{
		int num = 0;
		num = ((missionModel == null) ? 1 : (missionModel.AreaIndex + 2)) * -1;
		if (missionModel != null && ParentPopup != null && ParentPopup.CurrentAreaEnemyMissions.ContainsKey(missionModel.Id))
		{
			return num - 100;
		}
		if (enemyCompleted)
		{
			return num - 10000;
		}
		return num - 1000;
	}

	public void OnUIEvent(string type, object parameter)
	{
		if (parameter is GuildBattlePvpTeam guildBattlePvpTeam && GetData() != null)
		{
			if (type == "OnGuildBattleEnemyUnlocked" && guildBattlePvpTeam.MissionId == GetData().MissionId)
			{
				UpdateWithOverride(enemyFoundOverride: true);
			}
			else if (type == "OnGuildBattleEnemyCompleted" && guildBattlePvpTeam.MissionId == GetData().MissionId)
			{
				UpdateWithOverride(enemyFoundOverride: false, enemyCompletedOverride: true);
			}
		}
	}

	public void OnEnable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		UIEvent.OnUIEvent += OnUIEvent;
	}

	public void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	public void Show()
	{
		TweenManager.PlayTweenGroup(base.gameObject, TweenGroupShow);
	}

	public void Hide()
	{
		TweenManager.PlayTweenGroup(base.gameObject, TweenGroupHide);
	}

	public override void Clear()
	{
		base.Clear();
		pvpPlayer = null;
		if (Button != null)
		{
			Button.Clear();
		}
		missionModelRef = null;
		ParentPopup = null;
		enemyCompleted = false;
	}

	private void OnClickCallback(UIButtonExtended button)
	{
		if (pvpPlayer != null && GetData() != null)
		{
			if (!string.IsNullOrEmpty(participantsTooltipText))
			{
				TooltipManager.OpenTextBoxWithText(base.gameObject, participantsTooltipText);
			}
			else
			{
				UIEvent.Send("OnClickedPvpEnemyInfo", GetData());
			}
		}
	}
}
