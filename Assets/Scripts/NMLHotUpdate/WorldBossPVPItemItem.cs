using System;
using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class WorldBossPVPItemItem : MonoBehaviour
{
	public enum CellState
	{
		Empty = 0,
		Uncross = 1,
		Fight = 2,
		FightHero = 3,
		GetOtherGroup = 4,
		GetMyGroup = 5,
		GetMyGroupMySelf = 6
	}

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private GameObject emptyGM;

	[SerializeField]
	private UILabel emptyLabel;

	[SerializeField]
	private GameObject uncrossGM;

	[SerializeField]
	private GameObject fightGM;

	[SerializeField]
	private GameObject fightHeroGM;

	[SerializeField]
	private GameObject fightHeroAndGetCommonGM;

	[SerializeField]
	private GameObject getOtherGroupGM;

	[SerializeField]
	private GameObject getMyGroupGM;

	[SerializeField]
	private GameObject getMyGroupMySelfGM;

	[SerializeField]
	private UISprite bgBottomSprite;

	[SerializeField]
	private UISprite bgTopSprite;

	[SerializeField]
	private UILabel timeLabel;

	[SerializeField]
	private GameObject Block;

	[SerializeField]
	private GameObject BlockUp;

	[SerializeField]
	private PlayerEmblemIcon playerEmblemIcon;

	private const int BlockNum = 10;

	private const string BlockNamePre = "Icon";

	private const string HeroIconContainerName = "Icon_Bg";

	private static readonly Color DurabilityColorGreen = Helpers.HexToColor("#a0c92f");

	private static readonly Color DurabilityColorYellow = Helpers.HexToColor("#f7c225");

	private static readonly Color DurabilityColorRed = Helpers.HexToColor("#fd3535");

	private static readonly Color DefaultTitleColor = new Color(0.2627451f, 0.2627451f, 0.2627451f, 1f);

	private static readonly Color DefaultBgBottomColor = new Color(0.20588237f, 0.19953468f, 0.19377165f, 1f);

	private static readonly Color DefaultBgTopColor = new Color(47f / 85f, 47f / 85f, 47f / 85f, 1f);

	private WorldBossPVPCellSlotData data;

	private long fightTimeRemainingMs;

	private bool isFightCountdownActive;

	private Color blueColor = Helpers.HexToColor("#354462");

	private Color redColor = Helpers.HexToColor("#5F302E");

	private Color blueTitleColor = Helpers.HexToColor("#4964AF");

	private Color redTitleColor = Helpers.HexToColor("#882F28");

	private Color blueEmblemColor = Helpers.HexToColor("#4D6491");

	private Color redEmblemColor = Helpers.HexToColor("#854A43");

	private CellState cellStateNow;

	public void SetData(WorldBossPVPCellSlotData slotData)
	{
		if (slotData != null && slotData.HasValue)
		{
			data = slotData;
			UpdateUI();
		}
	}

	public void OpneRetreatPopup()
	{
		WorldBossRetreatPopup worldBossRetreatPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossRetreatPopup) as WorldBossRetreatPopup;
		if (!(worldBossRetreatPopup == null))
		{
			worldBossRetreatPopup.Open();
		}
	}

	public void OpneRetreatConfirmPopup()
	{
		WorldBossRetreatConfirmPopup worldBossRetreatConfirmPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossRetreatConfirmPopup) as WorldBossRetreatConfirmPopup;
		if (!(worldBossRetreatConfirmPopup == null))
		{
			WorldBossDispatchedTeamView worldBossDispatchedTeamView = FindDispatchedTeamForCurrentCell();
			if (worldBossDispatchedTeamView != null)
			{
				worldBossRetreatConfirmPopup.SetTeam(worldBossDispatchedTeamView);
				worldBossRetreatConfirmPopup.Open();
			}
		}
	}

	private WorldBossDispatchedTeamView FindDispatchedTeamForCurrentCell()
	{
		WorldBossModelManager worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		if (worldBossModelManager == null || data?.CellDefinition == null)
		{
			return null;
		}
		List<string> occupyingSurvivorIds = data.CellStateSnapshot?.OccupyingSurvivorIds;
		List<WorldBossDispatchedTeamView> myDispatchedTeams = worldBossModelManager.GetMyDispatchedTeams();
		if (myDispatchedTeams == null || myDispatchedTeams.Count == 0)
		{
			return null;
		}
		string capturePoint = data.CellDefinition.CapturePoint;
		string cell = data.CellDefinition.Cell;
		for (int i = 0; i < myDispatchedTeams.Count; i++)
		{
			WorldBossDispatchedTeamView worldBossDispatchedTeamView = myDispatchedTeams[i];
			if (worldBossDispatchedTeamView != null)
			{
				if (!string.IsNullOrEmpty(capturePoint) && !string.IsNullOrEmpty(cell) && worldBossDispatchedTeamView.CapturePoint == capturePoint && worldBossDispatchedTeamView.Cell == cell)
				{
					return worldBossDispatchedTeamView;
				}
				if (IsSameSurvivorTeam(worldBossDispatchedTeamView.SurvivorIds, occupyingSurvivorIds))
				{
					return worldBossDispatchedTeamView;
				}
			}
		}
		return null;
	}

	private static bool IsSameSurvivorTeam(List<string> teamSurvivorIds, List<string> occupyingSurvivorIds)
	{
		if (teamSurvivorIds == null || occupyingSurvivorIds == null || teamSurvivorIds.Count == 0 || occupyingSurvivorIds.Count == 0 || teamSurvivorIds.Count != occupyingSurvivorIds.Count)
		{
			return false;
		}
		for (int i = 0; i < occupyingSurvivorIds.Count; i++)
		{
			string text = occupyingSurvivorIds[i];
			if (string.IsNullOrEmpty(text) || !teamSurvivorIds.Contains(text))
			{
				return false;
			}
		}
		return true;
	}

	public void UpdateUI()
	{
		ResetDefaultVisuals();
		Helpers.GameObjectSetActive(emptyGM, value: false);
		Helpers.GameObjectSetActive(uncrossGM, value: false);
		Helpers.GameObjectSetActive(fightGM, value: false);
		Helpers.GameObjectSetActive(fightHeroGM, value: false);
		Helpers.GameObjectSetActive(fightHeroAndGetCommonGM, value: false);
		Helpers.GameObjectSetActive(getOtherGroupGM, value: false);
		Helpers.GameObjectSetActive(getMyGroupGM, value: false);
		Helpers.GameObjectSetActive(getMyGroupMySelfGM, value: false);
		Helpers.GameObjectSetActive(emptyLabel, value: false);
		Helpers.GameObjectSetActive(timeLabel, value: false);
		Helpers.GameObjectSetActive(titleLabel, value: false);
		isFightCountdownActive = false;
		UpdateDurabilityBlocks(GetDurabilityLine());
		if (data == null || !data.HasValue)
		{
			return;
		}
		if (data.CellStateSnapshot == null)
		{
			if (data.CellDefinition.HaveBattle)
			{
				cellStateNow = CellState.Uncross;
				Helpers.GameObjectSetActive(uncrossGM, value: true);
				if (data.CellDefinition.EnemyName != null && data.CellDefinition.EnemyName != "")
				{
					titleLabel.text = LocalizationManager.GetText(data.CellDefinition.EnemyName);
				}
				else
				{
					titleLabel.text = data.CellDefinition.Cell;
				}
				Helpers.GameObjectSetActive(titleLabel, value: true);
			}
			else
			{
				cellStateNow = CellState.Empty;
				Helpers.GameObjectSetActive(emptyGM, value: true);
				emptyLabel.text = LocalizationManager.GetText("World.Boss.NOONEHERE");
				Helpers.GameObjectSetActive(emptyLabel, value: true);
			}
			SetFightTimeLabelVisible(visible: false);
		}
		else if (data.CellStateSnapshot.Status == 0)
		{
			switch (GameManager.Instance.playerModel.WorldBossModelManager.GetCellEnterAction(data.CellDefinition.CapturePoint, data.CellDefinition.Cell))
			{
			case WorldBossCellEnterAction.DirectOccupy:
				cellStateNow = CellState.Empty;
				Helpers.GameObjectSetActive(emptyGM, value: true);
				emptyLabel.text = LocalizationManager.GetText("World.Boss.NOONEHERE");
				Helpers.GameObjectSetActive(emptyLabel, value: true);
				break;
			case WorldBossCellEnterAction.FightPve:
				cellStateNow = CellState.Uncross;
				Helpers.GameObjectSetActive(uncrossGM, value: true);
				if (data.CellDefinition.EnemyName != null && data.CellDefinition.EnemyName != "")
				{
					titleLabel.text = LocalizationManager.GetText(data.CellDefinition.EnemyName);
				}
				else
				{
					titleLabel.text = data.CellDefinition.Cell;
				}
				Helpers.GameObjectSetActive(titleLabel, value: true);
				break;
			}
		}
		else if (data.CellStateSnapshot.Status == 1)
		{
			if (data.CellStateSnapshot.OccupyingPlayerHashedId != null && data.CellStateSnapshot.OccupyingPlayerHashedId != "")
			{
				cellStateNow = CellState.FightHero;
				Helpers.GameObjectSetActive(fightHeroGM, value: true);
				Helpers.GameObjectSetActive(fightHeroAndGetCommonGM, value: true);
				titleLabel.text = GameManager.Instance.playerModel.WorldBossModelManager.GetCellOccupierName(data.CellDefinition.CapturePoint, data.CellDefinition.Cell);
				playerEmblemIcon.SetEmblem(GameManager.Instance.playerModel.WorldBossModelManager.GetCellOccupierEmblem(data.CellDefinition.CapturePoint, data.CellDefinition.Cell));
				string text = data.MyColorFlag;
				if (data.MyColorFlag == "Blue")
				{
					if (data.CellStateSnapshot.OccupyingGroupId != GameManager.Instance.playerModel.GuildId)
					{
						text = "Red";
					}
				}
				else if (data.CellStateSnapshot.OccupyingGroupId != GameManager.Instance.playerModel.GuildId)
				{
					text = "Blue";
				}
				titleLabel.color = ((text == "Blue") ? blueTitleColor : redTitleColor);
				bgBottomSprite.color = ((text == "Blue") ? blueColor : redColor);
				ApplyBgTopGradient((text == "Blue") ? blueEmblemColor : redEmblemColor);
			}
			else
			{
				cellStateNow = CellState.Fight;
				Helpers.GameObjectSetActive(fightGM, value: true);
				if (data.CellDefinition.EnemyName != null && data.CellDefinition.EnemyName != "")
				{
					titleLabel.text = LocalizationManager.GetText(data.CellDefinition.EnemyName);
				}
			}
			Helpers.GameObjectSetActive(titleLabel, value: true);
			RefreshFightCountdown();
		}
		else
		{
			if (data.CellStateSnapshot.Status != 2)
			{
				return;
			}
			Helpers.GameObjectSetActive(fightHeroAndGetCommonGM, value: true);
			titleLabel.text = GameManager.Instance.playerModel.WorldBossModelManager.GetCellOccupierName(data.CellDefinition.CapturePoint, data.CellDefinition.Cell);
			playerEmblemIcon.SetEmblem(GameManager.Instance.playerModel.WorldBossModelManager.GetCellOccupierEmblem(data.CellDefinition.CapturePoint, data.CellDefinition.Cell));
			string text2 = data.MyColorFlag;
			if (data.MyColorFlag == "Blue")
			{
				if (data.CellStateSnapshot.OccupyingGroupId != GameManager.Instance.playerModel.GuildId)
				{
					text2 = "Red";
				}
			}
			else if (data.CellStateSnapshot.OccupyingGroupId != GameManager.Instance.playerModel.GuildId)
			{
				text2 = "Blue";
			}
			titleLabel.color = ((text2 == "Blue") ? blueTitleColor : redTitleColor);
			bgBottomSprite.color = ((text2 == "Blue") ? blueColor : redColor);
			ApplyBgTopGradient((text2 == "Blue") ? blueEmblemColor : redEmblemColor);
			if (text2 == data.MyColorFlag)
			{
				if (data.CellStateSnapshot.OccupyingPlayerHashedId == GameManager.Instance.playerModel.HashedId)
				{
					cellStateNow = CellState.GetMyGroupMySelf;
					Helpers.GameObjectSetActive(getMyGroupMySelfGM, value: true);
					UpdateOccupyingSurvivorTokenIcons(getMyGroupMySelfGM, data.CellStateSnapshot.OccupyingSurvivorIds);
				}
				else
				{
					cellStateNow = CellState.GetMyGroup;
					Helpers.GameObjectSetActive(getMyGroupGM, value: true);
				}
			}
			else
			{
				cellStateNow = CellState.GetOtherGroup;
				Helpers.GameObjectSetActive(getOtherGroupGM, value: true);
			}
			Helpers.GameObjectSetActive(titleLabel, value: true);
			SetFightTimeLabelVisible(visible: false);
		}
	}

	private void ResetDefaultVisuals()
	{
		if (titleLabel != null)
		{
			titleLabel.color = DefaultTitleColor;
		}
		if (bgBottomSprite != null)
		{
			bgBottomSprite.color = DefaultBgBottomColor;
		}
		if (bgTopSprite != null)
		{
			bgTopSprite.color = DefaultBgTopColor;
			bgTopSprite.applyGradient = false;
			bgTopSprite.applyHorizontalGradient = false;
		}
	}

	private void ApplyBgTopGradient(Color leftColor)
	{
		if (!(bgTopSprite == null))
		{
			bgTopSprite.applyGradient = false;
			bgTopSprite.applyHorizontalGradient = true;
			bgTopSprite.gradientLeft = leftColor;
			bgTopSprite.color = Color.white;
		}
	}

	private void UpdateOccupyingSurvivorTokenIcons(GameObject container, List<string> survivorAnalyticsIds)
	{
		if (container == null)
		{
			return;
		}
		if (survivorAnalyticsIds != null)
		{
			string.Join(", ", survivorAnalyticsIds);
		}
		Transform transform = container.transform.Find("Icon_Bg") ?? container.transform;
		for (int i = 0; i < 3; i++)
		{
			Transform transform2 = transform.Find("Icon" + (i + 1));
			if (transform2 == null)
			{
				continue;
			}
			bool flag = survivorAnalyticsIds != null && i < survivorAnalyticsIds.Count && !string.IsNullOrEmpty(survivorAnalyticsIds[i]);
			Helpers.GameObjectSetActive(transform2.gameObject, flag);
			if (!flag)
			{
				continue;
			}
			SurvivorModel survivorByAnalyticsId = GetSurvivorByAnalyticsId(survivorAnalyticsIds[i]);
			UISprite component = transform2.GetComponent<UISprite>();
			if (survivorByAnalyticsId != null && !(component == null))
			{
				if (survivorByAnalyticsId.Definition != null)
				{
					_ = survivorByAnalyticsId.Definition.FullName;
				}
				string currencyIconName = HelpersGfx.GetCurrencyIconName(SurvivorModel.GetSurvivorTraitUpgradeCurrencyType(survivorByAnalyticsId));
				component.spriteName = currencyIconName;
			}
		}
	}

	private static SurvivorModel GetSurvivorByAnalyticsId(string analyticsId)
	{
		if (string.IsNullOrEmpty(analyticsId))
		{
			return null;
		}
		ModelList<SurvivorModel> modelList = GameManager.Instance?.playerModel?.SurvivorContainer?.Survivors;
		if (modelList == null)
		{
			return null;
		}
		for (int i = 0; i < modelList.Count; i++)
		{
			if (modelList[i].IdForAnalytics == analyticsId)
			{
				return modelList[i];
			}
		}
		return null;
	}

	private int GetDurabilityLine()
	{
		if (data?.CellStateSnapshot == null)
		{
			return 10;
		}
		if (data.CellStateSnapshot.Status == 2)
		{
			return data.CellStateSnapshot.DefenderRemainingDurability;
		}
		if (data.CellDefinition != null && data.CellDefinition.HaveBattle && !data.CellStateSnapshot.PveCleared)
		{
			return data.CellStateSnapshot.RemainingDurability;
		}
		return data.CellStateSnapshot.DefenderRemainingDurability;
	}

	private void UpdateDurabilityBlocks(int durabilityLine)
	{
		if (BlockUp == null)
		{
			return;
		}
		durabilityLine = Mathf.Clamp(durabilityLine, 0, 10);
		Color durabilityColor = GetDurabilityColor(durabilityLine);
		for (int i = 0; i < 10; i++)
		{
			int num = i + 1;
			Transform transform = BlockUp.transform.Find("Icon" + num);
			if (transform == null)
			{
				continue;
			}
			bool flag = num <= durabilityLine;
			Helpers.GameObjectSetActive(transform.gameObject, flag);
			if (flag)
			{
				UISprite component = transform.GetComponent<UISprite>();
				if (component != null)
				{
					component.color = durabilityColor;
				}
			}
		}
	}

	private static Color GetDurabilityColor(int durabilityLine)
	{
		if (durabilityLine >= 7)
		{
			return DurabilityColorGreen;
		}
		if (durabilityLine >= 4)
		{
			return DurabilityColorYellow;
		}
		return DurabilityColorRed;
	}

	private void Update()
	{
		if (isFightCountdownActive && fightTimeRemainingMs > 0)
		{
			fightTimeRemainingMs -= (long)(Time.deltaTime * 1000f);
			if (fightTimeRemainingMs <= 0)
			{
				fightTimeRemainingMs = 0L;
				isFightCountdownActive = false;
			}
			UpdateFightTimeLabel();
		}
	}

	private void RefreshFightCountdown()
	{
		fightTimeRemainingMs = (GameManager.Instance?.playerModel?.WorldBossModelManager)?.GetCellRemainingLockMs(data.CellStateSnapshot) ?? 0;
		isFightCountdownActive = fightTimeRemainingMs > 0;
		SetFightTimeLabelVisible(visible: true);
		UpdateFightTimeLabel();
	}

	private void UpdateFightTimeLabel()
	{
		if (timeLabel != null)
		{
			timeLabel.text = FormatMinutesSeconds(fightTimeRemainingMs);
		}
	}

	private void SetFightTimeLabelVisible(bool visible)
	{
		if (timeLabel != null)
		{
			Helpers.GameObjectSetActive(timeLabel.gameObject, visible);
		}
	}

	private static string FormatMinutesSeconds(long milliSeconds)
	{
		milliSeconds = Math.Max(0L, milliSeconds);
		int num = (int)(milliSeconds / 1000);
		int num2 = num / 60;
		int num3 = num % 60;
		return $"{num2:00}:{num3:00}";
	}

	public void OnWorldBossCellStatusAsync(string responseJson)
	{
		WorldBossCellStatusResult worldBossCellStatusResult = GameManager.Instance.jsonSerializer.Deserialize<WorldBossCellStatusResult>(responseJson);
		bool flag = worldBossCellStatusResult.IsOccupied && worldBossCellStatusResult.OccupyingGroupId == GameManager.Instance.playerModel.GuildId;
		bool flag2 = cellStateNow == CellState.Empty && worldBossCellStatusResult.IsOccupied;
		if (worldBossCellStatusResult.Success && !worldBossCellStatusResult.IsFighting && !flag && !flag2)
		{
			MapContainerModel mapContainerModel = GameManager.Instance.playerModel.MapContainerModel;
			MissionSpawnPointGroup mapDefinitionById = GameManager.Instance.gameEconomyData.GetMapDefinitionById("Episode2");
			MapMissionGroupModel missionGroupModelForSpawnPointGroup = mapContainerModel.GetMissionGroupModelForSpawnPointGroup(mapDefinitionById);
			if (missionGroupModelForSpawnPointGroup != null && missionGroupModelForSpawnPointGroup.Missions != null && missionGroupModelForSpawnPointGroup.Missions.Count != 0)
			{
				MapMissionModel model = missionGroupModelForSpawnPointGroup.Missions[0];
				TeamSelectionPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MapTeamSelection) as TeamSelectionPopup;
				obj.SurvivorType = SurvivorContainerModel.SurvivorType.WorldBossPVP;
				obj.WorldBossCapturePoint = data.CellDefinition.CapturePoint;
				obj.WorldBossCell = data.CellDefinition.Cell;
				obj.WorldBossCellState = cellStateNow;
				obj.WorldBossOccupyingSurvivorIds = ((data.CellStateSnapshot != null && data.CellStateSnapshot.OccupyingSurvivorIds != null) ? new List<string>(data.CellStateSnapshot.OccupyingSurvivorIds) : null);
				obj.WorldBossOccupyingPlayerName = ((data.CellStateSnapshot != null) ? data.CellStateSnapshot.OccupyingPlayerName : null);
				obj.OpenForModel(model);
				EventManager.NotifyClick("SelectTeam");
			}
		}
		else
		{
			HUDNotification.Info(LocalizationManager.GetText((worldBossCellStatusResult != null && worldBossCellStatusResult.IsOccupied) ? "World.Boss.Occupied.Tips" : "World.Boss.AtWar.Tips"));
			WorldBossPVPDetailPopup worldBossPVPDetailPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossPVPDetailPopup) as WorldBossPVPDetailPopup;
			if (worldBossPVPDetailPopup != null)
			{
				worldBossPVPDetailPopup.GetWorldBossFullSnapshot();
			}
		}
	}

	public void OnClickClean()
	{
		WorldBossModelManager worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		if (worldBossModelManager != null && worldBossModelManager.GetMyDispatchedTeamCount() >= worldBossModelManager.GetDispatchTeamLimit())
		{
			OpneRetreatPopup();
			return;
		}
		WorldBossCellStatusRequest value = new WorldBossCellStatusRequest
		{
			GroupId = GameManager.Instance.playerModel.GuildId,
			SeasonId = GameManager.Instance.playerModel.WorldBossModelManager.GetCurrentSeasonId(),
			CycleId = GameManager.Instance.playerModel.WorldBossModelManager.GetCurrentCycleId(),
			CapturePoint = data.CellDefinition.CapturePoint,
			Cell = data.CellDefinition.Cell
		};
		string arg = GameManager.Instance.jsonSerializer.Serialize(value);
		SignalRClient.Instance.RequestCommand("WorldBossCellStatus", arg, OnWorldBossCellStatusAsync, waitForResponse: true);
	}
}
