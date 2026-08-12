using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;

namespace TWDModel
{
	[Serializable]
	public class RouletteActivityDataModel : TWDModelObject, IActivityManagerIntegrationInterface
	{
		public int ConfigId;

		public int EventPeriod;

		public List<int> DrawnType1SlotIndices = new List<int>();

		public List<int> DrawnType2SlotIndices = new List<int>();

		public int Type1DrawCount;

		public int Type2DrawCount;

		public long LastDrawTime;

		public bool IsCanPopOpenStatus { get; set; }

		public RouletteActivityDataModel(int configId, int eventPeriod)
		{
			ConfigId = configId;
			EventPeriod = eventPeriod;
		}

		public override void Initialize()
		{
			base.Initialize();
			if (DrawnType1SlotIndices == null)
			{
				DrawnType1SlotIndices = new List<int>();
			}
			if (DrawnType2SlotIndices == null)
			{
				DrawnType2SlotIndices = new List<int>();
			}
		}

		public override void Start()
		{
			base.Start();
			if (DrawnType1SlotIndices == null)
			{
				DrawnType1SlotIndices = new List<int>();
			}
			if (DrawnType2SlotIndices == null)
			{
				DrawnType2SlotIndices = new List<int>();
			}
			InitializeDefinitions();
		}

		public override bool IsValid()
		{
			return true;
		}

		public void ResetDrawnRewards()
		{
			if (DrawnType1SlotIndices == null)
			{
				DrawnType1SlotIndices = new List<int>();
			}
			else
			{
				DrawnType1SlotIndices.Clear();
			}
			if (DrawnType2SlotIndices == null)
			{
				DrawnType2SlotIndices = new List<int>();
			}
			else
			{
				DrawnType2SlotIndices.Clear();
			}
			Type1DrawCount = 0;
			Type2DrawCount = 0;
		}

		public string GetIntegrationEventId()
		{
			return "Roulette";
		}

		public bool CanShowInActivityList()
		{
			if (ConfigId < 0)
			{
				return false;
			}
			if (base.manager == null)
			{
				return false;
			}
			if (base.manager.Player == null)
			{
				return false;
			}
			return IsRouletteActive();
		}

		public bool AreThereAnyUnclaimedReward()
		{
			return false;
		}

		public bool AreThereCanCompleteTask()
		{
			return false;
		}

		public bool IsActivityOpen()
		{
			return IsCanPopOpenStatus;
		}

		public int GetAvailableRewardCount(int rouletteType)
		{
			if (rouletteType < 1 || rouletteType > 2)
			{
				base.manager?.Debug?.LogError($"GetAvailableRewardCount: Invalid rouletteType {rouletteType}");
				return 0;
			}
			RouletteConfig config = GetConfig();
			List<RouletteDefinition> definitions = GetDefinitions();
			if (config == null || definitions == null || definitions.Count == 0)
			{
				return 0;
			}
			List<RouletteDefinition> source = definitions.Where((RouletteDefinition x) => x != null && x.EventPeriod == config.EventPeriod).ToList();
			List<int> drawnIndices = ((rouletteType == 1) ? DrawnType1SlotIndices : DrawnType2SlotIndices);
			if (drawnIndices == null)
			{
				base.manager?.Debug?.LogWarning($"GetAvailableRewardCount: Drawn indices null for type {rouletteType}, config {ConfigId}");
				return 0;
			}
			return source.Count((RouletteDefinition x) => x != null && x.RouletteType == rouletteType && !drawnIndices.Contains(x.SlotsIndex) && x.SlotsWeight > 0);
		}

		public bool IsType2PoolEmpty()
		{
			bool num = GetAvailableRewardCount(2) <= 0;
			if (num)
			{
				TWDModelManager tWDModelManager = base.manager;
				if (tWDModelManager == null)
				{
					return num;
				}
				IModelDebug debug = tWDModelManager.Debug;
				if (debug == null)
				{
					return num;
				}
				debug.LogInfo($"IsType2PoolEmpty: Type 2 pool is empty for activity {ConfigId}");
			}
			return num;
		}

		public bool IsActivityCompleted()
		{
			RouletteConfig config = GetConfig();
			List<RouletteDefinition> definitions = GetDefinitions();
			if (config == null || definitions == null || definitions.Count == 0)
			{
				base.manager?.Debug?.LogWarning($"IsActivityCompleted: Activity {ConfigId} not found");
				return false;
			}
			bool num = IsType2PoolEmpty();
			if (num)
			{
				base.manager?.Debug?.LogInfo($"IsActivityCompleted: Activity {ConfigId} is completed - all prizes have been drawn");
				OnActivityCompleted();
			}
			return num;
		}

