using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class RouletteManager : TWDModelObject
	{
		[JsonIgnore]
		private long AccumulatedTime;

		[JsonIgnore]
		private long _lastCheckedUtcSeconds = -1L;

		public ModelList<RouletteActivityDataModel> ActivityDataList { get; set; }

		public override void Initialize()
		{
			base.Initialize();
			if (ActivityDataList == null)
			{
				ActivityDataList = new ModelList<RouletteActivityDataModel>();
			}
		}

		public override void Start()
		{
			if (base.manager?.Player == null)
			{
				base.manager?.Debug?.LogError("RouletteManager Start: manager or Player is null");
				return;
			}
			if (ActivityDataList == null)
			{
				ActivityDataList = new ModelList<RouletteActivityDataModel>();
			}
			ActivityDataList.SetManager(base.manager);
			AccumulatedTime = 0L;
			_lastCheckedUtcSeconds = -1L;
			RefreshRouletteData();
			base.Start();
		}

		public bool RefreshRouletteData()
		{
			List<RouletteConfig> list = ValidateAndGetActiveConfigs();
			if (list == null || list.Count == 0)
			{
				return false;
			}
			if (ActivityDataList == null)
			{
				ActivityDataList = new ModelList<RouletteActivityDataModel>();
			}
			RemoveExpiredActivities(list);
			AddNewActivities(list);
			return true;
		}

		private List<RouletteConfig> ValidateAndGetActiveConfigs()
		{
			if (base.manager?.GameEconomyData == null || base.manager?.Player == null)
			{
				base.manager?.Debug?.LogError("ValidateAndGetActiveConfigs: GameEconomyData or Player is null");
				return new List<RouletteConfig>();
			}
			long utcTimeStamp = base.manager.Player.UtcTimeStamp;
			int councilLevel = base.manager.Player.CouncilLevel;
			List<RouletteConfig> allCurrentRouletteConfigs = base.manager.Player.gameEconomyData.GetAllCurrentRouletteConfigs(utcTimeStamp, councilLevel);
			if (allCurrentRouletteConfigs == null || allCurrentRouletteConfigs.Count == 0)
			{
				base.manager?.Debug?.LogInfo($"ValidateAndGetActiveConfigs: No active configs for council level {councilLevel}");
				return new List<RouletteConfig>();
			}
			List<RouletteConfig> list = allCurrentRouletteConfigs.Where((RouletteConfig config) => config != null && config.ID >= 0).ToList();
			if (list.Count == 0)
			{
				base.manager?.Debug?.LogError("ValidateAndGetActiveConfigs: No valid roulette configs");
				return new List<RouletteConfig>();
			}
			return list;
		}

		private void RemoveExpiredActivities(List<RouletteConfig> validConfigs)
		{
			if (ActivityDataList == null || ActivityDataList.Count == 0)
			{
				return;
			}
			HashSet<int> hashSet = new HashSet<int>(validConfigs.Select((RouletteConfig c) => c.ID));
			List<RouletteActivityDataModel> list = new List<RouletteActivityDataModel>();
			foreach (RouletteActivityDataModel item in ActivityDataList.ToList())
			{
				if (item == null)
				{
					continue;
				}
				RouletteConfig config = item.GetConfig();
				if (config == null)
				{
					list.Add(item);
					continue;
				}
				bool flag = config.EndTimeMilliseconds > 0 && base.manager.Player.UtcTimeStamp >= config.EndTimeMilliseconds;
				bool flag2 = !hashSet.Contains(item.ConfigId);
				if (flag || flag2)
				{
					list.Add(item);
					base.manager?.Debug?.LogInfo($"RemoveExpiredActivities: Marking for removal - Activity {item.ConfigId}, Expired: {flag}, NotActive: {flag2}");
				}
			}
			foreach (RouletteActivityDataModel item2 in list)
			{
				ActivityDataList.Remove(item2);
			}
		}

		private void AddNewActivities(List<RouletteConfig> validConfigs)
		{
			foreach (RouletteConfig config in validConfigs)
			{
				if (ActivityDataList.Find((RouletteActivityDataModel a) => a != null && a.ConfigId == config.ID) == null)
				{
					RouletteActivityDataModel rouletteActivityDataModel = new RouletteActivityDataModel(config.ID, config.EventPeriod);
					rouletteActivityDataModel.SetManager(base.manager);
					rouletteActivityDataModel.Initialize();
					rouletteActivityDataModel.Start();
					rouletteActivityDataModel.IsCanPopOpenStatus = true;
					ActivityDataList.Add(rouletteActivityDataModel);
					base.manager?.Debug?.LogInfo($"AddNewActivities: Added new activity {config.ID}");
				}
			}
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (base.manager?.Player != null)
			{
				AccumulatedTime += deltaTime;
				long num = base.manager.Player.UtcTimeStamp / 1000;
				if (num != _lastCheckedUtcSeconds)
				{
					_lastCheckedUtcSeconds = num;
					CheckActivityChange();
					base.manager.Player.ActivityIntegrationManager?.RegisterRouletteActivities();
				}
			}
		}

		private void CheckActivityChange()
		{
			if (base.manager?.Player == null || base.manager?.Player?.gameEconomyData == null)
			{
				base.manager?.Debug?.LogError("CheckActivityChange: manager, GameEconomyData or Player is null");
				return;
			}
			HashSet<int> hashSet = new HashSet<int>();
			if (ActivityDataList != null)
			{
				hashSet = new HashSet<int>(from a in ActivityDataList.ToList()
					where a != null
					select a.ConfigId);
			}
			int councilLevel = base.manager.Player.CouncilLevel;
			HashSet<int> hashSet2 = new HashSet<int>(from c in base.manager.Player.gameEconomyData.GetAllCurrentRouletteConfigs(base.manager.Player.UtcTimeStamp, councilLevel)
				where c != null && c.ID >= 0
				select c.ID);
			if (!hashSet.SetEquals(hashSet2))
			{
				RefreshRouletteData();
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public List<RouletteConfig> GetActiveConfigs()
		{
			if (base.manager?.Player == null || base.manager?.Player.gameEconomyData == null)
			{
				return new List<RouletteConfig>();
			}
			long utcTimeStamp = base.manager.Player.UtcTimeStamp;
			int councilLevel = base.manager.Player.CouncilLevel;
			return (from config in base.manager.Player.gameEconomyData.GetAllCurrentRouletteConfigs(utcTimeStamp, councilLevel)
				where config != null
				orderby config.ID
				select config).ToList();
		}

		public RouletteActivityDataModel GetActivityData(int configId)
		{
			return ActivityDataList?.Find((RouletteActivityDataModel a) => a != null && a.ConfigId == configId);
		}
	}
}
