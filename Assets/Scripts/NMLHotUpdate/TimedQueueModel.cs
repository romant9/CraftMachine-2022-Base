using System.Collections.Generic;
using System.Linq;
using TWDModel;

public class TimedQueueModel : TWDModelObject
{
	private List<TimedQueueItemModel> itemsCompleted = new List<TimedQueueItemModel>();

	public List<TimedQueueItemModel> Active { get; set; }

	public List<TimedQueueItemModel> Queued { get; set; }

	public int NumberSlots { get; set; }

	public long TotalTime { get; set; }

	public Cashier GetFinishAllCashier(PurchaseType purchaseType)
	{
		Cashier cashier = new Cashier(base.manager);
		int num = 0;
		foreach (TimedQueueItemModel item in Active)
		{
			num += base.gameEconomyData.TimeToDiamonds(item.MillisecondsTillCompletion);
		}
		foreach (TimedQueueItemModel item2 in Queued)
		{
			num += base.gameEconomyData.TimeToDiamonds(item2.MillisecondsTillCompletion);
		}
		CashierItem cashierItem = new CashierItem(purchaseType);
		cashierItem.SetCost(CurrencyType.Diamonds, num);
		cashier.AddItem(cashierItem);
		return cashier;
	}

	public override void Initialize()
	{
		base.Initialize();
		Active = new List<TimedQueueItemModel>();
		Queued = new List<TimedQueueItemModel>();
	}

	public override void Tick(long deltaTime)
	{
		base.Tick(deltaTime);
		TryActivateQueuedItems();
		if (TotalTime > 0 && Active.Count > 0)
		{
			TotalTime -= deltaTime;
			if (TotalTime < 0)
			{
				TotalTime = 0L;
			}
		}
		while (deltaTime > 0 && Active.Count > 0)
		{
			long num = deltaTime;
			foreach (TimedQueueItemModel item in Active)
			{
				if (item.MillisecondsTillCompletion < num)
				{
					num = item.MillisecondsTillCompletion;
				}
			}
			deltaTime -= num;
			itemsCompleted.Clear();
			foreach (TimedQueueItemModel item2 in Active)
			{
				item2.MillisecondsTillCompletion -= num;
				if (item2.MillisecondsTillCompletion <= 0)
				{
					itemsCompleted.Add(item2);
				}
			}
			foreach (TimedQueueItemModel item3 in itemsCompleted)
			{
				Active.Remove(item3);
				ActivateNextItem();
				NotifyChange("ActionFinishedEvent", item3.Item);
			}
		}
	}

	public void Add(TWDModelObject item, int timeToCompletion)
	{
		TimedQueueItemModel timedQueueItemModel = new TimedQueueItemModel();
		timedQueueItemModel.Item = item;
		timedQueueItemModel.MillisecondsTillCompletion = (long)timeToCompletion * 1000L;
		timedQueueItemModel.OriginalActionTime = timedQueueItemModel.MillisecondsTillCompletion;
		if (Active.Count < NumberSlots)
		{
			ActivateItem(timedQueueItemModel);
		}
		else
		{
			Queued.Add(timedQueueItemModel);
		}
		CalculateTotalTime();
	}

	private void ActivateItem(TimedQueueItemModel queueItem)
	{
		Active.Add(queueItem);
	}

	public bool ActivateNextItem()
	{
		if (Active.Count < NumberSlots && Queued.Count > 0)
		{
			ActivateItem(Queued[0]);
			Queued.RemoveAt(0);
			return true;
		}
		return false;
	}

	private void TryActivateQueuedItems()
	{
		int num = 10;
		int num2 = 0;
		while (ActivateNextItem())
		{
			num2++;
			if (num2 > num)
			{
				break;
			}
		}
		if (num2 > 0)
		{
			CalculateTotalTime();
		}
	}

	public void UpdateNumberSlots(int slotsNumber)
	{
		NumberSlots = slotsNumber;
		TryActivateQueuedItems();
	}

	public bool IsActive(TimedQueueItemModel queueItem)
	{
		return Active.Contains(queueItem);
	}

	public bool IsQueued(TimedQueueItemModel queueItem)
	{
		return Queued.Contains(queueItem);
	}

	public bool Exists(TWDModelObject item)
	{
		TimedQueueItemModel queueItemFromItem = GetQueueItemFromItem(item);
		if (queueItemFromItem == null)
		{
			return false;
		}
		if (!IsActive(queueItemFromItem))
		{
			return IsQueued(queueItemFromItem);
		}
		return true;
	}