		public bool IsDrawAllowed()
		{
			RouletteConfig config = GetConfig();
			List<RouletteDefinition> definitions = GetDefinitions();
			if (config == null || definitions == null || definitions.Count == 0)
			{
				base.manager?.Debug?.LogWarning($"IsDrawAllowed: Activity {ConfigId} not found");
				return false;
			}
			if (IsActivityCompleted())
			{
				base.manager?.Debug?.LogInfo($"IsDrawAllowed: Activity {ConfigId} is completed - draw not allowed");
				return false;
			}
			return true;
		}

		public long GetRemainingTime()
		{
			RouletteConfig config = GetConfig();
			if (config == null)
			{
				base.manager?.Debug?.LogError($"GetRemainingTime: Config {ConfigId} is null");
				return 0L;
			}
			if (base.manager?.Player == null)
			{
				base.manager?.Debug?.LogError("GetRemainingTime: manager or Player is null");
				return 0L;
			}
			long utcTimeStamp = base.manager.Player.UtcTimeStamp;
			if (utcTimeStamp < 0)
			{
				base.manager?.Debug?.LogWarning($"GetRemainingTime: Invalid timestamp {utcTimeStamp}");
				return 0L;
			}
			if (config.EndTimeMilliseconds < 0)
			{
				base.manager?.Debug?.LogWarning($"GetRemainingTime: Invalid EndTimeMilliseconds {config.EndTimeMilliseconds} for config {ConfigId}");
				return 0L;
			}
			if (config.EndTimeMilliseconds > 0 && utcTimeStamp >= config.EndTimeMilliseconds)
			{
				return 0L;
			}
			long num = 0L;
			if (config.EndTimeMilliseconds > 0)
			{
				num = config.EndTimeMilliseconds - utcTimeStamp;
				if (num < 0)
				{
					num = 0L;
				}
			}
			return num;
		}

		public bool IsRouletteActive()
		{
			RouletteConfig config = GetConfig();
			if (config == null)
			{
				base.manager?.Debug?.LogError($"IsRouletteActive: Config {ConfigId} is null");
				return false;
			}
			if (base.manager?.Player == null)
			{
				base.manager?.Debug?.LogError("IsRouletteActive: manager or Player is null");
				return false;
			}
			long utcTimeStamp = base.manager.Player.UtcTimeStamp;
			int councilLevel = base.manager.Player.CouncilLevel;
			if (utcTimeStamp < 0)
			{
				base.manager?.Debug?.LogWarning($"IsRouletteActive: Invalid timestamp {utcTimeStamp}");
				return false;
			}
			bool num = utcTimeStamp >= config.StartTimeMilliseconds && (config.EndTimeMilliseconds == 0L || utcTimeStamp <= config.EndTimeMilliseconds);
			bool flag = config.OpenLevel < 0 || config.OpenLevel <= councilLevel;
			return num && flag;
		}

		public bool GrantRouletteRewards(RouletteResult rouletteResult, out Rewards grantedRewards)
		{
			grantedRewards = null;
			if (rouletteResult == null)
			{
				base.manager?.Debug?.LogError("GrantRouletteRewards: rouletteResult is null");
				return false;
			}
			List<RouletteDefinition> allRewardsList = rouletteResult.GetAllRewardsList();
			if (allRewardsList == null || allRewardsList.Count == 0)
			{
				base.manager?.Debug?.LogWarning($"GrantRouletteRewards: No rewards to grant for config {ConfigId}");
				return true;
			}
			List<string> list = new List<string>();
			foreach (RouletteDefinition item in allRewardsList)
			{
				if (item != null && !string.IsNullOrEmpty(item.Rewards) && item.Rewards != "GetALL")
				{
					list.Add(item.Rewards);
				}
			}
			bool flag = true;
			int grantedRewardCount = 0;
			long valueOrDefault = (base.manager?.Player?.UtcTimeStamp).GetValueOrDefault();
			if (list.Count > 0)
			{
				Rewards rewards = new Rewards(string.Join(";", list), base.manager);
				List<object> list2 = rewards.Give(base.manager);
				if (list2 == null || list2.Count == 0)
				{
					flag = false;
					base.manager?.Debug?.LogError($"GrantRouletteRewards: Failed to grant rewards for config {ConfigId}");
				}
				else
				{
					grantedRewardCount = list2.Count;
					base.manager?.Debug?.LogInfo($"GrantRouletteRewards: Successfully granted {list2.Count} rewards for config {ConfigId}");
					grantedRewards = rewards;
				}
			}
			RouletteConfig config = GetConfig();
			TryRecordGrantMetrics(ConfigId, config, flag, list, allRewardsList, grantedRewardCount, valueOrDefault);
			return flag;
		}

