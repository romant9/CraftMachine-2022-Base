using System;
using BaseModel;

namespace TWDModel
{
	public class DebugResetRouletteCommand : ModelCommand
	{
		public int ConfigId { get; set; }

		public DebugResetRouletteCommand()
		{
		}

		public DebugResetRouletteCommand(int configId)
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
					tWDModelManager.Debug.LogError("[DebugResetRoulette] RouletteManager not found");
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				if (ConfigId == -1)
				{
					foreach (object activityData2 in player.RouletteManager.ActivityDataList)
					{
						_ = activityData2;
					}
					tWDModelManager.Debug.LogInfo("[DebugResetRoulette] Reset all roulette activities");
				}
				else
				{
					RouletteActivityDataModel activityData = player.RouletteManager.GetActivityData(ConfigId);
					if (activityData != null)
					{
						activityData.ResetDrawnRewards();
						tWDModelManager.Debug.LogInfo($"[DebugResetRoulette] Reset roulette activity with ConfigId: {ConfigId}");
					}
					else
					{
						tWDModelManager.Debug.LogWarning($"[DebugResetRoulette] Activity with ConfigId {ConfigId} not found");
					}
				}
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			catch (Exception ex)
			{
				tWDModelManager.Debug.LogError("[DebugResetRoulette] Exception: " + ex.Message);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
		}
	}
}
