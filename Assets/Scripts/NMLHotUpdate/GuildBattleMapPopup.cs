using BaseModel;
using TWDModel;
using UnityEngine;

public class GuildBattleMapPopup : HUDElement
{
	[SerializeField]
	private GuildBattleMapView viewPrefab;

	[SerializeField]
	private UIButtonExtended battleStatsButton;

	[SerializeField]
	private GameObject mapTutorialContent;

	[Header("Active Bonuses")]
	[SerializeField]
	private GuildBattleActiveBonusList activeBonusesList;

	[Header("Containers")]
	[SerializeField]
	private GameObject BattleActiveElementsRoot;

	[SerializeField]
	private GameObject SpectatorModeElementsRoot;

	[SerializeField]
	private GameObject genericItemContainer;

	private GuildBattleMissionButton.InitState initState;

	private float refreshTimer;

	private bool isMapVisible;

	private GuildBattleMapView viewInstance;

	private bool isBonusUiVisible;

	public GuildBattleMapMissionModel MapMissionModel { get; set; }

	private bool isInSpectatorMode => !GuildWarHelper.IsPlayerRegisteredForBattle();

	public GuildBattleMapView GetViewInstance()
	{
		return viewInstance;
	}

	public GuildBattleMissionButton.InitState GetInitState()
	{
		return initState;
	}

	public void ClearToMap()
	{
		initState = GuildBattleMissionButton.InitState.FromMap;
	}

	public override void Open()
	{
		SingularityMonoBehaviour<GuildWarManager>.Instance.SubscribeToEvents();
		initState = ((MapMissionModel != null) ? GuildBattleMissionButton.InitState.ReturnFromCombat : GuildBattleMissionButton.InitState.FromMap);
		base.Open();
		UITypeOpenOnClose = UIType.GvGHubPopup;
		TryOpenGvGHubPopup();
		if (viewInstance == null)
		{
			viewInstance = Object.Instantiate(viewPrefab);
		}
		isMapVisible = !IsGvgStartBattleFlowPopupOpen() && !IsGuildBattleSelectMissionPopupOpen() && !IsGvgHubPopupOpen();
		UpdateUI();
		GuildWarHelper.CheckGuildWarFlowPopups();
	}

	private bool TryToOpenGvgStartBattlePopup()
	{
		bool num = GvGStartBattleFlowPopup.CanShow();
		if (num)
		{
			OpenGvgStarBattlePopup();
		}
		return num;
	}

