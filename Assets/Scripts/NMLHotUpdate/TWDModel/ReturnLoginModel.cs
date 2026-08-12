using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ReturnLoginModel : TWDModelObject
	{
		public const string ReturnLoginChanged = "ReturnLoginChanged";

		public int DefinitionId { get; set; }

		public int AccumulatedLoginDays { get; set; }

		public long LastRefreshTimestamp { get; set; }

		public long LastPopupRefreshTimestamp { get; set; }

		public ModelList<ReturnLoginDayItemModel> RewardDays { get; private set; }

		public bool IsCompleted
		{
			get
			{
				if (RewardDays != null && RewardDays.Count > 0)
				{
					return RewardDays.All((ReturnLoginDayItemModel x) => x.HaveClaimed);
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool ShouldPopupOnCurrentLogin { get; private set; }

		[JsonIgnore]
		public bool HasRedDot
		{
			get
			{
				if (RewardDays == null)
				{
					return false;
				}
				for (int i = 0; i < RewardDays.Count; i++)
				{
					if (RewardDays[i].Day <= AccumulatedLoginDays && !RewardDays[i].HaveClaimed)
					{
						return true;
					}
				}
				return false;
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void Initialize()
		{
			base.Initialize();
			if (RewardDays == null)
			{
				RewardDays = new ModelList<ReturnLoginDayItemModel>();
				RewardDays.SetManager(base.manager);
				RewardDays.Initialize();
			}
		}

		public override void Start()
		{
			if (RewardDays == null)
			{
				RewardDays = new ModelList<ReturnLoginDayItemModel>();
				RewardDays.SetManager(base.manager);
				RewardDays.Initialize();
			}
			SyncRewardDaysWithDefinitions();
			base.Start();
			refreshPopupStateForCurrentLogin((base.manager?.Player?.UtcTimeStamp).GetValueOrDefault());
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (tryAdvanceLoginDay((base.manager?.Player?.UtcTimeStamp).GetValueOrDefault()))
			{
				NotifyChange("ReturnLoginChanged");
			}
		}

		public void ResetForNewActivity(long currentTimestamp)
		{
			int valueOrDefault = (base.manager?.Player?.ReturnActivityManager?.IdentityCouncilLevelSnapshot).GetValueOrDefault();
			DefinitionId = (base.gameEconomyData?.GetReturnLoginDefinitionByCouncilLevel(valueOrDefault)?.Id).GetValueOrDefault();
			AccumulatedLoginDays = 0;
			LastRefreshTimestamp = 0L;
			LastPopupRefreshTimestamp = 0L;
			SyncRewardDaysWithDefinitions();
			refreshPopupStateForCurrentLogin(currentTimestamp);
			NotifyChange("ReturnLoginChanged");
		}

		public bool TryClaimReward(int day)
		{
			if (RewardDays == null)
			{
				return false;
			}
			ReturnLoginDayItemModel returnLoginDayItemModel = RewardDays.FirstOrDefault((ReturnLoginDayItemModel x) => x.Day == day);
			if (returnLoginDayItemModel == null)
			{
				return false;
			}
			if (!returnLoginDayItemModel.TryClaimReward())
			{
				return false;
			}
			NotifyChange("ReturnLoginChanged");
			return true;
		}

		public void MarkPopupShownOnCurrentLogin()
		{
			long valueOrDefault = (base.manager?.Player?.UtcTimeStamp).GetValueOrDefault();
			if (valueOrDefault > 0)
			{
				LastPopupRefreshTimestamp = GetCurrentRefreshWindowStart(valueOrDefault);
				ShouldPopupOnCurrentLogin = false;
				NotifyChange("ReturnLoginChanged");
			}
		}

		private bool tryAdvanceLoginDay(long currentTimestamp)
		{
			if (currentTimestamp <= 0 || DefinitionId <= 0)
			{
				return false;
			}
			ReturnActivityManager returnActivityManager = base.manager?.Player?.ReturnActivityManager;
			if (returnActivityManager == null || !returnActivityManager.IsReturnActivityAvailable())
			{
				return false;
			}
			long currentRefreshWindowStart = GetCurrentRefreshWindowStart(currentTimestamp);
			if (AccumulatedLoginDays <= 0 || LastRefreshTimestamp <= 0)
			{
				AccumulatedLoginDays = 1;
				LastRefreshTimestamp = currentRefreshWindowStart;
				return true;
			}
			if (LastRefreshTimestamp < currentRefreshWindowStart)
			{
				int num = Math.Min(GetLoginTotalDays(), AccumulatedLoginDays + 1);
				bool result = num != AccumulatedLoginDays || LastRefreshTimestamp != currentRefreshWindowStart;
				AccumulatedLoginDays = num;
				LastRefreshTimestamp = currentRefreshWindowStart;
				return result;
			}
			return false;
		}

		private void refreshPopupStateForCurrentLogin(long currentTimestamp)
		{
			ShouldPopupOnCurrentLogin = shouldPopupOnCurrentLogin(currentTimestamp);
		}

		private bool shouldPopupOnCurrentLogin(long currentTimestamp)
		{
			if (currentTimestamp <= 0)
			{
				return false;
			}
			if (base.gameEconomyData?.ReturnConfig == null)
			{
				return false;
			}
			ReturnActivityManager returnActivityManager = base.manager?.Player?.ReturnActivityManager;
			if (returnActivityManager == null || !returnActivityManager.IsReturnActivityAvailable())
			{
				return false;
			}
			if (DefinitionId <= 0 || RewardDays == null || RewardDays.Count == 0 || IsCompleted)
			{
				return false;
			}
			long currentRefreshWindowStart = GetCurrentRefreshWindowStart(currentTimestamp);
			return LastPopupRefreshTimestamp < currentRefreshWindowStart;
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

		private void SyncRewardDaysWithDefinitions()
		{
			List<ReturnLoginRewardDefinition> list = base.gameEconomyData?.GetReturnLoginRewardDefinitions(DefinitionId);
			if (DefinitionId <= 0 || list == null || list.Count == 0)
			{
				RewardDays.Clear();
				return;
			}
			bool flag = RewardDays.Count != list.Count;
			if (!flag)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (RewardDays[i].Day != list[i].Day || RewardDays[i].RewardDefinitionId != list[i].Id)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return;
			}
			Dictionary<int, bool> dictionary = RewardDays.ToDictionary((ReturnLoginDayItemModel x) => x.Day, (ReturnLoginDayItemModel x) => x.HaveClaimed);
			RewardDays.Clear();
			foreach (ReturnLoginRewardDefinition item in list)
			{
				ReturnLoginDayItemModel returnLoginDayItemModel = new ReturnLoginDayItemModel(item.Day, item.Id);
				returnLoginDayItemModel.SetManager(base.manager);
				returnLoginDayItemModel.Initialize();
				returnLoginDayItemModel.Start();
				if (dictionary.TryGetValue(item.Day, out var value))
				{
					returnLoginDayItemModel.HaveClaimed = value;
				}
				RewardDays.Add(returnLoginDayItemModel);
			}
		}

		private int GetLoginTotalDays()
		{
			if (base.gameEconomyData?.ReturnConfig == null || base.gameEconomyData.ReturnConfig.LoginTotalDays <= 0)
			{
				return 7;
			}
			return base.gameEconomyData.ReturnConfig.LoginTotalDays;
		}
	}
}
