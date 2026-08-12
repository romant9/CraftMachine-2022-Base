using System;

namespace TWDModel
{
	[Serializable]
	public class ReturnConfig
	{
		public bool Disabled;

		public int CouncilLockLevel;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int InactiveDays;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int IdentityCooldownDays;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int ActivityDurationDays;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int ExchangeDurationDays;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int DailyRefreshTime;

		public int RefreshSlotNum;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int ReturnExchangeStoreRefreshTime;

		public int EndlessDealRefreshDays;

		public string ReturnExchangeStoreRefreshSlotSpend;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int ManualRefreshBanTime;

		public int LoginTotalDays;

		[GEDType(GEDSpecialType.TimeSeconds)]
		public int DefaultPrivilegeDuration;

		public int PrivilegeLimit;
	}
}
