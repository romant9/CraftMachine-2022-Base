using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class BlackMarket : TWDModelObject
	{
		public List<string> HeroHistory;

		public BlackMarketHeroSlot[] Slots;

		public List<int> PurchaseHistory;

		public bool ContentInitialized;

		public int LastAmountMissingTokensGiven;

		[JsonIgnore]
		private PlayerModel PlayerModel => base.manager.Player;

		public override bool IsValid()
		{
			return true;
		}

		public bool NeedToUpdate()
		{
			BlackMarketHeroSlot[] slots = Slots;
			for (int i = 0; i < slots.Length; i++)
			{
				if (slots[i].ShouldUpdate(PlayerModel))
				{
					return true;
				}
			}
			return false;
		}

		public void UpdateSlots()
		{
			if (Slots == null)
			{
				return;
			}
			BlackMarketHeroSlot[] slots = Slots;
			foreach (BlackMarketHeroSlot blackMarketHeroSlot in slots)
			{
				if (blackMarketHeroSlot.ShouldUpdate(PlayerModel))
				{
					blackMarketHeroSlot.UpdateSlot(PlayerModel, HeroHistory, refreshTime: true);
				}
			}
			PlayerModel.Blackboard.SetToggle("Toggle.ToggleBlackMarketSlotUpdated");
		}

		public override void Start()
		{
			base.Start();
			InitializeSlots();
		}

		private void InitializeSlots()
		{
			if (Slots == null)
			{
				return;
			}
			string[] array = new string[3]
			{
				PlayerModel.gameEconomyData.ConfigData.BlackMarketRefreshTimerSlot1,
				PlayerModel.gameEconomyData.ConfigData.BlackMarketRefreshTimerSlot2,
				PlayerModel.gameEconomyData.ConfigData.BlackMarketRefreshTimerSlot3
			};
			for (int i = 0; i < Slots.Length; i++)
			{
				if (Slots[i] == null)
				{
					Slots[i] = new BlackMarketHeroSlot();
				}
				Slots[i].SetRefreshTimeSpan(array[i]);
				Slots[i].SetLockdown(PlayerModel.gameEconomyData.ConfigData.BlackMarketPlayerRefreshLockTime);
			}
		}

		public void Init()
		{
			if (!ContentInitialized)
			{
				ContentInitialized = true;
				if (HeroHistory == null)
				{
					HeroHistory = new List<string>();
				}
				if (PurchaseHistory == null)
				{
					PurchaseHistory = new List<int>();
				}
				if (Slots == null)
				{
					Slots = new BlackMarketHeroSlot[3];
				}
				InitializeSlots();
				UpdateSlots();
			}
		}

		public bool RefreshHero(string actorId, bool forceRefresh = false)
		{
			int num = Slots.ToList().FindIndex((BlackMarketHeroSlot x) => x.ActiveActorDefinitionID == actorId);
			if (num < 0)
			{
				base.Debug.LogError("Slot for " + actorId + " not found");
				return false;
			}
			BlackMarketHeroSlot blackMarketHeroSlot = Slots[num];
			if (blackMarketHeroSlot == null)
			{
				return false;
			}
			if (!forceRefresh && !blackMarketHeroSlot.CanRefresh(PlayerModel))
			{
				return false;
			}
			blackMarketHeroSlot.UpdateSlot(PlayerModel, HeroHistory, refreshTime: false);
			return true;
		}

		public void AddToPurchaseHistory(BlackMarketDefinition blackMarketDefinition)
		{
			PurchaseHistory.Add(blackMarketDefinition.UniqueId);
			BlackMarketHeroSlot[] slots = Slots;
			foreach (BlackMarketHeroSlot blackMarketHeroSlot in slots)
			{
				if (blackMarketHeroSlot.ActiveActorDefinitionID == blackMarketDefinition.ActorDefinitionID)
				{
					blackMarketHeroSlot.AddToPurchaseHistory(blackMarketDefinition.UniqueId);
					return;
				}
			}
			base.Debug.LogError("Slot for " + blackMarketDefinition.ActorDefinitionID + " not found");
		}

		public TWDModelResult GiveReward(BlackMarketDefinition blackMarketDefinition)
		{
			IReward reward = new Rewards(blackMarketDefinition.Reward).RewardsList[0];
			if (reward == null)
			{
				return TWDModelResult.Error;
			}
			if (!(reward is RewardCurrency rewardCurrency))
			{
				if (!(reward is RewardEquipment rewardEquipment))
				{
					if (!(reward is RewardRandomEquipment rewardRandomEquipment))
					{
						if (!(reward is RewardOutfit rewardOutfit))
						{
							if (!(reward is RewardTimedBonus rewardTimedBonus))
							{
								if (!(reward is RewardMissingTokens rewardMissingTokens))
								{
									if (!(reward is RewardEquipToken rewardEquipToken))
									{
										base.Debug.LogError("Black market reward not recognized");
										return TWDModelResult.Error;
									}
									if (rewardEquipToken.Give(base.manager) == null)
									{
										return TWDModelResult.Error;
									}
									base.manager.Metrics.AddFind().AddEquipToken(rewardEquipToken).AddBlackMarket(blackMarketDefinition)
										.Send();
									base.manager.TdMetrics.SetEventType("BlackMarketConsumption").AddProperty("resource_id", rewardEquipToken?.EquipTokenId.ToString()).AddProperty("resource_num", 1)
										.AddProperty("diamonds_used", blackMarketDefinition.GetCurrencyType().ToString())
										.AddProperty("diamonds_used_num", blackMarketDefinition.GetPrice(base.manager))
										.Send();
								}
								else
								{
									if (rewardMissingTokens.Give(base.manager) == null)
									{
										return TWDModelResult.Error;
									}
									base.manager.Metrics.AddFind().AddResources(rewardMissingTokens.RewardCurrencyType, LastAmountMissingTokensGiven, LastAmountMissingTokensGiven).AddBlackMarket(blackMarketDefinition)
										.Send();
									base.manager.TdMetrics.SetEventType("BlackMarketConsumption").AddProperty("resource_id", rewardMissingTokens.RewardCurrencyType.ToString()).AddProperty("resource_num", LastAmountMissingTokensGiven)
										.AddProperty("diamonds_used", blackMarketDefinition.GetCurrencyType().ToString())
										.AddProperty("diamonds_used_num", blackMarketDefinition.GetPrice(base.manager))
										.Send();
								}
							}
							else
							{
								if (rewardTimedBonus.Give(base.manager) == null)
								{
									return TWDModelResult.Error;
								}
								base.manager.Metrics.AddFind().AddTimedBonus(rewardTimedBonus).AddBlackMarket(blackMarketDefinition)
									.Send();
								base.manager.TdMetrics.SetEventType("BlackMarketConsumption").AddProperty("resource_id", rewardTimedBonus.TimedBonusType.ToString()).AddProperty("resource_num", rewardTimedBonus.Duration.ToString())
									.AddProperty("diamonds_used", blackMarketDefinition.GetCurrencyType().ToString())
									.AddProperty("diamonds_used_num", blackMarketDefinition.GetPrice(base.manager))
									.Send();
							}
						}
						else
						{
							string text = rewardOutfit.Give(base.manager) as string;
							OutfitDefinition outfitDefinition = PlayerModel.gameEconomyData.GetOutfitDefinition(text);
							if (string.IsNullOrEmpty(text))
							{
								return TWDModelResult.Error;
							}
							base.manager.Metrics.AddFind().AddOutfit(outfitDefinition).AddBlackMarket(blackMarketDefinition)
								.Send();
							base.manager.TdMetrics.SetEventType("BlackMarketConsumption").AddProperty("resource_id", outfitDefinition?.ID).AddProperty("resource_num", 1)
								.AddProperty("diamonds_used", blackMarketDefinition.GetCurrencyType().ToString())
								.AddProperty("diamonds_used_num", blackMarketDefinition.GetPrice(base.manager))
								.Send();
						}
					}
					else
					{
						rewardRandomEquipment.EquipmentSource = EquipmentSource.BlackMarket;
						ModelRandom modelRandom = new ModelRandom(blackMarketDefinition.UniqueId + PurchaseHistory.Count + PlayerModel.GetCurrencyAmount(CurrencyType.Diamonds));
						if (!(rewardRandomEquipment.Give(base.manager, new object[1] { modelRandom }) is EquipmentItemModel equipmentItemModel))
						{
							return TWDModelResult.Error;
						}
						PlayerModel.LootManager.LastTradedEquipment = equipmentItemModel;
						base.manager.Metrics.AddFind().AddEquipment(equipmentItemModel).AddBlackMarket(blackMarketDefinition)
							.Send();
						base.manager.TdMetrics.SetEventType("BlackMarketConsumption").AddProperty("resource_id", equipmentItemModel?.Definition.ID).AddProperty("resource_num", 1)
							.AddProperty("diamonds_used", blackMarketDefinition.GetCurrencyType().ToString())
							.AddProperty("diamonds_used_num", blackMarketDefinition.GetPrice(base.manager))
							.Send();
					}
				}
				else
				{
					rewardEquipment.EquipmentSource = EquipmentSource.BlackMarket;
					ModelRandom modelRandom2 = new ModelRandom(blackMarketDefinition.UniqueId + PurchaseHistory.Count + PlayerModel.GetCurrencyAmount(CurrencyType.Diamonds));
					if (!(rewardEquipment.Give(base.manager, new object[1] { modelRandom2 }) is EquipmentItemModel equipmentItemModel2))
					{
						return TWDModelResult.Error;
					}
					PlayerModel.LootManager.LastTradedEquipment = equipmentItemModel2;
					base.manager.Metrics.AddFind().AddEquipment(equipmentItemModel2, "Equipment", rewardEquipment.Amount).AddBlackMarket(blackMarketDefinition)
						.Send();
					base.manager.TdMetrics.SetEventType("BlackMarketConsumption").AddProperty("resource_id", rewardEquipment.EquipmentId).AddProperty("resource_num", rewardEquipment.Amount)
						.AddProperty("diamonds_used", blackMarketDefinition.GetCurrencyType().ToString())
						.AddProperty("diamonds_used_num", blackMarketDefinition.GetPrice(base.manager))
						.Send();
				}
			}
			else
			{
				LootEntry lootEntry = PlayerModel.LootManager.CreateCurrencyLoot(rewardCurrency.CurrencyType, rewardCurrency.Amount, DropType.None, DropCurrenciesProbabilitiesDefinition.DropCurrency.AnyCurrency);
				if (lootEntry == null)
				{
					return TWDModelResult.Error;
				}
				PlayerModel.LootManager.GiveLoot(lootEntry);
				base.manager.Metrics.AddFind().AddResources(rewardCurrency.CurrencyType, lootEntry.RewardedAmount, lootEntry.ActualAmountAdded).AddBlackMarket(blackMarketDefinition)
					.Send();
				base.manager.TdMetrics.SetEventType("BlackMarketConsumption").AddProperty("resource_id", rewardCurrency.CurrencyType.ToString()).AddProperty("resource_num", lootEntry.RewardedAmount)
					.AddProperty("diamonds_used", blackMarketDefinition.GetCurrencyType().ToString())
					.AddProperty("diamonds_used_num", blackMarketDefinition.GetPrice(base.manager))
					.Send();
			}
			int num = blackMarketDefinition.BlackMarketToken;
			if (reward is RewardMissingTokens)
			{
				num *= base.manager.Player.BlackMarket.LastAmountMissingTokensGiven;
			}
			base.manager.Player.GetCurrency(CurrencyType.BlackMarketToken).Add(num);
			base.manager.Metrics.AddFind().AddResources(CurrencyType.BlackMarketToken, num, num).AddBlackMarket(blackMarketDefinition)
				.Send();
			return TWDModelResult.OK;
		}

		public bool CanPurchaseItem(BlackMarketDefinition blackMarketDefinition)
		{
			if (!blackMarketDefinition.CanBePurchasedAgain && PurchaseHistory.Contains(blackMarketDefinition.UniqueId))
			{
				return false;
			}
			int num = Slots.ToList().FindIndex((BlackMarketHeroSlot x) => x.ActiveActorDefinitionID == blackMarketDefinition.ActorDefinitionID);
			if (num < 0)
			{
				base.Debug.LogError("Slot for " + blackMarketDefinition.ActorDefinitionID + " not found");
				return false;
			}
			if (blackMarketDefinition.GetPrice(base.manager) < 0)
			{
				return false;
			}
			return Slots[num].GetPurchaseCount(blackMarketDefinition.UniqueId) < blackMarketDefinition.Quantity;
		}

		public bool IsUniqueItemAlreadySold(BlackMarketDefinition blackMarketDefinition)
		{
			if (!blackMarketDefinition.CanBePurchasedAgain)
			{
				return PurchaseHistory.Contains(blackMarketDefinition.UniqueId);
			}
			return false;
		}
	}
}
