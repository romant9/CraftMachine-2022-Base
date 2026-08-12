using BaseModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;

namespace TWDModel
{
	public class SevenDayLoginManager : TWDModelObject
	{
		public const string SevenDayLoginChangeToday = "SevenDayLoginChangeToday";

		[JsonProperty("AlreadyUpdateThisPeriod")]
		private bool _alreadyUpdateThisPeriod;

		private bool _isNewPeriodOpened;

		public ModelList<SevenDayLoginPeriodModel> ParticipatedPeriodList { get; private set; }

		public int BeginPeriodId { get; set; }

		public bool IsDailyLoginCampaignCompleted { get; set; }

		public bool Version2 { get; set; }

		public long MarkDailyLoginCampaignCompletedTimestamp { get; set; }

		public int CurrentPeriodId { get; private set; }

		public string BundleIdentifier { get; private set; }

		[JsonIgnore]
		public SevenDayLoginPeriodModel CurrentPeriodModel
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
		public int CouncilLockLevel { get; private set; }

		[JsonIgnore]
		private CurrencyModel PremiumFlagCurrency => base.manager.Player.GetCurrency(CurrencyType.SevenDayPremium);

		[JsonIgnore]
		public SevenDaysDefinition CurrentPeriodSevenDaysDefinition { get; private set; }

		public override bool IsValid()
		{
			return true;
		}

		public override void Initialize()
		{
			base.Initialize();
			BeginPeriodId = -1;
			CurrentPeriodId = -1;
			BundleIdentifier = "";
		}

		public void OnDailyLoginCampaignCompleted(ModelObject m, string changed, object args)
		{
			if (changed == "DailyLoginCampaignCompleted")
			{
				if (!IsDailyLoginCampaignCompleted)
				{
					IsDailyLoginCampaignCompleted = true;
				}
				if (MarkDailyLoginCampaignCompletedTimestamp == 0L)
				{
					MarkDailyLoginCampaignCompletedTimestamp = base.manager.Player.UtcTimeStamp;
				}
			}
		}

		public override void Start()
		{
			base.Start();
			Fix7_0_0Earlier();
			CouncilLockLevel = base.manager.GameEconomyData.SevenDayConfig.CouncilLockLevel;
		}

		private void Fix7_0_0Earlier()
		{
			if (Version2 || !IsDailyLoginCampaignCompleted || BeginPeriodId != -1 || MarkDailyLoginCampaignCompletedTimestamp != 0L || base.manager.GameEconomyData.SevenDaysDefinitions.Length == 0)
			{
				return;
			}
			SevenDaysDefinition[] sevenDaysDefinitions = base.manager.GameEconomyData.SevenDaysDefinitions;
			foreach (SevenDaysDefinition sevenDaysDefinition in sevenDaysDefinitions)
			{
				if (sevenDaysDefinition.StartDateTime <= base.manager.Player.UtcTime && sevenDaysDefinition.EndDateTime > base.manager.Player.UtcTime)
				{
					BeginPeriodId = sevenDaysDefinition.Id;
					MarkDailyLoginCampaignCompletedTimestamp = sevenDaysDefinition.StartTimestamp * 1000;
					break;
				}
			}
			Version2 = true;
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (!IsDailyLoginCampaignCompleted)
			{
				return;
			}
			if (BeginPeriodId == -1)
			{
				tickToInitialize();
			}
			if (BeginPeriodId != -1)
			{
				tickUpdateCurrentPeriod();
				if (!_alreadyUpdateThisPeriod)
				{
					if (IsLoadDataManager)
					{
						string message = "Награды 7 Дней не сгенерированы на этот период";
						MyTools.UpdateLogPanel(message);
						Debug.Log(message, DebugType.Random);
					}

					tickUpdateSevenDayModel();
				}
			}
		}

		private void tickToInitialize()
		{
			if (base.manager.Player.CouncilLevel < CouncilLockLevel)
			{
				return;
			}
			List<SevenDaysDefinition> list = base.manager.GameEconomyData.SevenDaysDefinitions.OrderBy((SevenDaysDefinition x) => x.StartTimestamp).ToList();
			SevenDaysDefinition sevenDaysDefinition = null;
			foreach (SevenDaysDefinition item in list)
			{
				if (item.StartTimestamp * 1000 >= MarkDailyLoginCampaignCompletedTimestamp)
				{
					sevenDaysDefinition = item;
					break;
				}
			}
			if (sevenDaysDefinition != null)
			{
				BeginPeriodId = sevenDaysDefinition.Id;
			}
		}

