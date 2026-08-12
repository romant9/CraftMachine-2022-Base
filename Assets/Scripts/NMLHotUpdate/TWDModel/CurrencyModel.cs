using System;
using Newtonsoft.Json;

namespace TWDModel
{
	public class CurrencyModel : TWDModelObject
	{
		private static readonly CurrencyType[] classTokenCurrencyTypes = new CurrencyType[6]
		{
			CurrencyType.AssaultToken,
			CurrencyType.ScoutToken,
			CurrencyType.BruiserToken,
			CurrencyType.WarriorToken,
			CurrencyType.ShooterToken,
			CurrencyType.HunterToken
		};

		private static readonly CurrencyType[] heroTokenCurrencyTypes = new CurrencyType[47]
		{
			CurrencyType.CarolToken,
			CurrencyType.RickToken,
			CurrencyType.AbrahamToken,
			CurrencyType.NeganToken,
			CurrencyType.MichonneToken,
			CurrencyType.MorganToken,
			CurrencyType.MaggieToken,
			CurrencyType.JesusToken,
			CurrencyType.GlennToken,
			CurrencyType.DarylToken,
			CurrencyType.CarlToken,
			CurrencyType.TaraToken,
			CurrencyType.RositaToken,
			CurrencyType.EugeneToken,
			CurrencyType.AaronToken,
			CurrencyType.GabrielToken,
			CurrencyType.EzekielToken,
			CurrencyType.DwightToken,
			CurrencyType.SashaToken,
			CurrencyType.MerleToken,
			CurrencyType.GovernorToken,
			CurrencyType.JerryToken,
			CurrencyType.ScoutRickToken,
			CurrencyType.HunterMorganToken,
			CurrencyType.ScoutDarylToken,
			CurrencyType.BruiserGlennToken,
			CurrencyType.ShooterMaggieToken,
			CurrencyType.AlphaToken,
			CurrencyType.TDogToken,
			CurrencyType.ShaneToken,
			CurrencyType.PrincessToken,
			CurrencyType.YumikoToken,
			CurrencyType.BethToken,
			CurrencyType.MercerToken,
			CurrencyType.MagnaToken,
			CurrencyType.JadisToken,
			CurrencyType.CowboyNeganToken,
			CurrencyType.QuinnToken,
			CurrencyType.SimonToken,
			CurrencyType.ProtectorDarylToken,
			CurrencyType.GauntletAaronToken,
			CurrencyType.PerlieToken,
			CurrencyType.CroatToken,
			CurrencyType.QuickdrawCarolToken,
			CurrencyType.LydiaToken,
			CurrencyType.StrandToken,
			CurrencyType.ScoutMaggieToken
		};

		public CurrencyType Type { get; protected set; }

		public int Value { get; protected set; }

		public long SuppliesOverflow { get; protected set; }

		public long SurvivalPointsOverflow { get; protected set; }

		public int Bought { get; protected set; }

		public int Max { get; protected set; }

		public bool CanOverflowOnBuyDiamonds { get; set; }

		public long TicksToRecharge { get; private set; }

		public long AccumulatedRechargeTime { get; set; }

		public FixedPoint AddMultiplier { get; set; }

		[JsonIgnore]
		public int LastAdded { get; protected set; }

		[JsonIgnore]
		public bool IsFull => TotalValue >= Max;

		[JsonIgnore]
		public long TotalValue
		{
			get
			{
				if (Type == CurrencyType.Supplies)
				{
					return Value + SuppliesOverflow;
				}
				if (Type == CurrencyType.SurvivalPoints)
				{
					return Value + SurvivalPointsOverflow;
				}
				return Value;
			}
		}

		[JsonIgnore]
		public int MillisecondsToNextRecharge
		{
			get
			{
				if (TotalValue >= Max || TicksToRecharge == 0L)
				{
					return 0;
				}
				return (int)(TicksToRecharge - AccumulatedRechargeTime);
			}
		}

		[JsonIgnore]
		public long MillisecondsToFullRecharge
		{
			get
			{
				int millisecondsToNextRecharge = MillisecondsToNextRecharge;
				if (millisecondsToNextRecharge == 0)
				{
					return 0L;
				}
				return (Max - TotalValue - 1) * TicksToRecharge + millisecondsToNextRecharge;
			}
		}

