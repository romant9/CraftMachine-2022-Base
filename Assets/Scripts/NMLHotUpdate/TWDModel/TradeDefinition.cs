using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class TradeDefinition
	{
		public int UniqueId;

		public string BucketId;

		public string SoldItem;

		[NonSerialized]
		[JsonIgnore]
		public Rewards SoldItems;

		public string Tag;

		public bool ShowProbability;

		[NonSerialized]
		[JsonIgnore]
		public string TagName;

		[NonSerialized]
		[JsonIgnore]
		public int TagAmount;

		public string PriceNormal;

		[NonSerialized]
		[JsonIgnore]
		public CurrencyType PriceNormalType;

		[NonSerialized]
		[JsonIgnore]
		public int PriceNormalAmount;

		public string PriceDiscount;

		[NonSerialized]
		[JsonIgnore]
		public CurrencyType PriceDiscountType;

		[NonSerialized]
		[JsonIgnore]
		public int PriceDiscountAmount;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUtc;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUtc;

		public int CouncilLevelRequired;

		private long startTime;

		private long endTime;

		[JsonIgnore]
		public long StartTimeMilliseconds => startTime;

		[JsonIgnore]
		public long EndTimeMilliseconds => endTime;

		[JsonIgnore]
		public bool HasDateLimit
		{
			get
			{
				if (StartTimeMilliseconds > 0)
				{
					return EndTimeMilliseconds > 0;
				}
				return false;
			}
		}

		public TradeDefinition()
		{
		}

		public TradeDefinition(TradeDefinition other)
		{
			UniqueId = other.UniqueId;
			BucketId = other.BucketId;
			SoldItem = other.SoldItem;
			Tag = other.Tag;
			ShowProbability = other.ShowProbability;
			PriceNormal = other.PriceNormal;
			PriceDiscount = other.PriceDiscount;
			StartTimeUtc = other.StartTimeUtc;
			EndTimeUtc = other.EndTimeUtc;
		}

		public override string ToString()
		{
			string text = "";
			if (SoldItems != null && SoldItems.RewardsList != null)
			{
				for (int i = 0; i < SoldItems.RewardsList.Count; i++)
				{
					text = text + SoldItems.RewardsList[i].Type.ToString() + ",";
				}
			}
			return $"[TradeDefinition: UniqueId={UniqueId}, BucketId={BucketId}, SoldItem={SoldItem}, SoldItems={SoldItems}, Tag={Tag}, PriceCategory={Tag}, PriceNormal={PriceNormal}, PriceNormalType={PriceNormalType}, PriceNormalAmount={PriceNormalAmount}, PriceDiscount={PriceDiscount}, PriceDiscountType={PriceDiscountType}, PriceDiscountAmount={PriceDiscountAmount}, StartTimeUtc={StartTimeUtc}, EndTimeUtc={EndTimeUtc}, StartTimeMilliseconds={StartTimeMilliseconds}, EndTimeMilliseconds={EndTimeMilliseconds}, HasDateLimit={HasDateLimit}, items={text}]";
		}

		public void SetTimeLimits(DateTime origin)
		{
			startTime = (long)(GameEconomyData.ParseDateTime(StartTimeUtc) - origin).TotalSeconds * 1000;
			endTime = (long)(GameEconomyData.ParseDateTime(EndTimeUtc) - origin).TotalSeconds * 1000;
		}

		public bool IsAvailable(long timeUtc)
		{
			if (string.IsNullOrEmpty(StartTimeUtc) || string.IsNullOrEmpty(EndTimeUtc))
			{
				return true;
			}
			if (startTime < timeUtc && endTime > timeUtc)
			{
				return true;
			}
			return false;
		}

		public long GetTimeLeft(long utcTime)
		{
			return Math.Max(endTime - utcTime, 0L);
		}

		public void Setup()
		{
			DateTime timeLimits = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
			if (SoldItems == null)
			{
				try
				{
					SoldItems = new Rewards(SoldItem);
				}
				catch (Exception)
				{
					SoldItems = new Rewards();
				}
			}
			if (!string.IsNullOrEmpty(PriceNormal))
			{
				string[] array = PriceNormal.Split('(');
				if (array[0] == "Gold")
				{
					PriceNormalType = CurrencyType.Diamonds;
				}
				else
				{
					PriceNormalType = (CurrencyType)Enum.Parse(typeof(CurrencyType), array[0]);
				}
				PriceNormalAmount = int.Parse(array[1].Replace(")", ""));
			}
			if (!string.IsNullOrEmpty(PriceDiscount))
			{
				string[] array2 = PriceDiscount.Split('(');
				if (array2[0] == "Gold")
				{
					PriceDiscountType = CurrencyType.Diamonds;
				}
				else
				{
					PriceDiscountType = (CurrencyType)Enum.Parse(typeof(CurrencyType), array2[0]);
				}
				PriceDiscountAmount = int.Parse(array2[1].Replace(")", ""));
			}
			if (!string.IsNullOrEmpty(Tag))
			{
				string[] array3 = Tag.Split(';');
				TagName = array3[0];
				TagAmount = int.Parse(array3[1]);
			}
			if (!string.IsNullOrEmpty(StartTimeUtc) && !string.IsNullOrEmpty(EndTimeUtc))
			{
				SetTimeLimits(timeLimits);
			}
		}
	}
}
