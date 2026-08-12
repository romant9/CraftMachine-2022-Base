using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	public class TimedActionModel : TWDModelObject
	{
		public const string ActionStartEvent = "ActionStartEvent";

		public const string ActionFinishedEvent = "ActionFinishedEvent";

		public const string ActionCanceledEvent = "ActionCanceledEvent";

		public const string ActionUpdatedEvent = "ActionUpdatedEvent";

		private Cashier cashier;

		public long MillisecondsTillCompletion { get; set; }

		public long OriginalActionTime { get; protected set; }

		public bool Paused { get; set; }

		public PurchaseType PurchaseType { get; set; }

		[JsonIgnore]
		public long RemainingTimeForRepetitiveAction { get; set; }

		[JsonIgnore]
		public bool WasSpeedUp { get; private set; }

		[JsonIgnore]
		public bool WasInstant { get; private set; }

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (MillisecondsTillCompletion > 0 && !Paused)
			{
				long remainingTimeForRepetitiveAction = deltaTime - MillisecondsTillCompletion;
				MillisecondsTillCompletion -= deltaTime;
				if (MillisecondsTillCompletion <= 0)
				{
					RemainingTimeForRepetitiveAction = remainingTimeForRepetitiveAction;
					FinishAction();
				}
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public TWDModelResult StartActionInstant(Cashier cashier, TWDModelObject sourceObject = null)
		{
			TWDModelResult tWDModelResult = TWDModelResult.OK;
			if (cashier != null)
			{
				this.cashier = cashier;
				if (cashier.useTokensForPayment)
				{
					tWDModelResult = cashier.PayWithTokens(sourceObject);
				}
				else
				{
					if (sourceObject is EquipmentItemModel)
					{
						cashier.UsedReason = "UpgradeEquipmentInstanst";
					}
					tWDModelResult = cashier.Pay(sourceObject);
				}
				if (tWDModelResult != TWDModelResult.OK)
				{
					return tWDModelResult;
				}
			}
			WasInstant = true;
			FinishAction();
			return TWDModelResult.OK;
		}

		public void Restart()
		{
			StartAction((int)OriginalActionTime / 1000, cashier);
		}

		public void SetCashier(Cashier cashier)
		{
			this.cashier = cashier;
		}

		public Cashier GetCashier()
		{
			return cashier;
		}

		public TWDModelResult StartAction(int time, Cashier cashier = null, TWDModelObject sourceObject = null)
		{
			if (IsActionUnderway())
			{
				return TWDModelResult.TimedActionAlreadyRunning;
			}
			if (cashier != null)
			{
				this.cashier = cashier;
				TWDModelResult tWDModelResult = cashier.Pay(sourceObject);
				if (tWDModelResult != TWDModelResult.OK)
				{
					return tWDModelResult;
				}
			}
			InitAction(time);
			return TWDModelResult.OK;
		}

		public TWDModelResult SpeedUpAction(TWDModelObject sourceObject = null)
		{
			TWDModelResult tWDModelResult = TWDModelResult.OK;
			if (MillisecondsTillCompletion <= 0)
			{
				return TWDModelResult.TimedActionNotRunning;
			}
			cashier = GetSpeedUpCashier();
			tWDModelResult = cashier.Pay(sourceObject);
			if (tWDModelResult != TWDModelResult.OK)
			{
				return tWDModelResult;
			}
			WasSpeedUp = true;
			FinishAction();
			return TWDModelResult.OK;
		}

		public TWDModelResult SpeedUpSurvivorUpgradeAction(TWDModelObject sourceObject = null, Cashier cashier = null)
		{
			TWDModelResult tWDModelResult = TWDModelResult.OK;
			if (MillisecondsTillCompletion <= 0)
			{
				return TWDModelResult.TimedActionNotRunning;
			}
			if (cashier == null)
			{
				goto IL_02d4;
			}
			if (!cashier.useTokensForPayment)
			{
				Dictionary<CurrencyType, int> useExtraTokens = cashier.UseExtraTokens;
				if (useExtraTokens == null || useExtraTokens.Count <= 0)
				{
					goto IL_02d4;
				}
			}
			if (cashier.useTokensForPayment)
			{
				tWDModelResult = GetSpeedUpCashierWithTokens(CurrencyType.TrainingTokenBP).PayWithTokens(sourceObject);
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
							if (speedupTokenTimeDefinitionByCurrency == null || speedupTokenTimeDefinitionByCurrency.SpeedupType != SpeedupType.Training || speedupTokenTimeDefinitionByCurrency.GetSpeedupMSTime() <= 0)
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
					long num = MillisecondsTillCompletion;
					bool flag = false;
					foreach (KeyValuePair<long, SpeedupTokenTimeTranslate> item in list)
					{
						for (int num2 = 0; num2 < item.Value.ConsumeAmount; num2++)
						{
							num -= item.Value.SpeedupTimeMilliseconds;
							if (dictionary2.ContainsKey(item.Value.CurrencyType))
							{
								dictionary2[item.Value.CurrencyType]++;
							}
							else
							{
								dictionary2[item.Value.CurrencyType] = 1;
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
					foreach (KeyValuePair<CurrencyType, int> item2 in dictionary2)
					{
						tWDModelResult = Cashier.CreateOneItemCashier(base.manager, PurchaseType.SpeedUpSurvivorUpgrade, item2.Key, item2.Value).PayWithTokens(sourceObject);
						if (tWDModelResult != TWDModelResult.OK)
						{
							return tWDModelResult;
						}
					}
					MillisecondsTillCompletion = num;
					WasSpeedUp = true;
					if (MillisecondsTillCompletion <= 0)
					{
						FinishAction();
					}
					return TWDModelResult.OK;
				}
			}
			goto IL_02f2;
			IL_02f2:
			if (tWDModelResult != TWDModelResult.OK)
			{
				return tWDModelResult;
			}
			WasSpeedUp = true;
			FinishAction();
			return TWDModelResult.OK;
			IL_02d4:
			cashier = GetSpeedUpCashier();
			cashier.UsedReason = "UpgradeSurvivorSpeedUp";
			tWDModelResult = cashier.Pay(sourceObject);
			goto IL_02f2;
		}

		public TWDModelResult SpeedUpEquipmentUpgradeAction(TWDModelObject sourceObject = null, Cashier cashier = null)
		{
			TWDModelResult tWDModelResult = TWDModelResult.OK;
			if (MillisecondsTillCompletion <= 0)
			{
				return TWDModelResult.TimedActionNotRunning;
			}
			if (cashier == null)
			{
				goto IL_02d4;
			}
			if (!cashier.useTokensForPayment)
			{
				Dictionary<CurrencyType, int> useExtraTokens = cashier.UseExtraTokens;
				if (useExtraTokens == null || useExtraTokens.Count <= 0)
				{
					goto IL_02d4;
				}
			}
			if (cashier.useTokensForPayment)
			{
				tWDModelResult = GetSpeedUpCashierWithTokens(CurrencyType.EquipmentTokenBP).PayWithTokens(sourceObject);
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
							if (speedupTokenTimeDefinitionByCurrency == null || speedupTokenTimeDefinitionByCurrency.SpeedupType != SpeedupType.Equipment || speedupTokenTimeDefinitionByCurrency.GetSpeedupMSTime() <= 0)
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
					long num = MillisecondsTillCompletion;
					bool flag = false;
					foreach (KeyValuePair<long, SpeedupTokenTimeTranslate> item in list)
					{
						for (int num2 = 0; num2 < item.Value.ConsumeAmount; num2++)
						{
							num -= item.Value.SpeedupTimeMilliseconds;
							if (dictionary2.ContainsKey(item.Value.CurrencyType))
							{
								dictionary2[item.Value.CurrencyType]++;
							}
							else
							{
								dictionary2[item.Value.CurrencyType] = 1;
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
					foreach (KeyValuePair<CurrencyType, int> item2 in dictionary2)
					{
						tWDModelResult = Cashier.CreateOneItemCashier(base.manager, PurchaseType.SpeedUpEquipmentUpgrade, item2.Key, item2.Value).PayWithTokens(sourceObject);
						if (tWDModelResult != TWDModelResult.OK)
						{
							return tWDModelResult;
						}
					}
					MillisecondsTillCompletion = num;
					WasSpeedUp = true;
					if (MillisecondsTillCompletion <= 0)
					{
						FinishAction();
					}
					return TWDModelResult.OK;
				}
			}
			goto IL_02f2;
			IL_02f2:
			if (tWDModelResult != TWDModelResult.OK)
			{
				return tWDModelResult;
			}
			WasSpeedUp = true;
			FinishAction();
			return TWDModelResult.OK;
			IL_02d4:
			cashier = GetSpeedUpCashier();
			cashier.UsedReason = "UpgradeEquipmentSpeedUp";
			tWDModelResult = cashier.Pay(sourceObject);
			goto IL_02f2;
		}

		public Cashier GetSpeedUpCashier()
		{
			Cashier obj = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType);
			int cost = base.gameEconomyData.TimeToDiamonds(MillisecondsTillCompletion);
			if (!base.manager.Player.Tutorial.Completed && !base.manager.Player.Tutorial.ShowDiamondsHud && MillisecondsTillCompletion < 200000)
			{
				cost = 0;
			}
			cashierItem.SetCost(CurrencyType.Diamonds, cost);
			obj.AddItem(cashierItem);
			return obj;
		}

		public Cashier GetSpeedUpCashierWithTokens(CurrencyType currencyType)
		{
			Cashier obj = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType);
			obj.useTokensForPayment = true;
			cashierItem.SetCost(currencyType, 1);
			obj.AddItem(cashierItem);
			return obj;
		}

		public virtual TWDModelResult Cancel(List<CurrencyType> fullRefundForCurrencies = null)
		{
			if (MillisecondsTillCompletion <= 0)
			{
				return TWDModelResult.NotUpgrading;
			}
			MillisecondsTillCompletion = 0L;
			NotifyChange("ActionCanceledEvent");
			if (cashier != null)
			{
				cashier.Refund(base.manager.Player.gameEconomyData.ConfigData.CancelUpgradeRefundPercentage, dontAllowMultiplier: true, fullRefundForCurrencies);
			}
			return TWDModelResult.OK;
		}

		public bool IsActionUnderway()
		{
			return MillisecondsTillCompletion > 0;
		}

		public void FinishAction()
		{
			MillisecondsTillCompletion = 0L;
			NotifyChange("ActionFinishedEvent", this);
		}

		private void InitAction(int time)
		{
			Paused = false;
			MillisecondsTillCompletion = time * 1000;
			OriginalActionTime = MillisecondsTillCompletion;
			WasSpeedUp = false;
			WasInstant = false;
			NotifyChange("ActionStartEvent", this);
		}
	}
}
