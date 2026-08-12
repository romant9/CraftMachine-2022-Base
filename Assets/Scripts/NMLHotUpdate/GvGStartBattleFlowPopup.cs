using System.Linq;
using TWDModel;
using UnityEngine;

public class GvGStartBattleFlowPopup : HUDElement
{
	[SerializeField]
	private UIButtonExtended guildButton;

	[Header("Registration")]
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel matchTimeLabel;

	[SerializeField]
	private UIGuildTierProgressBar guildEmblem;

	[SerializeField]
	private GameObject guildEmblemEmpty;

	[SerializeField]
	private UIGuildTierProgressBar enemyGuildEmblem;

	[SerializeField]
	private GvGFakeBattleContainer fakeEnemyGuildEmblem;

	[SerializeField]
	private GameObject enemyGuildEmblemEmpty;

	[Header("StartBattle")]
	[SerializeField]
	private GameObject startBattleContainer;

	[SerializeField]
	private GvGFakeBattleContainer startFakeBattleContainer;

	[SerializeField]
	private UIButtonExtended goButton;

	[SerializeField]
	private UIGuildBattleVictoryPointsProgressBar progressBar;

	[SerializeField]
	private UILabel vpRewardLabel;

	[SerializeField]
	private UILabel rewardPointsRewardLabel;

	[SerializeField]
	private UILabel fakeBattleVpRewardLabel;

	[SerializeField]
	private UILabel fakeBattleRewardPointsRewardLabel;

	[SerializeField]
	private UILabel vpDrawRewardLabel;

	[SerializeField]
	private UILabel drawRewardPointsRewardLabel;

	[SerializeField]
	private UILabel fakeDrawBattleVpRewardLabel;

	[SerializeField]
	private UILabel fakeDrawBattleRewardPointsRewardLabel;

	[SerializeField]
	private UIButtonExtendedToggle rewardsStatsToggle;

	[SerializeField]
	private GameObject rewardsContainer;

	[SerializeField]
	private GameObject highScoreContainer;

	[SerializeField]
	private UIButton closeButton;

	public GvgStartBattleStateBase.States previousState;

	public GvgStartBattleStateBase.States currentState;

	private UIStateMachine stateMachine;

	public static bool CanShow()
	{
		return (byte)(0u | (ShowStartParticipant() ? 1u : 0u) | (ShowStartNoParticipant() ? 1u : 0u) | ((!GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.HasSeenBattleStart()) ? 1u : 0u)) != 0;
	}

	private static bool ShowStartParticipant()
	{
		if (!GameManager.Instance.gameEconomyData.GetFeature("GuildBattleStartPopup").Enabled)
		{
			return false;
		}
		bool num = GuildWarHelper.IsPlayerRegisteredForBattle();
		bool flag = GuildWarHelper.IsBattleOnGoing();
		if (num)
		{
			return !flag;
		}
		return false;
	}

	private static bool ShowStartNoParticipant()
	{
		if (!GameManager.Instance.gameEconomyData.GetFeature("GuildBattleStartPopup").Enabled)
		{
			return false;
		}
		bool num = GuildWarHelper.IsPlayerRegisteredForBattle();
		bool flag = GuildWarHelper.CanPlayerRegisterForBattle();
		return !num && flag;
	}

