using System;

namespace TWDModel
{
	[Serializable]
	public class ActiveFoundationConfig
	{
		[GEDType(GEDSpecialType.TimeSeconds)]
		public int RefreshTime;

		public int CouncilLockLevel;

		public int RemedyCapPaid;

		public int RemedyCapFree;

		public int RemedyCostGold;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public long RegDay;

		public float RechargeLimit;
	}
}
