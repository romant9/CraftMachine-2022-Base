using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class RouletteConfig
	{
		public int ID;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUtc;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUtc;

		public int EventPeriod;

		public string RouletteSingleCost;

		public string RouletteMultiCost;

		public int OpenLevel;

		public int Discount;

		public string NameDesc;

		[JsonIgnore]
		private long _StartTimeMilliseconds;

		[JsonIgnore]
		private long _EndTimeMilliseconds;

		[JsonIgnore]
		public long StartTimeMilliseconds
		{
			get
			{
				if (_StartTimeMilliseconds > 0)
				{
					return _StartTimeMilliseconds;
				}
				if (string.IsNullOrEmpty(StartTimeUtc))
				{
					return 0L;
				}
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return _StartTimeMilliseconds = (long)(GameEconomyData.ParseDateTime(StartTimeUtc) - dateTime).TotalSeconds * 1000;
			}
		}

		[JsonIgnore]
		public long EndTimeMilliseconds
		{
			get
			{
				if (_EndTimeMilliseconds > 0)
				{
					return _EndTimeMilliseconds;
				}
				if (string.IsNullOrEmpty(EndTimeUtc))
				{
					return 0L;
				}
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return _EndTimeMilliseconds = (long)(GameEconomyData.ParseDateTime(EndTimeUtc) - dateTime).TotalSeconds * 1000;
			}
		}

		public bool IsActive(long currentUtcTime)
		{
			if (StartTimeMilliseconds == 0L || EndTimeMilliseconds == 0L)
			{
				return true;
			}
			if (StartTimeMilliseconds <= currentUtcTime)
			{
				return EndTimeMilliseconds > currentUtcTime;
			}
			return false;
		}

		public bool IsExpired(long currentUtcTime)
		{
			if (EndTimeMilliseconds == 0L)
			{
				return false;
			}
			return EndTimeMilliseconds <= currentUtcTime;
		}

		public bool IsNotStarted(long currentUtcTime)
		{
			if (StartTimeMilliseconds == 0L)
			{
				return false;
			}
			return StartTimeMilliseconds > currentUtcTime;
		}

		public void SetTimeLimits(DateTime origin)
		{
			if (!string.IsNullOrEmpty(StartTimeUtc))
			{
				_StartTimeMilliseconds = (long)(GameEconomyData.ParseDateTime(StartTimeUtc) - origin).TotalSeconds * 1000;
			}
			if (!string.IsNullOrEmpty(EndTimeUtc))
			{
				_EndTimeMilliseconds = (long)(GameEconomyData.ParseDateTime(EndTimeUtc) - origin).TotalSeconds * 1000;
			}
		}

		public long GetRemainingTime(long currentUtcTime)
		{
			if (EndTimeMilliseconds == 0L)
			{
				return long.MaxValue;
			}
			long num = EndTimeMilliseconds - currentUtcTime;
			if (num <= 0)
			{
				return 0L;
			}
			return num;
		}

		public void ResetCachedTime()
		{
			_StartTimeMilliseconds = 0L;
			_EndTimeMilliseconds = 0L;
		}

		public Dictionary<CurrencyType, int> GetSingleCostInfo()
		{
			Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
			if (string.IsNullOrEmpty(RouletteSingleCost))
			{
				return dictionary;
			}
			string[] array = RouletteSingleCost.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				if (string.IsNullOrEmpty(array[i]))
				{
					continue;
				}
				string[] array2 = array[i].Split('(');
				if (array2.Length < 2)
				{
					continue;
				}
				string text = array2[0].ToLowerInvariant();
				if (array2.Length > 1)
				{
					array2[1] = array2[1].Replace(")", "");
				}
				if (text == "gold")
				{
					try
					{
						if (array2.Length > 1)
						{
							dictionary.Add(CurrencyType.Diamonds, int.Parse(array2[1]));
						}
					}
					catch (Exception)
					{
					}
					continue;
				}
				try
				{
					CurrencyType key = (CurrencyType)Enum.Parse(typeof(CurrencyType), array2[0]);
					if (array2.Length > 1)
					{
						dictionary.Add(key, int.Parse(array2[1]));
					}
				}
				catch (Exception)
				{
				}
			}
			return dictionary;
		}

		public Dictionary<CurrencyType, int> GetMultiCostInfo()
		{
			Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
			if (string.IsNullOrEmpty(RouletteMultiCost))
			{
				return dictionary;
			}
			string[] array = RouletteMultiCost.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				if (string.IsNullOrEmpty(array[i]))
				{
					continue;
				}
				string[] array2 = array[i].Split('(');
				if (array2.Length < 2)
				{
					continue;
				}
				string text = array2[0].ToLowerInvariant();
				if (array2.Length > 1)
				{
					array2[1] = array2[1].Replace(")", "");
				}
				if (text == "gold")
				{
					try
					{
						if (array2.Length > 1)
						{
							dictionary.Add(CurrencyType.Diamonds, int.Parse(array2[1]));
						}
					}
					catch (Exception)
					{
					}
					continue;
				}
				try
				{
					CurrencyType key = (CurrencyType)Enum.Parse(typeof(CurrencyType), array2[0]);
					if (array2.Length > 1)
					{
						dictionary.Add(key, int.Parse(array2[1]));
					}
				}
				catch (Exception)
				{
				}
			}
			return dictionary;
		}

		public int GetSingleCostAmountByCurrencyType(CurrencyType currencyType)
		{
			Dictionary<CurrencyType, int> singleCostInfo = GetSingleCostInfo();
			if (singleCostInfo != null && singleCostInfo.ContainsKey(currencyType))
			{
				return singleCostInfo[currencyType];
			}
			return 0;
		}

		public int GetMultiCostAmountByCurrencyType(CurrencyType currencyType)
		{
			Dictionary<CurrencyType, int> multiCostInfo = GetMultiCostInfo();
			if (multiCostInfo != null && multiCostInfo.ContainsKey(currencyType))
			{
				return multiCostInfo[currencyType];
			}
			return 0;
		}

		public bool SingleCostContainsCurrencyType(CurrencyType currencyType)
		{
			return GetSingleCostInfo()?.ContainsKey(currencyType) ?? false;
		}

		public bool MultiCostContainsCurrencyType(CurrencyType currencyType)
		{
			return GetMultiCostInfo()?.ContainsKey(currencyType) ?? false;
		}

		public List<CurrencyType> GetSingleCostCurrencyTypes()
		{
			Dictionary<CurrencyType, int> singleCostInfo = GetSingleCostInfo();
			if (singleCostInfo != null)
			{
				return new List<CurrencyType>(singleCostInfo.Keys);
			}
			return new List<CurrencyType>();
		}

		public List<CurrencyType> GetMultiCostCurrencyTypes()
		{
			Dictionary<CurrencyType, int> multiCostInfo = GetMultiCostInfo();
			if (multiCostInfo != null)
			{
				return new List<CurrencyType>(multiCostInfo.Keys);
			}
			return new List<CurrencyType>();
		}
	}
}
