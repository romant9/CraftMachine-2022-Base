using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class SPTraitsRemoldConfig
	{
		public string SPTraitsRemoldCost;

		public string SPTraitsRemoldCostForLocked;

		public string SPTraitsUpgradeCost;

		public List<string> SPTraitsRatingRange;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long SPTraitsUpgradeSecondConfirmationClosed;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public long SPTraitsRemoldSecondConfirmationClosed;

		public List<string> WeaponDefaultSpTraits;

		[JsonIgnore]
		private Dictionary<string, int> cachedRatingRanges;

		[JsonIgnore]
		private TimeSpan cachedUpgradeConfirmDuration;

		[JsonIgnore]
		private TimeSpan cachedRemoldConfirmDuration;

		[JsonIgnore]
		private List<SPTraitsDefaultTrait> cachedDefaultTraits;

		[JsonIgnore]
		public Dictionary<CurrencyType, int> RemoldCost;

		[JsonIgnore]
		public Dictionary<CurrencyType, int> RemoldCostForLocked;

		[JsonIgnore]
		public Dictionary<CurrencyType, int> UpgradeCost;

		public Dictionary<CurrencyType, int> GetRemoldCost()
		{
			if (RemoldCost == null && SPTraitsRemoldCost != null)
			{
				RemoldCost = new Dictionary<CurrencyType, int>();
				string[] array = SPTraitsRemoldCost.Split(';');
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split('(');
					string text = array2[0].ToLowerInvariant();
					array2[1] = array2[1].Replace(")", "");
					CurrencyType currencyType = CurrencyType.None;
					currencyType = ((!(text == "gold")) ? ((CurrencyType)Enum.Parse(typeof(CurrencyType), array2[0])) : CurrencyType.Diamonds);
					RemoldCost.Add(currencyType, int.Parse(array2[1]));
				}
			}
			return RemoldCost;
		}

		public Dictionary<CurrencyType, int> GetRemoldCostForLocked()
		{
			if (RemoldCostForLocked == null && SPTraitsRemoldCostForLocked != null)
			{
				RemoldCostForLocked = new Dictionary<CurrencyType, int>();
				string[] array = SPTraitsRemoldCostForLocked.Split(';');
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split('(');
					string text = array2[0].ToLowerInvariant();
					array2[1] = array2[1].Replace(")", "");
					CurrencyType currencyType = CurrencyType.None;
					currencyType = ((!(text == "gold")) ? ((CurrencyType)Enum.Parse(typeof(CurrencyType), array2[0])) : CurrencyType.Diamonds);
					RemoldCostForLocked.Add(currencyType, int.Parse(array2[1]));
				}
			}
			return RemoldCostForLocked;
		}

		public Dictionary<CurrencyType, int> GetUpgradeCost()
		{
			if (UpgradeCost == null && SPTraitsUpgradeCost != null)
			{
				UpgradeCost = new Dictionary<CurrencyType, int>();
				string[] array = SPTraitsUpgradeCost.Split(';');
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split('(');
					string text = array2[0].ToLowerInvariant();
					array2[1] = array2[1].Replace(")", "");
					CurrencyType currencyType = CurrencyType.None;
					currencyType = ((!(text == "gold")) ? ((CurrencyType)Enum.Parse(typeof(CurrencyType), array2[0])) : CurrencyType.Diamonds);
					UpgradeCost.Add(currencyType, int.Parse(array2[1]));
				}
			}
			return UpgradeCost;
		}

		public Dictionary<string, int> GetRatingRanges()
		{
			if (cachedRatingRanges != null && cachedRatingRanges.Count > 0)
			{
				return cachedRatingRanges;
			}
			cachedRatingRanges = new Dictionary<string, int>();
			if (SPTraitsRatingRange == null || SPTraitsRatingRange.Count == 0)
			{
				return cachedRatingRanges;
			}
			try
			{
				if (SPTraitsRatingRange.Count % 2 == 0)
				{
					int num = SPTraitsRatingRange.Count / 2;
					for (int i = 0; i < num; i++)
					{
						string key = SPTraitsRatingRange[i];
						if (int.TryParse(SPTraitsRatingRange[num + i], out var result))
						{
							cachedRatingRanges[key] = result;
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return cachedRatingRanges;
		}

		public List<SPTraitsDefaultTrait> GetDefaultTraits()
		{
			if (cachedDefaultTraits != null)
			{
				return cachedDefaultTraits;
			}
			cachedDefaultTraits = new List<SPTraitsDefaultTrait>();
			if (WeaponDefaultSpTraits == null || WeaponDefaultSpTraits.Count == 0)
			{
				return cachedDefaultTraits;
			}
			try
			{
				for (int i = 0; i < WeaponDefaultSpTraits.Count; i += 2)
				{
					if (i + 1 < WeaponDefaultSpTraits.Count)
					{
						string type = WeaponDefaultSpTraits[i];
						if (int.TryParse(WeaponDefaultSpTraits[i + 1], out var result))
						{
							cachedDefaultTraits.Add(new SPTraitsDefaultTrait
							{
								Type = type,
								Count = result
							});
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return cachedDefaultTraits;
		}

		public string GetRatingByScore(int score)
		{
			Dictionary<string, int> ratingRanges = GetRatingRanges();
			string result = "";
			int num = int.MinValue;
			foreach (KeyValuePair<string, int> item in ratingRanges)
			{
				if (score >= item.Value && item.Value > num)
				{
					result = item.Key;
					num = item.Value;
				}
			}
			return result;
		}
	}
}
