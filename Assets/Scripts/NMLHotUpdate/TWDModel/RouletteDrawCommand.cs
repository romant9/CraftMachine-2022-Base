using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class RouletteDrawCommand : ConsumeCurrencyCommand
	{
		[JsonIgnore]
		public Rewards Rewards;

		[JsonIgnore]
		public RouletteResult RouletteResult;

		public int RouletteConfigId { get; set; }

		public RouletteDrawCommand()
		{
		}

		public RouletteDrawCommand(int rouletteConfigId)
		{
			RouletteConfigId = rouletteConfigId;
		}

		public static Cashier GetCashier(TWDModelManager manager, int configId, bool isMultiDraw = false)
		{
			if (manager?.GameEconomyData == null)
			{
				return null;
			}
			RouletteConfig rouletteConfig = manager.GameEconomyData.GetRouletteConfig(configId);
			if (rouletteConfig == null)
			{
				return null;
			}
			Dictionary<CurrencyType, int> dictionary = (isMultiDraw ? rouletteConfig.GetMultiCostInfo() : rouletteConfig.GetSingleCostInfo());
			if (dictionary == null || dictionary.Count == 0)
			{
				manager?.Debug?.LogError($"No cost info found for roulette config {configId}");
				return null;
			}
			Cashier cashier = new Cashier(manager);
			foreach (KeyValuePair<CurrencyType, int> item in dictionary)
			{
				CashierItem cashierItem = new CashierItem(PurchaseType.Roulette);
				cashierItem.SetCost(item.Key, item.Value);
				cashier.AddItem(cashierItem);
			}
			cashier.UseDiamondsAmount = -2;
			return cashier;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager { Player: var player } tWDModelManager)
			{
				if (!tWDModelManager.GameEconomyData.ConfigData.EnableRouletteSystem)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				if (player.RouletteManager == null)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				if (RouletteConfigId <= 0)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				RouletteActivityDataModel activityData = player.RouletteManager.GetActivityData(RouletteConfigId);
				if (activityData == null)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				if (!activityData.IsRouletteActive())
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				if (!activityData.IsDrawAllowed())
				{
					manager?.Debug?.LogWarning($"RouletteDrawCommand: Draw not allowed for activity {RouletteConfigId} - activity may be completed");
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				if (activityData.LastDrawTime > 0)
				{
					long num = tWDModelManager.Player.UtcTimeStamp - activityData.LastDrawTime;
					if (num < 3000)
					{
						manager?.Debug?.LogWarning($"RouletteDrawCommand: Draw too fast for activity {RouletteConfigId}, time since last draw: {num}ms");
						return new NGModelCommandRespond(this, TWDModelResult.Error);
					}
				}
				Cashier cashier = GetCashier(tWDModelManager, RouletteConfigId);
				if (cashier == null)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				TWDModelResult tWDModelResult = cashier.Pay();
				if (tWDModelResult != TWDModelResult.OK)
				{
					return new NGModelCommandRespond(this, tWDModelResult);
				}
				RouletteResult rouletteResult = activityData?.ExecuteRoulette();
				if (rouletteResult == null)
				{
					cashier.Refund();
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				if (activityData != null)
				{
					activityData.LastDrawTime = tWDModelManager.Player.UtcTimeStamp;
				}
				RouletteResult = rouletteResult;
				List<RouletteDefinition> list = rouletteResult?.GetAllRewardsList() ?? new List<RouletteDefinition>();
				if (activityData != null && activityData.IsActivityCompleted())
				{
					tWDModelManager.TdMetrics.SetEventType("roulette_activity_completed").AddProperty("config_id", RouletteConfigId.ToString()).AddProperty("has_get_all", rouletteResult.HasGetAllReward.ToString())
						.Send();
				}
				Rewards grantedRewards = null;
				if (activityData == null || !activityData.GrantRouletteRewards(rouletteResult, out grantedRewards))
				{
					tWDModelManager.Debug.LogWarning($"RouletteDrawCommand: Reward granting failed for config {RouletteConfigId}, recorded for retry");
				}
				Rewards = grantedRewards;
				Metrics metrics = tWDModelManager.Metrics;
				metrics.ResourceChangeUsedReason = "RouletteDraw";
				metrics.AddItemChange().AddResources(cashier).Send();
				RouletteConfig rouletteConfig = activityData?.GetConfig();
				SendRouletteRollMetrics(tWDModelManager, activityData, rouletteResult, rouletteConfig);
				Dictionary<CurrencyType, int> dictionary = rouletteConfig?.GetSingleCostInfo();
				string value = "";
				if (dictionary != null && dictionary.Count > 0)
				{
					value = string.Join(", ", dictionary.Select((KeyValuePair<CurrencyType, int> kvp) => $"{kvp.Key}:{kvp.Value}"));
				}
				string value2 = "";
				if (rouletteConfig != null)
				{
					List<CurrencyType> singleCostCurrencyTypes = rouletteConfig.GetSingleCostCurrencyTypes();
					if (singleCostCurrencyTypes != null)
					{
						value2 = string.Join(",", singleCostCurrencyTypes);
					}
				}
				tWDModelManager.TdMetrics.SetEventType("roulette_draw").AddProperty("config_id", activityData.ConfigId).AddProperty("event_period", rouletteConfig?.EventPeriod ?? 0)
					.AddProperty("draw_count", (list?.Count ?? 0).ToString())
					.AddProperty("has_get_all", rouletteResult?.HasGetAllReward ?? false)
					.AddProperty("cost_used", value)
					.AddProperty("cost_currency_types", value2)
					.Send();
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}

		private static void SendRouletteRollMetrics(TWDModelManager manager, RouletteActivityDataModel activityData, RouletteResult rouletteResult, RouletteConfig config)
		{
			try
			{
				if (manager == null || activityData == null || rouletteResult == null || config == null)
				{
					return;
				}
				Dictionary<CurrencyType, int> singleCostInfo = config.GetSingleCostInfo();
				string value = "";
				int num = 0;
				if (singleCostInfo != null && singleCostInfo.Count > 0)
				{
					KeyValuePair<CurrencyType, int> keyValuePair = singleCostInfo.First();
					value = keyValuePair.Key.ToString();
					num = keyValuePair.Value;
				}
				int num2 = (rouletteResult.HasGetAllReward ? (rouletteResult.GetAllRewardsList()?.Count ?? 0) : 0);
				string text = "";
				int num3 = 0;
				if (rouletteResult.Type2Rewards != null && rouletteResult.Type2Rewards.Count > 0)
				{
					RouletteDefinition rouletteDefinition = rouletteResult.Type2Rewards.FirstOrDefault((RouletteDefinition r) => r != null);
					if (rouletteDefinition != null)
					{
						text = rouletteDefinition.Rewards ?? "";
						if (!string.IsNullOrEmpty(text) && text != "GetALL")
						{
							string[] array = text.Split('(');
							if (array.Length >= 2 && int.TryParse(array[1].Replace(")", "").Trim(), out var result))
							{
								num3 = result;
							}
						}
					}
				}
				else if (rouletteResult.MainRewards != null && rouletteResult.MainRewards.Count > 0)
				{
					RouletteDefinition rouletteDefinition2 = rouletteResult.MainRewards.FirstOrDefault((RouletteDefinition r) => r != null);
					if (rouletteDefinition2 != null)
					{
						text = rouletteDefinition2.Rewards ?? "";
						if (!string.IsNullOrEmpty(text) && text != "GetALL")
						{
							string[] array2 = text.Split('(');
							if (array2.Length >= 2 && int.TryParse(array2[1].Replace(")", "").Trim(), out var result2))
							{
								num3 = result2;
							}
						}
					}
				}
				bool isEnd = rouletteResult.IsEnd;
				manager.TdMetrics.SetEventType("roulette_roll").AddProperty("roulette_roll", activityData.ConfigId.ToString()).AddProperty("change_type", value)
					.AddProperty("change_num", num.ToString())
					.AddProperty("is_getall", num2.ToString())
					.AddProperty("reward_type", text)
					.AddProperty("reward_num", num3.ToString())
					.AddProperty("is_end", isEnd ? "1" : "0")
					.Send();
			}
			catch (Exception ex)
			{
				manager?.Debug?.LogError("SendRouletteRollMetrics: Error sending metrics: " + ex.Message);
			}
		}
	}
}
