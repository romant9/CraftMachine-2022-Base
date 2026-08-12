using BaseModel;
using TWDModel;
using UnityEngine;

public class MissionHubPanelGuildBattle : MissionHubGameModePanel
{
	[Header("Guild War")]
	public GameObject BattleActiveEffect;

	public GameObject GuildBattleParticipantsContainer;

	public UILabel BattleTimerLabel;

	public UILabel BattleDescription;

	public UILabel BattleParticipants;

	public Material GvGDefaultMaterial;

	public Material PrepareForWarMaterial;

	public MissionHubGvGStateBase.States previousState;

	public MissionHubGvGStateBase.States currentState;

	private UIStateMachine stateMachine;

	public override void Start()
	{
		base.Start();
		if (IsLoadDataManager) return;
		InitStateMachine();
		DetermineAndSetState();
		UpdateUI();
	}

	protected override void OpenDialog()
	{
		DebugTWD.Log("OnClick MissionHubPanelGuildBattle. Try Open GvGBattleMap. Set Maybe IsFakeBattle", DebugType.Wars);
		//GuildWarHelper.GetCurrentBattle().IsFakeBattle = true;
		MissionHubNavigation.TryOpenGvGBattleMap();
	}

	public override void Update()
	{
		if (stateMachine != null)
		{
			stateMachine.Update();
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (stateMachine != null)
		{
			stateMachine.UpdateUI();
			if (OfflineManager.IsLoadDataManager)
			{
				string count = GuildWarHelper.GetCurrentBattle()?.RegisteredPlayersCount.ToString() ?? "0";

				BattleParticipants.text = count + "/10";
			}
		}
	}

	public override void CheckLockedState()
	{
		UpdateLockedState(GuildWarHelper.IsLockedByCouncilLevelOrTutorial() || !GameManager.Instance.playerModel.IsGuildMember);
	}

	protected override void AddListeners()
	{
		base.AddListeners();
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel != null)
		{
			playerModel.Changed -= OnPlayerModelChanged;
			playerModel.Changed += OnPlayerModelChanged;
		}
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			guildWarModel.Changed -= OnModelChange;
			guildWarModel.Changed += OnModelChange;
		}
		GuildModel guildModel = GameManager.Instance.playerModel.GuildModel;
		if (guildModel != null)
		{
			guildModel.Changed -= OnGuildModelChanged;
			guildModel.Changed += OnGuildModelChanged;
		}
		GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
		if (currentBattle != null)
		{
			currentBattle.Changed -= OnModelChange;
			currentBattle.Changed += OnModelChange;
		}
	}

	protected override void RemoveListeners()
	{
		base.RemoveListeners();
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel != null)
		{
			playerModel.Changed -= OnPlayerModelChanged;
		}
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			guildWarModel.Changed -= OnModelChange;
		}
		GuildModel guildModel = GameManager.Instance.playerModel.GuildModel;
		if (guildModel != null)
		{
			guildModel.Changed -= OnGuildModelChanged;
		}
		GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
		if (currentBattle != null)
		{
			currentBattle.Changed -= OnModelChange;
		}
	}

	private void OnModelChange(TWDGroupModelChild model, string changed, object args)
	{
		UpdateStateMachineOnModelChange();
	}

	private void OnGuildModelChanged(GroupModelBase model, string changed, object args)
	{
		if (changed == "MemberRemoved" || changed == "MemberAccepted")
		{
			CheckLockedState();
			UpdateStateMachineOnModelChange();
		}
	}

	private void OnPlayerModelChanged(ModelObject model, string changed, object args)
	{
		if (changed == "guildChanged")
		{
			AddListeners();
		}
	}

	private void UpdateStateMachineOnModelChange()
	{
		DetermineAndSetState();
		UpdateUI();
	}

	public void DetermineAndSetState()
	{
		if (base.isLocked)
		{
			ChangeState(MissionHubGvGStateBase.States.Locked);
		}
		else if (GuildWarHelper.CanShowBattleEnd())
		{
			ChangeState(MissionHubGvGStateBase.States.BattleEnded);
		}
		else if (GuildWarHelper.IsBattleOnGoing())
		{
			ChangeState(MissionHubGvGStateBase.States.BattleOnGoing);
		}
		else if (GuildWarHelper.IsWarOngoing())
		{
			ChangeState(MissionHubGvGStateBase.States.WarOnGoing);
		}
		else if (GuildWarHelper.IsSeasonOngoing())
		{
			ChangeState(MissionHubGvGStateBase.States.SeasonOngoing);
		}
		else
		{
			ChangeState(MissionHubGvGStateBase.States.EndOfSeason);
		}
	}

	private bool ChangeState(MissionHubGvGStateBase.States state, bool forceUpdate = false)
	{
		if (stateMachine != null)
		{
			previousState = currentState;
			currentState = state;
			return stateMachine.TrySwitchToState((int)state, forceUpdate);
		}
		return false;
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
			stateMachine.AddState(new MissionHubGvGStateBattleEnded());
			stateMachine.AddState(new MissionHubGvGStateBattleOngoing());
			stateMachine.AddState(new MissionHubGvGStateEndOfSeason());
			stateMachine.AddState(new MissionHubGvGStateLocked());
			stateMachine.AddState(new MissionHubGvGStateSeasonOngoing());
			stateMachine.AddState(new MissionHubGvGStateWarOngoing());
			stateMachine.SetDefaultState(4);
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
			if (stateMachine.StatesList[i] != null && stateMachine.StatesList[i] is MissionHubGvGStateBase)
			{
				MissionHubGvGStateBase obj = stateMachine.StatesList[i] as MissionHubGvGStateBase;
				obj.BattleActiveEffect = BattleActiveEffect;
				obj.BattleDescription = BattleDescription;
				obj.BattleParticipants = BattleParticipants;
				obj.BattleTimerLabel = BattleTimerLabel;
				obj.GuildBattleParticipantsContainer = GuildBattleParticipantsContainer;
				obj.GvGDefaultMaterial = GvGDefaultMaterial;
				obj.LocationTexture = locationTexture;
				obj.LockedLabel = lockedLabel;
				obj.PrepareForWarMaterial = PrepareForWarMaterial;
				obj.ProgressBar = progressBar;
				obj.TimerGameobject = timerGameobject;
				obj.TimerLabel = timerLabel;
			}
		}
	}

	protected override void ButtonMainClicked(UIButtonExtended button)
	{
		base.ButtonMainClicked(button);
		EventManager.NotifyClick("GuildBattle");
	}



	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	#endregion

	#region mycode
	public void OnEnable()
	{
		if (!IsLoadDataManager) return;
		InitStateMachine();
		DetermineAndSetState();
		UpdateUI();
	}

	public void OnDisable()
	{
		if (!IsLoadDataManager) return;
		stateMachine = null;
	}
	#endregion
}
