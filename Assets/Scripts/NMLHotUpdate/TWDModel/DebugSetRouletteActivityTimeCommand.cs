using System;
using BaseModel;

namespace TWDModel
{
	public class DebugSetRouletteActivityTimeCommand : ModelCommand
	{
		public string Parameter { get; set; }

		public DebugSetRouletteActivityTimeCommand()
		{
		}

		public DebugSetRouletteActivityTimeCommand(string parameter)
		{
			Parameter = parameter;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			try
			{
				if (string.IsNullOrEmpty(Parameter))
				{
					tWDModelManager.Debug.LogError("[DebugSetRouletteActivityTime] Parameter is null or empty");
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				string[] array = Parameter.Split(',');
				if (array.Length != 3)
				{
					tWDModelManager.Debug.LogError("[DebugSetRouletteActivityTime] Invalid parameter format. Expected: \"ConfigId,StartTime,EndTime\", Got: \"" + Parameter + "\"");
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				if (!int.TryParse(array[0].Trim(), out var result))
				{
					tWDModelManager.Debug.LogError("[DebugSetRouletteActivityTime] Invalid ConfigId: " + array[0]);
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				RouletteConfig rouletteConfig = tWDModelManager.GameEconomyData.GetRouletteConfig(result);
				if (rouletteConfig == null)
				{
					tWDModelManager.Debug.LogError($"[DebugSetRouletteActivityTime] RouletteConfig not found for ConfigId: {result}");
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				string text = array[1].Trim();
				if (text.ToLower() == "now")
				{
					rouletteConfig.StartTimeUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
				}
				else
				{
					rouletteConfig.StartTimeUtc = text;
				}
				string text2 = array[2].Trim();
				if (text2 == "0")
				{
					rouletteConfig.EndTimeUtc = "0";
				}
				else
				{
					rouletteConfig.EndTimeUtc = text2;
				}
				rouletteConfig.ResetCachedTime();
				tWDModelManager.Debug.LogInfo($"[DebugSetRouletteActivityTime] Roulette {result} time updated:");
				tWDModelManager.Debug.LogInfo($"  StartTime: {rouletteConfig.StartTimeUtc} ({rouletteConfig.StartTimeMilliseconds})");
				tWDModelManager.Debug.LogInfo($"  EndTime: {rouletteConfig.EndTimeUtc} ({rouletteConfig.EndTimeMilliseconds})");
				if (tWDModelManager.Player?.RouletteManager != null)
				{
					tWDModelManager.Player.RouletteManager.RefreshRouletteData();
					tWDModelManager.Debug.LogInfo("[DebugSetRouletteActivityTime] Roulette data refreshed");
				}
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			catch (Exception ex)
			{
				tWDModelManager.Debug.LogError("[DebugSetRouletteActivityTime] Exception: " + ex.Message);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
		}
	}
}