	public TimedQueueItemModel GetQueueItemFromItem(TWDModelObject item)
	{
		foreach (TimedQueueItemModel item2 in Active)
		{
			if (item2.Item == item)
			{
				return item2;
			}
		}
		foreach (TimedQueueItemModel item3 in Queued)
		{
			if (item3.Item == item)
			{
				return item3;
			}
		}
		return new TimedQueueItemModel();
	}

	public TWDModelResult FinishOne(TWDModelObject item, PurchaseType purchaseType, Cashier cashier = null)
	{
		TWDModelResult tWDModelResult = TWDModelResult.OK;
		TimedQueueItemModel queueItemFromItem = GetQueueItemFromItem(item);
		if (queueItemFromItem.MillisecondsTillCompletion <= 0)
		{
			return TWDModelResult.TimedActionNotRunning;
		}
		if (cashier == null)
		{
			goto IL_0300;
		}
		if (!cashier.useTokensForPayment)
		{
			Dictionary<CurrencyType, int> useExtraTokens = cashier.UseExtraTokens;
			if (useExtraTokens == null || useExtraTokens.Count <= 0)
			{
				goto IL_0300;
			}
		}
		if (cashier.useTokensForPayment)
		{
			tWDModelResult = Cashier.CreateOneItemCashier(base.manager, PurchaseType.SpeedUpCuringSurvivor, CurrencyType.HealingTokenBP, 1).PayWithTokens(queueItemFromItem.Item);
		}
		else
		{
			Dictionary<CurrencyType, int> useExtraTokens2 = cashier.UseExtraTokens;
			if (useExtraTokens2 != null && useExtraTokens2.Count > 0)
			{
				Dictionary<long, SpeedupTokenTimeTranslate> dictionary = new Dictionary<long, SpeedupTokenTimeTranslate>();
				foreach (KeyValuePair<CurrencyType, int> useExtraToken in cashier.UseExtraTokens)
				{
					if (useExtraToken.Value > 0)
					{
						SpeedupTokenTimeDefinition speedupTokenTimeDefinitionByCurrency = base.manager.GameEconomyData.GetSpeedupTokenTimeDefinitionByCurrency(useExtraToken.Key.ToString());
						if (speedupTokenTimeDefinitionByCurrency == null || speedupTokenTimeDefinitionByCurrency.SpeedupType != SpeedupType.Healing || speedupTokenTimeDefinitionByCurrency.GetSpeedupMSTime() <= 0)
						{
							return TWDModelResult.Error;
						}
						if (dictionary.ContainsKey(useExtraToken.Value))
						{
							return TWDModelResult.Error;
						}
						SpeedupTokenTimeTranslate value = new SpeedupTokenTimeTranslate
						{
							CurrencyType = useExtraToken.Key,
							SpeedupTimeMilliseconds = speedupTokenTimeDefinitionByCurrency.GetSpeedupMSTime(),
							ConsumeAmount = useExtraToken.Value,
							SpeedupTokenTimeDefinition = speedupTokenTimeDefinitionByCurrency
						};
						dictionary.Add(speedupTokenTimeDefinitionByCurrency.GetSpeedupMSTime(), value);
					}
				}
				if (dictionary.Count == 0)
				{
					return TWDModelResult.Error;
				}
				Dictionary<CurrencyType, int> dictionary2 = new Dictionary<CurrencyType, int>();
				List<KeyValuePair<long, SpeedupTokenTimeTranslate>> list = dictionary.OrderByDescending((KeyValuePair<long, SpeedupTokenTimeTranslate> kv) => kv.Key).ToList();
				long num = queueItemFromItem.MillisecondsTillCompletion;
				bool flag = false;
				foreach (KeyValuePair<long, SpeedupTokenTimeTranslate> item2 in list)
				{
					for (int num2 = 0; num2 < item2.Value.ConsumeAmount; num2++)
					{
						num -= item2.Value.SpeedupTimeMilliseconds;
						if (dictionary2.ContainsKey(item2.Value.CurrencyType))
						{
							dictionary2[item2.Value.CurrencyType]++;
						}
						else
						{
							dictionary2[item2.Value.CurrencyType] = 1;
						}
						if (num <= 0)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
				foreach (KeyValuePair<CurrencyType, int> item3 in dictionary2)
				{
					tWDModelResult = Cashier.CreateOneItemCashier(base.manager, PurchaseType.SpeedUpCuringSurvivor, item3.Key, item3.Value).PayWithTokens(queueItemFromItem.Item);
					if (tWDModelResult != TWDModelResult.OK)
					{
						return tWDModelResult;
					}
				}
				queueItemFromItem.MillisecondsTillCompletion = num;
				if (queueItemFromItem.MillisecondsTillCompletion <= 0)
				{
					RemoveItemFromList(queueItemFromItem);
				}
				else
				{
					NotifyChange("ActionUpdatedEvent", queueItemFromItem.Item);
				}
				return TWDModelResult.OK;
			}
		}
		goto IL_0322;
		IL_0322:
		if (tWDModelResult != TWDModelResult.OK)
		{
			return tWDModelResult;
		}
		RemoveItemFromList(queueItemFromItem);
		return TWDModelResult.OK;
		IL_0300:
		Cashier finishOneCashier = GetFinishOneCashier(item, purchaseType);
		finishOneCashier.UsedReason = "CuringSurvivorSpeedUp";
		tWDModelResult = finishOneCashier.Pay(queueItemFromItem.Item);
		goto IL_0322;
	}

	public void RemoveItemFromList(TWDModelObject item)
	{
		RemoveItemFromList(GetQueueItemFromItem(item));
	}

	public void RemoveItemFromList(TimedQueueItemModel queueItemModel)
	{
		if (Active.Contains(queueItemModel))
		{
			Active.Remove(queueItemModel);
		}
		if (Queued.Contains(queueItemModel))
		{
			Queued.Remove(queueItemModel);
		}
		ActivateNextItem();
		CalculateTotalTime();
		NotifyChange("ActionFinishedEvent", queueItemModel.Item);
	}

	public Cashier GetFinishOneCashier(TWDModelObject item, PurchaseType purchaseType)
	{
		return GetFinishOneCashier(GetQueueItemFromItem(item), purchaseType);
	}

	public Cashier GetFinishOneCashier(TimedQueueItemModel queueItem, PurchaseType purchaseType)
	{
		Cashier cashier = new Cashier(base.manager);
		CashierItem cashierItem = new CashierItem(purchaseType);
		int cost = base.gameEconomyData.TimeToDiamonds(queueItem.MillisecondsTillCompletion);
		cashierItem.SetCost(CurrencyType.Diamonds, cost);
		cashier.AddItem(cashierItem);
		return cashier;
	}

	public TWDModelResult FinishAll(PurchaseType purchaseType)
	{
		Cashier finishAllCashier = GetFinishAllCashier(purchaseType);
		if (purchaseType == PurchaseType.SpeedUpCuringAllSurvivors)
		{
			finishAllCashier.UsedReason = "CuringAllSurvivorsSpeedUp";
		}
		TWDModelResult tWDModelResult = finishAllCashier.Pay(this);
		if (tWDModelResult != TWDModelResult.OK)
		{
			return tWDModelResult;
		}
		foreach (TimedQueueItemModel item in Active)
		{
			NotifyChange("ActionFinishedEvent", item.Item);
		}
		foreach (TimedQueueItemModel item2 in Queued)
		{
			NotifyChange("ActionFinishedEvent", item2.Item);
		}
		Active.Clear();
		Queued.Clear();
		TotalTime = 0L;
		return TWDModelResult.OK;
	}

	private void CalculateTotalTime()
	{
		List<long> list = new List<long>();
		List<long> list2 = new List<long>();
		TotalTime = 0L;
		foreach (TimedQueueItemModel item in Active)
		{
			list.Add(item.MillisecondsTillCompletion);
		}
		foreach (TimedQueueItemModel item2 in Queued)
		{
			list2.Add(item2.MillisecondsTillCompletion);
		}
		while (list.Count > 0)
		{
			int index = -1;
			long num = long.MaxValue;
			for (int i = 0; i < list.Count; i++)
			{
				long num2 = list[i];
				if (num2 < num)
				{
					index = i;
					num = num2;
				}
			}
			TotalTime += num;
			list.RemoveAt(index);
			for (int j = 0; j < list.Count; j++)
			{
				list[j] -= num;
			}
			if (list2.Count > 0)
			{
				list.Add(list2[0]);
				list2.RemoveAt(0);
			}
		}
	}

	public override bool IsValid()
	{
		return true;
	}
}
