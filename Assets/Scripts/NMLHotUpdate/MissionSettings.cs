using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

[ExecuteInEditMode]
public class MissionSettings : RunLocationItem
{
	[Tooltip("Displayed Mission Type")]
	public MissionType DisplayType;

	[Tooltip("Displayed Text ID")]
	public string DisplayTextID;

	[Tooltip("Customized faction names on the mission.")]
	public MissionFactionNames[] FactionNames = new MissionFactionNames[1]
	{
		new MissionFactionNames
		{
			Faction = Faction.Raider
		}
	};

	[Tooltip("Ground Type")]
	public GroundType Ground;

	[Tooltip("Ambience Sound Type")]
	public AmbienceType Ambience;

	[Tooltip("For debugging purposes you can specify if the mission is deadly or not. This won't affect how mission is spawned in the map.")]
	public bool IsDeadly;

	[Tooltip("For debugging purposes you can specify mission random seed which affects where loot can be found for example. This won't affect how mission is spawned in the map.")]
	public int MissionRandomSeed;

	[Tooltip("Turn count to first walker wave.")]
	public int InitialTurnCountToWave = 4;

	[Tooltip("Walker count of the first wave.")]
	public int InitialThreatLevel = 2;

	[Tooltip("This defines how many optional loot keys will be randomized to loot boxes set to 'Optional'. Primary loot box will always contain a key and it is not counted as an optional.")]
	public int OptionalLootKeys = 2;

	[Tooltip("Difficulty offset for survivor level requirements, -5 means a survivor can be 5 levels lower than the requirement to still be valid for the mission, +5 would mean survivor has to be 5 levels higher than the minimum requirement.")]
	public int SurvivorLevelRequirementOffset;

	[Tooltip("Determined what kind of incremenetal difficulty adjustment settings are used for this mission")]
	public IncrementalDifficultyMissionType IncrementalDifficultyType;

	[Tooltip("Conditions to get the stars")]
	public MissionStarCondition[] StarConditions = new MissionStarCondition[3]
	{
		new MissionStarCondition
		{
			Type = MissionStarsType.CompleteMission
		},
		new MissionStarCondition
		{
			Type = MissionStarsType.NoStruggle
		},
		new MissionStarCondition
		{
			Type = MissionStarsType.MaxTurns,
			Parameter = "7"
		}
	};

	[Tooltip("How many turns after alarm triggered.")]
	public int PvPAfterAlarmTurns;

	[Tooltip("Which of the objective types are in multiples, first in the level, primary.")]
	public PvPMissionType PvPMissionType;

	[Tooltip("Is this mission a PvP mission base")]
	public bool IsPvP;

	public List<int> MissionTags = new List<int>();

	[Tooltip("For debugging purposes you can specify from which table to draw the rewards.")]
	public DropEventDefinition.DropEventTag LootTag;

	public static bool IsExporting;

	private void OnEnable()
	{
		if (IsExporting)
		{
			return;
		}
		Transform parent = base.transform.parent;
		if (!(parent != null) || !(parent.GetComponent<Scenario>() != null))
		{
			return;
		}
		MissionSettings[] componentsInChildren = parent.GetComponentsInChildren<MissionSettings>();
		foreach (MissionSettings missionSettings in componentsInChildren)
		{
			if (missionSettings != this)
			{
				missionSettings.gameObject.SetActive(value: false);
			}
		}
	}

	public string GetUniqueIdentifier()
	{
		Transform parent = base.transform.parent;
		return (parent.GetComponent<Scenario>().BackgroundScene + "_" + parent.name + "_" + base.gameObject.name).ToLower();
	}

	public string GetMissionID()
	{
		return "PVE-G02-" + ModelHelpers.MD5Sum(GetUniqueIdentifier()).PadLeft(40, '0');
	}

	public override TWDModelObject Apply(IRunLocationItemContainer runLocation, IRunLocationErrorContext errors)
	{
		MissionModel missionModel = new MissionModel();
		missionModel.TypeOfMission = DisplayType;
		missionModel.MissionName = base.gameObject.name;
		missionModel.Id = GetMissionID();
		missionModel.DisplayTextID = DisplayTextID;
		missionModel.FactionNames = FactionNames;
		missionModel.MissionTags = new List<int>();
		missionModel.MissionTags.AddRange(MissionTags);
		missionModel.OptionalLootKeys = Mathf.Clamp(OptionalLootKeys, 0, 3);
		missionModel.CompletionBonusLootKeys = Mathf.Clamp(3 - OptionalLootKeys, 0, 3);
		missionModel.SurvivorLevelRequirementOffset = SurvivorLevelRequirementOffset;
		missionModel.MissionStarConditions = StarConditions;
		missionModel.IncrementalDifficultyType = IncrementalDifficultyType;
		missionModel.PvPAfterAlarmTurns = PvPAfterAlarmTurns;
		missionModel.PVPType = PvPMissionType;
		StartingLocationConfig[] componentsInChildren = GetComponentsInChildren<StartingLocationConfig>();
		missionModel.MaxTeamSize = ((componentsInChildren != null) ? componentsInChildren.Length : 0);
		runLocation.AddMission(missionModel);
		if (StarConditions == null || StarConditions.Length != 3)
		{
			errors.ReportError("There should be 3 star conditions");
		}
		return missionModel;
	}
}
