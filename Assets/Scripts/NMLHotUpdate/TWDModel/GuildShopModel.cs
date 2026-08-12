using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class GuildShopModel : TWDModelObject
	{
		public const string GuildShopRestocked = "GuildShopRestocked";

		public int CurrentSeason;

		public Dictionary<int, GuildShopItemInfo> GuildShopAvailableItems;

		public bool InitializedThisSeason;

		public int RandomSeed;

		public int HighestTierUnlocked;

		public override void Initialize()
		{
			base.Initialize();
			CurrentSeason = -1;
			GuildShopAvailableItems = new Dictionary<int, GuildShopItemInfo>();
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void Start()
		{
			base.Start();
			SetupGuildShop();
		}

		public void StartForNewSeason()
		{
			GuildShopAvailableItems.Clear();
			HighestTierUnlocked = base.manager.GameEconomyData.GuildWarConfig.GuildBattleMinimumTier;
			CurrentSeason = base.manager.Player.GvGSeasonModelPlayer.StartedGvGSeasonId;
			RandomSeed = (int)base.manager.Player.LifeTime;
			SetupGuildShop();
			UpdateGuildShopItemsOnNewTier();
		}

		public void UpdateGuildShopItemsOnNewTier()
		{
			GuildModel guildModel = base.manager.Player.GuildModel;
			int warPositionInSeason = base.manager.GameEconomyData.GetWarPositionInSeason(guildModel.GvGSeasonModel.SeasonDefinitionId, guildModel.GuildWarModel.WarDefinitionId);
			int guildBattleTier = guildModel.GuildBattleTier;
			int num = HighestTierUnlocked - guildBattleTier;
			if (num > 0)
			{
				HighestTierUnlocked = guildBattleTier;
			}
			foreach (GuildShopItemInfo value in GuildShopAvailableItems.Values)
			{
				int num2 = value.ItemDefinition.TierRequirement - HighestTierUnlocked;
				if (num2 < 0)
				{
					continue;
				}
				if (!value.Unlocked)
				{
					value.Unlocked = true;
					value.RestockNewTier(num2);
					if (warPositionInSeason > 0)
					{
						value.RestockNewWar(warPositionInSeason);
					}
				}
				else if (num > 0)
				{
					value.RestockNewTier(num);
				}
			}
			NotifyChange("GuildShopRestocked");
			InitializedThisSeason = true;
		}

		public void RestockGuildShopItems(bool onNewTier, bool onNewWar, int times = 1)
		{
			foreach (GuildShopItemInfo value in GuildShopAvailableItems.Values)
			{
				if (value.Unlocked)
				{
					if (onNewTier)
					{
						value.RestockNewTier(times);
					}
					if (onNewWar)
					{
						value.RestockNewWar(times);
					}
				}
			}
			NotifyChange("GuildShopRestocked");
		}

		public void MarkItemsAsSeen(List<int> itemsIds)
		{
			for (int i = 0; i < itemsIds.Count; i++)
			{
				if (GuildShopAvailableItems.ContainsKey(itemsIds[i]))
				{
					GuildShopAvailableItems[itemsIds[i]].Seen = true;
				}
			}
		}

		public void SetupGuildShop()
		{
			if (GuildShopAvailableItems == null)
			{
				GuildShopAvailableItems = new Dictionary<int, GuildShopItemInfo>();
			}
			int lastStartedSeasonId = base.manager.Player.GvGSeasonModelPlayer.LastStartedSeasonId;
			if (lastStartedSeasonId != CurrentSeason)
			{
				GuildShopAvailableItems.Clear();
				HighestTierUnlocked = base.manager.GameEconomyData.GuildWarConfig.GuildBattleMinimumTier;
				InitializedThisSeason = false;
			}
			if (lastStartedSeasonId < 0 || base.gameEconomyData.GuildShopDefinitions == null)
			{
				return;
			}
			GuildShopDefinition[] guildShopDefinitions = base.gameEconomyData.GuildShopDefinitions;
			foreach (GuildShopDefinition guildShopDefinition in guildShopDefinitions)
			{
				if (guildShopDefinition.Season == lastStartedSeasonId && !GuildShopAvailableItems.ContainsKey(guildShopDefinition.ID))
				{
					GuildShopAvailableItems.Add(guildShopDefinition.ID, new GuildShopItemInfo(guildShopDefinition));
				}
				else if (GuildShopAvailableItems.ContainsKey(guildShopDefinition.ID))
				{
					GuildShopAvailableItems[guildShopDefinition.ID].ItemDefinition = guildShopDefinition;
				}
			}
		}

		public List<GuildShopDefinition> GetUnlocksForTier(int tier)
		{
			List<GuildShopDefinition> list = new List<GuildShopDefinition>();
			if (HighestTierUnlocked > tier)
			{
				GuildShopItemInfo[] array = GuildShopAvailableItems.Values.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].ItemDefinition.TierRequirement == tier)
					{
						list.Add(array[i].ItemDefinition);
					}
				}
			}
			return list;
		}

		public bool HasNewItems()
		{
			GuildShopItemInfo[] array = GuildShopAvailableItems.Values.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Unlocked && !array[i].Seen)
				{
					return true;
				}
			}
			return false;
		}

		public int GetSlotsWithStockCount()
		{
			int num = 0;
			GuildShopItemInfo[] array = GuildShopAvailableItems.Values.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Unlocked && array[i].AvailableAmount > 0)
				{
					num++;
				}
			}
			return num;
		}

		public Cashier GetCashierForItem(GuildShopDefinition itemDefinition)
		{
			Cashier result = null;
			if (itemDefinition != null)
			{
				result = Cashier.CreateOneItemCashier(base.manager, PurchaseType.TradeCrate, itemDefinition.PriceCurrency, itemDefinition.PriceAmount);
			}
			return result;
		}

		public bool HasAnyAffordableItem()
		{
			int currencyAmount = base.manager.Player.GetCurrencyAmount(CurrencyType.GuildBattleRP);
			foreach (GuildShopItemInfo value in GuildShopAvailableItems.Values)
			{
				if (value.Unlocked && value.AvailableAmount > 0 && currencyAmount >= value.ItemDefinition.PriceAmount)
				{
					return true;
				}
			}
			return false;
		}
	}
}
