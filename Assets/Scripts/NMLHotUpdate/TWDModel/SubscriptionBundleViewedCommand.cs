using BaseModel;

namespace TWDModel
{
	public class SubscriptionBundleViewedCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			(manager as TWDModelManager).Player.SubscriptionBuyedBundleIds.Clear();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