		public CurrencyModel()
		{
		}

		public CurrencyModel(CurrencyType type)
		{
			Type = type;
			CanOverflowOnBuyDiamonds = true;
		}

		public static CurrencyType[] GetClassTokenCurrencyTypes()
		{
			return classTokenCurrencyTypes;
		}

		public static CurrencyType[] GetHeroTokenCurrencyTypes()
		{
			return heroTokenCurrencyTypes;
		}

		public override void Start()
		{
			base.Start();
			if (AddMultiplier == 0.0)
			{
				AddMultiplier = 1.0;
			}
			UpdateSpeedUpTokenCapacityOnStart();
		}

		private void UpdateSpeedUpTokenCapacityOnStart()
		{
			if (base.manager.GameEconomyData.IsSpeedUpTokenCurrencyType(Type))
			{
				int capacity = base.manager.Player.GetCapacity(Type);
				if (Max != capacity)
				{
					SetCapacity(capacity);
				}
			}
		}

		public override bool IsValid()
		{
			if (Value < 0)
			{
				base.Debug.LogError("Currency value should be >= 0 " + Value + " " + Type);
			}
			if (Bought < 0)
			{
				base.Debug.LogError("Currency Bought value should be >= 0 " + Type);
			}
			if (Value >= 0)
			{
				return Bought >= 0;
			}
			return false;
		}

		public void SetValue(int newValue)
		{
			if (Value != newValue)
			{
				Value = newValue;
				if (Value < 0)
				{
					Value = 0;
				}
				if (Type == CurrencyType.Supplies)
				{
					SuppliesOverflow = 0L;
				}
				else if (Type == CurrencyType.SurvivalPoints)
				{
					SurvivalPointsOverflow = 0L;
				}
				NotifyChange("value");
			}
		}

		public void SetValue(long newValue)
		{
			if (Type == CurrencyType.Supplies)
			{
				if (newValue > int.MaxValue)
				{
					SuppliesOverflow = newValue - int.MaxValue;
					Value = int.MaxValue;
				}
				else
				{
					Value = (int)newValue;
					SuppliesOverflow = 0L;
				}
				NotifyChange("value");
			}
			else if (Type == CurrencyType.SurvivalPoints)
			{
				if (newValue > int.MaxValue)
				{
					SurvivalPointsOverflow = newValue - int.MaxValue;
					Value = int.MaxValue;
				}
				else
				{
					Value = (int)newValue;
					SurvivalPointsOverflow = 0L;
				}
				NotifyChange("value");
			}
			else
			{
				SetValue((int)newValue);
			}
		}

		public void SetCapacity(int capacity)
		{
			if (Max != capacity)
			{
				Max = capacity;
				NotifyChange("value");
			}
		}

		public void Add(int amount, bool canOverflowMax = false, bool dontAllowMultiplier = false)
		{
			if (amount < 0)
			{
				base.Debug.LogError("Currency.Add invalid amount " + amount);
			}
			else
			{
				if (amount <= 0)
				{
					return;
				}
				if (!dontAllowMultiplier)
				{
					amount = (int)(amount * AddMultiplier);
				}
				if (canOverflowMax)
				{
					LastAdded = amount;
				}
				else
				{
					LastAdded = (int)Math.Min(amount, Math.Max(0L, Max - TotalValue));
				}
				if (!(TotalValue < Max || canOverflowMax))
				{
					return;
				}
				if (Type == CurrencyType.Supplies)
				{
					long num = TotalValue + LastAdded;
					if (num > int.MaxValue)
					{
						SuppliesOverflow = num - int.MaxValue;
						Value = int.MaxValue;
					}
					else
					{
						Value = (int)num;
						SuppliesOverflow = 0L;
					}
					base.manager.Blackboard.IncreaseCounter("Counter.Supplies.Collected", LastAdded);
				}
				else if (Type == CurrencyType.SurvivalPoints)
				{
					long num2 = TotalValue + LastAdded;
					if (num2 > int.MaxValue)
					{
						SurvivalPointsOverflow = num2 - int.MaxValue;
						Value = int.MaxValue;
					}
					else
					{
						Value = (int)num2;
						SurvivalPointsOverflow = 0L;
					}
					base.manager.Blackboard.IncreaseCounter("Counter.SurvivalPoints.Collected", LastAdded);
				}
				else
				{
					Value += LastAdded;
					if (Value > Max && !canOverflowMax)
					{
						Value = Max;
					}
					if (Type == CurrencyType.Gas)
					{
						base.manager.Blackboard.IncreaseCounter("Counter.Gas.Collected", LastAdded);
					}
				}
				NotifyChange("value");
			}
		}