		public RouletteConfig GetConfig()
		{
			return base.manager?.GameEconomyData?.GetRouletteConfig(ConfigId);
		}

		public List<RouletteDefinition> GetDefinitions()
		{
			RouletteConfig config = GetConfig();
			if (config == null)
			{
				return new List<RouletteDefinition>();
			}
			return base.manager?.GameEconomyData?.GetRouletteDefinitionsByPeriod(config.EventPeriod) ?? new List<RouletteDefinition>();
		}

		public bool InitializeDefinitions()
		{
			RouletteConfig config = GetConfig();
			if (config == null)
			{
				base.manager?.Debug?.LogError($"InitializeDefinitions: Config {ConfigId} is null");
				return false;
			}
			List<RouletteDefinition> definitions = GetDefinitions();
			if (definitions == null || definitions.Count == 0)
			{
				base.manager?.Debug?.LogWarning($"InitializeDefinitions: No definitions found for config {ConfigId}, period {config.EventPeriod}");
				return false;
			}
			int valueOrDefault = (base.manager?.Player?.CouncilLevel).GetValueOrDefault();
			foreach (RouletteDefinition item in definitions)
			{
				item?.InitializeRewards(base.manager, valueOrDefault);
			}
			return true;
		}

		private void TryRecordGrantMetrics(int configId, RouletteConfig config, bool grantSuccess, List<string> rewardStrings, List<RouletteDefinition> allRewards, int grantedRewardCount, long grantStartTime)
		{
			TWDModelManager tWDModelManager = base.manager;
			if (tWDModelManager != null)
			{
				if (grantSuccess)
				{
					tWDModelManager.TdMetrics.SetEventType("roulette_reward_grant_success").AddProperty("config_id", configId.ToString()).AddProperty("event_period", config?.EventPeriod.ToString() ?? "0")
						.AddProperty("reward_count", allRewards.Count.ToString())
						.AddProperty("granted_count", grantedRewardCount.ToString())
						.AddProperty("reward_string", string.Join(";", rewardStrings.Take(10)))
						.AddProperty("type1_draw_count", Type1DrawCount.ToString())
						.AddProperty("type2_draw_count", Type2DrawCount.ToString())
						.Send();
					return;
				}
				string text = string.Join(";", rewardStrings);
				tWDModelManager.TdMetrics.SetEventType("roulette_reward_grant_failed").AddProperty("config_id", configId.ToString()).AddProperty("event_period", config?.EventPeriod.ToString() ?? "0")
					.AddProperty("reward_count", rewardStrings.Count.ToString())
					.AddProperty("reward_string", text.Substring(0, Math.Min(500, text.Length)))
					.AddProperty("type1_draw_count", Type1DrawCount.ToString())
					.AddProperty("type2_draw_count", Type2DrawCount.ToString())
					.AddProperty("timestamp", grantStartTime.ToString())
					.Send();
			}
		}

		private void OnActivityCompleted()
		{
			NotifyChange("RouletteActivityCompleted", ConfigId);
		}

