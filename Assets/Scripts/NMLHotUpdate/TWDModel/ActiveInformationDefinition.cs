using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ActiveInformationDefinition
	{
		public string ID;

		public int Type;

		public int Order;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string Show;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string Open;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string End;

		private long ShowTime;

		private long EndTime;

		public string Title;

		public string TitleSecond;

		public string Image;

		public int ForcedDisplay;

		public string Function;

		public string FunctionValue;

		public float PosOffset;

		public float RotateOffset;

		public float SizeOffset;

		public string Button1;

		public string Button1Name;

		public string Button1Value;

		public string Button2;

		public string Button2Name;

		public string Button2Value;

		public List<string> SpenderTiers;

		[JsonIgnore]
		public long ShowTimeMilliseconds => ShowTime;

		[JsonIgnore]
		public long EndTimeMilliseconds => EndTime;

		public void SetShowTime(DateTime origin)
		{
			ShowTime = (long)(GameEconomyData.ParseDateTime(Show) - origin).TotalSeconds * 1000;
		}

		public void SetEndTime(DateTime origin)
		{
			EndTime = (long)(GameEconomyData.ParseDateTime(End) - origin).TotalSeconds * 1000;
		}
	}
}