	public override void Open()
	{
		DebugTWD.Log("Open GvGStartBattleFlowPopup", DebugType.ActivateObject);
		HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.SocialPopupGuild);
		bool flag = noCreation != null && noCreation.IsOpen;
		if (!base.IsOpen || flag)
		{
			base.Open();
			if (guildButton != null)
			{
				guildButton.SetClickCallback(OnGuildButtonClicked);
			}
			if (goButton != null)
			{
				goButton.SetClickCallback(OnConfirmBattleStartClick);
			}
			InitStateMachine();
			DetermineAndSetState();
			UpdateUI();
			if (flag)
			{
				noCreation.Close();
			}
		}
	}

	public override void Close()
	{
		if (stateMachine != null)
		{
			stateMachine.currentState.Exit();
		}
		base.Close();
	}

	public bool CanClose()
	{
		if (stateMachine != null && (currentState == GvgStartBattleStateBase.States.BattleActive || currentState == GvgStartBattleStateBase.States.BattleStarted || currentState == GvgStartBattleStateBase.States.FakeBattleStarted || currentState == GvgStartBattleStateBase.States.Spectating))
		{
			return true;
		}
		return false;
	}

	private void SubscribeForEvents()
	{
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			guildWarModel.Changed -= OnModelChanged;
			guildWarModel.Changed += OnModelChanged;
		}
		EventManager.OnEvent -= OnEvent;
		EventManager.OnEvent += OnEvent;
	}

	private void OnEnable()
	{
		SubscribeForEvents();
		if (OfflineManager.IsLoadDataManager)
		{
			var back = GetComponentsInChildren<UITexture>(true).FirstOrDefault(x => x.name == "Background");
			if (back)
			{
				var rext = back.uvRect;
				rext.Set(0f, .02f, 1f, .49f);
				back.uvRect = rext;
				back.applyGradient = false;
			}
		}
	}

	private void OnDisable()
	{
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			guildWarModel.Changed -= OnModelChanged;
		}
		EventManager.OnEvent -= OnEvent;
	}

	private void OnCloseButtonClick(UIButtonExtended button)
	{
		OnClickClose();
	}

	public override void OnBackButtonClicked()
	{
		if (CanClose())
		{
			Close();
		}
		else
		{
			HUDManager.TryClosePopup(UIType.GuildBattleMapPopup);
		}
	}

	private void OnConfirmBattleStartClick(UIButtonExtended button)
	{
		if (stateMachine.currentState.AllowExit())
		{
			base.Close();
			if (stateMachine != null)
			{
				stateMachine.currentState.Exit();
			}
		}
	}

	private void OnGuildButtonClicked(UIButtonExtended button)
	{
		CampHUD.OpenGuildOrChallenge(UIType.SocialPopupGuild);
	}

	private void OnJoinButtonClick(UIButtonExtended button)
	{
		UpdateUI();
	}

	public override void Update()
	{
		base.Update();
		if (stateMachine != null)
		{
			stateMachine.Update();
		}
	}

	public override void UpdateUI()
	{
		if (stateMachine != null)
		{
			stateMachine.UpdateUI();
		}
	}

	private void SetPopupState(GvgStartBattleStateBase.States newState)
	{
		previousState = currentState;
		currentState = newState;
	}

	private void UpdateState(GvgStartBattleStateBase.States state, bool forceUpdate = false)
	{
		if (TryChangeState(state, forceUpdate))
		{
			SetPopupState(state);
		}
	}

	private bool TryChangeState(GvgStartBattleStateBase.States state, bool forceUpdate = false)
	{
		if (stateMachine != null)
		{
			return stateMachine.TrySwitchToState((int)state, forceUpdate);
		}
		return false;
	}

	private void OnEvent(EventManager.EventType eventType, object parameter)
	{
		switch (eventType)
		{
		case EventManager.EventType.GuildBattleLockdownTimeEvent:
			UpdateStateMachineOnChange();
			break;
		case EventManager.EventType.GroupModelLoaded:
			SubscribeForEvents();
			UpdateStateMachineOnChange();
			break;
		}
	}

	private void OnModelChanged(TWDGroupModelChild twdGroupModelChild, string changed, object args)
	{
		if (changed == "GuildBattleStarted")
		{
			UpdateStateMachineOnChange();
		}
	}

	private void UpdateStateMachineOnChange()
	{
		DetermineAndSetState();
		UpdateUI();
	}

	private void DetermineAndSetState()
	{
		if (GuildWarHelper.IsBattleOnGoing())
		{
			if (GuildWarHelper.IsPlayerRegisteredForBattle())
			{
				if (!GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.HasSeenBattleStart())
				{
					if (GuildWarHelper.GetCurrentBattle().IsFakeBattle)
					{
						UpdateState(GvgStartBattleStateBase.States.FakeBattleStarted);
					}
					else
					{
						UpdateState(GvgStartBattleStateBase.States.BattleStarted);
					}
				}
				else
				{
					UpdateState(GvgStartBattleStateBase.States.BattleActive);
				}
			}
			else if (!GuildWarHelper.IsWarOngoing())
			{
				UpdateState(GvgStartBattleStateBase.States.WarNotActive);
			}
			else
			{
				UpdateState(GvgStartBattleStateBase.States.Spectating);
			}
		}
		else
		{
			UpdateState(GvgStartBattleStateBase.States.WarNotActive);
		}
	}

	private void InitStateMachine()
	{
		if (stateMachine == null)
		{
			stateMachine = UIStateMachine.AddTo(base.gameObject);
		}
		if (stateMachine != null)
		{
			stateMachine.Clear();
			stateMachine.AddState(new GvgStartBattleStateBattleStarted());
			stateMachine.AddState(new GvgStartBattleStateFakeBattleStarted());
			stateMachine.AddState(new GvgStartBattleStateBattleActive());
			stateMachine.AddState(new GvgStartBattleStateWarNotActive());
			stateMachine.AddState(new GvgStartBattleStateSpectating());
			stateMachine.SetDefaultState(1);
			PassReferencesToStates();
		}
	}

	private void PassReferencesToStates()
	{
		if (!(stateMachine != null))
		{
			return;
		}
		for (int i = 0; i < stateMachine.StatesList.Count; i++)
		{
			if (stateMachine.StatesList[i] != null && stateMachine.StatesList[i] is GvgStartBattleStateBase)
			{
				GvgStartBattleStateBase obj = stateMachine.StatesList[i] as GvgStartBattleStateBase;
				obj.EnemyGuildEmblem = enemyGuildEmblem;
				obj.FakeEnemyGuildEmblem = fakeEnemyGuildEmblem;
				obj.EnemyGuildEmblemEmpty = enemyGuildEmblemEmpty;
				obj.GuildEmblem = guildEmblem;
				obj.GuildEmblemEmpty = guildEmblemEmpty;
				obj.MatchTimeLabel = matchTimeLabel;
				obj.TitleLabel = titleLabel;
				obj.GoButton = goButton;
				obj.VpRewardLabel = vpRewardLabel;
				obj.FakeBattleVpRewardLabel = fakeBattleVpRewardLabel;
				obj.RewardPointsRewardLabel = rewardPointsRewardLabel;
				obj.FakeBattleRewardPointsRewardLabel = fakeBattleRewardPointsRewardLabel;
				obj.VpDrawRewardLabel = vpDrawRewardLabel;
				obj.DrawFakeBattleVpRewardLabel = fakeDrawBattleVpRewardLabel;
				obj.DrawRewardPointsRewardLabel = drawRewardPointsRewardLabel;
				obj.DrawFakeBattleRewardPointsRewardLabel = fakeDrawBattleRewardPointsRewardLabel;
				obj.ProgressBar = progressBar;
				obj.StartBattleContainer = startBattleContainer;
				obj.FakeBattleStartContainer = startFakeBattleContainer;
				obj.PopupContainter = base.gameObject;
				obj.RewardsStatsToggle = rewardsStatsToggle;
				obj.HighScoreContainer = highScoreContainer;
				obj.RewardsContainer = rewardsContainer;
				obj.CloseButton = closeButton;
			}
		}
	}

	#region mycode
	public void ChangeRealTymeResults(UIToggle tg)
	{
		highScoreContainer.GetComponent<GuildBattleHighscoresPanel>().UpdateScoreExt(tg.value, out int Vp, out int enemyVp);
		progressBar.SetVictoryPoints(Vp, enemyVp);
	}
	#endregion
}
