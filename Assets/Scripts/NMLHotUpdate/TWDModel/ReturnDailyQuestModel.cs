using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ReturnDailyQuestModel : TWDModelObject
	{
		public const string ReturnDailyQuestChanged = "ReturnDailyQuestChanged";

		public int CurrentGroup { get; set; }

		public long LastRefreshTimestamp { get; set; }

		public ModelList<ReturnDailyQuestItemModel> Tasks { get; private set; }

		[JsonIgnore]
		public bool HasTask
		{
			get
			{
				if (Tasks != null && Tasks.Count > 0)
				{
					TWDModelManager tWDModelManager = base.manager;
					if (tWDModelManager == null)
					{
						return false;
					}
					return tWDModelManager.Player?.ReturnActivityManager?.IsReturnActivityAvailable() == true;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool CanClaimAnyReward
		{
			get
			{
				if (!HasTask)
				{
					return false;
				}
				for (int i = 0; i < Tasks.Count; i++)
				{
					if (CanClaim(Tasks[i]))
					{
						return true;
					}
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool HasRedDot => CanClaimAnyReward;

		public override bool IsValid()
		{
			return true;
		}

		public override void Initialize()
		{
			base.Initialize();
			EnsureTasksInitialized();
		}

		public override void Start()
		{
			EnsureTasksInitialized();
			base.Start();
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (ensureCurrentTasks((base.manager?.Player?.UtcTimeStamp).GetValueOrDefault()))
			{
				NotifyChange("ReturnDailyQuestChanged");
			}
		}

		public void ResetForNewActivity(long currentTimestamp)
		{
			CurrentGroup = 0;
			LastRefreshTimestamp = 0L;
			EnsureTasksInitialized();
			Tasks.Clear();
			NotifyChange("ReturnDailyQuestChanged");
		}

		public bool OnLogin(long currentTimestamp)
		{
			bool flag = ensureCurrentTasks(currentTimestamp);
			for (int i = 0; i < Tasks.Count; i++)
			{
				ReturnDailyQuestItemModel returnDailyQuestItemModel = Tasks[i];
				if (returnDailyQuestItemModel != null && !returnDailyQuestItemModel.Claimed)
				{
					ReturnDailyQuestDefinition definition = returnDailyQuestItemModel.Definition;
					if (definition != null && definition.QuestType == ReturnQuestType.DailyLogin)
					{
						returnDailyQuestItemModel.CurrentProgress++;
						flag = true;
					}
				}
			}
			if (flag)
			{
				NotifyChange("ReturnDailyQuestChanged");
			}
			return flag;
		}

		public bool OnWalkersKilled(int amount)
		{
			if (amount <= 0)
			{
				return false;
			}
			bool flag = ensureCurrentTasks((base.manager?.Player?.UtcTimeStamp).GetValueOrDefault());
			for (int i = 0; i < Tasks.Count; i++)
			{
				ReturnDailyQuestItemModel returnDailyQuestItemModel = Tasks[i];
				if (returnDailyQuestItemModel != null && !returnDailyQuestItemModel.Claimed)
				{
					ReturnDailyQuestDefinition definition = returnDailyQuestItemModel.Definition;
					if (definition != null && definition.QuestType == ReturnQuestType.KillWalkers)
					{
						returnDailyQuestItemModel.CurrentProgress += amount;
						flag = true;
					}
				}
			}
			if (flag)
			{
				NotifyChange("ReturnDailyQuestChanged");
			}
			return flag;
		}

		public bool OnItemUpgraded(ReturnQuestType upgradeQuestType)
		{
			if (!upgradeQuestType.IsUpgradeQuest())
			{
				return false;
			}
			bool flag = ensureCurrentTasks((base.manager?.Player?.UtcTimeStamp).GetValueOrDefault());
			for (int i = 0; i < Tasks.Count; i++)
			{
				ReturnDailyQuestItemModel returnDailyQuestItemModel = Tasks[i];
				if (returnDailyQuestItemModel != null && !returnDailyQuestItemModel.Claimed)
				{
					ReturnDailyQuestDefinition definition = returnDailyQuestItemModel.Definition;
					if (definition != null && definition.QuestType == upgradeQuestType)
					{
						returnDailyQuestItemModel.CurrentProgress++;
						flag = true;
					}
				}
			}
			if (flag)
			{
				NotifyChange("ReturnDailyQuestChanged");
			}
			return flag;
		}

		public bool OnMissionCompleted()
		{
			bool flag = ensureCurrentTasks((base.manager?.Player?.UtcTimeStamp).GetValueOrDefault());
			for (int i = 0; i < Tasks.Count; i++)
			{
				ReturnDailyQuestItemModel returnDailyQuestItemModel = Tasks[i];
				if (returnDailyQuestItemModel != null && !returnDailyQuestItemModel.Claimed)
				{
					ReturnDailyQuestDefinition definition = returnDailyQuestItemModel.Definition;
					if (definition != null && definition.QuestType == ReturnQuestType.CompleteMission)
					{
						returnDailyQuestItemModel.CurrentProgress++;
						flag = true;
					}
				}
			}
			if (flag)
			{
				NotifyChange("ReturnDailyQuestChanged");
			}
			return flag;
		}

		public bool OnCurrencySpent(CurrencyType currencyType, int amount)
		{
			if (amount <= 0)
			{
				return false;
			}
			bool flag = ensureCurrentTasks((base.manager?.Player?.UtcTimeStamp).GetValueOrDefault());
			for (int i = 0; i < Tasks.Count; i++)
			{
				ReturnDailyQuestItemModel returnDailyQuestItemModel = Tasks[i];
				if (returnDailyQuestItemModel != null && !returnDailyQuestItemModel.Claimed)
				{
					ReturnDailyQuestDefinition definition = returnDailyQuestItemModel.Definition;
					ReturnQuestType questType = definition?.QuestType ?? ReturnQuestType.None;
					if (definition != null && ReturnQuestRuleHelper.TryGetCurrencyType(questType, out var currencyType2) && currencyType2 == currencyType)
					{
						returnDailyQuestItemModel.CurrentProgress += amount;
						flag = true;
					}
				}
			}
			if (flag)
			{
				NotifyChange("ReturnDailyQuestChanged");
			}
			return flag;
		}

		public bool TryClaimReward(int definitionId)
		{
			ReturnDailyQuestItemModel returnDailyQuestItemModel = Tasks?.Find((ReturnDailyQuestItemModel x) => x.DefinitionId == definitionId);
			if (!CanClaim(returnDailyQuestItemModel) || returnDailyQuestItemModel.Definition?.RewardEntries == null)
			{
				return false;
			}
			returnDailyQuestItemModel.Definition.RewardEntries.Give(base.manager);
			returnDailyQuestItemModel.Claimed = true;
			ReturnerAnalytics.SendTask(base.manager, definitionId);
			NotifyChange("ReturnDailyQuestChanged");
			return true;
		}

		private bool ensureCurrentTasks(long currentTimestamp)
		{
			if (currentTimestamp > 0)
			{
				TWDModelManager tWDModelManager = base.manager;
				if (tWDModelManager != null && tWDModelManager.Player?.ReturnActivityManager?.IsReturnActivityAvailable() == true)
				{
					long currentRefreshWindowStart = GetCurrentRefreshWindowStart(currentTimestamp);
					if (LastRefreshTimestamp >= currentRefreshWindowStart && Tasks != null && Tasks.Count > 0)
					{
						return false;
					}
					int currentCouncilLevel = GetCurrentCouncilLevel();
					List<ReturnDailyQuestDefinition> list = base.gameEconomyData?.GetReturnDailyQuestDefinitions(currentCouncilLevel) ?? new List<ReturnDailyQuestDefinition>();
					if (list.Count == 0)
					{
						CurrentGroup = 0;
						Tasks.Clear();
						LastRefreshTimestamp = currentRefreshWindowStart;
						return true;
					}
					int num = list[0].Group;
					List<ReturnDailyQuestDefinition> list2 = base.gameEconomyData?.GetReturnDailyQuestDefinitions(currentCouncilLevel, num) ?? new List<ReturnDailyQuestDefinition>();
					if (list2.Count == 0)
					{
						return false;
					}
					CurrentGroup = num;
					RebuildTasks(list2);
					LastRefreshTimestamp = currentRefreshWindowStart;
					return true;
				}
			}
			return false;
		}

		private void EnsureTasksInitialized()
		{
			if (Tasks == null)
			{
				Tasks = new ModelList<ReturnDailyQuestItemModel>();
				Tasks.SetManager(base.manager);
				Tasks.Initialize();
			}
		}

		private void RebuildTasks(List<ReturnDailyQuestDefinition> definitions)
		{
			EnsureTasksInitialized();
			Tasks.Clear();
			for (int i = 0; i < definitions.Count; i++)
			{
				ReturnDailyQuestDefinition returnDailyQuestDefinition = definitions[i];
				if (returnDailyQuestDefinition != null)
				{
					ReturnDailyQuestItemModel returnDailyQuestItemModel = new ReturnDailyQuestItemModel(returnDailyQuestDefinition.Id);
					returnDailyQuestItemModel.SetManager(base.manager);
					returnDailyQuestItemModel.Initialize();
					if (base.ModelId != 0)
					{
						returnDailyQuestItemModel.Start();
					}
					Tasks.Add(returnDailyQuestItemModel);
				}
			}
		}

		private bool IsCompleted(ReturnDailyQuestItemModel task)
		{
			if (task != null && HasTask)
			{
				return task.CurrentProgress >= GetRequiredAmount(task);
			}
			return false;
		}

		private bool CanClaim(ReturnDailyQuestItemModel task)
		{
			if (task != null && HasTask && IsCompleted(task))
			{
				return !task.Claimed;
			}
			return false;
		}

		private int GetRequiredAmount(ReturnDailyQuestItemModel task)
		{
			if (task?.Definition != null)
			{
				return ReturnQuestRuleHelper.GetRequiredAmount(task.Definition.Params);
			}
			return 0;
		}

		private int GetCurrentCouncilLevel()
		{
			if (base.manager?.Player == null)
			{
				return 0;
			}
			return base.manager.Player.CouncilLevel;
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
