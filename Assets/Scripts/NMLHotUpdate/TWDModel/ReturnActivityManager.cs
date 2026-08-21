using System;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ReturnActivityManager : TWDModelObject
	{
		public const string ReturnActivityChanged = "ReturnActivityChanged";

		[JsonIgnore]
		private bool _currentSessionProcessed;

		public bool IsReturnIdentityActive { get; set; }

		public long ReturnIdentityStartTimestamp { get; set; }

		public long ReturnActivityEndTimestamp { get; set; }

		public long ReturnExchangeEndTimestamp { get; set; }

		public long LastReturnIdentityTimestamp { get; set; }

		public long LastLoginTimestamp { get; set; }

		public int IdentityCouncilLevelSnapshot { get; set; }

		public ReturnLoginModel ReturnLogin { get; private set; }

		public ReturnPrivilegeModel ReturnPrivilege { get; private set; }

		public ReturnQuestAndExchangeModel ReturnQuestAndExchange { get; private set; }

		public ReturnThreeDayModel ReturnThreeDay { get; private set; }

		public ReturnEndlessDealModel ReturnEndlessDeal { get; private set; }

		[JsonIgnore]
		public bool HasRedDot
		{
			get
			{
				if (!IsReturnActivityAvailable())
				{
					return false;
				}
				ReturnLoginModel returnLogin = ReturnLogin;
				if (returnLogin == null || !returnLogin.HasRedDot)
				{
					ReturnPrivilegeModel returnPrivilege = ReturnPrivilege;
					if (returnPrivilege == null || !returnPrivilege.HasRedDot)
					{
						ReturnQuestAndExchangeModel returnQuestAndExchange = ReturnQuestAndExchange;
						if (returnQuestAndExchange == null || !returnQuestAndExchange.HasRedDot)
						{
							ReturnThreeDayModel returnThreeDay = ReturnThreeDay;
							if (returnThreeDay == null || !returnThreeDay.HasRedDot)
							{
								return ReturnEndlessDeal?.HasRedDot ?? false;
							}
						}
					}
				}
				return true;
			}
		}

		public bool IsActivityPhaseEnded { get; set; }

		[JsonIgnore]
		public bool IsReturnFeatureEnabled => base.gameEconomyData?.ReturnConfig?.Disabled != true;

		public override bool IsValid()
		{
			return true;
		}

		public override void Initialize()
		{
			base.Initialize();
			ensureChildren();
		}

		public override void Start()
		{
			ensureChildren();
			base.Start();
			tryProcessCurrentSession();
		}

		public override void Tick(long deltaTime)
		{
			if (!_currentSessionProcessed)
			{
				tryProcessCurrentSession();
			}
			long valueOrDefault = (base.manager?.Player?.UtcTimeStamp).GetValueOrDefault();
			if (valueOrDefault > 0 && IsReturnIdentityActive)
			{
				if (!IsActivityPhaseEnded && ReturnActivityEndTimestamp > 0 && valueOrDefault >= ReturnActivityEndTimestamp)
				{
					IsActivityPhaseEnded = true;
					ReturnThreeDay?.OnActivityEnded();
					ReturnEndlessDeal?.OnActivityEnded();
				}
				if (ReturnExchangeEndTimestamp > 0 && valueOrDefault >= ReturnExchangeEndTimestamp)
				{
					IsReturnIdentityActive = false;
					NotifyChange("ReturnActivityChanged");
				}
			}
			base.Tick(deltaTime);
		}

		public bool IsReturnActivityAvailable()
		{
			if (IsReturnFeatureEnabled && IsReturnIdentityActive && ReturnActivityEndTimestamp > 0)
			{
				return (base.manager?.Player?.UtcTimeStamp).GetValueOrDefault() < ReturnActivityEndTimestamp;
			}
			return false;
		}

		public bool IsReturnExchangeAvailable()
		{
			if (IsReturnFeatureEnabled && IsReturnIdentityActive && ReturnExchangeEndTimestamp > 0)
			{
				return (base.manager?.Player?.UtcTimeStamp).GetValueOrDefault() < ReturnExchangeEndTimestamp;
			}
			return false;
		}

		public bool TryGetFastUpgradeTime(out int timeInSeconds)
		{
			if (ReturnPrivilege != null)
			{
				return ReturnPrivilege.TryGetFastUpgradeTime(out timeInSeconds);
			}
			timeInSeconds = 0;
			return false;
		}

		public bool OnThreeDayBundleBought()
		{
			if (!IsReturnActivityAvailable())
			{
				return false;
			}
			return ReturnThreeDay?.OnBuyBundle() ?? false;
		}

		public bool OnEndlessDealBundleBought(string bundleId)
		{
			if (!IsReturnActivityAvailable())
			{
				return false;
			}
			return ReturnEndlessDeal?.OnBuyBundle(bundleId) ?? false;
		}

		private void tryProcessCurrentSession()
		{
			if (base.manager?.Player != null && base.manager.Player.UtcTimeStamp > 0)
			{
				_currentSessionProcessed = true;
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				long num = (long)(base.manager.Player.Created.ToUniversalTime() - dateTime).TotalSeconds * 1000 + base.manager.Time;
				if (shouldActivateReturnIdentity(num, LastLoginTimestamp))
				{
					activateReturnIdentity(num);
				}
				LastLoginTimestamp = num;
				ReturnQuestAndExchange?.OnLogin(num);
				NotifyChange("ReturnActivityChanged");
			}
		}

		private bool shouldActivateReturnIdentity(long currentTimestamp, long previousLoginTimestamp)
		{
			ReturnConfig returnConfig = base.gameEconomyData?.ReturnConfig;
			if (returnConfig == null)
			{
				return false;
			}
			if (!IsReturnFeatureEnabled)
			{
				return false;
			}
			if (IsReturnExchangeAvailable())
			{
				return false;
			}
			if (base.manager.Player.CouncilLevel < returnConfig.CouncilLockLevel)
			{
				return false;
			}
			if (previousLoginTimestamp <= 0)
			{
				return false;
			}
			long num = (long)Math.Max(returnConfig.InactiveDays, 0) * 1000L;
			if (currentTimestamp - previousLoginTimestamp < num)
			{
				return false;
			}
			long num2 = (long)Math.Max(returnConfig.IdentityCooldownDays, 0) * 1000L;
			if (LastReturnIdentityTimestamp > 0 && currentTimestamp - LastReturnIdentityTimestamp < num2)
			{
				return false;
			}
			return true;
		}

		private void activateReturnIdentity(long currentTimestamp)
		{
			ReturnConfig returnConfig = base.gameEconomyData?.ReturnConfig;
			if (returnConfig != null)
			{
				IsReturnIdentityActive = true;
				IsActivityPhaseEnded = false;
				ReturnIdentityStartTimestamp = currentTimestamp;
				ReturnActivityEndTimestamp = currentTimestamp + (long)Math.Max(returnConfig.ActivityDurationDays, 0) * 1000L;
				ReturnExchangeEndTimestamp = currentTimestamp + (long)Math.Max(returnConfig.ExchangeDurationDays, 0) * 1000L;
				LastReturnIdentityTimestamp = currentTimestamp;
				IdentityCouncilLevelSnapshot = base.manager.Player.CouncilLevel;
				resetChildrenForNewActivity(currentTimestamp);
				ReturnerAnalytics.SendState(base.manager, IdentityCouncilLevelSnapshot);
			}
		}

		private void resetChildrenForNewActivity(long currentTimestamp)
		{
			ensureChildren();
			ReturnLogin.ResetForNewActivity(currentTimestamp);
			ReturnPrivilege.ResetForNewActivity(currentTimestamp);
			ReturnQuestAndExchange.ResetForNewActivity(currentTimestamp);
			ReturnThreeDay.ResetForNewActivity(currentTimestamp);
			ReturnEndlessDeal.ResetForNewActivity(currentTimestamp);
		}

		public void RefreshCouncilLevelSnapshot()
		{
			if (IsReturnIdentityActive && base.manager?.Player != null)
			{
				int councilLevel = base.manager.Player.CouncilLevel;
				if (councilLevel > IdentityCouncilLevelSnapshot)
				{
					IdentityCouncilLevelSnapshot = councilLevel;
					resetChildrenForNewActivity(ReturnIdentityStartTimestamp);
					NotifyChange("ReturnActivityChanged");
				}
			}
		}

		private void ensureChildren()
		{
			if (ReturnLogin == null)
			{
				ReturnLogin = new ReturnLoginModel();
				ReturnLogin.SetManager(base.manager);
				ReturnLogin.Initialize();
			}
			if (ReturnPrivilege == null)
			{
				ReturnPrivilege = new ReturnPrivilegeModel();
				ReturnPrivilege.SetManager(base.manager);
				ReturnPrivilege.Initialize();
			}
			if (ReturnQuestAndExchange == null)
			{
				ReturnQuestAndExchange = new ReturnQuestAndExchangeModel();
				ReturnQuestAndExchange.SetManager(base.manager);
				ReturnQuestAndExchange.Initialize();
			}
			if (ReturnThreeDay == null)
			{
				ReturnThreeDay = new ReturnThreeDayModel();
				ReturnThreeDay.SetManager(base.manager);
				ReturnThreeDay.Initialize();
			}
			if (ReturnEndlessDeal == null)
			{
				ReturnEndlessDeal = new ReturnEndlessDealModel();
				ReturnEndlessDeal.SetManager(base.manager);
				ReturnEndlessDeal.Initialize();
			}
		}
	}
}