		public RouletteResult ExecuteRoulette()
		{
			RouletteResult rouletteResult = new RouletteResult();
			RouletteConfig config = GetConfig();
			List<RouletteDefinition> definitions = GetDefinitions();
			if (config == null || definitions == null || definitions.Count == 0)
			{
				base.manager?.Debug?.LogError($"ExecuteRoulette: Config or definitions not found for ConfigId {ConfigId}");
				return null;
			}
			if (IsActivityCompleted())
			{
				base.manager?.Debug?.LogWarning($"ExecuteRoulette: Activity {ConfigId} is already completed");
				return null;
			}
			if (Type1DrawCount >= 2147483646)
			{
				base.manager?.Debug?.LogError($"ExecuteRoulette: Type1DrawCount overflow for activity {ConfigId}");
				return null;
			}
			Type1DrawCount++;
			List<RouletteDefinition> list = definitions.Where((RouletteDefinition x) => x != null && x.EventPeriod == config.EventPeriod).ToList();
			List<RouletteDefinition> list2 = list.Where((RouletteDefinition x) => x.RouletteType == 1).ToList();
			if (list2.Count == 0)
			{
				base.manager?.Debug?.LogWarning($"ExecuteRoulette: No type1 definitions found for activity {ConfigId}");
				return null;
			}
			if (DrawnType1SlotIndices == null)
			{
				DrawnType1SlotIndices = new List<int>();
			}
			List<RouletteDefinition> list3 = list2.Where((RouletteDefinition x) => x != null && !DrawnType1SlotIndices.Contains(x.SlotsIndex) && x.SlotsWeight > 0).ToList();
			if (list3.Count == 0)
			{
				base.manager?.Debug?.LogWarning($"ExecuteRoulette: No available definitions for activity {ConfigId}");
				return null;
			}
			rouletteResult.IsEnd = list3.Count == 1;
			RouletteDefinition rouletteDefinition = WeightedRandomSelection(list3, Type1DrawCount);
			if (rouletteDefinition != null)
			{
				rouletteResult.AddMainReward(rouletteDefinition);
				if (!DrawnType1SlotIndices.Contains(rouletteDefinition.SlotsIndex))
				{
					DrawnType1SlotIndices.Add(rouletteDefinition.SlotsIndex);
				}
				if (!string.IsNullOrEmpty(rouletteDefinition.Rewards) && rouletteDefinition.Rewards == "GetALL")
				{
					ProcessGetAllReward(rouletteResult, list2, list);
				}
			}
			return rouletteResult;
		}

		public RouletteResult ExecuteMultiDraw()
		{
			RouletteResult rouletteResult = new RouletteResult();
			RouletteConfig config = GetConfig();
			List<RouletteDefinition> definitions = GetDefinitions();
			if (config == null || definitions == null || definitions.Count == 0)
			{
				base.manager?.Debug?.LogError($"ExecuteMultiDraw: Config or definitions not found for ConfigId {ConfigId}");
				return null;
			}
			if (IsActivityCompleted())
			{
				base.manager?.Debug?.LogWarning($"ExecuteMultiDraw: Activity {ConfigId} is already completed");
				return null;
			}
			List<RouletteDefinition> source = definitions.Where((RouletteDefinition x) => x != null && x.EventPeriod == config.EventPeriod).ToList();
			List<RouletteDefinition> source2 = source.Where((RouletteDefinition x) => x != null && x.RouletteType == 1).ToList();
			if (DrawnType1SlotIndices == null)
			{
				DrawnType1SlotIndices = new List<int>();
			}
			List<RouletteDefinition> list = source2.Where((RouletteDefinition x) => x != null && !DrawnType1SlotIndices.Contains(x.SlotsIndex)).ToList();
			rouletteResult.IsEnd = list.Count > 0;
			foreach (RouletteDefinition item in list)
			{
				if (item != null)
				{
					rouletteResult.AddMainReward(item);
					if (!DrawnType1SlotIndices.Contains(item.SlotsIndex))
					{
						DrawnType1SlotIndices.Add(item.SlotsIndex);
					}
				}
			}
			bool hasGetAllReward = list.Any((RouletteDefinition x) => x != null && !string.IsNullOrEmpty(x.Rewards) && x.Rewards == "GetALL");
			rouletteResult.HasGetAllReward = hasGetAllReward;
			List<RouletteDefinition> list2 = (from x in source.Where((RouletteDefinition x) => x != null && x.RouletteType == 2).ToList()
				where x != null && x.SlotsWeight > 0
				select x).ToList();
			if (list2.Count > 0)
			{
				if (DrawnType2SlotIndices == null)
				{
					DrawnType2SlotIndices = new List<int>();
				}
				RouletteDefinition rouletteDefinition = WeightedRandomSelectionForType2(list2);
				if (rouletteDefinition != null)
				{
					rouletteResult.AddType2Reward(rouletteDefinition);
				}
			}
			DrawnType1SlotIndices.Clear();
			Type1DrawCount = 0;
			return rouletteResult;
		}

		private void ProcessGetAllReward(RouletteResult result, List<RouletteDefinition> type1Definitions, List<RouletteDefinition> currentPeriodDefinitions)
		{
			foreach (RouletteDefinition item in type1Definitions.Where((RouletteDefinition x) => x != null && !DrawnType1SlotIndices.Contains(x.SlotsIndex)).ToList())
			{
				if (item != null)
				{
					result.AddGetAllReward(item);
				}
			}
			List<RouletteDefinition> list = (from x in currentPeriodDefinitions.Where((RouletteDefinition x) => x != null && x.RouletteType == 2).ToList()
				where x != null && x.SlotsWeight > 0
				select x).ToList();
			if (list.Count > 0)
			{
				if (DrawnType2SlotIndices == null)
				{
					DrawnType2SlotIndices = new List<int>();
				}
				RouletteDefinition rouletteDefinition = WeightedRandomSelectionForType2(list);
				if (rouletteDefinition != null)
				{
					result.AddType2Reward(rouletteDefinition);
				}
			}
			DrawnType1SlotIndices.Clear();
			Type1DrawCount = 0;
		}

