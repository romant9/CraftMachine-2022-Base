using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class BlackMarketHeroSlot
	{
		public string ActiveActorDefinitionID;

		public long NextUpdate;

		public List<int> ActiveOfferIds;

		public int RefreshCounter;

		public List<int> SessionPurchaseHistory;

		[JsonIgnore]
		private TimeSpan oneDay = new TimeSpan(1, 0, 0, 0);

		[JsonIgnore]
		private long refreshTimeSpanAsLong;

		[JsonIgnore]
		private TimeSpan refreshTimeSpan;

		[JsonIgnore]
		private long refreshLockTime;

		public void SetRefreshTimeSpan(string refreshTimeSpanAsString)
		{
			refreshTimeSpan = TimeSpan.Parse(refreshTimeSpanAsString);
			refreshTimeSpanAsLong = (long)refreshTimeSpan.TotalMilliseconds;
		}

		public bool ShouldUpdate(PlayerModel playerModel)
		{
			return playerModel.UtcTimeStamp > NextUpdate;
		}

		public void UpdateSlot(PlayerModel playerModel, List<string> history, bool refreshTime)
		{
			if (history.Count > playerModel.gameEconomyData.ConfigData.BlackMarketHeroHistorySize)
			{
				history.RemoveAt(0);
			}
			List<BlackMarketHeroDefinition> possibleHeroes = GetPossibleHeroes(playerModel, history);
			SelectRandomHero(playerModel, history, possibleHeroes);
			UpdateOffers(playerModel);
			SessionPurchaseHistory?.Clear();
			if (refreshTime)
			{
				UpdateTimer(playerModel);
			}
			else
			{
				RefreshCounter++;
			}
		}

		private List<BlackMarketHeroDefinition> GetPossibleHeroes(PlayerModel playerModel, List<string> history)
		{
			return playerModel.gameEconomyData.BlackMarketHeroDefinitions.Where((BlackMarketHeroDefinition x) => !history.Contains(x.ActorDefinitionID)).ToList();
		}

		private void SelectRandomHero(PlayerModel playerModel, List<string> history, List<BlackMarketHeroDefinition> possibleActors)
		{
			BlackMarketHeroDefinition randomWeighted = HelpersModel.GetRandomWeighted(playerModel, possibleActors);
			if (randomWeighted != null)
			{
				ActiveActorDefinitionID = randomWeighted.ActorDefinitionID;
				history.Add(ActiveActorDefinitionID);
			}
		}

		private void UpdateTimer(PlayerModel playerModel)
		{
			DateTime utcTime = playerModel.UtcTime;
			DateTime dateTime = new DateTime(utcTime.Year, utcTime.Month, utcTime.Day);
			for (NextUpdate = dateTime.TotalMilliseconds() + refreshTimeSpanAsLong - (long)oneDay.TotalMilliseconds; NextUpdate < playerModel.UtcTimeStamp; NextUpdate += (long)oneDay.TotalMilliseconds)
			{
			}
			RefreshCounter = 0;
		}

		private void UpdateOffers(PlayerModel playerModel)
		{
			IOrderedEnumerable<BlackMarketSlotDefinition> slotsForActor = GetSlotsForActor(playerModel);
			ActiveOfferIds = new List<int>();
			foreach (BlackMarketSlotDefinition item in slotsForActor)
			{
				List<BlackMarketDefinition> possibleOffers = GetPossibleOffers(playerModel, item);
				BlackMarketDefinition randomWeighted = HelpersModel.GetRandomWeighted(playerModel, possibleOffers);
				if (randomWeighted != null)
				{
					ActiveOfferIds.Add(randomWeighted.UniqueId);
				}
			}
		}

		private IOrderedEnumerable<BlackMarketSlotDefinition> GetSlotsForActor(PlayerModel playerModel)
		{
			return from x in playerModel.gameEconomyData.BlackMarketSlotDefinitions
				where x.ActorDefinitionID == ActiveActorDefinitionID
				orderby x.SlotId
				select x;
		}

		private List<BlackMarketDefinition> GetPossibleOffers(PlayerModel playerModel, BlackMarketSlotDefinition slotDefinition)
		{
			SurvivorModel survivorModel = playerModel.SurvivorContainer.GetSurvivorById(slotDefinition.ActorDefinitionID);
			List<string> categories = slotDefinition.Categories;
			return playerModel.gameEconomyData.BlackMarketDefinitions.Where((BlackMarketDefinition x) => x.ActorDefinitionID == ActiveActorDefinitionID && categories.Contains(x.ItemCategory) && playerModel.CouncilLevel >= x.MinCouncilLevel && playerModel.CouncilLevel <= x.MaxCouncilLevel && (survivorModel == null || survivorModel.SurvivorRarityLevel < x.MaxStars) && ActiveOfferIds.All((int activeOfferId) => activeOfferId != x.UniqueId) && (x.CanBePurchasedAgain || (!x.CanBePurchasedAgain && !playerModel.BlackMarket.PurchaseHistory.Contains(x.UniqueId)))).ToList();
		}

		public bool CanRefresh(PlayerModel playerModel)
		{
			if (RefreshCounter == 0)
			{
				return playerModel.UtcTimeStamp < NextUpdate - refreshLockTime;
			}
			return false;
		}

		public void SetLockdown(long refreshLockTime)
		{
			this.refreshLockTime = refreshLockTime;
		}

		public List<BlackMarketDefinition> GetActiveOffers(GameEconomyData gameEconomyData)
		{
			List<BlackMarketDefinition> list = new List<BlackMarketDefinition>();
			foreach (int offerId in ActiveOfferIds)
			{
				BlackMarketDefinition item = gameEconomyData.BlackMarketDefinitions.FirstOrDefault((BlackMarketDefinition x) => x.UniqueId == offerId);
				list.Add(item);
			}
			return list;
		}

		public void AddToPurchaseHistory(int uniqueId)
		{
			if (SessionPurchaseHistory == null)
			{
				SessionPurchaseHistory = new List<int>();
			}
			SessionPurchaseHistory.Add(uniqueId);
		}

		public int GetPurchaseCount(int uniqueId)
		{
			return SessionPurchaseHistory?.Count((int x) => x == uniqueId) ?? 0;
		}
	}
}
