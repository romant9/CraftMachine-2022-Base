using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class WorldBossSelectDifficultyCommand : TWDWorldBossInternalCommand
	{
		private int _resolvedMemberRole;

		private int _resolvedMaxDifficulty;

		private long _resolvedPassScore;

		private long _resolvedCycleStartTimeUtcMs;

		private int _resolvedDifficultyCloseTime;

		public int SelectedDifficulty { get; private set; }

		public WorldBossSelectDifficultyCommand()
		{
		}

		public WorldBossSelectDifficultyCommand(int seasonId, int cycleId, int selectedDifficulty)
			: base(seasonId, cycleId)
		{
			SelectedDifficulty = selectedDifficulty;
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager manager)
		{
			if (WorldBossGuildFullSnapshot == null)
			{
				manager.Debug.LogError("WorldBossSelectDifficultyCommand: Guild has no WorldBossGuildMatchSnapshot");
				return TWDModelResult.Error;
			}
			WorldBossCycleDefinition worldBossCycleDefinition = manager.GameEconomyData.FindWorldBossCycleDefinition(base.SeasonId, base.CycleId);
			if (worldBossCycleDefinition == null)
			{
				manager.Debug.LogError("WorldBossSelectDifficultyCommand: Cycle definition not found: " + base.CycleId);
				return TWDModelResult.Error;
			}
			long num = WorldBossGuildFullSnapshot.GuildFullState?.DifficultySelectedAtUtcMs ?? 0;
			if (num > 0 && manager.Player.UtcTimeStamp - num < 300000)
			{
				manager.Debug.LogError("WorldBossSelectDifficultyCommand: Guild is in selected cooldown time");
				return TWDModelResult.Error;
			}
			if (!worldBossCycleDefinition.IsDifficultySelectionOpen(manager.Player.UtcTimeStamp))
			{
				manager.Debug.LogError("WorldBossSelectDifficultyCommand: Difficulty selection window not open for CycleId: " + base.CycleId);
				return TWDModelResult.Error;
			}
			WorldBossDifficultyDefinition[] worldBossDifficultyDefinitions = manager.GameEconomyData.WorldBossDifficultyDefinitions;
			int num2 = ((worldBossDifficultyDefinitions == null || worldBossDifficultyDefinitions.Length == 0) ? 1 : (from d in worldBossDifficultyDefinitions
				where d.Season == base.SeasonId
				select d.Difficulty).DefaultIfEmpty(1).Max());
			if (SelectedDifficulty < 1 || SelectedDifficulty > num2)
			{
				manager.Debug.LogError("WorldBossSelectDifficultyCommand: SelectedDifficulty out of range: " + SelectedDifficulty);
				return TWDModelResult.Error;
			}
			if (SelectedDifficulty > WorldBossGuildFullSnapshot.MaxUnlockedDifficulty)
			{
				manager.Debug.LogError("WorldBossSelectDifficultyCommand: SelectedDifficulty is higher than UnlockedDifficulty: " + SelectedDifficulty);
				return TWDModelResult.Error;
			}
			WorldBossDifficultyDefinition worldBossDifficultyDefinition = manager.GameEconomyData.FindWorldBossDifficultyDefinition(base.SeasonId, SelectedDifficulty);
			if (worldBossDifficultyDefinition == null)
			{
				manager.Debug.LogError("WorldBossSelectDifficultyCommand: Difficulty definition not found for SeasonId/Difficulty: " + base.SeasonId + "/" + SelectedDifficulty);
				return TWDModelResult.Error;
			}
			_resolvedPassScore = ((worldBossDifficultyDefinition.PassScore > 0) ? worldBossDifficultyDefinition.PassScore : 0);
			GuildMemberInfo memberInfo = GuildModel.GetMemberInfo(manager.Player.HashedId);
			if (memberInfo == null || memberInfo.Role < GuildMemberRole.Elder)
			{
				manager.Debug.LogError("WorldBossSelectDifficultyCommand: Sender role below Elder: " + manager.Player.HashedId);
				return TWDModelResult.Error;
			}
			_resolvedMemberRole = (int)memberInfo.Role;
			_resolvedMaxDifficulty = num2;
			_resolvedCycleStartTimeUtcMs = worldBossCycleDefinition.StartTimeMilliseconds;
			_resolvedDifficultyCloseTime = ((manager.GameEconomyData.WorldBossConfig != null && manager.GameEconomyData.WorldBossConfig.SelectDiffcCloseTime > 0) ? manager.GameEconomyData.WorldBossConfig.SelectDiffcCloseTime : 60);
			return TWDModelResult.OK;
		}

		protected override TWDModelResult ExecuteOnServer(TWDModelManager manager)
		{
			string guildId = manager.Player.GuildId;
			WorldBossOperationResult worldBossOperationResult = manager.ServerService.WorldBossSelectDifficulty(new WorldBossSelectDifficultyOperationRequest
			{
				GroupId = guildId,
				PlayerHashedId = manager.Player.HashedId,
				SeasonId = base.SeasonId,
				CycleId = base.CycleId,
				CycleStartTimeUtcMs = _resolvedCycleStartTimeUtcMs,
				DifficultyCloseTime = _resolvedDifficultyCloseTime,
				Difficulty = SelectedDifficulty,
				MemberRole = _resolvedMemberRole,
				MaxDifficulty = _resolvedMaxDifficulty,
				PassScore = _resolvedPassScore
			});
			if (worldBossOperationResult == null || !worldBossOperationResult.Success)
			{
				manager.Debug.LogError("WorldBossSelectDifficultyCommand: IServerService returned null");
				return TWDModelResult.Error;
			}
			return TWDModelResult.OK;
		}
	}
}
