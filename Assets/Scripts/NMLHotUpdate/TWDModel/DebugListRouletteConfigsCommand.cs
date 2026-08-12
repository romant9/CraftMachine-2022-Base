using System;
using BaseModel;

namespace TWDModel
{
	public class DebugListRouletteConfigsCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			try
			{
				RouletteConfig[] rouletteConfigs = tWDModelManager.GameEconomyData.RouletteConfigs;
				if (rouletteConfigs == null || rouletteConfigs.Length == 0)
				{
					tWDModelManager.Debug.LogInfo("[DebugListRouletteConfigs] No roulette configs found");
					return new NGModelCommandRespond(this, TWDModelResult.OK);
				}
				tWDModelManager.Debug.LogInfo($"[DebugListRouletteConfigs] Found {rouletteConfigs.Length} roulette configs:");
				tWDModelManager.Debug.LogInfo("========================================");
				long num = tWDModelManager.Player?.UtcTimeStamp ?? 0;
				int num2 = tWDModelManager.Player?.CouncilLevel ?? 0;
				for (int i = 0; i < rouletteConfigs.Length; i++)
				{
					RouletteConfig rouletteConfig = rouletteConfigs[i];
					if (rouletteConfig == null)
					{
						tWDModelManager.Debug.LogInfo($"  [{i}] NULL config");
						continue;
					}
					bool flag = num >= rouletteConfig.StartTimeMilliseconds && (rouletteConfig.EndTimeMilliseconds == 0L || num <= rouletteConfig.EndTimeMilliseconds);
					bool flag2 = rouletteConfig.OpenLevel < 0 || rouletteConfig.OpenLevel <= num2;
					bool flag3 = flag && flag2;
					tWDModelManager.Debug.LogInfo(string.Format("  [{0}] ID: {1} | Period: {2} | Status: {3}", i, rouletteConfig.ID, rouletteConfig.EventPeriod, flag3 ? "ACTIVE" : "INACTIVE"));
					tWDModelManager.Debug.LogInfo($"      OpenLevel: {rouletteConfig.OpenLevel} | Discount: {rouletteConfig.Discount}%");
					tWDModelManager.Debug.LogInfo($"      StartTime: {rouletteConfig.StartTimeUtc} ({rouletteConfig.StartTimeMilliseconds})");
					tWDModelManager.Debug.LogInfo($"      EndTime: {rouletteConfig.EndTimeUtc} ({rouletteConfig.EndTimeMilliseconds})");
					tWDModelManager.Debug.LogInfo("      SingleCost: " + rouletteConfig.RouletteSingleCost + " | MultiCost: " + rouletteConfig.RouletteMultiCost);
					tWDModelManager.Debug.LogInfo("      NameDesc: " + rouletteConfig.NameDesc);
					tWDModelManager.Debug.LogInfo($"      Time Valid: {flag} | Level Valid: {flag2} (Current Level: {num2})");
					tWDModelManager.Debug.LogInfo("      ---");
				}
				tWDModelManager.Debug.LogInfo("========================================");
				tWDModelManager.Debug.LogInfo($"[DebugListRouletteConfigs] Total: {rouletteConfigs.Length} configs");
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			catch (Exception ex)
			{
				tWDModelManager.Debug.LogError("[DebugListRouletteConfigs] Exception: " + ex.Message);
				tWDModelManager.Debug.LogError("[DebugListRouletteConfigs] StackTrace: " + ex.StackTrace);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
		}
	}
}
