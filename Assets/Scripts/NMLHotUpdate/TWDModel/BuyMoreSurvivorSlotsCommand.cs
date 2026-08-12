using BaseModel;

namespace TWDModel
{
	public class BuyMoreSurvivorSlotsCommand : ConsumeCurrencyCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = (manager as TWDModelManager).Player.SurvivorContainer.BuyNextSetOfSurvivorSlots();
			return new NGModelCommandRespond(this, result);
		}
	}
}