		public bool AddWithOverflowToDiamonds(int amount)
		{
			if (amount < 0)
			{
				base.Debug.LogError("Currency.Add invalid amount " + amount);
				return false;
			}
			bool result = false;
			if (amount > 0)
			{
				LastAdded = Math.Min(amount, Max - Value);
				if (Value <= Max)
				{
					Value += amount;
					if (Value > Max)
					{
						int value = Value;
						int max = Max;
						Value = Max;
						int amount2 = base.manager.GameEconomyData.CurrencyToDiamonds(Type, value - max, base.manager.Player);
						base.manager.Player.GetCurrency(CurrencyType.Diamonds).Add(amount2);
						result = true;
						NotifyChange("CurrencyConvertToDiamondsEvent");
					}
					else
					{
						NotifyChange("SpeedUpTokenAcquired");
						result = false;
					}
					NotifyChange("value");
				}
				else
				{
					int amount3 = base.manager.GameEconomyData.CurrencyToDiamonds(Type, amount, base.manager.Player);
					base.manager.Player.GetCurrency(CurrencyType.Diamonds).Add(amount3);
					result = true;
					NotifyChange("CurrencyConvertToDiamondsEvent");
					Value = Max;
				}
			}
			return result;
		}

		public void AddFromDiamondExchange(int amount, bool notify = false)
		{
			if (amount < 0)
			{
				base.Debug.LogError("Currency.Add invalid amount " + amount);
				return;
			}
			if (Type == CurrencyType.Supplies)
			{
				long num = TotalValue + amount;
				if (num > int.MaxValue)
				{
					SuppliesOverflow = num - int.MaxValue;
					Value = int.MaxValue;
				}
				else
				{
					Value = (int)num;
					SuppliesOverflow = 0L;
				}
			}
			else if (Type == CurrencyType.SurvivalPoints)
			{
				long num2 = TotalValue + amount;
				if (num2 > int.MaxValue)
				{
					SurvivalPointsOverflow = num2 - int.MaxValue;
					Value = int.MaxValue;
				}
				else
				{
					Value = (int)num2;
					SurvivalPointsOverflow = 0L;
				}
			}
			else
			{
				Value += amount;
			}
			if (!CanOverflowOnBuyDiamonds && TotalValue > Max)
			{
				Value = Max;
				if (Type == CurrencyType.Supplies)
				{
					SuppliesOverflow = 0L;
				}
				else if (Type == CurrencyType.SurvivalPoints)
				{
					SurvivalPointsOverflow = 0L;
				}
			}
			if (notify)
			{
				NotifyChange("value");
			}
		}

		public void AddBought(int amount)
		{
			Bought += amount;
		}

		public void Subtract(int amount)
		{
			if (amount < 0)
			{
				if (base.manager != null)
				{
					base.manager.Debug.LogError("Currency.Subtract invalid amount " + amount);
				}
			}
			else
			{
				if (amount <= 0)
				{
					return;
				}
				Bought = Math.Max(Bought - amount, 0);
				if (Type == CurrencyType.Supplies && SuppliesOverflow > 0)
				{
					long num = TotalValue - amount;
					if (num > int.MaxValue)
					{
						SuppliesOverflow = num - int.MaxValue;
						Value = int.MaxValue;
					}
					else
					{
						Value = (int)num;
						SuppliesOverflow = 0L;
					}
				}
				else if (Type == CurrencyType.SurvivalPoints && SurvivalPointsOverflow > 0)
				{
					long num2 = TotalValue - amount;
					if (num2 > int.MaxValue)
					{
						SurvivalPointsOverflow = num2 - int.MaxValue;
						Value = int.MaxValue;
					}
					else
					{
						Value = (int)num2;
						SurvivalPointsOverflow = 0L;
					}
				}
				else
				{
					Value -= amount;
					if (Value < 0)
					{
						Value = 0;
					}
				}
				if (base.manager?.Player != null)
				{
					base.manager.Player.NotifyCurrencySpent(Type, amount);
				}
				NotifyChange("value");
			}
		}

