using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class WorldBossSignUpCycleCommand : TWDWorldBossBaseCommand
	{
		private int _resolvedSignUpNumNeed;

		private long _resolvedSeasonStartTimeUtcMs;

		private long _resolvedSeasonEndTimeUtcMs;

		private long _resolvedCycleStartTimeUtcMs;

		private long _resolvedCycleEndTimeUtcMs;

		private int _resolvedMaxDifficulty;

		private int _resolvedMatchBeforeStart;

		private int _resolvedSignUpCloseTime;

		private int _resolvedDifficultyCloseTime;

		private int _resolvedStartDifficulty;

		private int[] _resolvedPassScoreDifficulties;

		private long[] _resolvedPassScoreValues;

		public WorldBossSignUpCycleCommand()
		{
		}

		public WorldBossSignUpCycleCommand(int seasonId, int cycleId)
			: base(seasonId, cycleId)
		{
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager manager)
		{
			WorldBossSeasonDefinition worldBossSeasonDefinition = manager.GameEconomyData.FindWorldBossSeasonDefinition(base.SeasonId);
			if (worldBossSeasonDefinition == null)
			{
				manager.Debug.LogError("WorldBossSignUpCycleCommand: Season definition not found: " + base.SeasonId);
				return TWDModelResult.Error;
			}
			if (!worldBossSeasonDefinition.IsOpen(manager.Player.UtcTimeStamp))
			{
				manager.Debug.LogError("WorldBossSignUpCycleCommand: Season is not open: " + base.SeasonId);
				return TWDModelResult.Error;
			}
			WorldBossCycleDefinition worldBossCycleDefinition = manager.GameEconomyData.FindWorldBossCycleDefinition(base.SeasonId, base.CycleId);
			if (worldBossCycleDefinition == null)
			{
				manager.Debug.LogError("WorldBossSignUpCycleCommand: Cycle definition not found: " + base.CycleId);
				return TWDModelResult.Error;
			}
			if (!worldBossCycleDefinition.IsSignUpOpen(manager.Player.UtcTimeStamp))
			{
				manager.Debug.LogError("WorldBossSignUpCycleCommand: Sign-up window has closed for CycleId: " + base.CycleId);
				return TWDModelResult.Error;
			}
			WorldBossConfig worldBossConfig = manager.GameEconomyData.WorldBossConfig;
			_resolvedSignUpNumNeed = worldBossConfig?.SignUpNumNeed ?? 0;
			_resolvedCycleStartTimeUtcMs = worldBossCycleDefinition.StartTimeMilliseconds;
			_resolvedCycleEndTimeUtcMs = worldBossCycleDefinition.EndTimeMilliseconds;
			_resolvedSeasonStartTimeUtcMs = worldBossSeasonDefinition.StartTimeMilliseconds;
			_resolvedSeasonEndTimeUtcMs = worldBossSeasonDefinition.EndTimeMilliseconds;
			_resolvedMaxDifficulty = ResolveMaxDifficulty(manager);
			_resolvedMatchBeforeStart = ((worldBossConfig != null && worldBossConfig.MatchBeforeStart > 0) ? worldBossConfig.MatchBeforeStart : 5);
			_resolvedSignUpCloseTime = ((worldBossConfig != null && worldBossConfig.SignUpCloseTime > 0) ? worldBossConfig.SignUpCloseTime : 120);
			_resolvedDifficultyCloseTime = ((worldBossConfig != null && worldBossConfig.SelectDiffcCloseTime > 0) ? worldBossConfig.SelectDiffcCloseTime : 60);
			_resolvedStartDifficulty = worldBossSeasonDefinition.StartDifficulty;
			ResolveSeasonPassScoreMap(manager, out _resolvedPassScoreDifficulties, out _resolvedPassScoreValues);
			return TWDModelResult.OK;
		}

		protected override TWDModelResult ExecuteOnServer(TWDModelManager manager)
		{
			string guildId = manager.Player.GuildId;
			WorldBossOperationResult worldBossOperationResult = manager.ServerService.WorldBossSignUpCycle(new WorldBossSignUpCycleOperationRequest
			{
				GroupId = guildId,
				PlayerHashedId = manager.Player.HashedId,
				SeasonId = base.SeasonId,
				CycleId = base.CycleId,
				SignUpNumNeed = _resolvedSignUpNumNeed,
				CycleStartTimeUtcMs = _resolvedCycleStartTimeUtcMs,
				CycleEndTimeUtcMs = _resolvedCycleEndTimeUtcMs,
				SeasonStartTimeUtcMs = _resolvedSeasonStartTimeUtcMs,
				SeasonEndTimeUtcMs = _resolvedSeasonEndTimeUtcMs,
				GuildName = manager.Player.GuildName,
				MaxDifficulty = _resolvedMaxDifficulty,
				MatchBeforeStart = _resolvedMatchBeforeStart,
				SignUpCloseTime = _resolvedSignUpCloseTime,
				DifficultyCloseTime = _resolvedDifficultyCloseTime,
				StartDifficulty = _resolvedStartDifficulty,
				PassScoreDifficulties = _resolvedPassScoreDifficulties,
				PassScoreValues = _resolvedPassScoreValues
			});
			if (worldBossOperationResult == null || !worldBossOperationResult.Success)
			{
				manager.Debug.LogError("WorldBossSignUpCycleCommand: IServerService returned null");
				return TWDModelResult.Error;
			}
			return TWDModelResult.OK;
		}

		private static int ResolveMaxDifficulty(TWDModelManager manager)
		{
			WorldBossDifficultyDefinition[] worldBossDifficultyDefinitions = manager.GameEconomyData.WorldBossDifficultyDefinitions;
			if (worldBossDifficultyDefinitions == null)
			{
				return 0;
			}
			int num = 0;
			WorldBossDifficultyDefinition[] array = worldBossDifficultyDefinitions;
			foreach (WorldBossDifficultyDefinition worldBossDifficultyDefinition in array)
			{
				if (worldBossDifficultyDefinition != null && worldBossDifficultyDefinition.Difficulty > num)
				{
					num = worldBossDifficultyDefinition.Difficulty;
				}
			}
			return num;
		}

		private void ResolveSeasonPassScoreMap(TWDModelManager manager, out int[] difficulties, out long[] passScores)
		{
			WorldBossDifficultyDefinition[] worldBossDifficultyDefinitions = manager.GameEconomyData.WorldBossDifficultyDefinitions;
			List<int> list = new List<int>();
			List<long> list2 = new List<long>();
			if (worldBossDifficultyDefinitions != null)
			{
				WorldBossDifficultyDefinition[] array = worldBossDifficultyDefinitions;
				foreach (WorldBossDifficultyDefinition worldBossDifficultyDefinition in array)
				{
					if (worldBossDifficultyDefinition != null && worldBossDifficultyDefinition.Season == base.SeasonId)
					{
						list.Add(worldBossDifficultyDefinition.Difficulty);
						list2.Add(worldBossDifficultyDefinition.PassScore);
					}
				}
			}
			difficulties = list.ToArray();
			passScores = list2.ToArray();
		}
	}
}
