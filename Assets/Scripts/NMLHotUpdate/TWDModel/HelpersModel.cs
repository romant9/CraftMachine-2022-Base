using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;

namespace TWDModel
{
	public static class HelpersModel
	{
		public static bool IsOfflineMode { get; set; }
        public static bool IsOffThinkingAnalytics { get; set; }
        public static bool IsDodge { get; set; }
		public static bool IsUnlockPVP { get; set; }
		public static bool IsUnlockAllSectors { get; set; }

		public static void ExecuteGroupCommand(TWDModelManager manager, TWDGroupCommand command)
		{
			if (manager.ServerService != null && !IsOfflineMode)
			{
				manager.SendGroupCommand(command);
			}
		}

		public static void ExecuteGroupCommand(TWDModelManager manager, GroupCommandBase command)
		{
			if (manager.ServerService != null && !IsOfflineMode)
			{
				manager.SendGroupCommand(command);
			}
		}

		public static T GetRandomWeighted<T>(PlayerModel playerModel, List<T> items) where T : IWeightedItem
		{
			if (items == null || items.Count == 0)
			{
				return default(T);
			}
			int num = items.Sum((T x) => x.GetWeight());
			int randomInRange = playerModel.PlayerRandom.GetRandomInRange(0, num - 1);
			int num2 = 0;
			foreach (T item in items)
			{
				num2 += item.GetWeight();
				if (randomInRange < num2)
				{
					return item;
				}
			}
			playerModel.Debug.LogError("Error selecting weighted random element");
			return default(T);
		}

		public static (int priceAmount, CurrencyType priceCurrency) ParsePrice(string PriceString)
		{
			if (!string.IsNullOrEmpty(PriceString))
			{
				string[] array = PriceString.Split('(');
				return new ValueTuple<int, CurrencyType>(item2: (!(array[0] == "Gold")) ? ((CurrencyType)Enum.Parse(typeof(CurrencyType), array[0])) : CurrencyType.Diamonds, item1: int.Parse(array[1].Replace(")", "")));
			}
			return default((int, CurrencyType));
		}
	}
}
