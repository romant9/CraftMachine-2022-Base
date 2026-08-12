using System;

namespace TWDModel
{
	public class StorageModel : TWDModelObject, IComparable<StorageModel>
	{
		public CurrencyType CurrencyType { get; private set; }

		public int Amount { get; private set; }

		public int Capacity { get; private set; }

		public StorageModel()
		{
		}

		public StorageModel(CurrencyType type)
		{
			CurrencyType = type;
		}

		public override bool IsValid()
		{
			if (CurrencyType != CurrencyType.None)
			{
				return Amount <= Capacity;
			}
			return false;
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

		public int CompareTo(StorageModel other)
		{
			if (Capacity < other.Capacity)
			{
				return -1;
			}
			if (Capacity > other.Capacity)
			{
				return 1;
			}
			return 0;
		}
	}
}
