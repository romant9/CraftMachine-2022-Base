namespace TWDModel
{
	internal class SurvivalMissionDifficultyLevelHelper
	{
		public static int CalculateResultingSurvivalMissionLevel(GameEconomyData ged, int missionOrderNumber, int userCouncilLevel, SurvivalDifficulty difficulty)
		{
			if (ged.SurvivalDifficultyLevels == null || missionOrderNumber < 0 || missionOrderNumber >= ged.SurvivalDifficultyLevels.Length)
			{
				return 0;
			}
			SurvivalDifficultyLevel survivalDifficultyLevel = ged.SurvivalDifficultyLevels[missionOrderNumber];
			MissionGenerationData missionGenerationDataForMaxWalkerLevel = ged.GetMissionGenerationDataForMaxWalkerLevel(userCouncilLevel);
			int integerValue = 1;
			if (missionGenerationDataForMaxWalkerLevel != null)
			{
				integerValue = missionGenerationDataForMaxWalkerLevel.MissionLevel;
			}
			FixedPoint userLevelFactor = survivalDifficultyLevel.UserLevelFactor;
			FixedPoint fixedPoint = new FixedPoint(integerValue);
			FixedPoint fixedPoint2 = 1L;
			switch (difficulty)
			{
			case SurvivalDifficulty.Normal:
				fixedPoint2 = new FixedPoint(survivalDifficultyLevel.MissionLevelNormal);
				break;
			case SurvivalDifficulty.Hard:
				fixedPoint2 = new FixedPoint(survivalDifficultyLevel.MissionLevelHard);
				break;
			case SurvivalDifficulty.Nightmare:
				fixedPoint2 = new FixedPoint(survivalDifficultyLevel.MissionLevelNightmare);
				break;
			default:
				return 0;
			}
			return (int)(fixedPoint * userLevelFactor * (new FixedPoint(1) - survivalDifficultyLevel.MissionLevelUsageFactor) + fixedPoint2 * survivalDifficultyLevel.MissionLevelUsageFactor);
		}
	}
}
