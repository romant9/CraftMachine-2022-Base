using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class MockData : MonoBehaviour
{
	public string ResourceAssetName;

	public string RunJSON;

	public string MissionFlavor;

	public int MissionLevel;

	public List<string> SurvivorIDs = new List<string>();

	public List<List<string>> SurvivorEquipmentOverrideIDs = new List<List<string>>();

	public int SurvivorRarityLevel;

	public int SurvivorLevel;

	public int SurvivorUpgradeLevel;

	public int EquipmentRarityLevel;

	public int EquipmentLevel;

	public int EquipmentUpgradeLevel;

	public bool IsDeadly;

	public bool IsSurvival;

	public string SurvivalMissionConfigName;

	public int SurvivalMissionOrderInSection;

	public int SurvivalMissionOrderNumber;

	public SurvivalMissionConfig.SurvivalObjectiveType SurvivalForcedObjectiveType;

	public bool IsGuildBattle;

	public string GuildBattleMissionConfigObjectivesString;

	public string GuildBattleMissionConfigEnemiesString;

	public string CronExpression;

	[Tooltip("Hashed player ID to force Outpost attack target.")]
	public string MatchMakingOverrideId;

	private MockDataResources resources;

	public string GetGameEconomyJSON()
	{
		EnsureResourcesLoaded();
		if (!(resources.GetGameEconomyJson() != null))
		{
			return OfflineManager.IsLoadDataManager ? ContentManager.Instance.GetCache("GED").GetContentById<string>("GameEconomyData") : null;
		}
		return resources.GetGameEconomyJson().text;
	}

	public string GetAdMediationJSON()
	{
		EnsureResourcesLoaded();
		return resources.adMediationJSON.text;
	}

	public string GetDemoPlayerJSON()
	{
		EnsureResourcesLoaded();
		if (!(resources.demoPlayerJSON != null))
		{
			return null;
		}
		return resources.demoPlayerJSON.text;
	}

	public string GetTestGuildModelJson()
	{
		EnsureResourcesLoaded();
		if (!(resources.playerGuildJSON != null))
		{
			return null;
		}
		return resources.playerGuildJSON.text;
	}

	public string GetOutpostJSON()
	{
		EnsureResourcesLoaded();
		if (!(resources.OutpostTemplateJSON != null))
		{
			return null;
		}
		return resources.OutpostTemplateJSON.text;
	}

	public string GetRunJSON()
	{
		return RunJSON;
	}

	private void EnsureResourcesLoaded()
	{
		if (resources == null)
		{
			resources = UnityUtils.LoadFromAssetBundle<MockDataResources>(ResourceAssetName, "scriptableobjects");
			if (resources == null)
			{
				Debug.LogError("Failed to load MockDataResources");
			}
		}
	}
}
