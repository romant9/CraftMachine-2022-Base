using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class HillTopStore : TWDModelObject
	{
		public bool ContentInitialized;

		public List<HillTopStoreSlot> HistorySlots;

		public List<HillTopStoreSlot> Slots;

		public List<int> PurchaseHistory;

		[JsonIgnore]
		private PlayerModel PlayerModel => base.manager.Player;

		public override bool IsValid()
		{
			return true;
		}

		public override void Start()
		{
			base.Start();
		}

		public void Init()
		{
			if (!ContentInitialized)
			{
				ContentInitialized = true;
				if (PurchaseHistory == null)
				{
					PurchaseHistory = new List<int>();
				}
				UpdateSlots();
			}
		}

		public void UpdateSlots()
		{
			if (!ContentInitialized)
			{
				return;
			}
			List<HillTopStoreSlotDefinition> list = PlayerModel.gameEconomyData.HillTopStoreSlotDefinitions.OrderBy((HillTopStoreSlotDefinition x) => x.SlotId).ToList();
			if (HistorySlots == null)
			{
				HistorySlots = new List<HillTopStoreSlot>();
			}
			foreach (HillTopStoreSlotDefinition hillTopStoreSlotDefinition in list)
			{
				if (HistorySlots.FirstOrDefault((HillTopStoreSlot x) => x.SlotType == hillTopStoreSlotDefinition.SlotType) == null)
				{
					HillTopStoreSlot hillTopStoreSlot = new HillTopStoreSlot();
					hillTopStoreSlot.SlotType = hillTopStoreSlotDefinition.SlotType;
					hillTopStoreSlot.SetManager(base.manager);
					hillTopStoreSlot.Initialize();
					HistorySlots.Add(hillTopStoreSlot);
				}
			}
			Slots = new List<HillTopStoreSlot>();
			foreach (HillTopStoreSlotDefinition hillTopStoreSlotDefinition2 in list)
			{
				HillTopStoreSlot hillTopStoreSlot2 = HistorySlots.FirstOrDefault((HillTopStoreSlot x) => x.SlotType == hillTopStoreSlotDefinition2.SlotType);
				if (hillTopStoreSlot2 != null)
				{
					hillTopStoreSlot2.SetManager(base.manager);
					Slots.Add(hillTopStoreSlot2);
				}
			}
		}

		private Func<int, HillTopStoreSlotDefinition> GetOrderedSlotDefinitionFunc()
		{
			List<HillTopStoreSlotDefinition> hillTopStoreSlotDefinitions = PlayerModel.gameEconomyData.HillTopStoreSlotDefinitions.OrderBy((HillTopStoreSlotDefinition x) => x.SlotId).ToList();
			return (int x) => hillTopStoreSlotDefinitions[x];
		}

		public void AddToPurchaseHistory(HillTopStoreDefinition hillTopStoreDefinition)
		{
			PurchaseHistory.Add(hillTopStoreDefinition.UniqueId);
			foreach (HillTopStoreSlot slot in Slots)
			{
				if (slot.SlotType == hillTopStoreDefinition.SlotType)
				{
					slot.AddToPurchaseHistory(hillTopStoreDefinition.UniqueId);
					return;
				}
			}
			base.Debug.LogError($"Slot for {hillTopStoreDefinition.UniqueId} not found");
		}

		public TWDModelResult GiveReward(HillTopStoreDefinition hillTopStoreDefinition)
		{
			IReward reward = new Rewards(hillTopStoreDefinition.Reward).RewardsList[0];
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
						if (!(reward is RewardHeroSkin rewardHeroSkin))
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
							base.manager.Metrics.AddFind().AddEquipToken(rewardEquipToken).AddHillTopStore(hillTopStoreDefinition)
								.Send();
						}
						else
						{
							rewardHeroSkin.Give(base.manager);
						}
					}
					else
					{
						rewardRandomEquipment.EquipmentSource = EquipmentSource.HillTopStore;
						ModelRandom modelRandom = new ModelRandom(hillTopStoreDefinition.UniqueId + PurchaseHistory.Count + PlayerModel.GetCurrencyAmount(CurrencyType.HillTopCoin));
						if (!(rewardRandomEquipment.Give(base.manager, new object[1] { modelRandom }) is EquipmentItemModel equipmentItemModel))
						{
							return TWDModelResult.Error;
						}
						PlayerModel.LootManager.LastTradedEquipment = equipmentItemModel;
						base.manager.Metrics.AddFind().AddEquipment(equipmentItemModel).AddHillTopStore(hillTopStoreDefinition)
							.Send();
					}
				}
				else
				{
					rewardEquipment.EquipmentSource = EquipmentSource.HillTopStore;
					ModelRandom modelRandom2 = new ModelRandom(hillTopStoreDefinition.UniqueId + PurchaseHistory.Count + PlayerModel.GetCurrencyAmount(CurrencyType.HillTopCoin));
					if (!(rewardEquipment.Give(base.manager, new object[1] { modelRandom2 }) is EquipmentItemModel equipmentItemModel2))
					{
						return TWDModelResult.Error;
					}
					PlayerModel.LootManager.LastTradedEquipment = equipmentItemModel2;
					base.manager.Metrics.AddFind().AddEquipment(equipmentItemModel2, "Equipment", rewardEquipment.Amount).AddHillTopStore(hillTopStoreDefinition)
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
				base.manager.Metrics.AddFind().AddResources(rewardCurrency.CurrencyType, lootEntry.RewardedAmount, lootEntry.ActualAmountAdded).AddHillTopStore(hillTopStoreDefinition)
					.Send();
			}
			return TWDModelResult.OK;
		}

		public bool CanPurchaseItem(HillTopStoreDefinition hillTopStoreDefinition)
		{
			int num = Slots.ToList().FindIndex((HillTopStoreSlot x) => x.SlotType == hillTopStoreDefinition.SlotType);
			if (num < 0)
			{
				base.Debug.LogError($"Slot for {hillTopStoreDefinition.SlotType} not found");
				return false;
			}
			return Slots[num].GetPurchaseCount(hillTopStoreDefinition.UniqueId) < hillTopStoreDefinition.LimitNum;
		}
	}
}
