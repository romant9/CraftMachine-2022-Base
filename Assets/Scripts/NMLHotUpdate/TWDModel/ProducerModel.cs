using System;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ProducerModel : TWDModelObject
	{
		public long Accumulated;

		public CurrencyType CurrencyType { get; private set; }

		public int Amount { get; private set; }

		public int Capacity { get; private set; }

		[JsonIgnore]
		public int Rate { get; protected set; }

		[JsonIgnore]
		public int LastCollectedAmount { get; protected set; }

		[JsonIgnore]
		private CurrencyModel Currency => base.manager.Player.GetCurrency(CurrencyType);

		[JsonIgnore]
		public int GetAmountCollectable
		{
			get
			{
				CurrencyModel currency = Currency;
				if (currency == null)
				{
					return 0;
				}
				return Math.Min(Amount, currency.Max - currency.Value);
			}
		}

		[JsonIgnore]
		public bool HasEnoughToCollect
		{
			get
			{
				int num = base.gameEconomyData.ConfigData.ProductionPercentShowCollect * Rate / 100;
				return (float)Amount >= Math.Max(1f, num);
			}
		}

		public long ProductionHaltedTimer { get; set; }

		[JsonIgnore]
		public bool IsProductionHalted => ProductionHaltedTimer > 0;

		public ProducerModel()
		{
		}

		public ProducerModel(CurrencyType type)
		{
			CurrencyType = type;
		}

		public override bool IsValid()
		{
			if (Amount > Capacity)
			{
				base.Debug.LogError("amount " + Amount + ", capacity " + Capacity);
				return false;
			}
			if (CurrencyType == CurrencyType.None)
			{
				base.Debug.LogError("No Currency type defined");
				return false;
			}
			return true;
		}

		public void TickProduction(long deltaTime)
		{
			if (ProductionHaltedTimer > 0)
			{
				ProductionHaltedTimer -= deltaTime;
				if (ProductionHaltedTimer >= 0)
				{
					return;
				}
				deltaTime = -ProductionHaltedTimer;
				ProductionHaltedTimer = 0L;
			}
			else
			{
				ProductionHaltedTimer = 0L;
			}
			Accumulated += Rate * deltaTime;
			long num = Accumulated / 3600000;
			if (num > 0)
			{
				Accumulated -= num * 3600000;
				if (Amount < Capacity)
				{
					Amount += (int)num;
					Amount = Math.Min(Amount, Capacity);
					NotifyChange("amount");
				}
			}
		}

		public int Steal(int stealAmount)
		{
			int num = Amount - stealAmount;
			Amount = Math.Max(0, num);
			NotifyChange("amount");
			if (num < 0)
			{
				return stealAmount + num;
			}
			return stealAmount;
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			TickProduction(deltaTime);
		}

		public void SetRate(int rate)
		{
			Rate = rate;
		}

		public void SetAmount(int amount)
		{
			Amount = amount;
			NotifyChange("amount");
		}

		public void SetCapacity(int capacity)
		{
			Capacity = capacity;
		}

		public int Collect()
		{
			LastCollectedAmount = GetAmountCollectable;
			if (LastCollectedAmount > 0)
			{
				SetAmount(Amount - LastCollectedAmount);
				Currency.Add(LastCollectedAmount);
				NotifyChange("collect", LastCollectedAmount);
			}
			return LastCollectedAmount;
		}

		public void RepairHaltedProduction()
		{
			ProductionHaltedTimer = 0L;
		}
	}
}
