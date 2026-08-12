using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class MatchmakingPopup : HUDElement
{
	public enum Tabs
	{
		Start = 0,
		Loading = 1,
		Success = 2,
		Error = 3
	}

	public Tabs CurrentTab;

	[SerializeField]
	private GameObject closeArea;

	[SerializeField]
	private UIToggleMenu tabsMenu;

	private int currentMatchIndex = -1;

	private List<MatchMakingInfo> matches;

	private List<string> excludeHashedIds = new List<string>();

	private MatchInfo CurrentMatchInfo;

	private string CurrentMatchInfonNickname;

	private int CurrentMatchInfoTotalInfluence;

	private string CurrentMatchInfoOutpostTierId = "";

	private string CurrentPlayerHashedId;

	private MatchmakingPopupState state;

	private bool attacking;

	public MatchmakingPopupState State
	{
		get
		{
			return state;
		}
		set
		{
			if (state != value)
			{
				state = value;
			}
		}
	}

	private bool TempLockSucceeded { get; set; }

	private string LastLockedPlayerHashedId { get; set; }

	public override void Start()
	{
		State = MatchmakingPopupState.None;
		QueryMatchMaking();
		SetUITypeOpenOnClose(UIType.OutpostPopup);
		base.Start();
	}

	public void OnEnable()
	{
		ClearLockInfo();
		attacking = false;
	}

	public void OnDisable()
	{
		if (!attacking)
		{
			TryTempUnlockPlayer();
		}
	}

	public override void Close()
	{
		base.Close();
		attacking = false;
	}

	private void SetState(MatchmakingPopupState newState)
	{
		State = newState;
	}

	public void QueryMatchMaking()
	{
		if (SignalRClient.Instance == null)
		{
			Debug.LogError("No connection, aborted.");
			return;
		}
		SetState(MatchmakingPopupState.WaitingMatchMaking);
		CurrentPlayerHashedId = null;
		CurrentMatchInfo = null;
		CurrentMatchInfonNickname = "";
		currentMatchIndex = -1;
		CurrentMatchInfoTotalInfluence = 0;
		OpenTab(Tabs.Loading);
		Helpers.ExecuteCommand(new SetExcludeMatchMakingTargetsCommand(excludeHashedIds));
		GetMatchParams getMatchParams = new GetMatchParams();
		getMatchParams.Count = 20;
		getMatchParams.Parameters = "";
		getMatchParams.Version = GameManager.Instance.gameEconomyData.ConfigData.MatchMakingVersion;
		if (GameManager.Instance.IsConnectedToServer)
		{
			SignalRClient.Instance.RequestCommand("GetMatch", GameManager.Instance.jsonSerializer.Serialize(getMatchParams), OnMatchDataLoaded, waitForResponse: true);
		}
		else
		{
			Debug.LogError("Not connected to server - matchmaking not possible");
		}
	}

	private void OnMatchDataLoaded(string response)
	{
		if (string.IsNullOrEmpty(response))
		{
			AnalyticsManager.instance.CreateEvent("MatchMaking_Error_EmptyResponse").Send();
			DebugLoadingDone("Empty response received to Request: GetMatch", Tabs.Error);
			return;
		}
		currentMatchIndex = 0;
		matches = GameManager.Instance.jsonSerializer.Deserialize<List<MatchMakingInfo>>(response);
		GameManager.Instance.modelManager.SetMatchData("", matches);
		matches = GameManager.Instance.modelManager.LastMatchMakingInfos;
		if (matches != null && matches.Count > 0)
		{
			for (int num = matches.Count - 1; num >= 0; num--)
			{
				MatchMakingInfo matchMakingInfo = matches[num];
				if (matchMakingInfo == null || matchMakingInfo.PlayerInformation == null || matchMakingInfo.PlayerInformation.Length == 0 || matchMakingInfo.PlayerHashedId == null || matchMakingInfo.PlayerHashedId.Length == 0)
				{
					matches.RemoveAt(num);
				}
			}
		}
		if (matches == null || matches.Count == 0 || matches[0].PlayerHashedId == "")
		{
			AnalyticsManager.instance.CreateEvent("MatchMaking_Error_NoMatches").Send();
			DebugLoadingDone("Empty PlayerModel List received to Request: GetMatch", Tabs.Error);
		}
		else
		{
			AnalyticsManager.instance.CreateEvent("MatchMaking_MatchesFound").AddProperty("MatchCount", matches.Count).Send();
		}
		State = MatchmakingPopupState.MatchMakingDone;
		int playerLevel = GameManager.Instance.playerModel.Level;
		int playerInfluence = GameManager.Instance.playerModel.RankingScore;
		FixedPoint influenceWeight = GameManager.Instance.gameEconomyData.ConfigData.InfluenceWeightOnMatchMakingSort;
		if (GameManager.Instance.gameEconomyData.ConfigData.SkipOutpostMatchPreview)
		{
			currentMatchIndex = Random.Range(0, matches.Count);
			return;
		}
		matches.Sort(delegate(MatchMakingInfo a, MatchMakingInfo b)
		{
			int num2 = Mathf.Abs(a.Rating - playerLevel);
			int num3 = Mathf.Abs(a.SecondaryRating - playerInfluence);
			int num4 = Mathf.Abs(b.Rating - GameManager.Instance.playerModel.Level);
			int num5 = Mathf.Abs(b.SecondaryRating - playerInfluence);
			int num6 = (int)(num2 + num3 * influenceWeight);
			int num7 = (int)(num4 + num5 * influenceWeight);
			return num6 - num7;
		});
	}

	public override void Update()
	{
		base.Update();
		switch (State)
		{
		case MatchmakingPopupState.MatchMakingDone:
			if (!TryTempLockPlayer())
			{
				State = MatchmakingPopupState.NoMatches;
				DebugLoadingDone("No More Matches Left", Tabs.Error);
			}
			break;
		case MatchmakingPopupState.LockPlayerDone:
			if (TempLockSucceeded)
			{
				RequestPreview();
			}
			else if (TryNextMatch())
			{
				if (!TryTempLockPlayer())
				{
					State = MatchmakingPopupState.NoMatches;
					DebugLoadingDone("No More Matches Left", Tabs.Error);
				}
			}
			else
			{
				State = MatchmakingPopupState.NoMatches;
				DebugLoadingDone("No More Matches Left", Tabs.Error);
			}
			break;
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
	}

	private void OnGUI()
	{
	}

	public void AttackCurrentMatch()
	{
		if (CurrentPlayerHashedId != null && State == MatchmakingPopupState.Preview)
		{
			SetUITypeOpenOnClose(UIType.None);
			OutpostEditManager.OutpostAttackData = new MapMissionParameters
			{
				MissionId = CurrentPlayerHashedId,
				IsPvP = true
			};
			TeamSelectionPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MapTeamSelection) as TeamSelectionPopup;
			obj.SurvivorType = SurvivorContainerModel.SurvivorType.CombatOutpost;
			obj.OutpostMatchInfo = GetCurrentMatchInfo();
			obj.OutpostDefenderHashedId = CurrentPlayerHashedId;
			obj.OutpostDefenderName = GetCurrentMatchInfoSurvivorName();
			obj.SetUITypeOpenOnClose(UIType.OutpostPopup);
			obj.Open();
			attacking = true;
			Close();
		}
	}

	public void OnNextMatch()
	{
		if (matches != null && currentMatchIndex >= 0 && currentMatchIndex < matches.Count)
		{
			ConsumeCurrencyCommandUtils.Execute(new OutpostNextMatchCommand
			{
				Cashier = GameManager.Instance.playerModel.OutpostModel.GetNextMatchCashier()
			}, OnNextMatchCallback);
		}
	}

	private void OnNextMatchCallback(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			if (TryNextMatch())
			{
				TryTempLockPlayer();
			}
			else
			{
				QueryMatchMaking();
			}
		}
	}

	public override void OnBackButtonClicked()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.StopEvent("global/match_search");
		base.OnBackButtonClicked();
	}

	public MatchInfo GetCurrentMatchInfo()
	{
		return CurrentMatchInfo;
	}

	public string GetCurrentMatchInfoSurvivorName()
	{
		return CurrentMatchInfonNickname;
	}

	public int GetOpponentTotalInfluence()
	{
		return CurrentMatchInfoTotalInfluence;
	}

	public string GetOpponentOutpostTierId()
	{
		return CurrentMatchInfoOutpostTierId;
	}

	public string GetCurrentMatchPlayerHashedId()
	{
		return CurrentPlayerHashedId;
	}

	private void RequestPreview()
	{
		MatchInfo matchInfo = null;
		bool skipOutpostMatchPreview = GameManager.Instance.gameEconomyData.ConfigData.SkipOutpostMatchPreview;
		if (matches.Count > 0 && matches.Count > currentMatchIndex && currentMatchIndex >= 0)
		{
			MatchMakingInfo matchMakingInfo = matches[currentMatchIndex];
			matchInfo = MatchInfo.CreateMatchInfo(GameManager.Instance.jsonSerializer, matchMakingInfo.PlayerInformation);
			if (matchInfo != null)
			{
				matchInfo.IsFake = matchMakingInfo.Availability > GameManager.Instance.playerModel.UtcTimeStamp / 1000;
				CurrentPlayerHashedId = matchMakingInfo.PlayerHashedId;
				CurrentMatchInfo = matchInfo;
				bool flag = GameManager.Instance.gameEconomyData.ConfigData.OutpostMatchMakingFakeNamesEnabled && matchInfo.IsFake;
				CurrentMatchInfonNickname = (flag ? FakeNameGenerator.GetFakeName(GameManager.Instance.modelManager, matchMakingInfo.PlayerHashedId) : matchMakingInfo.Nickname);
				CurrentMatchInfoTotalInfluence = matchInfo.RankingScore;
				CurrentMatchInfoOutpostTierId = matchInfo.OutpostTierId;
				if (skipOutpostMatchPreview)
				{
					State = MatchmakingPopupState.Preview;
					SingularityMonoBehaviour<AudioManager>.Instance.StopEvent("global/match_search");
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/match_found");
					AttackCurrentMatch();
				}
				else
				{
					OpenTab(Tabs.Success);
					State = MatchmakingPopupState.Preview;
				}
			}
		}
		else
		{
			DebugLoadingDone("No More Matches Left", Tabs.Error);
		}
	}

	private void ClearLockInfo()
	{
		TempLockSucceeded = false;
		LastLockedPlayerHashedId = null;
	}

	private bool TryTempLockPlayer()
	{
		if (State != MatchmakingPopupState.WaitingLockPlayer)
		{
			TempLockSucceeded = false;
			MatchMakingInfo matchMakingInfo = ((currentMatchIndex >= 0 && currentMatchIndex < matches.Count) ? matches[currentMatchIndex] : null);
			if (matchMakingInfo != null && !string.IsNullOrEmpty(matchMakingInfo.PlayerInformation))
			{
				State = MatchmakingPopupState.WaitingLockPlayer;
				LastLockedPlayerHashedId = matchMakingInfo.PlayerHashedId;
				SignalRClient.Instance.RequestCommand("LockPlayer", matchMakingInfo.PlayerHashedId, OnTempLockCompleted, waitForResponse: true);
				return true;
			}
		}
		return false;
	}

	private bool TryTempUnlockPlayer()
	{
		if (LastLockedPlayerHashedId != null)
		{
			if (GameManager.Instance.IsConnectedToServer)
			{
				SignalRClient.Instance.RequestCommand("UnLockPlayer", LastLockedPlayerHashedId, OnTempUnlockCompleted, waitForResponse: true);
			}
			return true;
		}
		return false;
	}

	private void OnTempUnlockCompleted(string message)
	{
		LastLockedPlayerHashedId = null;
	}

	private void OnTempLockCompleted(string message)
	{
		LockRespond lockRespond = GameManager.Instance.jsonSerializer.DeserializeObject<LockRespond>(message);
		if (lockRespond != null && lockRespond.Status == LockRespond.LockStatus.Locked)
		{
			TempLockSucceeded = true;
		}
		else
		{
			LastLockedPlayerHashedId = null;
		}
		State = MatchmakingPopupState.LockPlayerDone;
	}

	private bool TryNextMatch()
	{
		OpenTab(Tabs.Loading);
		CurrentPlayerHashedId = null;
		TryTempUnlockPlayer();
		currentMatchIndex++;
		if (matches != null)
		{
			return currentMatchIndex < matches.Count;
		}
		return false;
	}

	private void DebugLoadingDone(string debugText, Tabs tab)
	{
		UIToggleContent uIToggleContent = OpenTab(tab);
		if (uIToggleContent != null && uIToggleContent.gameObject.GetComponent<MatchmakingTabBase>() != null)
		{
			uIToggleContent.gameObject.GetComponent<MatchmakingTabBase>().SetDebugText(debugText);
		}
	}

	private UIToggleContent OpenTab(Tabs tab)
	{
		if (tabsMenu != null)
		{
			UIToggleContent contentByIndex = tabsMenu.GetContentByIndex((int)tab);
			if (contentByIndex != null && contentByIndex.gameObject.GetComponent<MatchmakingTabBase>() != null)
			{
				contentByIndex.gameObject.GetComponent<MatchmakingTabBase>().ParentPopup = this;
			}
			tabsMenu.OpenContentByIndex((int)tab);
			CurrentTab = tab;
			switch (tab)
			{
			case Tabs.Success:
				SingularityMonoBehaviour<AudioManager>.Instance.StopEvent("global/match_search");
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/match_found");
				break;
			case Tabs.Error:
				SingularityMonoBehaviour<AudioManager>.Instance.StopEvent("global/match_search");
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/match_fail");
				break;
			case Tabs.Loading:
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/match_search");
				break;
			}
			return contentByIndex;
		}
		return null;
	}
}
