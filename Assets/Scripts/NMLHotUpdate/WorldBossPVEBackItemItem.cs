using System;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class WorldBossPVEBackItemItem : MonoBehaviour
{
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private GameObject uncrossGM;

	[SerializeField]
	private GameObject fightGM;

	[SerializeField]
	private UILabel timeLabel;

	[SerializeField]
	private GameObject crossGM;

	[SerializeField]
	private UISprite playerIcon;

	[SerializeField]
	private UILabel playerNameLabel;

	[SerializeField]
	private GameObject Block;

	[SerializeField]
	private GameObject BlockUp;

	[SerializeField]
	private PlayerEmblemIcon playerEmblemIcon;

	private int BlockNum = 10;

	private string BlockNamePre = "Icon";

	private static readonly Color DurabilityColorGreen = Helpers.HexToColor("#a0c92f");

	private static readonly Color DurabilityColorYellow = Helpers.HexToColor("#f7c225");

	private static readonly Color DurabilityColorRed = Helpers.HexToColor("#fd3535");

	private static readonly Color ClearedTitleColor = Helpers.HexToColor("#8d8d8d");

	private static readonly Color ClearedOtherLabelColor = Helpers.HexToColor("#434343");

	private static readonly Color ClearedBlockIconColor = Helpers.HexToColor("#404040");

	private static readonly Color DefaultLabelColor = Color.white;

	private WorldBossPVECellSlotData data;

	private long fightTimeRemainingMs;

	private bool isFightCountdownActive;

	private int attackCount;

	public void SetData(WorldBossPVECellSlotData slotData)
	{
		if (slotData == null || !slotData.HasValue)
		{
			data = null;
			return;
		}
		data = slotData;
		UpdateUI();
	}

	public void UpdateUI()
	{
		Helpers.GameObjectSetActive(uncrossGM, value: false);
		Helpers.GameObjectSetActive(fightGM, value: false);
		Helpers.GameObjectSetActive(crossGM, value: false);
		isFightCountdownActive = false;
		ResetDefaultVisuals();
		bool flag = data != null && data.HasValue && data.CellStateSnapshot != null && data.CellStateSnapshot.Status == 2;
		UpdateDurabilityBlocks(GetDurabilityLine(), flag);
		if (data != null && data.HasValue)
		{
			if (!flag && data.CellDefinition.EnemyName != null && data.CellDefinition.EnemyName != "")
			{
				titleLabel.text = LocalizationManager.GetText(data.CellDefinition.EnemyName);
			}
			if (data.CellStateSnapshot == null || data.CellStateSnapshot.Status == 0)
			{
				uncrossGM.SetActive(value: true);
				SetFightTimeLabelVisible(visible: false);
			}
			else if (data.CellStateSnapshot.Status == 1)
			{
				fightGM.SetActive(value: true);
				RefreshFightCountdown();
			}
			else if (data.CellStateSnapshot.Status == 2)
			{
				crossGM.SetActive(value: true);
				SetFightTimeLabelVisible(visible: false);
				Debug.LogError("data.CellDefinition.CapturePoint: name " + GameManager.Instance.playerModel.WorldBossModelManager.GetCellOccupierName(data.CellDefinition.CapturePoint, data.CellDefinition.Cell));
				playerNameLabel.text = GameManager.Instance.playerModel.WorldBossModelManager.GetCellOccupierName(data.CellDefinition.CapturePoint, data.CellDefinition.Cell);
				playerEmblemIcon.SetEmblem(GameManager.Instance.playerModel.WorldBossModelManager.GetCellOccupierEmblem(data.CellDefinition.CapturePoint, data.CellDefinition.Cell));
				ApplyClearedVisuals();
			}
		}
	}

	private int GetDurabilityLine()
	{
		if (data?.CellStateSnapshot == null)
		{
			return BlockNum;
		}
		return data.CellStateSnapshot.RemainingDurability;
	}

	private void UpdateDurabilityBlocks(int durabilityLine, bool useClearedIconColor = false)
	{
		Debug.LogError("UpdateDurabilityBlocks: " + durabilityLine + " " + useClearedIconColor);
		if (BlockUp == null)
		{
			return;
		}
		durabilityLine = Mathf.Clamp(durabilityLine, 0, BlockNum);
		Color color = (useClearedIconColor ? ClearedBlockIconColor : GetDurabilityColor(durabilityLine));
		for (int i = 0; i < BlockNum; i++)
		{
			int num = i + 1;
			Transform transform = BlockUp.transform.Find(BlockNamePre + num);
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
					component.color = color;
				}
			}
		}
		if (useClearedIconColor)
		{
			SetBlockIconColors(ClearedBlockIconColor);
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
				RefreshPVEDetailPopupSnapshot();
			}
			UpdateFightTimeLabel();
		}
	}

	private static void RefreshPVEDetailPopupSnapshot()
	{
		WorldBossPVEDetailBackPopup worldBossPVEDetailBackPopup = ((SingularityMonoBehaviour<HUDManager>.Instance != null) ? (SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossPVEDetailBackPopup, null, createIfNotExist: false) as WorldBossPVEDetailBackPopup) : null);
		if (worldBossPVEDetailBackPopup != null && worldBossPVEDetailBackPopup.IsOpen)
		{
			worldBossPVEDetailBackPopup.GetWorldBossFullSnapshot();
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
		if (worldBossCellStatusResult.Success && worldBossCellStatusResult.IsEmpty)
		{
			WorldBossMissionModel worldBossMissionModel = GetWorldBossMissionModel();
			TeamSelectionPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MapTeamSelection) as TeamSelectionPopup;
			obj.SurvivorType = SurvivorContainerModel.SurvivorType.WorldBossPVE;
			obj.WorldBossCapturePoint = data.CellDefinition.CapturePoint;
			obj.WorldBossCell = data.CellDefinition.Cell;
			obj.OpenForWorldBoss(worldBossMissionModel, SurvivorContainerModel.SurvivorType.WorldBossPVE);
			EventManager.NotifyClick("SelectTeam");
		}
		else
		{
			HUDNotification.Info(LocalizationManager.GetText("World.Boss.AtWar.Tips"));
			WorldBossPVEDetailBackPopup worldBossPVEDetailBackPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossPVEDetailBackPopup) as WorldBossPVEDetailBackPopup;
			if (worldBossPVEDetailBackPopup != null)
			{
				worldBossPVEDetailBackPopup.GetWorldBossFullSnapshot();
			}
		}
	}

	public void OnClickAttack()
	{
		for (int i = 1 + attackCount; i <= 10 + attackCount; i++)
		{
			string cell = "PVE-1-Blue-" + i;
			ExecuteAttackAndEndCombat("PVE-1-Blue", cell);
		}
		attackCount += 10;
	}

	private void ExecuteAttackAndEndCombat(string capturePoint, string cell)
	{
		int currentSeasonId = GameManager.Instance.playerModel.WorldBossModelManager.GetCurrentSeasonId();
		int currentCycleId = GameManager.Instance.playerModel.WorldBossModelManager.GetCurrentCycleId();
		string guildId = GameManager.Instance.playerModel.GuildId;
		Helpers.ExecuteCommand(new AttackWorldBossCellCommand(currentSeasonId, currentCycleId, guildId, capturePoint, cell));
	}

	public void OnClickClean()
	{
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

	private WorldBossMissionModel GetWorldBossMissionModel()
	{
		if (data == null || data.CellDefinition == null)
		{
			return null;
		}
		WorldBossModelManager worldBossModelManager = GameManager.Instance.playerModel.WorldBossModelManager;
		WorldBossBattlegroundDefinition worldBossBattlegroundDefinition = GameManager.Instance.gameEconomyData.FindWorldBossBattlegroundDefinitionByCapturePoint(data.CellDefinition.CapturePoint, worldBossModelManager.GetCurrentBattleDifficulty());
		WorldBossMissionType worldBossMissionType = worldBossModelManager.ResolveMissionTypeForCell(worldBossBattlegroundDefinition, data.CellDefinition.CapturePoint, data.CellDefinition.Cell);
		return WorldBossMissionModel.Create(worldBossBattlegroundDefinition, data.CellDefinition.CapturePoint, data.CellDefinition.Cell, GameManager.Instance.gameEconomyData, worldBossMissionType);
	}

	private void ApplyClearedVisuals()
	{
		if (titleLabel != null)
		{
			titleLabel.text = LocalizationManager.GetText("World.Boss.PVE.Cleaner");
			titleLabel.color = ClearedTitleColor;
		}
		if (timeLabel != null)
		{
			timeLabel.color = ClearedOtherLabelColor;
		}
		SetDurabilityLabelColor(ClearedTitleColor);
		SetBlockIconColors(ClearedBlockIconColor);
	}

	private void ResetDefaultVisuals()
	{
		if (titleLabel != null)
		{
			titleLabel.color = DefaultLabelColor;
		}
		_ = playerNameLabel != null;
		if (timeLabel != null)
		{
			timeLabel.color = DefaultLabelColor;
		}
		SetDurabilityLabelColor(DefaultLabelColor);
	}

	private void SetDurabilityLabelColor(Color color)
	{
		UILabel uILabel = base.transform.Find("Durability")?.GetComponent<UILabel>();
		if (uILabel != null)
		{
			uILabel.color = color;
		}
	}

	private void SetBlockIconColors(Color color)
	{
		if (!(Block == null))
		{
			UISprite[] componentsInChildren = Block.GetComponentsInChildren<UISprite>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].color = color;
			}
		}
	}
}
