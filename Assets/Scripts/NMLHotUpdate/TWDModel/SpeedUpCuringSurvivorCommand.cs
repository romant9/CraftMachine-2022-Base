using BaseModel;

namespace TWDModel
{
	public class SpeedUpCuringSurvivorCommand : ConsumeCurrencyCommand
	{
		public SpeedUpCuringSurvivorCommand()
		{
		}

		public SpeedUpCuringSurvivorCommand(SurvivorModel survivor)
			: base(survivor)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			SurvivorModel item = (SurvivorModel)manager.GetModel(base.ModelId);
			TWDModelResult result = ((manager.GetPlayer() as PlayerModel).Camp.GetBuilding("MedicTent") as MedicTentModel).TimedQueueModel.FinishOne(item, PurchaseType.SpeedUpCuringSurvivor, base.Cashier);
			return new NGModelCommandRespond(this, result);
		}
	}
}
