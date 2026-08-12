using System;
using BaseModel;

namespace TWDModel
{
	public class DebugFreeRouletteDrawCommand : ModelCommand
	{
		public int ConfigId { get; set; }

		public bool IsMultiDraw { get; set; }

		public DebugFreeRouletteDrawCommand()
		{
		}

		public DebugFreeRouletteDrawCommand(int configId, bool isMultiDraw = false)
		{
			ConfigId = configId;
			IsMultiDraw = isMultiDraw;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			try
			{
				if (tWDModelManager.Player.RouletteManager == null)
				{
					tWDModelManager.Debug.LogError("[DebugFreeRouletteDraw] RouletteManager not found");
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				if (ConfigId <= 0)
				{
					tWDModelManager.Debug.LogError("[DebugFreeRouletteDraw] Invalid ConfigId");
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				IModelCommandRespond modelCommandRespond = ((!IsMultiDraw) ? new RouletteDrawCommand(ConfigId).Execute(manager) : new RouletteMultiDrawCommand(ConfigId).Execute(manager));
				if (modelCommandRespond.Code == 0)
				{
					tWDModelManager.Debug.LogInfo(string.Format("[DebugFreeRouletteDraw] {0} draw completed successfully for ConfigId: {1}", IsMultiDraw ? "Multi" : "Single", ConfigId));
				}
				else
				{
					tWDModelManager.Debug.LogError(string.Format("[DebugFreeRouletteDraw] {0} draw failed for ConfigId: {1}, Code: {2}", IsMultiDraw ? "Multi" : "Single", ConfigId, modelCommandRespond.Code));
				}
				return modelCommandRespond;
			}
			catch (Exception ex)
			{
				tWDModelManager.Debug.LogError("[DebugFreeRouletteDraw] Exception: " + ex.Message);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
		}
	}
}
