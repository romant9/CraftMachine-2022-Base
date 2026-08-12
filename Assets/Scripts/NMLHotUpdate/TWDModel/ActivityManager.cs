using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TWDModel
{
	public class ActivityManager
	{
		private readonly TWDModelManager _manager;

		private Dictionary<ActivityType, List<string>> OpenedActivityParam = new Dictionary<ActivityType, List<string>>();

		public bool IsActivityOpen(ActivityType type)
		{
			return OpenedActivityParam.ContainsKey(type);
		}

		public bool IsAnyActivityOpen(params ActivityType[] types)
		{
			foreach (ActivityType type in types)
			{
				if (IsActivityOpen(type))
				{
					return true;
				}
			}
			return false;
		}

		public bool TryGetActivityParam(ActivityType type, out List<string> activityParams)
		{
			return OpenedActivityParam.TryGetValue(type, out activityParams);
		}

		public ActivityManager(TWDModelManager manager)
		{
			_manager = manager;
			DateTime dateTime = manager.Player.Created.ToUniversalTime().AddMilliseconds(manager.Time);
			if (manager.GameEconomyData.ActivityDefinitions != null)
			{
				for (int i = 0; i < manager.GameEconomyData.ActivityDefinitions.Length; i++)
				{
					ActivityDefinition activityDefinition = manager.GameEconomyData.ActivityDefinitions[i];
					if (OpenedActivityParam.ContainsKey(activityDefinition.Event) || !(GameEconomyData.ParseDateTime(activityDefinition.StartTimeUtc) < dateTime) || !(GameEconomyData.ParseDateTime(activityDefinition.EndTimeUtc) > dateTime))
					{
						continue;
					}
					if (activityDefinition.SpenderTiers == null)
					{
						OpenedActivityParam.Add(activityDefinition.Event, activityDefinition.EventValue);
					}
					else if (activityDefinition.SpenderTiers == "CouncilGreaterThan")
					{
						if (manager.Player.CouncilLevel > activityDefinition.SpenderTiersValue)
						{
							OpenedActivityParam.Add(activityDefinition.Event, activityDefinition.EventValue);
						}
					}
					else if (activityDefinition.SpenderTiers == "CouncilLessThan" && manager.Player.CouncilLevel < activityDefinition.SpenderTiersValue)
					{
						OpenedActivityParam.Add(activityDefinition.Event, activityDefinition.EventValue);
					}
				}
			}
			if (manager.GameEconomyData.CircularActivityDefinitions != null)
			{
				for (int j = 0; j < manager.GameEconomyData.CircularActivityDefinitions.Length; j++)
				{
					CircularActivityDefinition circularActivityDefinition = manager.GameEconomyData.CircularActivityDefinitions[j];
					if (!OpenedActivityParam.ContainsKey(circularActivityDefinition.Event))
					{
						TimeSpan timeSpan = dateTime - GameEconomyData.ParseDateTime(circularActivityDefinition.StartTimeUtc);
						if (timeSpan >= TimeSpan.Zero && timeSpan.TotalDays % (double)circularActivityDefinition.CircularDays < (double)circularActivityDefinition.DurationTime)
						{
							OpenedActivityParam.Add(circularActivityDefinition.Event, circularActivityDefinition.EventValue);
						}
					}
				}
			}
			manager.Debug.LogInfo("OpenedActivity:(" + string.Join(";", OpenedActivityParam.Select((KeyValuePair<ActivityType, List<string>> x) => string.Format("{0}:{1}", x.Key, string.Join(",", x.Value)))) + ")");
		}

		public int GetBuildingUpgradeTime(BuildingUpgradeLevel definitions)
		{
			if (TryGetActivityParam(ActivityType.Buildings5s, out var activityParams))
			{
				return int.Parse(activityParams[0]);
			}
			if (_manager?.Player?.ReturnActivityManager != null && _manager.Player.ReturnActivityManager.TryGetFastUpgradeTime(out var timeInSeconds))
			{
				return timeInSeconds;
			}
			return definitions.UpgradeTime;
		}

		public int GetBuildingUpgradeRate(BuildingUpgradeLevel definitions)
		{
			int num = definitions.ProductionRate;
			if (definitions.BuildingType != "Tents")
			{
				return num;
			}
			if (TryGetActivityParam(ActivityType.ComponentEventMetal, out var activityParams))
			{
				num *= int.Parse(activityParams[0]);
			}
			if (TryGetActivityParam(ActivityType.ComponentEventCloth, out activityParams))
			{
				num *= int.Parse(activityParams[0]);
			}
			if (TryGetActivityParam(ActivityType.ComponentEventFood, out activityParams))
			{
				num *= int.Parse(activityParams[0]);
			}
			if (TryGetActivityParam(ActivityType.ComponentEventChemicals, out activityParams))
			{
				num *= int.Parse(activityParams[0]);
			}
			return num;
		}

		public DropCurrenciesStaticDefinition ModifyActivityDefinition(DropCurrenciesStaticDefinition definitions)
		{
			if (!IsActivityOpen(ActivityType.TomatoMonday))
			{
				return definitions;
			}
			return new DropCurrenciesStaticDefinition
			{
				ControlLevelMin = definitions.ControlLevelMin,
				ControlLevelMax = definitions.ControlLevelMax,
				MaxSupplies = definitions.EventMaxSupplies,
				MinSupplies = definitions.EventMinSupplies,
				MaxSurvivalPoints = definitions.EventMaxSurvivalPoints,
				MinSurvivalPoints = definitions.EventMinSurvivalPoints,
				Tag = definitions.Tag
			};
		}

		public bool CheckTradeSlotEventControl(TradeSlotDefinition slot)
		{
			if (IsActivityOpen(ActivityType.TomatoMonday))
			{
				if (slot.EventControl == "TomatoMonday2")
				{
					return false;
				}
			}
			else if (slot.EventControl == "TomatoMonday1")
			{
				return false;
			}
			if (IsActivityOpen(ActivityType.TGTuesday))
			{
				if (slot.EventControl == "TG Tuesday2")
				{
					return false;
				}
			}
			else if (slot.EventControl == "TG Tuesday1")
			{
				return false;
			}
			if (!IsActivityOpen(ActivityType.ComponentEventFood) && slot.EventControl == "Component Event Food1")
			{
				return false;
			}
			if (!IsActivityOpen(ActivityType.ComponentEventCloth) && slot.EventControl == "Component Event Cloth1")
			{
				return false;
			}
			if (!IsActivityOpen(ActivityType.ComponentEventChemicals) && slot.EventControl == "Component Event Chemicals1")
			{
				return false;
			}
			if (!IsActivityOpen(ActivityType.ComponentEventMetal) && slot.EventControl == "Component Event Metal1")
			{
				return false;
			}
			return true;
		}

		public bool CheckCanDrop(ComponentDropType dropType)
		{
			if (!IsActivityOpen(ActivityType.ComponentEventFood) && dropType.EventControl == "Component Event Food1")
			{
				return false;
			}
			if (!IsActivityOpen(ActivityType.ComponentEventCloth) && dropType.EventControl == "Component Event Cloth1")
			{
				return false;
			}
			if (!IsActivityOpen(ActivityType.ComponentEventChemicals) && dropType.EventControl == "Component Event Chemicals1")
			{
				return false;
			}
			if (!IsActivityOpen(ActivityType.ComponentEventMetal) && dropType.EventControl == "Component Event Metal1")
			{
				return false;
			}
			if (dropType.EventControl == "No Component Event" && IsAnyActivityOpen(ActivityType.ComponentEventFood, ActivityType.ComponentEventCloth, ActivityType.ComponentEventChemicals, ActivityType.ComponentEventMetal))
			{
				return false;
			}
			return true;
		}

		public static int ParseBuildTime(string buildtime)
		{
			List<string> list = buildtime.Split(' ').ToList();
			TimeSpan timeSpan = default(TimeSpan);
			foreach (string item in list)
			{
				string text = Regex.Replace(item, "\\d", string.Empty);
				switch (text)
				{
				case "s":
					timeSpan = timeSpan.Add(TimeSpan.FromSeconds(Convert.ToDouble(item.Replace(text, string.Empty))));
					break;
				case "m":
					timeSpan = timeSpan.Add(TimeSpan.FromMinutes(Convert.ToDouble(item.Replace(text, string.Empty))));
					break;
				case "h":
					timeSpan = timeSpan.Add(TimeSpan.FromHours(Convert.ToDouble(item.Replace(text, string.Empty))));
					break;
				case "d":
					timeSpan = timeSpan.Add(TimeSpan.FromDays(Convert.ToDouble(item.Replace(text, string.Empty))));
					break;
				}
			}
			return Convert.ToInt32(timeSpan.TotalSeconds);
		}

		public int GetReplayTokensRechargeSpeed(ConfigData config)
		{
			if (TryGetActivityParam(ActivityType.Gas1m, out var activityParams))
			{
				return ParseBuildTime(activityParams[0]);
			}
			return config.ReplayTokensRechargeSpeed;
		}

		public int GetLootKeySoftCap(ConfigData config)
		{
			if (TryGetActivityParam(ActivityType.FreeUnlocks6, out var activityParams))
			{
				return int.Parse(activityParams[0]);
			}
			return config.LootKeySoftCap;
		}

		public int GetBadgeReclaimCost(ConfigData config)
		{
			if (TryGetActivityParam(ActivityType.FreeBadgeUnequip, out var activityParams))
			{
				return int.Parse(activityParams[0]);
			}
			return config.BadgeReclaimCost;
		}

		public int GetEquipTraitsRemodelGold(ConfigData config)
		{
			if (TryGetActivityParam(ActivityType.EquipmentRemodelDiscount, out var activityParams))
			{
				return int.Parse(activityParams[0]);
			}
			return config.EquipTraitsRemodelGold;
		}
	}
}
