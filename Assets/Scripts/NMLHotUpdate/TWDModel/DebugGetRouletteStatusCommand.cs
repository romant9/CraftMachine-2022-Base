using System;
using BaseModel;

namespace TWDModel
{
	public class DebugGetRouletteStatusCommand : ModelCommand
	{
		public int ConfigId { get; set; }

		public DebugGetRouletteStatusCommand()
		{
		}

		public DebugGetRouletteStatusCommand(int configId)
		{
			ConfigId = configId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			try
			{
				PlayerModel player = tWDModelManager.Player;
				if (player.RouletteManager == null)
				{
					tWDModelManager.Debug.LogError("[DebugGetRouletteStatus] RouletteManager not found");
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				RouletteActivityDataModel activityData = player.RouletteManager.GetActivityData(ConfigId);
				if (activityData == null)
				{
					tWDModelManager.Debug.LogError($"[DebugGetRouletteStatus] Activity {ConfigId} not found");
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				tWDModelManager.Debug.LogInfo($"[DebugGetRouletteStatus] === Roulette {ConfigId} Status ===");
				tWDModelManager.Debug.LogInfo($"[DebugGetRouletteStatus] ConfigId: {activityData.ConfigId}");
				tWDModelManager.Debug.LogInfo($"[DebugGetRouletteStatus] EventPeriod: {activityData.EventPeriod}");
				tWDModelManager.Debug.LogInfo($"[DebugGetRouletteStatus] Type1DrawCount: {activityData.Type1DrawCount}");
				tWDModelManager.Debug.LogInfo($"[DebugGetRouletteStatus] Type2DrawCount: {activityData.Type2DrawCount}");
				tWDModelManager.Debug.LogInfo($"[DebugGetRouletteStatus] DrawnType1SlotIndicesCount: {activityData.DrawnType1SlotIndices?.Count ?? 0}");
				tWDModelManager.Debug.LogInfo($"[DebugGetRouletteStatus] DrawnType2SlotIndicesCount: {activityData.DrawnType2SlotIndices?.Count ?? 0}");
				tWDModelManager.Debug.LogInfo($"[DebugGetRouletteStatus] IsActivityCompleted: {activityData.IsActivityCompleted()}");
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			catch (Exception ex)
			{
				tWDModelManager.Debug.LogError("[DebugGetRouletteStatus] Exception: " + ex.Message);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
		}
	}
}