		private void tickUpdateCurrentPeriod()
		{
			int currentSevenDaysDefinitionId = getCurrentSevenDaysDefinitionId();
			if (currentSevenDaysDefinitionId != -1 && currentSevenDaysDefinitionId < BeginPeriodId)
			{
				return;
			}
			if (currentSevenDaysDefinitionId != CurrentPeriodId)
			{
				PremiumFlagCurrency.SetValue(0);
				_alreadyUpdateThisPeriod = false;
				if (currentSevenDaysDefinitionId >= 0)
				{
					_isNewPeriodOpened = true;
				}
			}
			if (currentSevenDaysDefinitionId < 0)
			{
				CurrentPeriodId = -1;
				BundleIdentifier = "";
				CurrentPeriodSevenDaysDefinition = null;
				CanSignInTodayDay = -1;
			}
			else
			{
				CurrentPeriodId = currentSevenDaysDefinitionId;
				CurrentPeriodSevenDaysDefinition = base.manager.GameEconomyData.GetSevenDaysDefinition(CurrentPeriodId);
				BundleIdentifier = CurrentPeriodSevenDaysDefinition.BundleIdentifier;
				CanSignInTodayDay = caculateDaysFromStartTime();
				if (IsLoadDataManager)
				{
					string text = "Все награды 7 Дней за сегодня получены!";
					for (int i = 0; i < CurrentPeriodModel.RewardDays.Count; i++)
					{
						if (CurrentPeriodModel.RewardDays[i].Day == CanSignInTodayDay && CurrentPeriodModel.RewardDays[i].FreeRewardStatus != SevenDayLoginRewardStatus.Claimed)
						{
							CurrentPeriodModel.RewardDays[i].DayStatus = SevenDayLoginDayStatus.TodayCanClaim;
							text = "Награда 7 Дней " + " (" + CanSignInTodayDay + ") - " + "не получена!";
							Debug.Log("Reward ReadyToBeClaim : " + CanSignInTodayDay, DebugType.System);
						}
					}

					if (!needToClaimLogged)
					{
						needToClaimLogged = true;
						MyTools.UpdateLogPanel(text);
					}
				}
			}
		}

		public bool TryRetrieveUnclaimedRewards(ref List<IReward> outputSevenDayLoginRewardList)
		{
			if (ParticipatedPeriodList == null || ParticipatedPeriodList.Count == 0)
			{
				return false;
			}
			if (outputSevenDayLoginRewardList == null)
			{
				outputSevenDayLoginRewardList = new List<IReward>();
			}
			foreach (SevenDayLoginPeriodModel participatedPeriod in ParticipatedPeriodList)
			{
				if (participatedPeriod.PeriodId != CurrentPeriodId)
				{
					participatedPeriod.TryRetrievePeriodUnclaimedRewards(ref outputSevenDayLoginRewardList);
				}
			}
			return outputSevenDayLoginRewardList.Count > 0;
		}

		public void GivePastPeriodsRewards()
		{
			if (ParticipatedPeriodList == null || ParticipatedPeriodList.Count == 0)
			{
				return;
			}
			List<SevenDayLoginPeriodModel> list = new List<SevenDayLoginPeriodModel>();
			foreach (SevenDayLoginPeriodModel participatedPeriod in ParticipatedPeriodList)
			{
				if (participatedPeriod.PeriodId != CurrentPeriodId)
				{
					participatedPeriod.GiveAllSignedInPremiumRewards();
					list.Add(participatedPeriod);
				}
			}
			foreach (SevenDayLoginPeriodModel item in list)
			{
				ParticipatedPeriodList.Remove(item);
			}
		}

		private void tickUpdateSevenDayModel()
		{
			if (CurrentPeriodId == -1)
			{
				return;
			}
			if (ParticipatedPeriodList == null)
			{
				ParticipatedPeriodList = new ModelList<SevenDayLoginPeriodModel>();
				ParticipatedPeriodList.SetManager(base.manager);
				ParticipatedPeriodList.Initialize();
			}
			if (ParticipatedPeriodList.FirstOrDefault((SevenDayLoginPeriodModel x) => x.PeriodId == CurrentPeriodId) == null)
			{
				SevenDayLoginPeriodModel sevenDayLoginPeriodModel = new SevenDayLoginPeriodModel(CurrentPeriodId);
				sevenDayLoginPeriodModel.SetManager(base.manager);
				sevenDayLoginPeriodModel.Initialize();
				sevenDayLoginPeriodModel.Start();
				if (_isNewPeriodOpened)
				{
					sevenDayLoginPeriodModel.IsCanPopOpenStatus = true;
					_isNewPeriodOpened = false;
				}
				ParticipatedPeriodList.Add(sevenDayLoginPeriodModel);
			}
			base.manager?.Player?.ActivityIntegrationManager?.RegisterSevenDayLoginActivities();
			_alreadyUpdateThisPeriod = true;
			UpdateModelObjects();
		}

		private int getCurrentSevenDaysDefinitionId()
		{
			return base.manager.GameEconomyData.SevenDaysDefinitions.FirstOrDefault((SevenDaysDefinition x) => base.manager.Player.UtcTime >= x.StartDateTime && base.manager.Player.UtcTime < x.EndDateTime)?.Id ?? (-1);
		}

		public int caculateDaysFromStartTime()
		{
			double totalSeconds = (base.manager.Player.UtcTime - CurrentPeriodSevenDaysDefinition.StartDateTime).TotalSeconds;
			if (totalSeconds <= 0.0)
			{
				return -1;
			}
			return (int)Math.Ceiling(totalSeconds / (double)base.manager.GameEconomyData.SevenDayConfig.RefreshTime);
		}

		public bool ActivatePremium()
		{
			if (CurrentPeriodId == -1)
			{
				return false;
			}
			SevenDayLoginPeriodModel sevenDayLoginPeriodModel = ParticipatedPeriodList.LastOrDefault((SevenDayLoginPeriodModel x) => x.PeriodId == CurrentPeriodId);
			if (sevenDayLoginPeriodModel == null)
			{
				return false;
			}
			sevenDayLoginPeriodModel.TryUnlockPremium();
			return true;
		}


		#region myparams
		[JsonIgnore]
		private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
		[JsonIgnore]
		private bool needToClaimLogged;
		#endregion
	}
}
