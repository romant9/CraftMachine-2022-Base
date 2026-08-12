using UnityEngine;

public class GvgStartBattleStateBase : UIStateObjectBase
{
	protected static class DescriptionText
	{
		public const string LockdownTime = "GuildBattleStartPopup.LockdownDescription{Minutes}";

		public const string WarNotActive = "GuildBattleStartPopup.WarNotActiveDescription";

		public const string MinimumPlayerRequiredInfo = "GuildBattleStartPopup.PlayerRegisteredDescription{parameter}";

		public const string GuildWarMaxParticipants = "GvG.StartBattle.NoGWNotification{Parameter}";

		public const string GuildBattleNewJoiner = "GvG.StartBattle.NewMemberNotification";
	}

	protected static class TitleText
	{
		public const string WaitingForPlayers = "GuildBattleStartPopup.WaitingForPlayers{Amount}";

		public const string BattleStarted = "GuildBattleStartPopup.BattleStarted";

		public const string FakeBattleStarted = "GuildBattleStartPopup.FakeBattleStarted";

		public const string BattleStarting = "GuildBattleStartPopup.BattleStarting";

		public const string JoinBattle = "GuildBattleStartPopup.JoinBattle";

		public const string LockdownTime = "GuildBattleStartPopup.LockdownTime";

		public const string MaxTeamSizeReached = "GuildBattleStartPopup.MaxTeamSizeReached";

		public const string WarNotActive = "GuildBattleStartPopup.WarNotActive";

		public const string WaitForBattle = "GuildBattleStartPopup.WaitForBattle";
	}

	public enum States
	{
		None = 0,
		WarNotActive = 1,
		BattleStarted = 7,
		FakeBattleStarted = 11,
		BattleActive = 8,
		Spectating = 9
	}

	public States currentState;

	public GameObject PopupContainter;

	public UILabel TitleLabel;

	public UILabel MatchTimeLabel;

	public UIGuildTierProgressBar GuildEmblem;

	public GameObject GuildEmblemEmpty;

	public UIGuildTierProgressBar EnemyGuildEmblem;

	public GvGFakeBattleContainer FakeEnemyGuildEmblem;

	public GameObject EnemyGuildEmblemEmpty;

	public UIButtonExtended GoButton;

	public UIGuildBattleVictoryPointsProgressBar ProgressBar;

	public UILabel VpRewardLabel;

	public UILabel RewardPointsRewardLabel;

	public UILabel FakeBattleVpRewardLabel;

	public UILabel FakeBattleRewardPointsRewardLabel;

	public UILabel VpDrawRewardLabel;

	public UILabel DrawRewardPointsRewardLabel;

	public UILabel DrawFakeBattleVpRewardLabel;

	public UILabel DrawFakeBattleRewardPointsRewardLabel;

	public GameObject StartBattleContainer;

	public GvGFakeBattleContainer FakeBattleStartContainer;

	public UIButtonExtendedToggle RewardsStatsToggle;

	public GameObject HighScoreContainer;

	public GameObject RewardsContainer;

	public UIButton CloseButton;

	private float refreshTimer;

	protected const int TweenBattleStartAnimation = 10;

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
		Helpers.GameObjectSetActive(CloseButton, value: true);
	}

	protected void SetState(States newState)
	{
		currentState = newState;
	}

	public override void Update()
	{
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
}
