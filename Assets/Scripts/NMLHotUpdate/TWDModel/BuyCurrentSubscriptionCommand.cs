using BaseModel;

namespace TWDModel
{
	public class BuyCurrentSubscriptionCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
