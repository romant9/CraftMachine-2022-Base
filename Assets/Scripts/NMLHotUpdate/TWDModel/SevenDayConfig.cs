using System;

namespace TWDModel
{
	[Serializable]
	public class SevenDayConfig
	{
		[GEDType(GEDSpecialType.TimeSeconds)]
		public int RefreshTime;

		public int CouncilLockLevel;

		public int RemedyCapPaid;

		public int RemedyCapFree;

		public int RemedyCostGold;
	}
}
