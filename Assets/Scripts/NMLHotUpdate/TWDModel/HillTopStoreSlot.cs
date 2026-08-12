using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class HillTopStoreSlot : TWDModelObject
	{
		public HillTopSlotType SlotType;

		public List<int> SessionPurchaseHistory;

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

		public List<HillTopStoreDefinition> GetActiveOffers()
		{
			return PlayerModel.gameEconomyData.HillTopStoreDefinitions?.Where((HillTopStoreDefinition x) => x.SlotType == SlotType && CanPurchaseItem(x))?.OrderBy((HillTopStoreDefinition x) => x.DisplayOrder)?.ToList();
		}

		public void UpdateSlot()
		{
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

		public bool CanPurchaseItem(HillTopStoreDefinition hillTopStoreDefinition)
		{
			return GetPurchaseCount(hillTopStoreDefinition.UniqueId) < hillTopStoreDefinition.LimitNum;
		}
	}
}
