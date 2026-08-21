using System;

namespace TWDModel
{
	internal static class ReturnerAnalytics
	{
		private const string PlayerIdProperty = "player_id";

		private const string CouncilLevelProperty = "council_level";

		private const string HistoricalSpendProperty = "historical_spend";

		private const string TaskIdProperty = "task_id";

		private const string RewardContentProperty = "reward_content";

		private const string TokenAmountProperty = "token_amount";

		private const string ExtraDaysProperty = "extra_days";

		private const string RewardIdProperty = "reward_id";

		public static void SendState(TWDModelManager manager, int councilLevel)
		{
			if (manager?.Player != null && !string.IsNullOrEmpty(manager.Player.HashedId))
			{
				Send(manager, "Returner_state", delegate(TdMetrics metrics)
				{
					metrics.AddProperty("player_id", manager.Player.HashedId).AddProperty("council_level", councilLevel).AddProperty("historical_spend", GetHistoricalSpend(manager.Player.TotalUSDSpent));
				});
			}
		}

		public static void SendTask(TWDModelManager manager, int taskId)
		{
			Send(manager, "Returner_task", delegate(TdMetrics metrics)
			{
				metrics.AddProperty("task_id", taskId);
			});
		}

		public static void SendExchange(TWDModelManager manager, ReturnExchangeStoreDefinition definition)
		{
			if (definition != null)
			{
				Send(manager, "Returner_exchange", delegate(TdMetrics metrics)
				{
					metrics.AddProperty("reward_content", definition.Reward ?? string.Empty).AddProperty("token_amount", GetCurrencyCostAmount(definition));
				});
			}
		}

		public static void SendPerkActive(TWDModelManager manager, int extraDays)
		{
			Send(manager, "Returner_perk_active", delegate(TdMetrics metrics)
			{
				metrics.AddProperty("extra_days", extraDays);
			});
		}

		public static void SendLogin(TWDModelManager manager, int rewardId)
		{
			Send(manager, "Returner_Login", delegate(TdMetrics metrics)
			{
				metrics.AddProperty("reward_id", rewardId);
			});
		}

		private static void Send(TWDModelManager manager, string eventType, Action<TdMetrics> addProperties)
		{
			TdMetrics tdMetrics = manager?.TdMetrics;
			if (tdMetrics == null)
			{
				return;
			}
			TdMetrics tdMetrics2 = null;
			try
			{
				tdMetrics2 = (TdMetrics)tdMetrics.Clone();
				tdMetrics2.SetEventType(eventType);
				addProperties?.Invoke(tdMetrics2);
				tdMetrics2.Send();
			}
			catch (Exception ex)
			{
				tdMetrics2?.Reset();
				manager.Debug?.LogWarning("[ReturnerAnalytics] Failed to send " + eventType + ": " + ex.Message);
			}
		}

		private static int GetHistoricalSpend(double historicalSpend)
		{
			if (double.IsNaN(historicalSpend) || historicalSpend <= 0.0)
			{
				return 0;
			}
			if (!(historicalSpend >= 2147483647.0))
			{
				return (int)Math.Floor(historicalSpend);
			}
			return int.MaxValue;
		}

		private static int GetCurrencyCostAmount(ReturnExchangeStoreDefinition definition)
		{
			if (definition.CostRewardEntries?.RewardsList == null)
			{
				return 0;
			}
			long num = 0L;
			for (int i = 0; i < definition.CostRewardEntries.RewardsList.Count; i++)
			{
				if (definition.CostRewardEntries.RewardsList[i] is RewardCurrency { Amount: >0 } rewardCurrency)
				{
					num += rewardCurrency.Amount;
				}
			}
			if (num < int.MaxValue)
			{
				return (int)num;
			}
			return int.MaxValue;
		}
	}
}