	private bool TryOpenGvGHubPopup()
	{
		bool num = GvGStartBattleFlowPopup.CanShow();
		if (num)
		{
			var parent = OfflineManager.IsLoadDataManager ? HUDManager.Instance.UIContainerTopCameras : null;
			(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GvGHubPopup, parent) as GvGHubPopup).Open();
		}
		return num;
	}

	private void CloseGvGHubPopup()
	{
		(SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GvGHubPopup) as GvGHubPopup)?.Close();
	}

	private void OpenGvgStarBattlePopup()
	{
		var parent = OfflineManager.IsLoadDataManager ? HUDManager.Instance.UIContainerTopCameras : null;
		GvGStartBattleFlowPopup gvGStartBattleFlowPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GvGStartBattleFlowPopup, parent) as GvGStartBattleFlowPopup;
		if (gvGStartBattleFlowPopup != null && !gvGStartBattleFlowPopup.IsOpen)
		{
			gvGStartBattleFlowPopup.Open();
		}
	}

	public void OnEnable()
	{
		SubscribeToEvents();
	}

	private void SubscribeToEvents()
	{
		UIEvent.OnUIEvent -= UIEventHandler;
		UIEvent.OnUIEvent += UIEventHandler;
		EventManager.OnEvent -= OnEvent;
		EventManager.OnEvent += OnEvent;
		GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.Changed -= OnGuildWarPlayerChange;
		GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.Changed += OnGuildWarPlayerChange;
		GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.Changed -= OnGuildBattlePlayerChange;
		GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.Changed += OnGuildBattlePlayerChange;
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			guildWarModel.Changed -= OnGuildWarModelChange;
			guildWarModel.Changed += OnGuildWarModelChange;
			if (guildWarModel.CurrentBattle != null)
			{
				guildWarModel.CurrentBattle.Changed -= OnGuildBattleModelChange;
				guildWarModel.CurrentBattle.Changed += OnGuildBattleModelChange;
			}
		}
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (guildModel != null)
		{
			guildModel.Changed -= OnGuildModelChanged;
			guildModel.Changed += OnGuildModelChanged;
		}
	}

	public void OnDisable()
	{
		UIEvent.OnUIEvent -= UIEventHandler;
		EventManager.OnEvent -= OnEvent;
		GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.Changed -= OnGuildWarPlayerChange;
		GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.Changed -= OnGuildBattlePlayerChange;
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			guildWarModel.Changed -= OnGuildWarModelChange;
			if (guildWarModel.CurrentBattle != null)
			{
				guildWarModel.CurrentBattle.Changed -= OnGuildBattleModelChange;
			}
		}
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (guildModel != null)
		{
			guildModel.Changed -= OnGuildModelChanged;
		}
	}

	private void OnGuildModelChanged(GroupModelBase model, string changed, object args)
	{
		string text = args as string;
		if (changed == "MemberRemoved" && text == GameManager.Instance.playerModel.HashedId)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.GuildBattleSelectMissionPopup);
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.GvGStartBattleFlowPopup);
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.GvGHubPopup);
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.GvGCalendarPopup);
			UITypeOpenOnClose = UIType.MissionHubPopup;
			Close();
		}
	}

	private void OnGuildWarModelChange(TWDGroupModelChild modelObject, string changed, object args)
	{
		if (isInSpectatorMode && changed == "GuildBattleEnded")
		{
			if (SingularityMonoBehaviour<HUDManager>.Instance.HasFullScreenPopup() && GuildWarHelper.CheckEndBattleFullPopup())
			{
				GuildBattleEndPopup guildBattleEndPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildBattleEndPopup) as GuildBattleEndPopup;
				if (guildBattleEndPopup != null && !guildBattleEndPopup.IsOpen)
				{
					guildBattleEndPopup.Open();
				}
			}
			else if (SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GuildBattleEndPopup) == null)
			{
				OpenGvgStarBattlePopup();
			}
		}
		UpdateUI();
	}

	private void OnGuildBattlePlayerChange(ModelObject m, string changed, object arg)
	{
		if (changed == "GuildBattleStarted")
		{
			SetupBattleMapUI(forceUpdate: true);
			if (activeBonusesList != null)
			{
				activeBonusesList.Clear();
			}
		}
		else if (changed == "GuildBattleEnded")
		{
			MapMissionModel = null;
		}
		UpdateUI();
	}

	private void OnGuildBattleModelChange(TWDGroupModelChild modelObject, string changed, object args)
	{
		if (isMapVisible)
		{
			UpdateUI();
		}
	}

	private void OnGuildWarPlayerChange(ModelObject m, string changed, object args)
	{
		if (isMapVisible && changed == "GuildWarStarted")
		{
			UpdateUI();
		}
	}

	private void SetupBonusesUI(bool show)
	{
		if (isBonusUiVisible != show)
		{
			if (show)
			{
				TweenManager.PlayTweenAnchors(base.gameObject);
			}
			else
			{
				TweenManager.PlayTweenAnchors(base.gameObject, forward: false);
			}
			isBonusUiVisible = show;
		}
	}

	private void SetupBattleMapUI(bool forceUpdate = false)
	{
		if (viewInstance != null)
		{
			if (forceUpdate)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.GuildBattleSelectMissionPopup);
				viewInstance.Clear();
			}
			if (viewInstance.IsCleared)
			{
				viewInstance.LoadAndPositionIcons(GuildWarHelper.GetCurrentBattle(), MapMissionModel);
			}
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		Helpers.GameObjectSetActive(battleStatsButton, isMapVisible && (GuildWarHelper.IsPlayerRegisteredForBattle() || GuildWarHelper.IsCurrentOrNextBattleFull()));
		Helpers.GameObjectSetActive(mapTutorialContent, isMapVisible);
		if (isMapVisible)
		{
			Helpers.GameObjectSetActive(genericItemContainer, value: true);
			if (GuildWarHelper.IsBattleOnGoing())
			{
				Helpers.GameObjectSetActive(BattleActiveElementsRoot, value: true);
				SetupBonusesUI(show: true);
				Helpers.GameObjectSetActive(SpectatorModeElementsRoot, !GuildWarHelper.IsPlayerRegisteredForBattle());
				if (activeBonusesList != null)
				{
					activeBonusesList.UpdateActiveBonuses();
				}
				SetupBattleMapUI();
			}
			else
			{
				Helpers.GameObjectSetActive(BattleActiveElementsRoot, value: false);
				Helpers.GameObjectSetActive(SpectatorModeElementsRoot, value: false);
			}
		}
		else if (GuildWarHelper.IsBattleOnGoing())
		{
			SetupBonusesUI(show: false);
			if (IsGvgStartBattleFlowPopupOpen())
			{
				Helpers.GameObjectSetActive(SpectatorModeElementsRoot, !GuildWarHelper.IsPlayerRegisteredForBattle());
				Helpers.GameObjectSetActive(genericItemContainer, value: true);
			}
			else if (IsGuildBattleSelectMissionPopupOpen())
			{
				Helpers.GameObjectSetActive(SpectatorModeElementsRoot, value: false);
				Helpers.GameObjectSetActive(genericItemContainer, value: false);
			}
			else
			{
				Helpers.GameObjectSetActive(SpectatorModeElementsRoot, !GuildWarHelper.IsPlayerRegisteredForBattle());
				Helpers.GameObjectSetActive(genericItemContainer, value: true);
			}
		}
		else
		{
			Helpers.GameObjectSetActive(BattleActiveElementsRoot, value: false);
			Helpers.GameObjectSetActive(SpectatorModeElementsRoot, value: false);
			Helpers.GameObjectSetActive(genericItemContainer, value: true);
		}
	}

	public override void OnClickClose()
	{
		GvGStartBattleFlowPopup gvGStartBattleFlowPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GvGStartBattleFlowPopup) as GvGStartBattleFlowPopup;
		if (gvGStartBattleFlowPopup != null && gvGStartBattleFlowPopup.IsOpen && gvGStartBattleFlowPopup.CanClose())
		{
			gvGStartBattleFlowPopup.Close();
		}
		else
		{
			base.OnClickClose();
		}
	}

	public override void Close()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.GuildBattleSelectMissionPopup);
		UIEvent.OnUIEvent -= UIEventHandler;
		EventManager.OnEvent -= OnEvent;
		HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GvGStartBattleFlowPopup);
		if (noCreation != null && noCreation.IsOpen && !noCreation.IsClosing)
		{
			noCreation.Close();
			noCreation.OnClose += delegate
			{
				CloseInternal();
			};
		}
		else
		{
			CloseInternal();
		}
	}

	private void CloseInternal()
	{
		base.Close();
		initState = GuildBattleMissionButton.InitState.None;
		SetupBonusesUI(show: false);
		if (viewInstance != null)
		{
			viewInstance.Clear();
			Object.Destroy(viewInstance.gameObject);
		}
		isMapVisible = false;
	}

	private void UIEventHandler(string type, object parameter)
	{
		if (type == "OnPopUpOpen")
		{
			if (parameter is GuildBattleSelectMissionPopup)
			{
				isMapVisible = false;
				UpdateUI();
			}
			else if (parameter is GvGStartBattleFlowPopup)
			{
				isMapVisible = false;
				UpdateUI();
			}
		}
		else
		{
			if (!(type == "OnPopUpClose"))
			{
				return;
			}
			if (parameter is GuildBattleSelectMissionPopup)
			{
				isMapVisible = true;
				UpdateUI();
			}
			else if (parameter is GuildBattleEndPopup)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.GuildBattleSelectMissionPopup);
				bool flag = TryOpenGvGHubPopup();
				isMapVisible = !flag;
				UpdateUI();
			}
			else if (parameter is GvGStartBattleFlowPopup)
			{
				isMapVisible = !IsGuildBattleSelectMissionPopupOpen() && !IsGvgHubPopupOpen();
				UpdateUI();
			}
			else if (parameter is GvGHubPopup)
			{
				if (!GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.HasSeenBattleStart())
				{
					bool flag2 = TryToOpenGvgStartBattlePopup();
					isMapVisible = !flag2;
				}
				else
				{
					isMapVisible = true;
				}
				UpdateUI();
			}
		}
	}

	private void OnEvent(EventManager.EventType eventType, object parameter)
	{
		if (eventType == EventManager.EventType.GroupModelLoaded)
		{
			SubscribeToEvents();
			if (viewInstance != null)
			{
				viewInstance.UpdateDataReference(GuildWarHelper.GetCurrentBattle(), MapMissionModel);
			}
			UpdateUI();
		}
	}

	private bool IsGuildBattleSelectMissionPopupOpen()
	{
		HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GuildBattleSelectMissionPopup);
		if (noCreation != null && noCreation.IsOpen)
		{
			return !noCreation.IsClosing;
		}
		return false;
	}

	private bool IsGvgStartBattleFlowPopupOpen()
	{
		HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GvGStartBattleFlowPopup, Helpers.GetUIParent(this.gameObject, true));
		if (noCreation != null && noCreation.IsOpen)
		{
			return !noCreation.IsClosing;
		}
		return false;
	}

	private bool IsGvgHubPopupOpen()
	{
		HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GvGHubPopup);
		if (noCreation != null && noCreation.IsOpen)
		{
			return !noCreation.IsClosing;
		}
		return false;
	}
}
