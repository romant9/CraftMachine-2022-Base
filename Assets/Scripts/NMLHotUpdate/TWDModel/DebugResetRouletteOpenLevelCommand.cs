using System;
using BaseModel;

namespace TWDModel
{
	public class DebugResetRouletteOpenLevelCommand : ModelCommand
	{
		public int ConfigId { get; set; }

		public int NewOpenLevel { get; set; }

		public DebugResetRouletteOpenLevelCommand()
		{
		}

		public DebugResetRouletteOpenLevelCommand(int configId, int newOpenLevel)
		{
			ConfigId = configId;
			NewOpenLevel = newOpenLevel;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			try
			{
				if (ConfigId == -1)
				{
					RouletteConfig[] rouletteConfigs = tWDModelManager.GameEconomyData.RouletteConfigs;
					if (rouletteConfigs == null || rouletteConfigs.Length == 0)
					{
						tWDModelManager.Debug.LogError("[DebugResetRouletteOpenLevel] No roulette configs found");
						return new NGModelCommandRespond(this, TWDModelResult.Error);
					}
					foreach (RouletteConfig rouletteConfig in rouletteConfigs)
					{
						if (rouletteConfig != null)
						{
							int openLevel = rouletteConfig.OpenLevel;
							rouletteConfig.OpenLevel = NewOpenLevel;
							tWDModelManager.Debug.LogInfo($"[DebugResetRouletteOpenLevel] Roulette {rouletteConfig.ID}: OpenLevel changed from {openLevel} to {NewOpenLevel}");
						}
					}
					tWDModelManager.Debug.LogInfo($"[DebugResetRouletteOpenLevel] All roulette OpenLevel reset to {NewOpenLevel}");
				}
				else
				{
					RouletteConfig rouletteConfig2 = tWDModelManager.GameEconomyData.GetRouletteConfig(ConfigId);
					if (rouletteConfig2 == null)
					{
						tWDModelManager.Debug.LogError($"[DebugResetRouletteOpenLevel] RouletteConfig not found for ConfigId: {ConfigId}");
						return new NGModelCommandRespond(this, TWDModelResult.Error);
					}
					int openLevel2 = rouletteConfig2.OpenLevel;
					rouletteConfig2.OpenLevel = NewOpenLevel;
					tWDModelManager.Debug.LogInfo($"[DebugResetRouletteOpenLevel] Roulette {ConfigId}: OpenLevel changed from {openLevel2} to {NewOpenLevel}");
				}
				if (tWDModelManager.Player?.RouletteManager != null)
				{
					tWDModelManager.Player.RouletteManager.RefreshRouletteData();
					tWDModelManager.Debug.LogInfo("[DebugResetRouletteOpenLevel] Roulette data refreshed");
				}
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			catch (Exception ex)
			{
				tWDModelManager.Debug.LogError("[DebugResetRouletteOpenLevel] Exception: " + ex.Message);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
		}
	}
}
