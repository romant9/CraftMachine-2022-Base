using BaseModel;

namespace TWDModel
{
	public class UpdateBlackMarketCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			((TWDModelManager)manager).Player.BlackMarket.UpdateSlots();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
