using System.Collections.Generic;

namespace TWDModel
{
	public class SupportGiveCurrenciesLoadQueueMessage : SupportLoadQueueMessage
	{
		public List<SupportGiveCurrenciesEntry> Currencies { get; set; }

		public SupportGiveCurrenciesLoadQueueMessage()
		{
		}

		public SupportGiveCurrenciesLoadQueueMessage(List<SupportGiveCurrenciesEntry> currencies)
		{
			Currencies = currencies;
		}

		public override bool Execute(TWDModelManager manager)
		{
			Metrics metrics = new Metrics(manager).AddFind();
			bool flag = false;
			Metrics metrics2 = new Metrics(manager).AddRemove();
			bool flag2 = false;
			foreach (SupportGiveCurrenciesEntry currency2 in Currencies)
			{
				CurrencyModel currency = manager.Player.GetCurrency(currency2.CurrencyType);
				if (currency == null)
				{
					continue;
				}
				if (currency2.FillUp)
				{
					int value = currency.Value;
					currency.SetValue(currency.Max);
					int num = currency.Value - value;
					if (num > 0)
					{
						metrics.PushResource(currency.Type, num);
						flag = true;
					}
					else if (num < 0)
					{
						metrics2.PushResource(currency.Type, num);
						flag2 = true;
					}
				}
				else if (currency2.AddValue < 0)
				{
					currency.SubtractLong(currency2.AddValue * -1);
					metrics2.PushResource(currency.Type, (int)currency2.AddValue);
					flag2 = true;
				}
				else
				{
					currency.AddLong(currency2.AddValue);
					metrics.PushResource(currency.Type, currency.LastAdded, ((int)currency2.AddValue != currency.LastAdded) ? ((int)currency2.AddValue - currency.LastAdded) : 0);
					flag = true;
				}
			}
			if (flag2)
			{
				metrics2.AddResources().AddSupport(base.SupportGivenTimestamp, base.SupportEntityGUID).Send();
			}
			if (flag)
			{
				metrics.AddResources().AddSupport(base.SupportGivenTimestamp, base.SupportEntityGUID).Send();
			}
			return true;
		}
	}
}
