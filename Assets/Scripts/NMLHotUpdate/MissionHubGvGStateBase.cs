using TWDModel;
using UnityEngine;

public class MissionHubGvGStateBase : UIStateObjectBase
{
	public enum States
	{
		None = 0,
		Locked = 1,
		BattleOnGoing = 2,
		BattleEnded = 3,
		EndOfSeason = 4,
		SeasonOngoing = 5,
		WarOnGoing = 6
	}

	public static class MissionHubGvGStateLocalizations
	{
		public static string NotAvailable = "Popup.MissionHub.NotAvailable";

		public static string GuildBattleOnGoing = "Popup.MissionHub.GuildBattleOngoing";

		public static string EndOfSeason = "Popup.MissionHub.EndOfSeason";

		public static string SeasonStartsIn = "Popup.MissionHub.GuildWarSeasonStartsIn";

		public static string NoSeasonActive = "Popup.MissionHub.NoSeasonActive";

		public static string PrepareForNextWar = "Popup.MissionHub.PrepareForNextWar";

		public static string WarEnds = "GvG.OverviewPopup.TimerWarEnd";

		public static string WarStartsIn = "Popup.MissionHub.WarStartsIn";

		public static string NoWarActive = "Popup.MissionHub.NoWarActive";

		public static string WaitingForBattleToStart = "Popup.MissionHub.WaitingForBattleToStart";

		public static string GuildWarUnlockAtLevel = "Popup.MissionHub.GuildWarUnlockAtLevel{CouncilLevel}";

		public static string GuildWarUnlockAfterTutorial = "Popup.MissionHub.GuildWarUnlockAfterTutorial";

		public static string GuildWarJoinGuild = "Popup.MissionHub.GuildWarJoinGuild";

		public static string BattleStarting = "GvG.BattleStarting";

		public static string BattleEnding = "GvG.BattleEnding";

		public static string BattleEndedCollectYourRewards = "GvG.MissionHub.CollectRewards";

		public static string GuildWarMaxParticipants = "GvG.StartBattle.NoGWNotification{Parameter}";

		public static string GuildBattleNewJoiner = "GvG.StartBattle.NewMemberNotification";
	}

	public States currentState;

	public GameObject BattleActiveEffect;

	public GameObject GuildBattleParticipantsContainer;

	public GameObject TimerGameobject;

	public UILabel BattleTimerLabel;

	public UILabel BattleDescription;

	public UILabel BattleParticipants;

	public UILabel TimerLabel;

	public UILabel LockedLabel;

	public UIProgressBarExtended ProgressBar;

	public Material GvGDefaultMaterial;

	public Material PrepareForWarMaterial;

	public UITexture LocationTexture;

	protected float refreshTimer;

	protected string labelLocalization = "";

	public override int Id
	{
		get
		{
			return (int)currentState;
		}
		set
		{
			currentState = (States)value;
		}
	}

	public override void Enter()
	{
		base.Enter();
		UpdateUIDynamic();
	}

	public override void Update()
	{
		if (OfflineManager.IsLoadDataManager) return;
		refreshTimer -= Time.deltaTime;
		if (refreshTimer <= 0f)
		{
			UpdateUIDynamic();
			refreshTimer = 1f;
		}
	}

	protected virtual void UpdateUIDynamic()
	{
	}

	protected void SetState(States newState)
	{
		currentState = newState;
	}

	protected void SetNumberOfParticipants()
	{
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		int registeredPlayersCountForBattleTimeSlot = GuildWarHelper.GetRegisteredPlayersCountForBattleTimeSlot();
		int maxPlayerCountInBattle = gameEconomyData.GuildWarConfig.MaxPlayerCountInBattle;
		bool flag = registeredPlayersCountForBattleTimeSlot >= gameEconomyData.GuildWarConfig.MinPlayersToStartBattle;
		BattleParticipants.color = (flag ? SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.ValidColor : SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.NotValidColor);
		HelpersUI.SetContentToLabel(BattleParticipants, $"{registeredPlayersCountForBattleTimeSlot}/{maxPlayerCountInBattle}");
	}

	protected static string FormatTimeLeft(long timeLeft)
	{
		if (timeLeft <= 0)
		{
			return "0";
		}
		return Helpers.FormatTimeNoZero(timeLeft);
	}
}
