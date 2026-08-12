using System;
using System.Collections;
using System.Collections.Generic;
using Client.Connectivity;
using NextGames.Sdk.AssetBundleManager;
using TWDModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevStartup : MonoBehaviour
{
	[Serializable]
	public class GuildBattleMissionOverrideConfig
	{
		[Tooltip("Remember to put weigth at the end, ie. ThreatStart(2);1")]
		public string ObjectivesString;

		public string EnemiesString;
	}

	[Serializable]
	public class SurvivalMissionOverrideConfig
	{
		public string SurvivalMissionConfigName = "";

		public int SurvivalMissionOrderInSection;

		public int SurvivalMissionOrderNumber;

		[Tooltip("When set to some other value than Unspecified, the objective type in survival mission config is overridden with this.")]
		public SurvivalMissionConfig.SurvivalObjectiveType SurvivalForcedObjectiveType;
	}

	private static int initStep;

	[HideInInspector]
	public string RunJSON;

	[Tooltip("If this is set to None the Survivor 1 ID is used")]
	public SurvivorClass Survivor1 = SurvivorClass.None;

	[Tooltip("If this is set to None the Survivor 2 ID is used")]
	public SurvivorClass Survivor2 = SurvivorClass.None;

	[Tooltip("If this is set to None the Survivor 3 ID is used")]
	public SurvivorClass Survivor3 = SurvivorClass.None;

	[Tooltip("These can be used to spawn heroes, set Survivor 1 to None and copy Actor ID from GED, for example Hero_Daryl")]
	public string Survivor1ActorId;

	[Tooltip("All equipments from this array will be equipped to Survivor 1, Equipment IDs are found in GED->EquipmentDefinitions")]
	public List<string> Survivor1OverrideEquipment = new List<string>(2);

	[Tooltip("These can be used to spawn heroes, set Survivor 2 to None and copy Actor ID from GED, for example Hero_Daryl")]
	public string Survivor2ActorId;

	[Tooltip("All equipments from this array will be equipped to Survivor 2, Equipment IDs are found in GED->EquipmentDefinitions")]
	public List<string> Survivor2OverrideEquipment = new List<string>(2);

	[Tooltip("These can be used to spawn heroes, set Survivor 3 to None and copy Actor ID from GED, for example Hero_Daryl")]
	public string Survivor3ActorId;

	[Tooltip("All equipments from this array will be equipped to Survivor 3, Equipment IDs are found in GED->EquipmentDefinitions")]
	public List<string> Survivor3OverrideEquipment = new List<string>(2);

	[Tooltip("Survivor level.")]
	public int SurvivorLevel = 1;

	[Tooltip("Survivor upgrade level. If this is 0 (default) then survivor has not been upgraded and does not have any traits unlocked (except for the charge ability).")]
	public int SurvivorUpgradeLevel = 1;

	[Tooltip("Survivor rarity. (0 = common, 1= uncommon...)")]
	public int SurvivorRarityLevel;

	public int MissionLevel = 1;

	public string MissionFlavor;

	[Tooltip("Equipment level for survivors. If this is 0 (default) then equipment level will match SurvivorLevel.")]
	public int EquipmentLevel;

	[Tooltip("Equipment upgrades. 0 (default) means equipment has not been upgraded at all (no traits unlocked).")]
	public int EquipmentUpgradeLevel;

	[Tooltip("Equipment rarity. (0 = common, 1= uncommon...)")]
	public int EquipmentRarityLevel;

	public bool IsDeadly;

	[Tooltip("If set to true, the mission is run with specified survival mode config (using SurvivalMissionConfigName and SurvivalMissionOrderNumber to choose the GED config).")]
	[Header("Distance Missions")]
	public bool IsSurvival;

	public SurvivalMissionOverrideConfig SurvivalMissionOverride;

	[Header("Guild Battle Missions")]
	public bool IsGuildBattle;

	public GuildBattleMissionOverrideConfig GuildBattleMissionOverride;

	public DevFastTrackType DevFastTrackType = DevFastTrackType.Combat;

	public int DevRandomSeed = -1;

	public bool UseTimeAsRandomDevSeed = true;

	public bool shouldCheckStart;

	public GameObject PortraitPhotoBooth;

	private void Awake()
	{
		if (DevFastTrackType == DevFastTrackType.None)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		shouldCheckStart = false;
		initStep++;
		if (!Application.isEditor || !(UnityEngine.Object.FindObjectOfType<GameManager>() == null) || initStep != 1)
		{
			return;
		}
		shouldCheckStart = true;
		if (UseTimeAsRandomDevSeed)
		{
			int devRandomSeed = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
			DevRandomSeed = devRandomSeed;
		}
		if (DevRandomSeed >= 0)
		{
			if (SignalRClient.Instance == null)
			{
				PlayerModel.DevRandomSeed = DevRandomSeed;
			}
			else
			{
				Debug.LogWarning("Connected to server - DevRandomSeed ignored");
			}
		}
		GameManager.ResetGameData();
		ContentCache.DeleteAll();
		string item = "scene_devstartup";
		string scenarioName = "dev_startup";
		SingularityMonoBehaviour<AssetBundleController>.Instance.LoadAssetBundle(new List<string> { item }, delegate
		{
			AssetBundleManager.Instance.LoadScene(scenarioName, LoadSceneMode.Additive);
		});
	}

	private void Start()
	{
		if (!Application.isEditor || !shouldCheckStart)
		{
			return;
		}
		initStep++;
		if (initStep != 2 || DevFastTrackType == DevFastTrackType.None)
		{
			return;
		}
		MockData mockData = UnityEngine.Object.FindObjectOfType<MockData>();
		mockData.RunJSON = RunJSON;
		mockData.MissionLevel = MissionLevel;
		mockData.MissionFlavor = MissionFlavor;
		mockData.SurvivorLevel = SurvivorLevel;
		mockData.SurvivorUpgradeLevel = SurvivorUpgradeLevel;
		mockData.SurvivorRarityLevel = SurvivorRarityLevel;
		mockData.EquipmentLevel = EquipmentLevel;
		mockData.EquipmentRarityLevel = EquipmentRarityLevel;
		mockData.EquipmentUpgradeLevel = EquipmentUpgradeLevel;
		mockData.IsDeadly = IsDeadly;
		mockData.IsSurvival = IsSurvival;
		mockData.SurvivalMissionConfigName = SurvivalMissionOverride.SurvivalMissionConfigName;
		mockData.SurvivalMissionOrderInSection = SurvivalMissionOverride.SurvivalMissionOrderInSection;
		mockData.SurvivalMissionOrderNumber = SurvivalMissionOverride.SurvivalMissionOrderNumber;
		mockData.SurvivalForcedObjectiveType = SurvivalMissionOverride.SurvivalForcedObjectiveType;
		mockData.IsGuildBattle = IsGuildBattle;
		mockData.GuildBattleMissionConfigObjectivesString = GuildBattleMissionOverride.ObjectivesString;
		mockData.GuildBattleMissionConfigEnemiesString = GuildBattleMissionOverride.EnemiesString;
		if (Survivor1 == SurvivorClass.None)
		{
			if (string.IsNullOrEmpty(Survivor1ActorId))
			{
				mockData.SurvivorIDs.Add("Default" + Enum.GetName(typeof(SurvivorClass), SurvivorClass.Scout));
			}
			else
			{
				mockData.SurvivorIDs.Add(Survivor1ActorId);
			}
		}
		else
		{
			mockData.SurvivorIDs.Add("Default" + Enum.GetName(typeof(SurvivorClass), Survivor1));
		}
		if (Survivor2 == SurvivorClass.None)
		{
			if (string.IsNullOrEmpty(Survivor2ActorId))
			{
				mockData.SurvivorIDs.Add("Default" + Enum.GetName(typeof(SurvivorClass), SurvivorClass.Scout));
			}
			else
			{
				mockData.SurvivorIDs.Add(Survivor2ActorId);
			}
		}
		else
		{
			mockData.SurvivorIDs.Add("Default" + Enum.GetName(typeof(SurvivorClass), Survivor2));
		}
		if (Survivor3 == SurvivorClass.None)
		{
			if (string.IsNullOrEmpty(Survivor3ActorId))
			{
				mockData.SurvivorIDs.Add("Default" + Enum.GetName(typeof(SurvivorClass), SurvivorClass.Scout));
			}
			else
			{
				mockData.SurvivorIDs.Add(Survivor3ActorId);
			}
		}
		else
		{
			mockData.SurvivorIDs.Add("Default" + Enum.GetName(typeof(SurvivorClass), Survivor3));
		}
		mockData.SurvivorEquipmentOverrideIDs.Clear();
		mockData.SurvivorEquipmentOverrideIDs.Add(Survivor1OverrideEquipment);
		mockData.SurvivorEquipmentOverrideIDs.Add(Survivor2OverrideEquipment);
		mockData.SurvivorEquipmentOverrideIDs.Add(Survivor3OverrideEquipment);
		UnityEngine.Object.Instantiate(PortraitPhotoBooth);
		StartCoroutine(LoadGame());
		CombatView.SkipAskGore = true;
	}

	private IEnumerator LoadGame()
	{
		yield return null;
		GameManager.DevFastTrackLoad = DevFastTrackType;
		GameManager.StartedFromScenario = true;
		GameManager.Instance.LoadGame(skipDefaultContentLoading: true);
	}
}
