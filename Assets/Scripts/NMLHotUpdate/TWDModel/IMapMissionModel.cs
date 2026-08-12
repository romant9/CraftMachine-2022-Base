namespace TWDModel
{
	public interface IMapMissionModel
	{
		int MissionLevel { get; }

		int RequiredSurvivorLevel { get; }

		MissionDifficulty MissionDifficulty { get; }

		int MaxTeamSize { get; }

		GuildBattleMapMissionModel.MissionType Type { get; }

		Cashier GetStartMissionCashier(TWDModelManager manager);

		Cashier GetStartMissionExpertModeCashier(TWDModelManager twdManager);

		SurvivalMissionConfig SolveSurvivalConfigForCurrentMission();

		bool IsUsingSurvivalConfig();
	}
}
