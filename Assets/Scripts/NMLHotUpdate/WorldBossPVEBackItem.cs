using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class WorldBossPVEBackItem : NUIListItem<WorldBossPVECellSlotData[]>
{
	[SerializeField]
	private WorldBossPVEBackItemItem item1;

	[SerializeField]
	private WorldBossPVEBackItemItem item2;

	public override void SetData(WorldBossPVECellSlotData[] data)
	{
		base.SetData(data);
		WorldBossPVECellSlotData slotData = GetSlotData(0);
		WorldBossPVECellSlotData slotData2 = GetSlotData(1);
		Helpers.GameObjectSetActive(item1, slotData?.HasValue ?? false);
		Helpers.GameObjectSetActive(item2, slotData2?.HasValue ?? false);
		UpdateUI();
	}

	public WorldBossPVECellSlotData GetSlotData(int index)
	{
		WorldBossPVECellSlotData[] data = GetData();
		if (data == null || index < 0 || index >= data.Length)
		{
			return null;
		}
		return data[index];
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		WorldBossPVECellSlotData slotData = GetSlotData(0);
		if (item1 != null && slotData != null && slotData.HasValue)
		{
			item1.SetData(slotData);
		}
		WorldBossPVECellSlotData slotData2 = GetSlotData(1);
		if (item2 != null && slotData2 != null && slotData2.HasValue)
		{
			item2.SetData(slotData2);
		}
	}

	public void OnWorldBossCellStatusAsync(string responseJson)
	{
		if (GameManager.Instance.jsonSerializer.Deserialize<WorldBossCellStatusResult>(responseJson).Success)
		{
			Helpers.ExecuteCommand(new AttackWorldBossCellCommand(GameManager.Instance.playerModel.WorldBossModelManager.GetCurrentSeasonId(), GameManager.Instance.playerModel.WorldBossModelManager.GetCurrentCycleId(), GameManager.Instance.playerModel.GuildId, "PVE-1-Red", "PVE-1-Red-1"));
		}
	}

	public void OnClickAttack()
	{
		for (int i = 1; i <= 20; i++)
		{
			string cell = "PVE-2-Blue-" + i;
			ExecuteAttackAndEndCombat("PVE-2-Blue", cell);
		}
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
			CapturePoint = "PVE-1-Red",
			Cell = "PVE-1-Red-1"
		};
		string arg = GameManager.Instance.jsonSerializer.Serialize(value);
		SignalRClient.Instance.RequestCommand("WorldBossCellStatus", arg, OnWorldBossCellStatusAsync, waitForResponse: true);
	}

	public override void Clear()
	{
		base.Clear();
	}
}
