using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ActivityIntegrationManager : TWDModelObject
	{
		[JsonIgnore]
		public List<IActivityManagerIntegrationInterface> InterfaceImplementers { get; set; }

		[JsonIgnore]
		private List<MethodInfo> InterfaceMethods { get; set; }

		public override void Start()
		{
			base.Start();
			List<MethodInfo> list = typeof(IActivityManagerIntegrationInterface).GetMethods()?.Where((MethodInfo x) => x.GetCustomAttributes(typeof(ActivityIntegrationInvokeOrderAttribute), inherit: false).Length != 0)?.ToList();
			if (list == null || list.Count == 0)
			{
				return;
			}
			InterfaceMethods = list.OrderBy((MethodInfo m) => ((ActivityIntegrationInvokeOrderAttribute)m.GetCustomAttributes(typeof(ActivityIntegrationInvokeOrderAttribute), inherit: false)[0]).InvokeOrder).ToList();
			if (InterfaceImplementers == null)
			{
				InterfaceImplementers = new List<IActivityManagerIntegrationInterface>();
			}
			List<IActivityManagerIntegrationInterface> list2 = new List<IActivityManagerIntegrationInterface>();
			PropertyInfo[] properties = typeof(PlayerModel).GetProperties();
			for (int num = 0; num < properties.Length; num++)
			{
				object value = properties[num].GetValue(base.manager.Player);
				if (value != null && value is IActivityManagerIntegrationInterface item)
				{
					list2.Add(item);
				}
			}
			if (list2 == null)
			{
				return;
			}
			BroadcastDefinition[] broadcastDefinitions = base.manager.GameEconomyData.BroadcastDefinitions;
			if (broadcastDefinitions == null || broadcastDefinitions.Length == 0)
			{
				return;
			}
			List<BroadcastDefinition> list3 = broadcastDefinitions.OrderByDescending((BroadcastDefinition x) => x.EventBroadcastOrder).ToList();
			if (list3 == null)
			{
				return;
			}
			foreach (BroadcastDefinition broadcastDefinition in list3)
			{
				IActivityManagerIntegrationInterface activityManagerIntegrationInterface = list2.Find((IActivityManagerIntegrationInterface x) => x.GetIntegrationEventId() == broadcastDefinition.EventID);
				if (activityManagerIntegrationInterface != null)
				{
					InterfaceImplementers.Add(activityManagerIntegrationInterface);
				}
			}
			RegisterRouletteActivities();
			RegisterRecycleWeaponActivities();
			RegisterSevenDayLoginActivities();
		}

		private int GetBroadcastOrderForImplementer(IActivityManagerIntegrationInterface implementer, BroadcastDefinition[] broadcasts)
		{
			if (implementer == null || broadcasts == null)
			{
				return int.MinValue;
			}
			string eventId = implementer.GetIntegrationEventId();
			RouletteActivityDataModel roulette = implementer as RouletteActivityDataModel;
			if (roulette != null)
			{
				return Array.Find(broadcasts, (BroadcastDefinition x) => x.EventID == eventId && x.Params == roulette.ConfigId)?.EventBroadcastOrder ?? int.MinValue;
			}
			RecycleWeaponActivityModel recycleWeapon = implementer as RecycleWeaponActivityModel;
			if (recycleWeapon != null)
			{
				return Array.Find(broadcasts, (BroadcastDefinition x) => x.EventID == eventId && x.Params == recycleWeapon.Identifier)?.EventBroadcastOrder ?? int.MinValue;
			}
			return Array.Find(broadcasts, (BroadcastDefinition x) => x.EventID == eventId)?.EventBroadcastOrder ?? int.MinValue;
		}

		private void InsertByDescendingBroadcastOrder(IActivityManagerIntegrationInterface implementer, int eventBroadcastOrder)
		{
			if (InterfaceImplementers == null)
			{
				return;
			}
			BroadcastDefinition[] array = base.manager.GameEconomyData?.BroadcastDefinitions;
			if (array == null)
			{
				InterfaceImplementers.Add(implementer);
				return;
			}
			int index = InterfaceImplementers.Count;
			for (int i = 0; i < InterfaceImplementers.Count; i++)
			{
				if (GetBroadcastOrderForImplementer(InterfaceImplementers[i], array) < eventBroadcastOrder)
				{
					index = i;
					break;
				}
			}
			InterfaceImplementers.Insert(index, implementer);
		}

		public void RegisterRouletteActivities()
		{
			if (base.manager?.Player == null)
			{
				return;
			}
			if (InterfaceImplementers != null)
			{
				InterfaceImplementers.RemoveAll((IActivityManagerIntegrationInterface x) => x is RouletteActivityDataModel);
			}
			if (base.manager.GameEconomyData?.ConfigData == null || !base.manager.GameEconomyData.ConfigData.EnableRouletteSystem)
			{
				return;
			}
			RouletteManager rouletteManager = base.manager.Player.RouletteManager;
			if (rouletteManager == null || rouletteManager.ActivityDataList == null)
			{
				return;
			}
			BroadcastDefinition[] broadcastDefinitions = base.manager.GameEconomyData.BroadcastDefinitions;
			if (broadcastDefinitions == null)
			{
				return;
			}
			List<BroadcastDefinition> list = broadcastDefinitions.Where((BroadcastDefinition x) => x.EventID == "Roulette")?.ToList();
			if (list == null || list.Count == 0)
			{
				return;
			}
			foreach (BroadcastDefinition item in list)
			{
				if (item == null)
				{
					continue;
				}
				foreach (RouletteActivityDataModel activityData in rouletteManager.ActivityDataList)
				{
					if (activityData != null && activityData.ConfigId >= 0 && activityData.IsRouletteActive() && activityData.ConfigId == item.Params)
					{
						InsertByDescendingBroadcastOrder(activityData, item.EventBroadcastOrder);
					}
				}
			}
		}

		public void RegisterSevenDayLoginActivities()
		{
			if (base.manager?.Player == null)
			{
				return;
			}
			SevenDayLoginManager sevenDayLoginManager = base.manager.Player.SevenDayLoginManager;
			if (sevenDayLoginManager == null || sevenDayLoginManager.ParticipatedPeriodList == null)
			{
				return;
			}
			BroadcastDefinition[] array = base.manager.GameEconomyData?.BroadcastDefinitions;
			if (array == null)
			{
				return;
			}
			BroadcastDefinition broadcastDefinition = array.FirstOrDefault((BroadcastDefinition x) => x.EventID == "SevenDayLogin");
			if (broadcastDefinition == null)
			{
				return;
			}
			if (InterfaceImplementers != null)
			{
				InterfaceImplementers.RemoveAll((IActivityManagerIntegrationInterface x) => x is SevenDayLoginPeriodModel sevenDayLoginPeriodModel2 && sevenDayLoginPeriodModel2.PeriodId != sevenDayLoginManager.CurrentPeriodId);
			}
			foreach (SevenDayLoginPeriodModel participatedPeriod in sevenDayLoginManager.ParticipatedPeriodList)
			{
				if (participatedPeriod != null && participatedPeriod.PeriodId == sevenDayLoginManager.CurrentPeriodId && InterfaceImplementers != null && !InterfaceImplementers.Contains(participatedPeriod))
				{
					InsertByDescendingBroadcastOrder(participatedPeriod, broadcastDefinition.EventBroadcastOrder);
				}
			}
		}

		public void RegisterRecycleWeaponActivities()
		{
			if (base.manager?.Player == null)
			{
				return;
			}
			if (InterfaceImplementers != null)
			{
				InterfaceImplementers.RemoveAll((IActivityManagerIntegrationInterface x) => x is RecycleWeaponActivityModel);
			}
			RecycleWeaponManager recycleWeaponManager = base.manager.Player.RecycleWeaponManager;
			if (recycleWeaponManager == null || recycleWeaponManager.ActivityDataList == null)
			{
				return;
			}
			BroadcastDefinition[] array = base.manager.GameEconomyData?.BroadcastDefinitions;
			if (array == null)
			{
				return;
			}
			List<BroadcastDefinition> list = array.Where((BroadcastDefinition x) => x.EventID == "RecycleWeapon")?.ToList();
			if (list == null || list.Count == 0)
			{
				return;
			}
			foreach (BroadcastDefinition item in list)
			{
				if (item == null)
				{
					continue;
				}
				foreach (RecycleWeaponActivityModel activityData in recycleWeaponManager.ActivityDataList)
				{
					if (activityData != null && activityData.IsRecycleWeaponActive() && activityData.Identifier == item.Params)
					{
						InsertByDescendingBroadcastOrder(activityData, item.EventBroadcastOrder);
					}
				}
			}
		}

		public List<IActivityManagerIntegrationInterface> GetIntegrationActivityList()
		{
			if (InterfaceImplementers == null)
			{
				return null;
			}
			List<IActivityManagerIntegrationInterface> list = new List<IActivityManagerIntegrationInterface>();
			foreach (IActivityManagerIntegrationInterface interfaceImplementer in InterfaceImplementers)
			{
				if (interfaceImplementer != null && interfaceImplementer.CanShowInActivityList())
				{
					list.Add(interfaceImplementer);
				}
			}
			return list;
		}

		public ActivityNotifyType GetCampNotifyType()
		{
			if (InterfaceMethods == null || InterfaceImplementers == null)
			{
				return ActivityNotifyType.None;
			}
			ActivityNotifyType result = ActivityNotifyType.None;
			int num = 1;
			foreach (MethodInfo interfaceMethod in InterfaceMethods)
			{
				foreach (IActivityManagerIntegrationInterface interfaceImplementer in InterfaceImplementers)
				{
					if ((bool)interfaceMethod.Invoke(interfaceImplementer, null))
					{
						return (ActivityNotifyType)num;
					}
				}
				num++;
			}
			return result;
		}

		public ActivityNotifyType GetNotifyTypeByEventId(string eventId)
		{
			if (InterfaceImplementers == null)
			{
				return ActivityNotifyType.None;
			}
			IActivityManagerIntegrationInterface activityManagerIntegrationInterface = InterfaceImplementers.Find((IActivityManagerIntegrationInterface x) => x.GetIntegrationEventId() == eventId);
			if (activityManagerIntegrationInterface == null)
			{
				return ActivityNotifyType.None;
			}
			return GetNotifyTypeByActivityManager(activityManagerIntegrationInterface);
		}

		public ActivityNotifyType GetNotifyTypeByActivityManager(IActivityManagerIntegrationInterface activityManager)
		{
			if (activityManager == null || InterfaceMethods == null)
			{
				return ActivityNotifyType.None;
			}
			int num = 1;
			foreach (MethodInfo interfaceMethod in InterfaceMethods)
			{
				if ((bool)interfaceMethod.Invoke(activityManager, null))
				{
					return (ActivityNotifyType)num;
				}
				num++;
			}
			return ActivityNotifyType.None;
		}

		public void CloseActivityCanPopOpenStatus(string eventId, int? configId = null)
		{
			if (InterfaceImplementers == null)
			{
				return;
			}
			IActivityManagerIntegrationInterface activityManagerIntegrationInterface = null;
			activityManagerIntegrationInterface = ((!configId.HasValue) ? InterfaceImplementers.Find((IActivityManagerIntegrationInterface x) => x.GetIntegrationEventId() == eventId) : InterfaceImplementers.Find((IActivityManagerIntegrationInterface x) => x.GetIntegrationEventId() == eventId && ((x is RouletteActivityDataModel rouletteActivityDataModel && rouletteActivityDataModel.ConfigId == configId.Value) || (x is RecycleWeaponActivityModel recycleWeaponActivityModel && recycleWeaponActivityModel.Identifier == configId.Value))));
			if (activityManagerIntegrationInterface != null)
			{
				PropertyInfo property = activityManagerIntegrationInterface.GetType().GetProperty("IsCanPopOpenStatus");
				if (property != null && property.CanWrite)
				{
					property.SetValue(activityManagerIntegrationInterface, false);
				}
			}
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
