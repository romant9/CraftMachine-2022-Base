using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class WorldBossOverviewPopup : HUDElement
{
	public enum OpenReason
	{
		Info = 0,
		LockedByCouncilLevel = 1,
		NotInGuild = 2
	}

	[SerializeField]
	private GameObject guildContainer;

	[SerializeField]
	private GameObject guildBG;

	[SerializeField]
	private GameObject guildLeft;

	[SerializeField]
	private UILabel guildTitleLabel;

	[SerializeField]
	private UILabel guildDesc;

	[SerializeField]
	private GameObject lockedContainer;

	[SerializeField]
	private GameObject lockedBG;

	[SerializeField]
	private GameObject lockedLeft;

	[SerializeField]
	private UILabel lockedTitleLabel;

	[SerializeField]
	private UILabel lockedDesc;

	[SerializeField]
	private GameObject infoContainer;

	[SerializeField]
	private GameObject infoBG;

	[SerializeField]
	private UILabel infoTimeLeft;

	[SerializeField]
	private UILabel infoTitleLabel;

	[SerializeField]
	private UILabel infoDesc;

	[SerializeField]
	private UILabel infoNumLabel;

	[SerializeField]
	private UISprite infoDIffIcon;

	[SerializeField]
	private UILabel infoBlueNameLabel;

	[SerializeField]
	private UILabel infoRedNameLabel;

	private WorldBossGuildFullSnapshot worldBossGuildFullSnapshot;

	private WorldBossModelManager worldBossModelManager;

	protected string timeLabelLocalisation = "World.Boss.Countdown";

	protected long gameModeTimeLeft;

	private const long delayOnCompleteMillisec = -1000L;

	public OpenReason Reason { get; private set; }

	public static void OpenPopup(OpenReason reason = OpenReason.Info)
	{
		WorldBossOverviewPopup worldBossOverviewPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossOverviewPopup) as WorldBossOverviewPopup;
		if (!(worldBossOverviewPopup == null))
		{
			worldBossOverviewPopup.Reason = reason;
			if (reason == OpenReason.Info)
			{
				worldBossOverviewPopup.GetWorldBossFullSnapshot();
			}
			else
			{
				worldBossOverviewPopup.Open();
			}
		}
	}

	private void GetWorldBossFullSnapshot()
	{
		if (GameManager.Instance.playerModel == null || GameManager.Instance.playerModel.WorldBossModelManager == null)
		{
			Open();
			return;
		}
		worldBossModelManager = GameManager.Instance.playerModel.WorldBossModelManager;
		WorldBossGetSnapshotRequest value = ((!worldBossModelManager.IsOffSeason()) ? new WorldBossGetSnapshotRequest
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
		string arg = GameManager.Instance.jsonSerializer.Serialize(value);
		SignalRClient.Instance.RequestCommand("WorldBossFullSnapshot", arg, OnWorldBossFullSnapshotAsync, waitForResponse: true);
	}

	private void OnWorldBossFullSnapshotAsync(string responseJson)
	{
		if (!string.IsNullOrEmpty(responseJson))
		{
			worldBossGuildFullSnapshot = GameManager.Instance.jsonSerializer.Deserialize<WorldBossGuildFullSnapshot>(responseJson);
			if (worldBossGuildFullSnapshot != null)
			{
				GameManager.Instance.modelManager.SetWorldBossGuildFullSnapshot(worldBossGuildFullSnapshot);
			}
		}
		Open();
	}

	public override void Open()
	{
		base.Open();
		UpdateUI();
	}

	public void ClickGuild()
	{
		CampHUD.OpenGuildOrChallenge(UIType.SocialPopupGuild);
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MissionHubPopup) as MissionHubPopup).Close();
		Close();
	}

	public void ClickGo()
	{
		Close();
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossMainPopup).Open();
	}

	public override void Update()
	{
		base.Update();
		if (gameModeTimeLeft > -1000)
		{
			gameModeTimeLeft -= (long)(Time.deltaTime * 1000f);
			if (infoTimeLeft != null)
			{
				HelpersUI.SetContentToLabel(infoTimeLeft, LocalizationManager.GetText(timeLabelLocalisation, FormatTimeLeft(gameModeTimeLeft)));
			}
		}
		if (gameModeTimeLeft <= -1000)
		{
			gameModeTimeLeft = -1001L;
		}
	}

	private static string FormatTimeLeft(long timeLeft)
	{
		if (timeLeft <= 0)
		{
			return "0";
		}
		return Helpers.FormatTimeNoZero(timeLeft);
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		switch (Reason)
		{
		case OpenReason.Info:
		{
			Helpers.GameObjectSetActive(infoContainer, value: true);
			Helpers.GameObjectSetActive(lockedContainer, value: false);
			Helpers.GameObjectSetActive(guildContainer, value: false);
			if (worldBossGuildFullSnapshot?.GuildFullState == null || worldBossModelManager == null)
			{
				break;
			}
			int difficulty = worldBossGuildFullSnapshot.GuildFullState.Difficulty;
			WorldBossDifficultyDefinition worldBossDifficultyDefinition = GameManager.Instance.gameEconomyData.FindWorldBossDifficultyDefinition(worldBossModelManager.GetCurrentSeasonId(), difficulty);
			if (worldBossDifficultyDefinition != null)
			{
				if (infoDIffIcon != null)
				{
					infoDIffIcon.spriteName = "UI_WB_Diffic_" + worldBossDifficultyDefinition.DifficultyClass;
				}
				if (!string.IsNullOrEmpty(worldBossDifficultyDefinition.Localization))
				{
					HelpersUI.SetContentToLabel(infoNumLabel, LocalizationManager.GetText(worldBossDifficultyDefinition.Localization));
				}
				if (worldBossModelManager.IsOffSeason())
				{
					timeLabelLocalisation = "World.Boss.NextCycle.Countdown";
					gameModeTimeLeft = worldBossModelManager.GetTimeUntilNextCycleStartMs();
				}
				else
				{
					timeLabelLocalisation = "World.Boss.Countdown";
					gameModeTimeLeft = worldBossModelManager.GetTimeUntilCycleEndMs();
				}
			}
			if (infoBlueNameLabel != null)
			{
				HelpersUI.SetContentToLabel(infoBlueNameLabel, worldBossModelManager.WorldBossGuildFullSnapshot?.Match?.GroupNameA);
			}
			if (infoRedNameLabel != null)
			{
				HelpersUI.SetContentToLabel(infoRedNameLabel, worldBossModelManager.WorldBossGuildFullSnapshot?.Match?.GroupNameB);
			}
			break;
		}
		case OpenReason.LockedByCouncilLevel:
		{
			Helpers.GameObjectSetActive(infoContainer, value: false);
			Helpers.GameObjectSetActive(lockedContainer, value: true);
			Helpers.GameObjectSetActive(guildContainer, value: false);
			int num = (GameManager.Instance?.gameEconomyData?.GetSystemOpenById("SystemBase.WorldBoss"))?.OpenCampLv ?? 14;
			HelpersUI.SetContentToLabel(lockedDesc, LocalizationManager.GetText("Popup.FeatureLocked.CouncilLevelNeeded{Level}", num));
			break;
		}
		case OpenReason.NotInGuild:
			Helpers.GameObjectSetActive(infoContainer, value: false);
			Helpers.GameObjectSetActive(lockedContainer, value: false);
			Helpers.GameObjectSetActive(guildContainer, value: true);
			break;
		}
	}
}
