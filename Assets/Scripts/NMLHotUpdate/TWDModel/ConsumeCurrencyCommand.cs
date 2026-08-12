using BaseModel;

namespace TWDModel
{
	public class ConsumeCurrencyCommand : ModelCommand
	{
		public Cashier Cashier { get; set; }

		public int UseDiamondsAmount { get; set; }

		public ConsumeCurrencyCommand()
		{
		}

		public ConsumeCurrencyCommand(int modelId)
			: base(modelId)
		{
		}

		public ConsumeCurrencyCommand(ModelObject model)
			: base(model)
		{
		}
	}
}
