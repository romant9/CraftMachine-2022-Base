using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class WeeklyChallengeClassTeamShopModel : TWDModelObject
	{
		public Dictionary<int, int> ExchangeBoughtCounts { get; set; }

		public Dictionary<int, bool> ExchangeReminderStates { get; set; }

		[JsonIgnore]
		public Rewards LastCloseExchangeRewards { get; private set; }

		public override bool IsValid()
		{
			return true;
		}

		public override void Initialize()
		{
			base.Initialize();
			ExchangeBoughtCounts = new Dictionary<int, int>();
			ExchangeReminderStates = new Dictionary<int, bool>();
		}

		public override void Start()
		{
			base.Start();
			EnsureStateInitialized();
			InitializeExchangeRewards();
		}

		public void InitializeExchangeRewards()
		{
			if (base.manager == null || base.manager.GameEconomyData == null)
			{
				return;
			}
			WeeklyChallengeClassTeamActivityModel weeklyChallengeClassTeamActivityModel = base.manager?.Player?.WeeklyChallengeClassTeamActivity;
			if (weeklyChallengeClassTeamActivityModel == null || weeklyChallengeClassTeamActivityModel.Id <= 0)
			{
				return;
			}
			List<ClassTeamExchangeDefinition> weeklyChallengeClassTeamChallengeExchanges = base.manager.GameEconomyData.GetWeeklyChallengeClassTeamChallengeExchanges(weeklyChallengeClassTeamActivityModel.Id);
			if (weeklyChallengeClassTeamChallengeExchanges != null)
			{
				for (int i = 0; i < weeklyChallengeClassTeamChallengeExchanges.Count; i++)
				{
					weeklyChallengeClassTeamChallengeExchanges[i].InitializeRewards(base.manager);
				}
			}
		}

		public void ResetForNewActivity()
		{
			EnsureStateInitialized();
			ExchangeBoughtCounts.Clear();
			ExchangeReminderStates.Clear();
			InitializeExchangeRewards();
		}

		public ClassTeamExchangeDefinition GetExchangeDefinition(int exchangeId)
		{
			if (base.manager == null || base.manager.GameEconomyData == null)
			{
				return null;
			}
			WeeklyChallengeClassTeamActivityModel weeklyChallengeClassTeamActivityModel = base.manager?.Player?.WeeklyChallengeClassTeamActivity;
			if (weeklyChallengeClassTeamActivityModel == null || weeklyChallengeClassTeamActivityModel.Id <= 0)
			{
				return null;
			}
			List<ClassTeamExchangeDefinition> weeklyChallengeClassTeamChallengeExchanges = base.manager.GameEconomyData.GetWeeklyChallengeClassTeamChallengeExchanges(weeklyChallengeClassTeamActivityModel.Id);
			if (weeklyChallengeClassTeamChallengeExchanges == null)
			{
				return null;
			}
			for (int i = 0; i < weeklyChallengeClassTeamChallengeExchanges.Count; i++)
			{
				if (weeklyChallengeClassTeamChallengeExchanges[i].ID == exchangeId)
				{
					EnsureExchangeDefinitionInitialized(weeklyChallengeClassTeamChallengeExchanges[i]);
					return weeklyChallengeClassTeamChallengeExchanges[i];
				}
			}
			return null;
		}

		public List<ClassTeamExchangeDefinition> GetExchangeDefinitions()
		{
			if (base.manager == null || base.manager.GameEconomyData == null)
			{
				return null;
			}
			WeeklyChallengeClassTeamActivityModel weeklyChallengeClassTeamActivityModel = base.manager?.Player?.WeeklyChallengeClassTeamActivity;
			if (weeklyChallengeClassTeamActivityModel == null || weeklyChallengeClassTeamActivityModel.Id <= 0)
			{
				return null;
			}
			List<ClassTeamExchangeDefinition> weeklyChallengeClassTeamChallengeExchanges = base.manager.GameEconomyData.GetWeeklyChallengeClassTeamChallengeExchanges(weeklyChallengeClassTeamActivityModel.Id);
			if (weeklyChallengeClassTeamChallengeExchanges != null)
			{
				for (int i = 0; i < weeklyChallengeClassTeamChallengeExchanges.Count; i++)
				{
					EnsureExchangeDefinitionInitialized(weeklyChallengeClassTeamChallengeExchanges[i]);
				}
			}
			return weeklyChallengeClassTeamChallengeExchanges;
		}

		public int GetBoughtCount(int exchangeId)
		{
			if (ExchangeBoughtCounts != null && ExchangeBoughtCounts.TryGetValue(exchangeId, out var value))
			{
				return value;
			}
			return 0;
		}

		public int GetRemainingCount(int exchangeId)
		{
			ClassTeamExchangeDefinition exchangeDefinition = GetExchangeDefinition(exchangeId);
			if (exchangeDefinition == null)
			{
				return 0;
			}
			if (exchangeDefinition.Limit < 0)
			{
				return -1;
			}
			return Math.Max(0, exchangeDefinition.Limit - GetBoughtCount(exchangeId));
		}

		public bool IsExchangeReminderEnabled(int exchangeId)
		{
			EnsureStateInitialized();
			if (ExchangeReminderStates != null && ExchangeReminderStates.TryGetValue(exchangeId, out var value))
			{
				return value;
			}
			return false;
		}

		public bool SetExchangeReminderEnabled(int exchangeId, bool enabled)
		{
			EnsureStateInitialized();
			if (GetExchangeDefinition(exchangeId) == null)
			{
				return false;
			}
			ExchangeReminderStates[exchangeId] = enabled;
			return true;
		}

		public bool ShouldShowExchangeRedPoint(int exchangeId)
		{
			WeeklyChallengeClassTeamActivityModel weeklyChallengeClassTeamActivityModel = base.manager?.Player?.WeeklyChallengeClassTeamActivity;
			if (weeklyChallengeClassTeamActivityModel == null || !weeklyChallengeClassTeamActivityModel.IsActive)
			{
				return false;
			}
			if (!IsExchangeReminderEnabled(exchangeId))
			{
				return false;
			}
			return CanExchange(exchangeId);
		}

		public bool HasAnyExchangeRedPoint()
		{
			List<ClassTeamExchangeDefinition> exchangeDefinitions = GetExchangeDefinitions();
			if (exchangeDefinitions == null)
			{
				return false;
			}
			for (int i = 0; i < exchangeDefinitions.Count; i++)
			{
				ClassTeamExchangeDefinition classTeamExchangeDefinition = exchangeDefinitions[i];
				if (classTeamExchangeDefinition != null && ShouldShowExchangeRedPoint(classTeamExchangeDefinition.ID))
				{
					return true;
				}
			}
			return false;
		}

		public bool CanExchange(int exchangeId)
		{
			WeeklyChallengeClassTeamActivityModel weeklyChallengeClassTeamActivityModel = base.manager?.Player?.WeeklyChallengeClassTeamActivity;
			if (weeklyChallengeClassTeamActivityModel == null || !weeklyChallengeClassTeamActivityModel.IsActive)
			{
				return false;
			}
			ClassTeamExchangeDefinition exchangeDefinition = GetExchangeDefinition(exchangeId);
			if (exchangeDefinition == null)
			{
				return false;
			}
			if (GetRemainingCount(exchangeId) == 0)
			{
				return false;
			}
			if (!HasValidCost(exchangeDefinition))
			{
				return false;
			}
			return BuildCashier(exchangeDefinition).CanAfford();
		}

		public TWDModelResult Exchange(int exchangeId)
		{
			WeeklyChallengeClassTeamActivityModel weeklyChallengeClassTeamActivityModel = base.manager?.Player?.WeeklyChallengeClassTeamActivity;
			if (weeklyChallengeClassTeamActivityModel == null || !weeklyChallengeClassTeamActivityModel.IsActive)
			{
				return TWDModelResult.Error;
			}
			ClassTeamExchangeDefinition exchangeDefinition = GetExchangeDefinition(exchangeId);
			if (exchangeDefinition == null)
			{
				return TWDModelResult.Error;
			}
			if (GetRemainingCount(exchangeId) == 0)
			{
				return TWDModelResult.Error;
			}
			if (!HasValidCost(exchangeDefinition))
			{
				return TWDModelResult.Error;
			}
			Rewards rewards = null;
			if (!string.IsNullOrEmpty(exchangeDefinition.Content))
			{
				try
				{
					rewards = new Rewards(exchangeDefinition.Content, base.manager);
				}
				catch (Exception)
				{
					return TWDModelResult.Error;
				}
			}
			Cashier cashier = BuildCashier(exchangeDefinition);
			cashier.UseDiamondsAmount = -2;
			TWDModelResult tWDModelResult = cashier.Pay();
			if (tWDModelResult != TWDModelResult.OK)
			{
				return tWDModelResult;
			}
			rewards?.Give(base.manager);
			if (ExchangeBoughtCounts == null)
			{
				ExchangeBoughtCounts = new Dictionary<int, int>();
			}
			ExchangeBoughtCounts.TryGetValue(exchangeId, out var value);
			ExchangeBoughtCounts[exchangeId] = value + 1;
			base.manager.TdMetrics.SetEventType("Class_Team_Challenge_Exchange").AddProperty("exchange_content", exchangeDefinition.Content).AddProperty("exchange_time", value + 1)
				.Send();
			base.manager.Metrics.AddClassTeamExchange(exchangeDefinition.Content, value + 1).Send();
			return TWDModelResult.OK;
		}

		public void ExchangeOnClose(int challengeId)
		{
			LastCloseExchangeRewards = null;
			if (base.manager == null || base.manager.GameEconomyData == null || base.manager.Player == null)
			{
				return;
			}
			ClassTeamDefinition classTeamDefinition = base.manager.GameEconomyData.GetClassTeamDefinition(challengeId);
			if (classTeamDefinition == null || classTeamDefinition.StarCurrencyType == CurrencyType.None)
			{
				return;
			}
			List<ClassTeamExchangeDefinition> weeklyChallengeClassTeamChallengeExchanges = base.manager.GameEconomyData.GetWeeklyChallengeClassTeamChallengeExchanges(challengeId);
			if (weeklyChallengeClassTeamChallengeExchanges == null)
			{
				return;
			}
			ClassTeamExchangeDefinition classTeamExchangeDefinition = null;
			for (int i = 0; i < weeklyChallengeClassTeamChallengeExchanges.Count; i++)
			{
				if (weeklyChallengeClassTeamChallengeExchanges[i].IsCloseExchange)
				{
					classTeamExchangeDefinition = weeklyChallengeClassTeamChallengeExchanges[i];
					break;
				}
			}
			if (classTeamExchangeDefinition == null)
			{
				return;
			}
			classTeamExchangeDefinition.InitializeRewards(base.manager);
			int num = 0;
			if (classTeamExchangeDefinition.CostRewards != null && classTeamExchangeDefinition.CostRewards.RewardsList != null)
			{
				for (int j = 0; j < classTeamExchangeDefinition.CostRewards.RewardsList.Count; j++)
				{
					if (classTeamExchangeDefinition.CostRewards.RewardsList[j] is RewardCurrency rewardCurrency)
					{
						num = rewardCurrency.Amount;
						break;
					}
				}
			}
			if (num <= 0)
			{
				return;
			}
			CurrencyModel currency = base.manager.Player.GetCurrency(classTeamDefinition.StarCurrencyType);
			if (currency == null || currency.Value <= 0)
			{
				return;
			}
			Rewards rewards = null;
			if (!string.IsNullOrEmpty(classTeamExchangeDefinition.Content))
			{
				try
				{
					rewards = new Rewards(classTeamExchangeDefinition.Content, base.manager);
				}
				catch (Exception)
				{
					return;
				}
			}
			int num2 = (currency.Value + num - 1) / num;
			currency.Subtract(currency.Value);
			if (rewards != null)
			{
				Rewards rewards2 = new Rewards();
				for (int k = 0; k < num2; k++)
				{
					rewards.Give(base.manager);
					rewards2.MergeCurrencies(rewards);
					base.manager.TdMetrics.SetEventType("Class_Team_Challenge_Exchange").AddProperty("exchange_content", classTeamExchangeDefinition.Content).AddProperty("exchange_time", k + 1)
						.Send();
					base.manager.Metrics.AddClassTeamExchange(classTeamExchangeDefinition.Content, k + 1).Send();
				}
				LastCloseExchangeRewards = rewards2;
			}
		}

		private Cashier BuildCashier(ClassTeamExchangeDefinition def)
		{
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.None);
			if (def.CostRewards != null && def.CostRewards.RewardsList != null)
			{
				for (int i = 0; i < def.CostRewards.RewardsList.Count; i++)
				{
					if (def.CostRewards.RewardsList[i] is RewardCurrency rewardCurrency)
					{
						cashierItem.SetCost(rewardCurrency.CurrencyType, rewardCurrency.Amount);
					}
				}
			}
			cashier.AddItem(cashierItem);
			return cashier;
		}

		private void EnsureStateInitialized()
		{
			if (ExchangeBoughtCounts == null)
			{
				ExchangeBoughtCounts = new Dictionary<int, int>();
			}
			if (ExchangeReminderStates == null)
			{
				ExchangeReminderStates = new Dictionary<int, bool>();
			}
		}

		private void EnsureExchangeDefinitionInitialized(ClassTeamExchangeDefinition def)
		{
			if (def != null && base.manager != null && (def.ContentRewards == null || def.CostRewards == null))
			{
				def.InitializeRewards(base.manager);
			}
		}

		private bool HasValidCost(ClassTeamExchangeDefinition def)
		{
			if (def == null)
			{
				return false;
			}
			EnsureExchangeDefinitionInitialized(def);
			if (def.CostRewards == null || def.CostRewards.RewardsList == null)
			{
				return false;
			}
			for (int i = 0; i < def.CostRewards.RewardsList.Count; i++)
			{
				if (def.CostRewards.RewardsList[i] is RewardCurrency { Amount: >0 })
				{
					return true;
				}
			}
			return false;
		}
	}
}
