using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TwdCustomMod;

namespace TWDModel
{
	public class RFMGiftManager : TWDModelObject
	{
		public const int GiftCount = 1;

		public List<PurchargeTimeInfo> PurchargeInfos { get; set; }

		public Dictionary<string, int> PushGiftCount { get; set; }

		public DateTime LastPushGiftTime { get; set; }

		public Dictionary<string, long> PushGiftTime { get; set; }

		public List<RFMEventInfo> RFMEvents { get; set; }

		public Dictionary<string, int> GiftPushRatio { get; set; }

		[JsonIgnore]
		public List<string> CurrentGift
		{
			get
			{
				long nowtimestamp = base.manager.Player.UtcTimeStamp;
				return (from x in PushGiftTime
					where x.Value > nowtimestamp
					select x.Key).ToList();
			}
		}

		public long GetGiftLeftTime(string bundleId)
		{
			if (PushGiftTime.TryGetValue(bundleId, out var value))
			{
				return value - base.manager.Player.UtcTimeStamp;
			}
			return -1L;
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void Initialize()
		{
			DateTime utcTime = base.manager.Player.UtcTime;
			base.Initialize();
			PushGiftCount = new Dictionary<string, int>();
			PurchargeInfos = new List<PurchargeTimeInfo>();
			PushGiftTime = new Dictionary<string, long>();
			RFMEvents = new List<RFMEventInfo>();
			GiftPushRatio = new Dictionary<string, int>();
			LastPushGiftTime = new DateTime(utcTime.Year, utcTime.Month, utcTime.Day);
		}

		public int GetRFMValue()
		{
			int num = 0;
			long now = base.manager.Player.UtcTimeStamp;
			ConditionBundleConfig config = base.manager.GameEconomyData.ConditionBundleConfig;
			if (config != null)
			{
				List<PurchargeTimeInfo> list = PurchargeInfos.OrderByDescending((PurchargeTimeInfo x) => x.PurchargeTime).ToList();
				if (list.Count > 2)
				{
					num |= ((list[list.Count - 1].PurchargeTime - list[list.Count - 2].PurchargeTime < config.RecencyBaseValue) ? 1 : 0);
				}
				num |= ((list.Count((PurchargeTimeInfo x) => x.PurchargeTime + config.FrequencyTimePeriod > now) >= config.FrequencyBaseValue) ? 1 : 0) << 1;
				num |= ((list.Where((PurchargeTimeInfo x) => x.PurchargeTime + config.MonetaryTimePeriod > now).Sum((PurchargeTimeInfo x) => x.Price) >= (float)config.MonetaryBaseValue) ? 1 : 0) << 2;
			}
			return num;
		}

		public void AddPurchargeInfo(float purchargePrice, long? time = null)
		{
			long utcTimeStamp = base.manager.Player.UtcTimeStamp;
			PurchargeInfos.Add(new PurchargeTimeInfo
			{
				PurchargeTime = (time ?? utcTimeStamp),
				Price = purchargePrice
			});
			base.Debug.LogInfo($"RFMGiftManager AddPurcharge:{purchargePrice},{time ?? utcTimeStamp}");
		}

		public void OnBuyBundle(string bundleId)
		{
			PushGiftTime.Remove(bundleId);
			ConditionBundleDefinition conditionBundleDefinition = base.manager.GameEconomyData.GetConditionBundleDefinition(bundleId);
			BundleContentDefinition bundleContentDefinition = base.manager.GameEconomyData.GetBundleContentDefinition(bundleId);
			if (conditionBundleDefinition != null && bundleContentDefinition != null)
			{
				base.manager.TdMetrics.SetEventType("Condi_Bundle_Purchase").AddProperty("BundleIdentifier", bundleId).AddProperty("Condition", conditionBundleDefinition.Condition.ToString())
					.AddProperty("RecencyLevel", conditionBundleDefinition.RecencyLevel.ToString())
					.AddProperty("FrequencyLevel", conditionBundleDefinition.FrequencyLevel.ToString())
					.AddProperty("MonetaryLevel", conditionBundleDefinition.MonetaryLevel.ToString())
					.AddProperty("TimeLimit", conditionBundleDefinition.TimeLimit)
					.AddProperty("Rewards", bundleContentDefinition.Rewards)
					.AddProperty("Price", bundleContentDefinition.IAPProduct)
					.Send();
			}
		}

		public void CheckCrossDay()
		{
			DateTime utcTime = base.manager.Player.UtcTime;
			if (!(LastPushGiftTime.AddDays(1.0).AddHours(8.0) < utcTime))
			{
				return;
			}
			LastPushGiftTime = new DateTime(utcTime.Year, utcTime.Month, utcTime.Day);
			RFMEvents.Clear();
			PushGiftCount.Clear();
			long nowtimestamp = base.manager.Player.UtcTimeStamp;
			foreach (string item in (from x in PushGiftTime
				where x.Value <= nowtimestamp
				select x.Key).ToList())
			{
				PushGiftTime.Remove(item);
			}
		}

		public List<ConditionBundleDefinition> TriggerRFMEvent(RFMEvent rfmEvent, params string[] args)
		{
			List<ConditionBundleDefinition> result = new List<ConditionBundleDefinition>();
			CheckCrossDay();
			if (rfmEvent != RFMEvent.ReturnCamp)
			{
				if (RFMEvents == null)
				{
					RFMEvents = new List<RFMEventInfo>();
				}
				RFMEvents.Add(new RFMEventInfo
				{
					Event = rfmEvent,
					Args = args
				});
				return result;
			}
			ConditionBundleConfig conditionBundleConfig = base.manager.GameEconomyData.ConditionBundleConfig;
			if (conditionBundleConfig != null)
			{
				if (base.manager.Player.CouncilLevel < conditionBundleConfig.UnlockCouncilLevel)
				{
					if (IsLoadDataManager)
					{
						var text = $"Сумка не работает. Уровень лагеря {base.manager.Player.CouncilLevel} должен быть больше {conditionBundleConfig.UnlockCouncilLevel}";
						AlertPopup.ShowPopup("", text, LocalizationManager.GetText("Button.Ok"));
					}
					return result;
				}
				long now = base.manager.Player.UtcTimeStamp;
				int num = PushGiftCount.Sum((KeyValuePair<string, int> x) => x.Value);
				List<string> list = (from x in PushGiftTime
					where x.Value > now
					select x.Key).ToList();
				if (num < conditionBundleConfig.MaxDailyPopLimit && list.Count < 1)
				{
					string playerBundleStatus = GetPlayerBundleStatus();
					List<ConditionBundleDefinition> bundelsByEvent = GetBundelsByEvent(list, playerBundleStatus);
					result = RandomPushGift(bundelsByEvent);
				}
				else
				{
					if (IsLoadDataManager)
					{
						var convertPushTimeValues = new List<string>();
						foreach (var str in list)
						{
							convertPushTimeValues.Add(str + " : " + MyTools.LongToTime(PushGiftTime[str]));
						}
						string dates = "";
						if (convertPushTimeValues.Count > 0)
						{
							dates = "Должны произойти push-события: " + string.Join("\n", convertPushTimeValues);
						}
						string text = $"Сегодня осталось еще {conditionBundleConfig.MaxDailyPopLimit - num}/{conditionBundleConfig.MaxDailyPopLimit} push-событие\n";
						if (!string.IsNullOrEmpty(dates))
						{
							text += dates;
						}
						AlertPopup.ShowPopup("", text, LocalizationManager.GetText("Button.Ok"));
					}
				}
			}
			RFMEvents.Clear();
			return result;
		}

		public List<ConditionBundleDefinition> RandomPushGift(List<ConditionBundleDefinition> gifts)
		{
			List<ConditionBundleDefinition> list = new List<ConditionBundleDefinition>();
			ConditionBundleConfig conditionBundleConfig = base.manager.GameEconomyData.ConditionBundleConfig;
			foreach (ConditionBundleDefinition gift in gifts)
			{
				if (!GiftPushRatio.TryGetValue(gift.BundleIdentifier, out var value))
				{
					value = conditionBundleConfig.BundleBaseChance;
				}
				if (base.manager.Player.RollDice(RollDiceType.PushGift, value) == PlayerRandomChanceResult.Success)
				{
					GiftPushRatio.Remove(gift.BundleIdentifier);
					if (!PushGiftCount.ContainsKey(gift.BundleIdentifier))
					{
						PushGiftCount[gift.BundleIdentifier] = 0;
					}
					PushGiftCount[gift.BundleIdentifier]++;
					PushGiftTime[gift.BundleIdentifier] = base.manager.Player.UtcTimeStamp + gift.TimeLimit;
					list.Add(gift);
				}
				else
				{
					GiftPushRatio[gift.BundleIdentifier] = value + conditionBundleConfig.BundleAccumulateChance;
				}
			}
			return list;
		}

		public List<ConditionBundleDefinition> GetBundelsByEvent(List<string> curEffectLst, string status)
		{
			int rfmValue = GetRFMValue();
			List<ConditionBundleDefinition> list = new List<ConditionBundleDefinition>();
			if (base.manager.GameEconomyData.ConditionBundleDefinitions != null)
			{
				List<ConditionBundleDefinition> list2 = base.manager.GameEconomyData.ConditionBundleDefinitions.Where((ConditionBundleDefinition x) => x.BundleStatusH == status && x.RMFValue == rfmValue).ToList();
				foreach (RFMEventInfo rFMEvent in RFMEvents)
				{
					foreach (ConditionBundleDefinition item in list2)
					{
						if (item.Condition == rFMEvent.Event && (item.Condition != RFMEvent.challengeLevelReached || !(rFMEvent.Args[0] != item.Params[0])) && !curEffectLst.Contains(item.BundleIdentifier) && (!PushGiftCount.TryGetValue(item.BundleIdentifier, out var value) || value < item.DailyPopLimit))
						{
							list.Add(item);
							BundleContentDefinition bundleContentDefinition = base.manager.GameEconomyData.GetBundleContentDefinition(item.BundleIdentifier);
							if (bundleContentDefinition != null)
							{
								base.manager.TdMetrics.SetEventType("Condi_Bundle_Pop").AddProperty("BundleIdentifier", item.BundleIdentifier).AddProperty("Condition", item.Condition.ToString())
									.AddProperty("RecencyLevel", item.RecencyLevel.ToString())
									.AddProperty("FrequencyLevel", item.FrequencyLevel.ToString())
									.AddProperty("MonetaryLevel", item.MonetaryLevel.ToString())
									.AddProperty("TimeLimit", item.TimeLimit)
									.AddProperty("Rewards", bundleContentDefinition.Rewards)
									.AddProperty("Price", bundleContentDefinition.IAPProduct)
									.Send();
							}
						}
					}
				}
			}
			return list.OrderBy((ConditionBundleDefinition x) => x.Priority).Take(1 - curEffectLst.Count).ToList();
		}

		public string GetPlayerBundleStatus()
		{
			List<RFMBundleStatusWeight> list = new List<RFMBundleStatusWeight>();
			ConditionBundleConfig conditionBundleConfig = base.manager.GameEconomyData.ConditionBundleConfig;
			PlayerModel player = base.manager.Player;
			CurrencyType[] array = new CurrencyType[4]
			{
				CurrencyType.Diamonds,
				CurrencyType.Supplies,
				CurrencyType.SurvivalPoints,
				CurrencyType.Phone
			};
			float[] array2 = new float[4]
			{
				conditionBundleConfig.GoldBaseValue,
				(float)conditionBundleConfig.ResourceBaseValue * 0.01f * (float)player.GetCurrency(CurrencyType.Supplies).Max,
				(float)conditionBundleConfig.XPBaseValue * 0.01f * (float)player.GetCurrency(CurrencyType.SurvivalPoints).Max,
				conditionBundleConfig.RadioBaseValue
			};
			int[] array3 = new int[4] { conditionBundleConfig.GoldPoolWeight, conditionBundleConfig.ResourcePoolWeight, conditionBundleConfig.XPPoolWeight, conditionBundleConfig.RadioPoolWeight };
			string[] array4 = new string[4] { "GL", "TL", "XL", "RL" };
			string bagConditionSteps = "";

			for (int i = 0; i < array.Length; i++)
			{
				CurrencyType currencyType = array[i];
				if ((float)player.GetCurrency(currencyType).Value <= array2[i])
				{
					list.Add(new RFMBundleStatusWeight
					{
						Status = array4[i],
						Weight = array3[i]
					});
				}
				else
				{
					bagConditionSteps += $"Нужно потратить {(float)player.GetCurrency(currencyType).Value - array2[i]} {LocalizeFromCurrency(currencyType)}\n";
				}
			}
			if (list.Count == 0)
			{
				if (IsLoadDataManager && !string.IsNullOrEmpty(bagConditionSteps))
				{
					AlertPopup.ShowPopup("", bagConditionSteps, LocalizationManager.GetText("Button.Ok"));
				}
				return "H";
			}
			RFMBundleStatusWeight rFMBundleStatusWeight = player.PlayerRandom.WeightedRandomList(list, 1, (RFMBundleStatusWeight x) => x.Weight, isRepeat: false).First();
			if (IsLoadDataManager)
			{
				PlayerRandomValues.Instance.InvokeBagCheck(true);
			}
			if (rFMBundleStatusWeight == null)
			{
				return "H";
			}
			return rFMBundleStatusWeight.Status;
		}


		#region myparams
		[JsonIgnore]
		private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
		#endregion

		#region mycode
		private string LocalizeFromCurrency(CurrencyType type)
		{
			switch (type)
			{
				case CurrencyType.Diamonds: return "золота (gold)";
				case CurrencyType.Supplies: return "ящиков (supply)";
				case CurrencyType.SurvivalPoints: return "опыта (XP)";
				case CurrencyType.Phone: return "раций (radio)";
				default: return "";
			}
		}
		#endregion
	}
}