		private RouletteDefinition WeightedRandomSelection(List<RouletteDefinition> definitions, int type1DrawCount)
		{
			if (definitions == null || definitions.Count == 0)
			{
				return null;
			}
			List<RouletteDefinition> list = definitions.Where((RouletteDefinition x) => x != null && ShouldIncludeInWeight(x, type1DrawCount)).ToList();
			if (list.Count == 0)
			{
				return null;
			}
			ModelRandom modelRandom = base.manager?.Player?.PlayerRandom;
			if (modelRandom == null)
			{
				base.manager?.Debug?.LogError("WeightedRandomSelection: PlayerRandom is null");
				return list.FirstOrDefault();
			}
			List<RouletteDefinition> list2 = modelRandom.WeightedRandomList(list, 1, (RouletteDefinition x) => x.GetWeight());
			if (list2 != null && list2.Count > 0)
			{
				return list2[0];
			}
			return list.FirstOrDefault();
		}

		private RouletteDefinition WeightedRandomSelectionForType2(List<RouletteDefinition> definitions)
		{
			if (definitions == null || definitions.Count == 0)
			{
				return null;
			}
			if (Type2DrawCount >= 2147483646)
			{
				base.manager?.Debug?.LogError("WeightedRandomSelectionForType2: Type2DrawCount overflow");
				return null;
			}
			if (DrawnType2SlotIndices == null)
			{
				DrawnType2SlotIndices = new List<int>();
			}
			Type2DrawCount++;
			List<RouletteDefinition> list = definitions.Where((RouletteDefinition x) => x != null && x.SlotsWeight > 0 && !DrawnType2SlotIndices.Contains(x.SlotsIndex)).ToList();
			if (list.Count == 0)
			{
				return null;
			}
			List<RouletteDefinition> list2 = list.Where((RouletteDefinition x) => x != null && ShouldIncludeInWeight(x, Type2DrawCount)).ToList();
			if (list2.Count == 0)
			{
				return null;
			}
			ModelRandom modelRandom = base.manager?.Player?.PlayerRandom;
			if (modelRandom == null)
			{
				base.manager?.Debug?.LogError("WeightedRandomSelectionForType2: PlayerRandom is null");
				RouletteDefinition rouletteDefinition = list2.FirstOrDefault();
				if (rouletteDefinition != null && !DrawnType2SlotIndices.Contains(rouletteDefinition.SlotsIndex))
				{
					DrawnType2SlotIndices.Add(rouletteDefinition.SlotsIndex);
				}
				return rouletteDefinition;
			}
			List<RouletteDefinition> list3 = modelRandom.WeightedRandomList(list2, 1, (RouletteDefinition x) => x.GetWeight());
			if (list3 != null && list3.Count > 0)
			{
				RouletteDefinition rouletteDefinition2 = list3[0];
				if (rouletteDefinition2 != null && !DrawnType2SlotIndices.Contains(rouletteDefinition2.SlotsIndex))
				{
					DrawnType2SlotIndices.Add(rouletteDefinition2.SlotsIndex);
				}
				return rouletteDefinition2;
			}
			RouletteDefinition rouletteDefinition3 = list2.FirstOrDefault();
			if (rouletteDefinition3 != null && !DrawnType2SlotIndices.Contains(rouletteDefinition3.SlotsIndex))
			{
				DrawnType2SlotIndices.Add(rouletteDefinition3.SlotsIndex);
			}
			return rouletteDefinition3;
		}

		private bool ShouldIncludeInWeight(RouletteDefinition definition, int currentDrawCount)
		{
			if (definition.Limitation == -1)
			{
				return true;
			}
			return currentDrawCount >= definition.Limitation;
		}

		private int SafeRandomNext(int maxValue)
		{
			if (maxValue <= 0)
			{
				return 0;
			}
			ModelRandom modelRandom = base.manager?.Player?.PlayerRandom;
			if (modelRandom != null)
			{
				return modelRandom.GetRandomInRange(0, maxValue - 1);
			}
			base.manager?.Debug?.LogError($"SafeRandomNext: PlayerRandom is null for config {ConfigId}");
			return 0;
		}
	}
}
