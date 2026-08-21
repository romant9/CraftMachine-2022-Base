using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class WorldBossOffSeasonPopup : HUDElement
{
	[SerializeField]
	private GameObject loadingContainer;

	[SerializeField]
	private GameObject confirmContainer;

	[SerializeField]
	private UISprite difficultySprite;

	[SerializeField]
	private UILabel timeTxt;

	[SerializeField]
	private UILabel difficultyTxt;

	[SerializeField]
	private UILabel levelTxt;

	[SerializeField]
	private UILabel resourceTxt;

	[SerializeField]
	private UILabel guranteeTxt;

	private WorldBossModelManager worldBossModelManager;

	private WorldBossGuildFullSnapshot worldBossGuildFullSnapshot;

	private const long delayOnCompleteMillisec = -1000L;

	protected long gameModeTimeLeft = -1000L;

	protected string timeLabelLocalisation = "";

	public MissionHubPanelWorldBoss.WorldBossState State { get; private set; }

	public static void OpenPopup(MissionHubPanelWorldBoss.WorldBossState state)
	{
		WorldBossOffSeasonPopup worldBossOffSeasonPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossOffSeasonPopup) as WorldBossOffSeasonPopup;
		if (!(worldBossOffSeasonPopup == null))
		{
			worldBossOffSeasonPopup.State = state;
			worldBossOffSeasonPopup.Open();
		}
	}

	public void Awake()
	{
		if (SignalRClient.Instance != null)
		{
			SignalRClient.Instance.OnWorldBossFullSnapshotMessage -= OnWorldBossFullSnapshotChanged;
			SignalRClient.Instance.OnWorldBossFullSnapshotMessage += OnWorldBossFullSnapshotChanged;
		}
	}

	private void OnDestroy()
	{
		if (SignalRClient.Instance != null)
		{
			SignalRClient.Instance.OnWorldBossFullSnapshotMessage -= OnWorldBossFullSnapshotChanged;
		}
	}

	public override void Open()
	{
		base.Open();
		Helpers.GameObjectSetActive(loadingContainer, value: false);
		UpdateUI();
	}

	private void OnWorldBossFullSnapshotChanged(string message, string type)
	{
		if (IsPopupAlive())
		{
			GetWorldBossFullSnapshot();
		}
	}

	private bool IsPopupAlive()
	{
		if (this != null && base.gameObject != null)
		{
			return base.IsOpen;
		}
		return false;
	}

	public void OnWorldBossFullSnapshotAsync(string responseJson)
	{
		Helpers.GameObjectSetActive(loadingContainer, value: false);
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(responseJson))
		{
			SignalRClient.Instance.ClearError();
			return;
		}
		worldBossGuildFullSnapshot = GameManager.Instance.jsonSerializer.Deserialize<WorldBossGuildFullSnapshot>(responseJson);
		if (worldBossGuildFullSnapshot != null)
		{
			GameManager.Instance.modelManager.SetWorldBossGuildFullSnapshot(worldBossGuildFullSnapshot);
			if (IsPopupAlive())
			{
				UpdateWorldBossFullSnapshot();
			}
		}
	}

	public void UpdateWorldBossFullSnapshot()
	{
		if (worldBossGuildFullSnapshot == null)
		{
			return;
		}
		int difficulty = worldBossGuildFullSnapshot.GuildFullState.Difficulty;
		WorldBossDifficultyDefinition worldBossDifficultyDefinition = GameManager.Instance.gameEconomyData.FindWorldBossDifficultyDefinition(worldBossModelManager?.GetCurrentSeasonId() ?? 0, difficulty);
		if (difficultySprite != null && worldBossDifficultyDefinition != null)
		{
			difficultySprite.spriteName = "UI_WB_Diffic_" + worldBossDifficultyDefinition.DifficultyClass;
		}
		if (worldBossDifficultyDefinition == null)
		{
			return;
		}
		WorldBossBattlegroundDefinition[] array = GameManager.Instance.gameEconomyData.FindWorldBossBattlegroundDefinitionsByDifficulty(difficulty);
		if (array != null && array.Length != 0)
		{
			WorldBossBattlegroundDefinition worldBossBattlegroundDefinition = array[0];
			if (levelTxt != null)
			{
				HelpersUI.SetContentToLabel(levelTxt, "lv" + worldBossBattlegroundDefinition.EnemyLevel);
			}
		}
		if (difficultyTxt != null)
		{
			string text = LocalizationManager.GetText(worldBossDifficultyDefinition.Localization);
			HelpersUI.SetContentToLabel(difficultyTxt, text);
		}
		if (resourceTxt != null)
		{
			HelpersUI.SetContentToLabel(resourceTxt, worldBossDifficultyDefinition.VSReward.ToString());
		}
		if (guranteeTxt != null)
		{
			HelpersUI.SetContentToLabel(guranteeTxt, worldBossDifficultyDefinition.Guarantee.ToString());
		}
	}

	public void SetContentToTimerLabel(string value)
	{
		HelpersUI.SetContentToLabel(timeTxt, value);
	}

	public static string FormatTimeLeft(long timeLeft)
	{
		int num = (int)((timeLeft > 0) ? ((timeLeft + 999) / 1000) : 0);
		int num2 = num / 86400;
		int num3 = num - num2 * 24 * 60 * 60;
		int num4 = num3 / 3600;
		int num5 = num3 - num4 * 60 * 60;
		int num6 = num5 / 60;
		int num7 = num5 - num6 * 60;
		return string.Format("{0}{1} {2}{3} {4}{5} {6}{7}", num2, LocalizationManager.GetText("Generic.Time.DaySmall"), num4, LocalizationManager.GetText("Generic.Time.HourSmall"), num6, LocalizationManager.GetText("Generic.Time.MinuteSmall"), num7, LocalizationManager.GetText("Generic.Time.SecondSmall"));
	}

	public override void Update()
	{
		base.Update();
		if (ClosePreBattlePopupsIfCycleStarted())
		{
			return;
		}
		if (gameModeTimeLeft > -1000)
		{
			gameModeTimeLeft -= (long)(Time.deltaTime * 1000f);
			if (timeTxt != null)
			{
				SetContentToTimerLabel(timeLabelLocalisation + FormatTimeLeft(gameModeTimeLeft));
			}
			if (gameModeTimeLeft <= -1000)
			{
				gameModeTimeLeft = -1001L;
			}
		}
		Helpers.GameObjectSetActive(confirmContainer, CanOpenWorldBossDifficultyPopup());
	}

	public void GetWorldBossFullSnapshot()
	{
		if (GameManager.Instance.playerModel != null && worldBossModelManager != null)
		{
			WorldBossGetSnapshotRequest worldBossGetSnapshotRequest = null;
			worldBossGetSnapshotRequest = ((!worldBossModelManager.IsOffSeason()) ? new WorldBossGetSnapshotRequest
			{
				GroupId = GameManager.Instance.playerModel.GuildId,
				SeasonId = worldBossModelManager.GetCurrentSeasonId(),
				CycleId = worldBossModelManager.GetCurrentCycleId()
			} : new WorldBossGetSnapshotRequest
			{
				GroupId = GameManager.Instance.playerModel.GuildId,
				SeasonId = worldBossModelManager.GetCurrentSeasonId(),
				CycleId = worldBossModelManager.GetNextCycleId()
			});
			string arg = GameManager.Instance.jsonSerializer.Serialize(worldBossGetSnapshotRequest);
			SignalRClient.Instance.RequestCommand("WorldBossFullSnapshot", arg, OnWorldBossFullSnapshotAsync, waitForResponse: true);
		}
	}

	public void ClickSelect()
	{
		if (!CanOpenWorldBossDifficultyPopup())
		{
			Helpers.GameObjectSetActive(confirmContainer, value: false);
			return;
		}
		Close();
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossDifficultyPopup).Open();
	}

	public void ClickShop()
	{
		HUDNotification.Info(LocalizationManager.GetText("World.Boss.ComingSoon"));
	}

	public void ClickLeaderboard()
	{
		HUDNotification.Info(LocalizationManager.GetText("World.Boss.ComingSoon"));
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		worldBossModelManager = GameManager.Instance.playerModel.WorldBossModelManager;
		Helpers.GameObjectSetActive(confirmContainer, CanOpenWorldBossDifficultyPopup());
		gameModeTimeLeft = worldBossModelManager.GetTimeUntilNextCycleStartMs();
		SetContentToTimerLabel(FormatTimeLeft(gameModeTimeLeft));
		GetWorldBossFullSnapshot();
		MissionHubPanelWorldBoss.WorldBossState state = State;
		_ = state - 3;
		_ = 2;
	}

	private static bool CanOpenWorldBossDifficultyPopup()
	{
		PlayerModel playerModel = GameManager.Instance?.playerModel;
		if (playerModel != null && playerModel.IsGuildMember)
		{
			return playerModel.WorldBossModelManager != null;
		}
		return false;
	}

	public static bool ClosePreBattlePopupsIfCycleStarted()
	{
		WorldBossModelManager worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		if (worldBossModelManager == null || !worldBossModelManager.IsCycleOpen())
		{
			return false;
		}
		CloseIfOpen(UIType.WorldBossDifficultyPopup);
		CloseIfOpen(UIType.WorldBossOffSeasonPopup);
		return true;
	}

	private static void CloseIfOpen(UIType uiType)
	{
		HUDElement hUDElement = ((SingularityMonoBehaviour<HUDManager>.Instance != null) ? SingularityMonoBehaviour<HUDManager>.Instance.Get(uiType, null, createIfNotExist: false) : null);
		if (hUDElement != null && hUDElement.IsOpen)
		{
			hUDElement.Close();
		}
	}
}
