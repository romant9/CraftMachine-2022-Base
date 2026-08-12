using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class CombatEndFlowSurvivorWidget : CombatEndWidget
{
	[SerializeField]
	private SurvivorCardEndFlow[] Cards;

	[SerializeField]
	[Tooltip("Color injury BG. Index 0: not injured. Then minor, major, critical, (survival mode only) out of action.")]
	private Color[] injuryColors;

	[SerializeField]
	[Tooltip("Color injury BG. The color for Dead survivors")]
	private Color DeadColor;

	public void SetSurvivors(CombatModel combatModel)
	{
		if (combatModel == null)
		{
			return;
		}
		List<SurvivorModel> missionRoster = combatModel.MissionRoster;
		if (missionRoster == null || Cards == null)
		{
			return;
		}
		for (int i = 0; i < Cards.Length; i++)
		{
			if (Cards[i] != null)
			{
				if (i < missionRoster.Count)
				{
					bool isDead = missionRoster[i].IsDead && combatModel.IsDeadly;
					bool isSurvivalMission = combatModel.IsSurvivalMission;
					bool isEndlessBattleMission = combatModel.IsEndlessBattleMission;
					Color injuryColor = (isEndlessBattleMission ? GetEndlessWaveSurvivedColor(missionRoster[i], combatModel.MissionResult) : GetInjuryOrDeadColor(missionRoster[i], isDead, isSurvivalMission));
					Cards[i].SetSurvivor(missionRoster[i], injuryColor, isDead, isSurvivalMission, isEndlessBattleMission);
				}
				else
				{
					Cards[i].SetSurvivor(null, GetInjuryOrDeadColor(null), isDead: false, isSurvival: false, isEndless: false);
				}
			}
		}
	}

	private Color GetInjuryOrDeadColor(SurvivorModel survivor, bool isDead = false, bool isSurvival = false)
	{
		if (survivor != null)
		{
			if (isSurvival)
			{
				int num = (int)survivor.PreviousCombatInjuryType;
				if (survivor.PreviousCombatInjuryType != InjuryType.OutOfAction)
				{
					num = 0;
				}
				if (injuryColors != null && injuryColors.Length > num)
				{
					return injuryColors[num];
				}
			}
			else
			{
				if (isDead)
				{
					return DeadColor;
				}
				int previousCombatInjuryType = (int)survivor.PreviousCombatInjuryType;
				if (injuryColors != null && injuryColors.Length > previousCombatInjuryType)
				{
					return injuryColors[previousCombatInjuryType];
				}
			}
		}
		return Color.grey;
	}

	private Color GetEndlessWaveSurvivedColor(SurvivorModel survivor, ECombatResult eCombatResult)
	{
		float num = (float)survivor.SurvivedUntilWave / (float)EndlessModeHelpers.OverAllWaveCount * 100f;
		if (survivor.SurvivedUntilWave == EndlessModeHelpers.OverAllWaveCount)
		{
			return injuryColors[0];
		}
		if (num < 50f)
		{
			return injuryColors[3];
		}
		if (num > 50f && num <= 66f)
		{
			return injuryColors[2];
		}
		return injuryColors[1];
	}
}
