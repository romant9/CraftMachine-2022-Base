using BaseModel;

namespace TWDModel
{
	public class InitializeBlackMarketCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			((TWDModelManager)manager).Player.BlackMarket.Init();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