		public void AddLong(long amount, bool canOverflowMax = true)
		{
			if (amount < 0)
			{
				base.Debug.LogError("Currency.AddLong invalid amount " + amount);
			}
			else
			{
				if (amount <= 0)
				{
					return;
				}
				long num = ((!canOverflowMax) ? Math.Min(amount, Math.Max(0L, Max - TotalValue)) : amount);
				LastAdded = (int)Math.Min(num, 2147483647L);
				if (!(TotalValue < Max || canOverflowMax))
				{
					return;
				}
				if (Type == CurrencyType.Supplies)
				{
					long num2 = TotalValue + num;
					if (num2 > int.MaxValue)
					{
						SuppliesOverflow = num2 - int.MaxValue;
						Value = int.MaxValue;
					}
					else
					{
						Value = (int)num2;
						SuppliesOverflow = 0L;
					}
				}
				else if (Type == CurrencyType.SurvivalPoints)
				{
					long num3 = TotalValue + num;
					if (num3 > int.MaxValue)
					{
						SurvivalPointsOverflow = num3 - int.MaxValue;
						Value = int.MaxValue;
					}
					else
					{
						Value = (int)num3;
						SurvivalPointsOverflow = 0L;
					}
				}
				else
				{
					Value += LastAdded;
					if (Value > Max && !canOverflowMax)
					{
						Value = Max;
					}
				}
				NotifyChange("value");
			}
		}

		public void SubtractLong(long amount)
		{
			if (amount < 0)
			{
				base.Debug.LogError("Currency.SubtractLong invalid amount " + amount);
			}
			else
			{
				if (amount <= 0)
				{
					return;
				}
				if (Type == CurrencyType.Supplies)
				{
					long num = TotalValue - amount;
					if (num < 0)
					{
						num = 0L;
					}
					if (num > int.MaxValue)
					{
						SuppliesOverflow = num - int.MaxValue;
						Value = int.MaxValue;
					}
					else
					{
						Value = (int)num;
						SuppliesOverflow = 0L;
					}
				}
				else if (Type == CurrencyType.SurvivalPoints)
				{
					long num2 = TotalValue - amount;
					if (num2 < 0)
					{
						num2 = 0L;
					}
					if (num2 > int.MaxValue)
					{
						SurvivalPointsOverflow = num2 - int.MaxValue;
						Value = int.MaxValue;
					}
					else
					{
						Value = (int)num2;
						SurvivalPointsOverflow = 0L;
					}
				}
				else
				{
					Value -= (int)Math.Min(amount, Value);
					if (Value < 0)
					{
						Value = 0;
					}
				}
				NotifyChange("value");
			}
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (TicksToRecharge > 0 && TotalValue < Max)
			{
				AccumulatedRechargeTime += deltaTime;
				long num = AccumulatedRechargeTime / TicksToRecharge;
				if (num > 0)
				{
					AccumulatedRechargeTime -= num * TicksToRecharge;
					Add((int)num);
					base.manager.Metrics.AddFind().AddResources(CurrencyType.ReplayToken, (int)num, LastAdded).AddAutoFillTank()
						.Send();
				}
			}
		}

		public void SetRechargeTime(long timeSeconds)
		{
			TicksToRecharge = timeSeconds * 1000;
		}



		#region mycode
		public void ChangeValue(int amount)
		{
			if (Value + amount > 0)
			{
				Value += amount;
			}
			else
			{
				Value = 0;
			}
			NotifyChange("value");
		}
		#endregion
	}
}
