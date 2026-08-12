using Newtonsoft.Json;

namespace TWDModel
{
	public class TimedBonusModelUnlimitedGasModel : TimedBonusModel
	{
		[JsonIgnore]
		private CurrencyModel gasCurrencyModel;

		public override void Start()
		{
			base.Start();
			base.TimedBonusTypeType = TimedBonusType.UnlimitedGas;
			gasCurrencyModel = base.manager.Player.GetCurrency(CurrencyType.ReplayToken);
		}

		protected override void DoInUpdate()
		{
			base.DoInUpdate();
			RefillGas();
		}

		public override TWDModelResult SetDuration(FixedPoint days)
		{
			RefillGas();
			return base.SetDuration(days);
		}

		private void RefillGas()
		{
			if (gasCurrencyModel != null)
			{
				int num = gasCurrencyModel.Max - gasCurrencyModel.Value;
				if (num > 0)
				{
					gasCurrencyModel.Add(num);
				}
			}
		}
	}
}
