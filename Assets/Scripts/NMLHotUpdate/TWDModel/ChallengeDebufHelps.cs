using System.Collections.Generic;

namespace TWDModel
{
	public static class ChallengeDebufHelps
	{
		public static FixedPoint GetDmgReductionByClass(List<DifficultyIncrementalDebuff> DebuffConfigs, SurvivorClass survivorClass)
		{
			FixedPoint result = 0.0;
			foreach (DifficultyIncrementalDebuff DebuffConfig in DebuffConfigs)
			{
				if (DebuffConfig.DebuffType == ChallengeDebuffType.DebuffDmgReduction && (DebuffConfig.ConstructionParameters[0] == (long)survivorClass || DebuffConfig.ConstructionParameters[0] == 6L))
				{
					result += DebuffConfig.ConstructionParameters[1];
				}
			}
			return result;
		}

		public static List<List<FixedPoint>> GetDebufAllParam(List<DifficultyIncrementalDebuff> DebuffConfigs, ChallengeDebuffType challengeDebuffType)
		{
			List<List<FixedPoint>> list = new List<List<FixedPoint>>();
			foreach (DifficultyIncrementalDebuff DebuffConfig in DebuffConfigs)
			{
				if (DebuffConfig.DebuffType == challengeDebuffType)
				{
					list.Add(DebuffConfig.ConstructionParameters);
				}
			}
			return list;
		}

		public static FixedPoint GetDebufTotalSecondParam(List<DifficultyIncrementalDebuff> DebuffConfigs, ChallengeDebuffType challengeDebuffType)
		{
			FixedPoint result = 0.0;
			foreach (DifficultyIncrementalDebuff DebuffConfig in DebuffConfigs)
			{
				if (DebuffConfig.DebuffType == challengeDebuffType)
				{
					result += DebuffConfig.ConstructionParameters[1];
				}
			}
			return result;
		}

		public static FixedPoint GetDebufTotalFirstParam(List<DifficultyIncrementalDebuff> DebuffConfigs, ChallengeDebuffType challengeDebuffType)
		{
			FixedPoint result = 0.0;
			foreach (DifficultyIncrementalDebuff DebuffConfig in DebuffConfigs)
			{
				if (DebuffConfig.DebuffType == challengeDebuffType)
				{
					result += DebuffConfig.ConstructionParameters[0];
				}
			}
			return result;
		}

		public static FixedPoint GetDebufMinFirstParam(List<DifficultyIncrementalDebuff> DebuffConfigs, ChallengeDebuffType challengeDebuffType)
		{
			FixedPoint fixedPoint = FixedPoint.MaxValue;
			foreach (DifficultyIncrementalDebuff DebuffConfig in DebuffConfigs)
			{
				if (DebuffConfig.DebuffType == challengeDebuffType)
				{
					fixedPoint = FixedPoint.Min(DebuffConfig.ConstructionParameters[0], fixedPoint);
				}
			}
			return fixedPoint;
		}

		public static FixedPoint GetMinDebuffParamPercentageByTraitId(List<DifficultyIncrementalDebuff> DebuffConfigs, ChallengeDebuffType challengeDebuffType, string traitId)
		{
			int traitLevelIdentifier = UpgradeTraitsData.GetTraitLevelIdentifier(traitId);
			FixedPoint fixedPoint = 0L;
			foreach (DifficultyIncrementalDebuff DebuffConfig in DebuffConfigs)
			{
				int num = DebuffConfig.ConstructionParameters.Count - 1;
				int index = ((traitLevelIdentifier > num) ? num : traitLevelIdentifier);
				if (DebuffConfig.DebuffType == challengeDebuffType)
				{
					fixedPoint = DebuffConfig.ConstructionParameters[index];
				}
			}
			if (fixedPoint > 0L)
			{
				return fixedPoint / 100.0;
			}
			return fixedPoint;
		}

		public static DifficultyIncrementalDebuff GetDebufConfig(List<DifficultyIncrementalDebuff> DebuffConfigs, ChallengeDebuffType challengeDebuffType)
		{
			foreach (DifficultyIncrementalDebuff DebuffConfig in DebuffConfigs)
			{
				if (DebuffConfig.DebuffType == challengeDebuffType)
				{
					return DebuffConfig;
				}
			}
			return null;
		}
	}
}
