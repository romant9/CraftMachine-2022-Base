using System;
using System.Linq;
using TWDModel;
using UnityEngine;

public class WorldBossCaptureBase : MonoBehaviour
{
	[SerializeField]
	private UILabel desLabel;

	protected string captureName;

	protected WorldBossCaptureDataClient data = new WorldBossCaptureDataClient();

	private WorldBossCellDefinition[] cellDefinitions;

	public WorldBossCaptureDataClient GetData()
	{
		return data;
	}

	public virtual void SetData(WorldBossCaptureDataClient dataClient)
	{
		data = dataClient;
		cellDefinitions = Array.Empty<WorldBossCellDefinition>();
		WorldBossCellDefinition[] worldBossCellDefinitions = GameManager.Instance.gameEconomyData.WorldBossCellDefinitions;
		foreach (WorldBossCellDefinition worldBossCellDefinition in worldBossCellDefinitions)
		{
			if (worldBossCellDefinition.CapturePoint == data.definition.CapturePoint)
			{
				cellDefinitions = cellDefinitions.Concat(new WorldBossCellDefinition[1] { worldBossCellDefinition }).ToArray();
			}
		}
	}

	public virtual void OnClick()
	{
		UpdateUI();
	}

	public virtual void UpdateUI()
	{
	}

	public static string GetCapturePointSpriteName(string capturePoint)
	{
		if (string.IsNullOrEmpty(capturePoint))
		{
			return string.Empty;
		}
		if (capturePoint.IndexOf("-1-", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			return "UI_Pic_WB_PVE_A";
		}
		if (capturePoint.IndexOf("-2-", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			return "UI_Pic_WB_PVE_B";
		}
		if (capturePoint.IndexOf("-3-", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			return "UI_Pic_WB_PVE_C";
		}
		if (capturePoint.IndexOf("-4-", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			return "UI_Pic_WB_PVE_D";
		}
		if (string.Equals(capturePoint, "TOWER-A", StringComparison.OrdinalIgnoreCase))
		{
			return "UI_Pic_WB_Tower_A";
		}
		if (string.Equals(capturePoint, "TOWER-B", StringComparison.OrdinalIgnoreCase))
		{
			return "UI_Pic_WB_Tower_B";
		}
		if (string.Equals(capturePoint, "DEPOT", StringComparison.OrdinalIgnoreCase))
		{
			return "UI_Pic_WB_Depot";
		}
		if (string.Equals(capturePoint, "BOSS", StringComparison.OrdinalIgnoreCase))
		{
			return "UI_Pic_WB_Tank";
		}
		return string.Empty;
	}
}
