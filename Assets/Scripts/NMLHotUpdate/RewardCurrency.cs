using System;
using TWDModel;

public class RewardCurrency : IReward
{
	public CurrencyType CurrencyType { get; set; }

	public int Amount { get; set; }

	public int AmountActuallyAdded { get; protected set; }

	public bool IsDiamondExchange { get; set; }

	public bool CanOverflowMax { get; set; }

	public bool WasConverted { get; private set; }

	public RewardType Type => RewardType.Currency;

	public object Give(TWDModelManager manager, object[] param = null)
	{
		CurrencyModel currency = manager.Player.GetCurrency(CurrencyType);
		if (Amount == -1)
		{
			int num = 0;
			bool flag = currency.Type == CurrencyType.Supplies || currency.Type == CurrencyType.SurvivalPoints;
			if (!flag)
			{
				num = (AmountActuallyAdded = Math.Max(0, currency.Max - currency.Value));
			}
			else
			{
				num = ((currency.Type == CurrencyType.SurvivalPoints) ? (currency.Max / manager.Player.GetSurvivalPointsMultiplierValue()) : ((currency.Type != CurrencyType.Supplies) ? currency.Max : (currency.Max / manager.Player.GetSuppliesMultiplierValue())));
				AmountActuallyAdded = currency.Max;
			}
			currency.Add(num, flag);
		}
		else
		{
			AmountActuallyAdded = Amount;
			if (IsDiamondExchange)
			{
				currency.AddFromDiamondExchange(Amount, notify: true);
			}
			else if (manager.GameEconomyData.IsSpeedUpTokenCurrencyType(CurrencyType))
			{
				WasConverted = currency.AddWithOverflowToDiamonds(Amount);
				AmountActuallyAdded = ((!WasConverted) ? AmountActuallyAdded : 0);
			}
			else
			{
				currency.Add(Amount, CanOverflowMax);
				if (Amount != currency.LastAdded)
				{
					Amount = (int)(Amount * currency.AddMultiplier);
					AmountActuallyAdded = currency.LastAdded;
				}
			}
		}
		return CurrencyType;
	}

	public int GetOverflowAmount()
	{
		return Amount - AmountActuallyAdded;
	}

	public RewardCurrency GetClone()
	{
		return new RewardCurrency
		{
			CurrencyType = CurrencyType,
			Amount = Amount,
			AmountActuallyAdded = AmountActuallyAdded,
			IsDiamondExchange = IsDiamondExchange,
			CanOverflowMax = CanOverflowMax
		};
	}
}
