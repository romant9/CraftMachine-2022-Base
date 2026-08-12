using System;

namespace TWDModel
{
	public class GuildBattleMissionConfigObjective : GuildBattleMissionConfigBase
	{
		public const string ColumnName = "Objectives";

		public const string KillAmountKeyword = "KillAmount";

		public const string KillBossKeyword = "KillBoss";

		public const string ThreatFrequencyKeyword = "ThreatFrequency";

		public const string ThreatStartKeyword = "ThreatStart";

		public const string PileSpawnsKeyKeyword = "PileSpawns";

		public const string SurviveTurnAmountKeyword = "SurviveTurnAmountAndExit";

		public SurvivalMissionConfig.SurvivalObjectiveType ObjectiveType;

		public string BossType;

		public string Raiders;

		public int ThreatFrequency = 999;

		public int ThreatStart;

		public int InteractiveDuration = -1;

		public int SurviveDuration = -1;

		public int KillsRequired;

		public int SpawnerCount;

		public override bool Parse(ref string wrapperName, ref string stringParams, ref int[] intParams, ref string errorMessage)
		{
			if (!base.Parse(ref wrapperName, ref stringParams, ref intParams, ref errorMessage))
			{
				errorMessage = "Objectives, base NULL check";
				return false;
			}
			if (Enum.IsDefined(typeof(SurvivalMissionConfig.SurvivalObjectiveType), wrapperName))
			{
				ObjectiveType = (SurvivalMissionConfig.SurvivalObjectiveType)Enum.Parse(typeof(SurvivalMissionConfig.SurvivalObjectiveType), wrapperName);
				int num = intParams[1];
				if (wrapperName.Contains("KillBoss"))
				{
					BossType = stringParams.Replace("Walker", "");
					num = intParams[0];
				}
				else if (wrapperName.Contains("KillAmount"))
				{
					KillsRequired = ((intParams[0] != -1) ? intParams[0] : KillsRequired);
				}
				else if (wrapperName.Contains("SurviveTurnAmountAndExit"))
				{
					SurviveDuration = ((intParams[0] != -1) ? intParams[0] : SurviveDuration);
				}
				InteractiveDuration = ((num != -1) ? num : InteractiveDuration);
			}
			else if (wrapperName == "ThreatFrequency" && intParams[0] != -1)
			{
				ThreatFrequency = intParams[0];
			}
			else if (wrapperName == "ThreatStart" && intParams[0] != -1)
			{
				ThreatStart = intParams[0];
			}
			else
			{
				if (!(wrapperName == "PileSpawns") || intParams[0] == -1)
				{
					errorMessage = "Could not parse objective";
					return false;
				}
				SpawnerCount = intParams[0];
			}
			return IsValid();
		}

		public override bool IsValid()
		{
			return ObjectiveType != SurvivalMissionConfig.SurvivalObjectiveType.Invalid;
		}
	}
}
