using BaseModel;

namespace TWDModel
{
	public class RepairProducerCommand : ModelCommand
	{
		public RepairProducerCommand()
		{
		}

		public RepairProducerCommand(ProducerModel producer)
			: base(producer)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			ProducerModel model = manager.GetModel<ProducerModel>(base.ModelId);
			TWDModelResult result = TWDModelResult.Error;
			if (model.IsProductionHalted)
			{
				model.RepairHaltedProduction();
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
