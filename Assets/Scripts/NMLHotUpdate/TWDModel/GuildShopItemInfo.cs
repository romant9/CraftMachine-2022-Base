using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class GuildShopItemInfo
	{
		[JsonIgnore]
		public GuildShopDefinition ItemDefinition;

		public int AvailableAmount;

		public bool Seen;

		public bool Unlocked;

		[JsonIgnore]
		public bool SoldOut
		{
			get
			{
				if (ItemDefinition.LimitedPurchases)
				{
					return AvailableAmount == 0;
				}
				return false;
			}
		}

		public GuildShopItemInfo()
		{
		}

		public GuildShopItemInfo(GuildShopDefinition definition)
		{
			ItemDefinition = definition;
			AvailableAmount = ItemDefinition.InitialAmount;
		}

		public void RestockNewWar(int times = 1)
		{
			int num = ItemDefinition.RestockOnNewWar * times;
			AvailableAmount += num;
			if (num > 0)
			{
				Seen = false;
			}
		}

		public void RestockNewTier(int times = 1)
		{
			int num = ItemDefinition.RestockOnNewTier * times;
			AvailableAmount += num;
			if (num > 0)
			{
				Seen = false;
			}
		}

		public static int Comparison(GuildShopItemInfo a, GuildShopItemInfo b)
		{
			int num = 0;
			num = a.SoldOut.CompareTo(b.SoldOut);
			if (num == 0)
			{
				num = a.ItemDefinition.VIPRequired.CompareTo(b.ItemDefinition.VIPRequired);
			}
			if (num == 0)
			{
				num = -a.ItemDefinition.TierRequirement.CompareTo(b.ItemDefinition.TierRequirement);
			}
			return num;
		}
	}
}
