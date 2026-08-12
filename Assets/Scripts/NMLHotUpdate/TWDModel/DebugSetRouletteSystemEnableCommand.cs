using System;
using BaseModel;

namespace TWDModel
{
	public class DebugSetRouletteSystemEnableCommand : ModelCommand
	{
		public int Enable { get; set; }

		public DebugSetRouletteSystemEnableCommand()
		{
		}

		public DebugSetRouletteSystemEnableCommand(int enable)
		{
			Enable = enable;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			try
			{
				bool flag = Enable == 1;
				tWDModelManager.GameEconomyData.ConfigData.EnableRouletteSystem = flag;
				tWDModelManager.Debug.LogInfo("[DebugSetRouletteSystemEnable] Roulette system " + (flag ? "ENABLED" : "DISABLED"));
				if (tWDModelManager.Player?.ActivityIntegrationManager != null)
				{
					tWDModelManager.Debug.LogInfo("[DebugSetRouletteSystemEnable] Activity integration manager notified");
				}
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			catch (Exception ex)
			{
				tWDModelManager.Debug.LogError("[DebugSetRouletteSystemEnable] Exception: " + ex.Message);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
		}
	}
}
