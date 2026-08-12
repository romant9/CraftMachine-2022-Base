using Newtonsoft.Json;

namespace TWDModel
{
	public class TradeSlotInfo
	{
		public TradeSlotDefinition SlotDefinition { get; set; }

		public TradeDefinition CurrentTradeDefinition { get; set; }

		[JsonIgnore]
		public bool Bought
		{
			get
			{
				int num = 1 + ((SlotDefinition.GoldRepeat != null) ? SlotDefinition.GoldRepeat.Count : 0);
				return PurchaseCount >= num;
			}
		}

		public int PurchaseCount { get; set; }

		public int GoldUnlockSlot { get; set; }

		public int GetPurchasePrice(out CurrencyType currencyType)
		{
			currencyType = CurrencyType.None;
			int result = 0;
			if (PurchaseCount == 0)
			{
				if (SlotDefinition.PriceCategory == PriceCategory.Discount)
				{
					currencyType = CurrentTradeDefinition.PriceDiscountType;
					result = CurrentTradeDefinition.PriceDiscountAmount;
				}
				else
				{
					currencyType = CurrentTradeDefinition.PriceNormalType;
					result = CurrentTradeDefinition.PriceNormalAmount;
				}
			}
			else if (SlotDefinition.GoldRepeat != null && PurchaseCount <= SlotDefinition.GoldRepeat.Count)
			{
				currencyType = CurrencyType.Diamonds;
				result = SlotDefinition.GoldRepeat[PurchaseCount - 1];
			}
			return result;
		}

		public void Setup()
		{
			SlotDefinition.Setup();
			CurrentTradeDefinition.Setup();
		}
	}
}
