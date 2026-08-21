using System;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ReturnPrivilegeModel : TWDModelObject
	{
		public const string ReturnPrivilegeChanged = "ReturnPrivilegeChanged";

		private const long MillisecondsPerDay = 86400000L;

		private const long MillisecondsPerSecond = 1000L;

		public const int RequiredMissionCount = 1;

		public int TaskRefreshCount { get; set; }

		public long LastTaskRefreshTimestamp { get; set; }

		public bool CurrentTaskCompleted { get; set; }

		public int CurrentProgress { get; set; }

		public long PrivilegeEndTimestamp { get; set; }

		[JsonIgnore]
		public bool IsPrivilegeAvailable
		{
			get
			{
				long valueOrDefault = (base.manager?.Player?.UtcTimeStamp).GetValueOrDefault();
				return IsPrivilegeAvailableAt(valueOrDefault);
			}
		}

		[JsonIgnore]
		public bool HasActiveTask
		{
			get
			{
				long valueOrDefault = (base.manager?.Player?.UtcTimeStamp).GetValueOrDefault();
				if (!IsPrivilegeTaskAvailableAt(valueOrDefault))
				{
					return false;
				}
				if (CurrentTaskCompleted)
				{
					return TaskRefreshCount < GetPrivilegeRefreshLimit();
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool IsCurrentTaskClaimable
		{
			get
			{
				if (HasActiveTask)
				{
					return !CurrentTaskCompleted;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsTaskProgressCompleted => CurrentProgress >= 1;

		[JsonIgnore]
		public int RemainingRefreshCount => Math.Max(GetPrivilegeRefreshLimit() - TaskRefreshCount, 0);

		[JsonIgnore]
		public int CompletedTaskCount => TaskRefreshCount + (CurrentTaskCompleted ? 1 : 0);

		[JsonIgnore]
		public bool HasRedDot => false;

		public bool HasDoubleSurvivalPointsBonus()
		{
			return IsPrivilegeAvailable;
		}

		public bool HasDoubleSuppliesBonus()
		{
			return IsPrivilegeAvailable;
		}

		public bool TryGetFastUpgradeTime(out int timeInSeconds)
		{
			if (IsPrivilegeAvailable)
			{
				timeInSeconds = 5;
				return true;
			}
			timeInSeconds = 0;
			return false;
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void Start()
		{
			base.Start();
			long valueOrDefault = (base.manager?.Player?.UtcTimeStamp).GetValueOrDefault();
			ensurePrivilegeEndTimestamp(valueOrDefault);
			ensureTaskWindow(valueOrDefault);
			base.manager?.Player?.RefreshSurvivalPointsAddMultiplier();
			base.manager?.Player?.RefreshSuppliesAddMultiplier();
			if (base.manager?.Player != null)
			{
				base.manager.Player.OnMissionCompletedEvent -= OnMissionCompleted;
				base.manager.Player.OnMissionCompletedEvent += OnMissionCompleted;
			}
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			long valueOrDefault = (base.manager?.Player?.UtcTimeStamp).GetValueOrDefault();
			ensurePrivilegeEndTimestamp(valueOrDefault);
			bool num = tryRefreshTask(valueOrDefault);
			base.manager?.Player?.RefreshSurvivalPointsAddMultiplier();
			base.manager?.Player?.RefreshSuppliesAddMultiplier();
			if (num)
			{
				NotifyChange("ReturnPrivilegeChanged");
			}
		}

		public void ResetForNewActivity(long currentTimestamp)
		{
			TaskRefreshCount = 0;
			CurrentTaskCompleted = false;
			CurrentProgress = 0;
			PrivilegeEndTimestamp = currentTimestamp + (long)GetDefaultPrivilegeDurationSeconds() * 1000L;
			LastTaskRefreshTimestamp = GetCurrentRefreshWindowStart(currentTimestamp);
			base.manager?.Player?.RefreshSurvivalPointsAddMultiplier();
			base.manager?.Player?.RefreshSuppliesAddMultiplier();
			NotifyChange("ReturnPrivilegeChanged");
		}

		private void OnMissionCompleted()
		{
			if (IsCurrentTaskClaimable && !IsTaskProgressCompleted)
			{
				CurrentProgress = Math.Min(CurrentProgress + 1, 1);
				NotifyChange("ReturnPrivilegeChanged");
			}
		}

		public bool TryCompleteCurrentTask()
		{
			if (!IsCurrentTaskClaimable || !IsTaskProgressCompleted)
			{
				return false;
			}
			long valueOrDefault = (base.manager?.Player?.UtcTimeStamp).GetValueOrDefault();
			if (valueOrDefault <= 0)
			{
				return false;
			}
			CurrentTaskCompleted = true;
			LastTaskRefreshTimestamp = GetCurrentRefreshWindowStart(valueOrDefault);
			long num = Math.Max(PrivilegeEndTimestamp, valueOrDefault);
			PrivilegeEndTimestamp = num + 86400000;
			int extraDays = (int)((PrivilegeEndTimestamp - num) / 86400000);
			base.manager?.Player?.RefreshSurvivalPointsAddMultiplier();
			base.manager?.Player?.RefreshSuppliesAddMultiplier();
			ReturnerAnalytics.SendPerkActive(base.manager, extraDays);
			NotifyChange("ReturnPrivilegeChanged");
			return true;
		}

		private bool tryRefreshTask(long currentTimestamp)
		{
			if (currentTimestamp <= 0)
			{
				return false;
			}
			if (!IsPrivilegeTaskAvailableAt(currentTimestamp))
			{
				return false;
			}
			long currentRefreshWindowStart = GetCurrentRefreshWindowStart(currentTimestamp);
			if (LastTaskRefreshTimestamp <= 0)
			{
				LastTaskRefreshTimestamp = currentRefreshWindowStart;
				return true;
			}
			if (!CurrentTaskCompleted || LastTaskRefreshTimestamp >= currentRefreshWindowStart)
			{
				return false;
			}
			if (TaskRefreshCount >= GetPrivilegeRefreshLimit())
			{
				return false;
			}
			TaskRefreshCount++;
			CurrentTaskCompleted = false;
			CurrentProgress = 0;
			LastTaskRefreshTimestamp = currentRefreshWindowStart;
			return true;
		}

		private void ensureTaskWindow(long currentTimestamp)
		{
			if (currentTimestamp > 0 && LastTaskRefreshTimestamp <= 0 && IsPrivilegeTaskAvailableAt(currentTimestamp))
			{
				LastTaskRefreshTimestamp = GetCurrentRefreshWindowStart(currentTimestamp);
			}
		}

		private bool IsPrivilegeAvailableAt(long currentTimestamp)
		{
			ReturnActivityManager returnActivityManager = base.manager?.Player?.ReturnActivityManager;
			if (returnActivityManager == null || !returnActivityManager.IsReturnFeatureEnabled || !returnActivityManager.IsReturnIdentityActive)
			{
				return false;
			}
			long privilegeEndTimestamp = PrivilegeEndTimestamp;
			if (privilegeEndTimestamp > 0)
			{
				return currentTimestamp < privilegeEndTimestamp;
			}
			return false;
		}

		private bool IsPrivilegeTaskAvailableAt(long currentTimestamp)
		{
			ReturnActivityManager returnActivityManager = base.manager?.Player?.ReturnActivityManager;
			if (currentTimestamp > 0 && returnActivityManager != null)
			{
				return returnActivityManager.IsReturnActivityAvailable();
			}
			return false;
		}

		private void ensurePrivilegeEndTimestamp(long currentTimestamp)
		{
			if (PrivilegeEndTimestamp <= 0)
			{
				ReturnActivityManager returnActivityManager = base.manager?.Player?.ReturnActivityManager;
				if (currentTimestamp > 0 && returnActivityManager != null && returnActivityManager.IsReturnIdentityActive && returnActivityManager.ReturnIdentityStartTimestamp > 0)
				{
					PrivilegeEndTimestamp = returnActivityManager.ReturnIdentityStartTimestamp + (long)GetDefaultPrivilegeDurationSeconds() * 1000L;
				}
			}
		}

		private int GetPrivilegeRefreshLimit()
		{
			return Math.Max((base.gameEconomyData?.ReturnConfig?.PrivilegeLimit).GetValueOrDefault(), 0);
		}

		private int GetDefaultPrivilegeDurationSeconds()
		{
			return Math.Max((base.gameEconomyData?.ReturnConfig?.DefaultPrivilegeDuration).GetValueOrDefault(), 0);
		}

		private long GetCurrentRefreshWindowStart(long timestamp)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
			DateTime dateTime2 = dateTime + TimeSpan.FromMilliseconds(timestamp);
			int num = Math.Max((base.gameEconomyData?.ReturnConfig?.DailyRefreshTime).GetValueOrDefault(), 0);
			DateTime dateTime3 = dateTime2.Date.AddSeconds(num);
			if (dateTime2 < dateTime3)
			{
				dateTime3 = dateTime3.AddDays(-1.0);
			}
			return (long)(dateTime3 - dateTime).TotalMilliseconds;
		}
	}
}
