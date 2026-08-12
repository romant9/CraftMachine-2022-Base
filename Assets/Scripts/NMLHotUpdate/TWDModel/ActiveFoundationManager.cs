using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ActiveFoundationManager : TWDModelObject, IActivityManagerIntegrationInterface
	{
		public const string ActiveFoundationChangeToday = "ActiveFoundationChangeToday";

		public const string PeriodEndEvent = "PeriodEndEvent";

		public const string UnlockedPremiumEvent = "UnlockedPremiumEvent";

		[JsonProperty("AlreadyUpdateThisPeriod")]
		private bool _alreadyUpdateThisPeriod;

		public ModelList<ActiveFoundationPeriodModel> ParticipatedPeriodList { get; private set; }

		public int BeginPeriodId { get; set; }

		public bool MarkedOpen { get; set; }

		public long MarkOpenCompletedTimestamp { get; set; }

		public int CurrentPeriodId { get; private set; }

		[JsonIgnore]
		public ActiveFoundationPeriodModel CurrentPeriodModel
		{
			get
			{
				if (CurrentPeriodId < 0)
				{
					return null;
				}
				for (int i = 0; i < ParticipatedPeriodList.Count; i++)
				{
					if (ParticipatedPeriodList[i].PeriodId == CurrentPeriodId)
					{
						return ParticipatedPeriodList[i];
					}
				}
				return null;
			}
		}

		[JsonIgnore]
		public int CanSignInTodayDay { get; private set; }

		[JsonIgnore]
		public int CouncilLockLevel => base.manager.GameEconomyData.ActiveFoundationConfig.CouncilLockLevel;

		[JsonIgnore]
		public long RegDayMilliSeconds => base.manager.GameEconomyData.ActiveFoundationConfig.RegDay * 1000;

		[JsonIgnore]
		public float RechargeLimit => base.manager.GameEconomyData.ActiveFoundationConfig.RechargeLimit;

		[JsonIgnore]
		private CurrencyModel PremiumFlagCurrency => base.manager.Player.GetCurrency(CurrencyType.ActiveFoundationPremium);

		[JsonIgnore]
		public ActiveFoundationDefinition CurrentPeriodActiveFoundationDefinition { get; private set; }

		public bool IsCanPopOpenStatus { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public override void Initialize()
		{
			base.Initialize();
			BeginPeriodId = -1;
			CurrentPeriodId = -1;
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (BeginPeriodId == -1)
			{
				tickToInitialize(deltaTime);
			}
			if (BeginPeriodId != -1)
			{
				tickUpdateCurrentPeriod();
				if (!_alreadyUpdateThisPeriod)
				{
					tickUpdateActiveFoundationModel();
				}
			}
		}

		private void tickToInitialize(long deltaTime)
		{
			if (!MarkedOpen)
			{
				if (base.manager.Player.CouncilLevel < CouncilLockLevel || base.manager.Player.UtcTimeStamp - base.manager.Player.CreationTimeStamp < RegDayMilliSeconds || base.manager.Player.TotalUSDSpent < (double)RechargeLimit)
				{
					return;
				}
				MarkOpenCompletedTimestamp = base.manager.Player.UtcTimeStamp - deltaTime;
				MarkedOpen = true;
			}
			List<ActiveFoundationDefinition> list = ((base.manager.GameEconomyData.ActiveFoundationDefinitions == null) ? new List<ActiveFoundationDefinition>() : base.manager.GameEconomyData.ActiveFoundationDefinitions.OrderBy((ActiveFoundationDefinition x) => x.StartTimestamp).ToList());
			ActiveFoundationDefinition activeFoundationDefinition = null;
			foreach (ActiveFoundationDefinition item in list)
			{
				if (item.StartTimestamp * 1000 <= MarkOpenCompletedTimestamp && item.EndTimestamp * 1000 >= MarkOpenCompletedTimestamp)
				{
					activeFoundationDefinition = item;
					break;
				}
			}
			if (activeFoundationDefinition == null)
			{
				ActiveFoundationDefinition activeFoundationDefinition2 = null;
				foreach (ActiveFoundationDefinition item2 in list)
				{
					if (item2.StartTimestamp * 1000 >= MarkOpenCompletedTimestamp)
					{
						activeFoundationDefinition2 = item2;
						break;
					}
				}
				if (activeFoundationDefinition2 != null)
				{
					activeFoundationDefinition = activeFoundationDefinition2;
				}
			}
			if (activeFoundationDefinition != null)
			{
				BeginPeriodId = activeFoundationDefinition.Id;
			}
		}

		private void tickUpdateCurrentPeriod()
		{
			int currentActiveFoundationDefinitionId = getCurrentActiveFoundationDefinitionId();
			if (currentActiveFoundationDefinitionId != -1 && currentActiveFoundationDefinitionId < BeginPeriodId)
			{
				return;
			}
			if (currentActiveFoundationDefinitionId != CurrentPeriodId)
			{
				if (currentActiveFoundationDefinitionId == -1)
				{
					NotifyChange("PeriodEndEvent");
				}
				PremiumFlagCurrency.SetValue(0);
				_alreadyUpdateThisPeriod = false;
			}
			if (currentActiveFoundationDefinitionId < 0)
			{
				CurrentPeriodId = -1;
				CurrentPeriodActiveFoundationDefinition = null;
				CanSignInTodayDay = -1;
			}
			else
			{
				CurrentPeriodId = currentActiveFoundationDefinitionId;
				CurrentPeriodActiveFoundationDefinition = base.manager.GameEconomyData.GetActiveFoundationDefinition(CurrentPeriodId);
				CanSignInTodayDay = caculateDaysFromStartTime();
			}
		}

		public bool TryRetrieveUnclaimedRewards(ref List<IReward> outputActiveFoundationRewardList)
		{
			if (ParticipatedPeriodList == null || ParticipatedPeriodList.Count == 0)
			{
				return false;
			}
			if (outputActiveFoundationRewardList == null)
			{
				outputActiveFoundationRewardList = new List<IReward>();
			}
			foreach (ActiveFoundationPeriodModel participatedPeriod in ParticipatedPeriodList)
			{
				if (participatedPeriod.PeriodId != CurrentPeriodId)
				{
					participatedPeriod.TryRetrievePeriodUnclaimedRewards(ref outputActiveFoundationRewardList);
				}
			}
			return outputActiveFoundationRewardList.Count > 0;
		}

		public void GivePastPeriodsRewards()
		{
			if (ParticipatedPeriodList == null || ParticipatedPeriodList.Count == 0)
			{
				return;
			}
			List<ActiveFoundationPeriodModel> list = new List<ActiveFoundationPeriodModel>();
			foreach (ActiveFoundationPeriodModel participatedPeriod in ParticipatedPeriodList)
			{
				if (participatedPeriod.PeriodId != CurrentPeriodId)
				{
					participatedPeriod.GiveAllSignedInPremiumRewards();
					list.Add(participatedPeriod);
				}
			}
			foreach (ActiveFoundationPeriodModel item in list)
			{
				ParticipatedPeriodList.Remove(item);
			}
		}

		private void tickUpdateActiveFoundationModel()
		{
			if (CurrentPeriodId != -1)
			{
				if (ParticipatedPeriodList == null)
				{
					ParticipatedPeriodList = new ModelList<ActiveFoundationPeriodModel>();
					ParticipatedPeriodList.SetManager(base.manager);
					ParticipatedPeriodList.Initialize();
				}
				if (ParticipatedPeriodList.FirstOrDefault((ActiveFoundationPeriodModel x) => x.PeriodId == CurrentPeriodId) == null)
				{
					ActiveFoundationPeriodModel activeFoundationPeriodModel = new ActiveFoundationPeriodModel(CurrentPeriodId);
					activeFoundationPeriodModel.SetManager(base.manager);
					activeFoundationPeriodModel.Initialize();
					activeFoundationPeriodModel.Start();
					ParticipatedPeriodList.Add(activeFoundationPeriodModel);
				}
				IsCanPopOpenStatus = true;
				_alreadyUpdateThisPeriod = true;
				UpdateModelObjects();
			}
		}

		private int getCurrentActiveFoundationDefinitionId()
		{
			return base.manager.GameEconomyData.ActiveFoundationDefinitions.FirstOrDefault((ActiveFoundationDefinition x) => base.manager.Player.UtcTime >= x.StartDateTime && base.manager.Player.UtcTime < x.EndDateTime)?.Id ?? (-1);
		}

		private int caculateDaysFromStartTime()
		{
			double totalSeconds = (base.manager.Player.UtcTime - CurrentPeriodActiveFoundationDefinition.StartDateTime).TotalSeconds;
			if (totalSeconds <= 0.0)
			{
				return -1;
			}
			return (int)Math.Ceiling(totalSeconds / (double)base.manager.GameEconomyData.ActiveFoundationConfig.RefreshTime);
		}

		public bool ActivatePremium()
		{
			if (CurrentPeriodId == -1)
			{
				return false;
			}
			ActiveFoundationPeriodModel activeFoundationPeriodModel = ParticipatedPeriodList.LastOrDefault((ActiveFoundationPeriodModel x) => x.PeriodId == CurrentPeriodId);
			if (activeFoundationPeriodModel == null)
			{
				return false;
			}
			activeFoundationPeriodModel.TryUnlockPremium();
			return true;
		}

		public string GetIntegrationEventId()
		{
			return "ActiveFoundation";
		}

		public bool CanShowInActivityList()
		{
			if (CurrentPeriodId <= 0)
			{
				return false;
			}
			if (CurrentPeriodModel == null)
			{
				return false;
			}
			if (base.manager.Player.CouncilLevel < CouncilLockLevel)
			{
				return false;
			}
			return true;
		}

		public bool AreThereAnyUnclaimedReward()
		{
			if (CurrentPeriodId <= 0)
			{
				return false;
			}
			if (CurrentPeriodModel == null)
			{
				return false;
			}
			return CurrentPeriodModel.IsHaveSomeRewardCanClaim();
		}

		public bool AreThereCanCompleteTask()
		{
			return false;
		}

		public bool IsActivityOpen()
		{
			if (CurrentPeriodId <= 0)
			{
				return false;
			}
			if (CurrentPeriodModel == null)
			{
				return false;
			}
			if (!IsCanPopOpenStatus)
			{
				return false;
			}
			return true;
		}
	}
}
