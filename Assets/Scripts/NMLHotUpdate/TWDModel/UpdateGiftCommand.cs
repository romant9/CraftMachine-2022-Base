using System;
using System.Collections.Generic;
using System.Reflection;
using BaseModel;

namespace TWDModel
{
	public class UpdateGiftCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager tWDModelManager)
			{
				if (tWDModelManager.Player.Blackboard.IsToggleOn("Toggle.ToggleUpdateGiftReceived"))
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				string updateGift = tWDModelManager.GameEconomyData.ConfigData.UpdateGift;
				if (string.IsNullOrEmpty(updateGift))
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				Rewards rewards = new Rewards(updateGift, null, 0, EquipmentSource.GuildGift);
				List<object> rewardsGiven = rewards.Give(tWDModelManager);
				SendMetrics(tWDModelManager, rewards, rewardsGiven, "AddUpdateGift");
				tWDModelManager.Player.Blackboard.SetToggle("Toggle.ToggleUpdateGiftReceived");
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}

		public void SendMetrics(TWDModelManager manager, Rewards rewards, List<object> rewardsGiven, string metricsContextMethodName)
		{
			MethodInfo method = typeof(Metrics).GetMethod(metricsContextMethodName);
			if (method == null)
			{
				throw new Exception("Cannot send Metrics with context '" + metricsContextMethodName + "'");
			}
			if (rewards == null || rewards.RewardsList == null || rewards.RewardsList.Count <= 0)
			{
				return;
			}
			Metrics.MetricsResourcesData metricsResourcesData = new Metrics.MetricsResourcesData();
			GameEconomyData gameEconomyData = manager.GameEconomyData;
			for (int i = 0; i < rewards.RewardsList.Count; i++)
			{
				IReward reward = rewards.RewardsList[i];
				if (reward.Type == RewardType.Equipment || reward.Type == RewardType.RandomEquipment)
				{
					if (rewardsGiven[i] is EquipmentItemModel equipment)
					{
						Metrics obj = manager.Metrics.AddFind().AddEquipment(equipment, "Equipment", (reward as RewardEquipment)?.Amount ?? 1);
						obj = method.Invoke(obj, null) as Metrics;
						obj.Send();
					}
				}
				else if (reward.Type == RewardType.Outfit)
				{
					string text = rewardsGiven[i] as string;
					if (!string.IsNullOrEmpty(text))
					{
						Metrics obj2 = manager.Metrics.AddFind().AddOutfit(gameEconomyData.GetOutfitDefinition(text));
						obj2 = method.Invoke(obj2, null) as Metrics;
						obj2.Send();
					}
				}
				else if (reward is RewardSurvivorSlot)
				{
					Metrics obj3 = manager.Metrics.AddFind().AddSurvivorSlot();
					obj3 = method.Invoke(obj3, null) as Metrics;
					obj3.Send();
				}
				else if (reward is RewardTimedBonus)
				{
					if (reward is RewardTimedBonus rewardTimedBonus)
					{
						Metrics obj4 = manager.Metrics.AddFind().AddTimedBonus(rewardTimedBonus);
						obj4 = method.Invoke(obj4, null) as Metrics;
						obj4.Send();
					}
				}
				else if (reward is RewardSurvivorClass)
				{
					SurvivorClass classUnlocked = (SurvivorClass)rewardsGiven[i];
					Metrics obj5 = manager.Metrics.AddFind().AddSurvivorClassUnlock(classUnlocked);
					obj5 = method.Invoke(obj5, null) as Metrics;
					obj5.Send();
				}
				else if (reward is RewardLootEntry || reward is RewardTradeCrate)
				{
					if (rewardsGiven[i] is LootEntry loot)
					{
						Metrics obj6 = manager.Metrics.AddFind().AddLoot(loot);
						obj6 = method.Invoke(obj6, null) as Metrics;
						obj6.AddLootCrate(loot);
						obj6.Send();
					}
				}
				else if (reward is RewardCurrency)
				{
					RewardCurrency rewardCurrency = reward as RewardCurrency;
					if (rewardCurrency.Amount > 0)
					{
						metricsResourcesData.SetOrAdd(rewardCurrency.CurrencyType, rewardCurrency.AmountActuallyAdded, rewardCurrency.GetOverflowAmount());
					}
					else if (rewardCurrency.Amount == -1)
					{
						int value = manager.Player.GetCurrency(rewardCurrency.CurrencyType).Value;
						int max = manager.Player.GetCurrency(rewardCurrency.CurrencyType).Max;
						metricsResourcesData.SetOrAdd(rewardCurrency.CurrencyType, max - value);
					}
				}
			}
			if (metricsResourcesData.HasResources())
			{
				Metrics obj7 = manager.Metrics.AddFind().AddResources(metricsResourcesData);
				obj7 = method.Invoke(obj7, null) as Metrics;
				obj7.Send();
			}
		}
	}
}
