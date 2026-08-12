using BaseModel;

namespace TWDModel
{
	public class BuyBundleViewedCommand : ModelCommand
	{
		public BuyBundleViewedCommand()
		{
		}

		public BuyBundleViewedCommand(BundleManagerModel bundleManagerModel)
			: base(bundleManagerModel)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			manager.GetModel<BundleManagerModel>(base.ModelId).MarkManagerAsSeen();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
