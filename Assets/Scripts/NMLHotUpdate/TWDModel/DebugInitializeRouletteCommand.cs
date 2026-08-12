using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class DebugInitializeRouletteCommand : ModelCommand
	{
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
					player.RouletteManager = new RouletteManager();
					player.RouletteManager.SetManager(manager);
					player.RouletteManager.Start();
				}
				else
				{
					player.RouletteManager.Start();
				}
				List<RouletteConfig> activeConfigs = player.RouletteManager.GetActiveConfigs();
				tWDModelManager.Debug.LogInfo($"[DebugInitializeRoulette] Roulette system initialized successfully. Found {activeConfigs?.Count ?? 0} active activities.");
				if (activeConfigs != null)
				{
					foreach (RouletteConfig item in activeConfigs)
					{
						tWDModelManager.Debug.LogInfo($"[DebugInitializeRoulette] Active Roulette - ID: {item.ID}, EventPeriod: {item.EventPeriod}, Name: {item.NameDesc}");
					}
				}
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			catch (Exception ex)
			{
				tWDModelManager.Debug.LogError("[DebugInitializeRoulette] Exception: " + ex.Message);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
		}
	}
}
