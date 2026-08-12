using System;
using BaseModel;

namespace TWDModel
{
	public class DebugAddRouletteCurrencyCommand : ModelCommand
	{
		public int Amount { get; set; }

		public DebugAddRouletteCurrencyCommand()
		{
		}

		public DebugAddRouletteCurrencyCommand(int amount)
		{
			Amount = amount;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			try
			{
				CurrencyModel currency = tWDModelManager.Player.GetCurrency(CurrencyType.Diamonds);
				int value = currency.Value;
				currency.SetValue(value + Amount);
				tWDModelManager.Debug.LogInfo($"[DebugAddRouletteCurrency] Added {Amount} diamonds to player. Current total: {currency.Value}");
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			catch (Exception ex)
			{
				tWDModelManager.Debug.LogError("[DebugAddRouletteCurrency] Exception: " + ex.Message);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
		}
	}
}
