using BaseModel;

namespace TWDModel
{
	public class BananaBundleViewedCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (tWDModelManager.Player.WebShopBuyedBundleIds != null)
			{
				tWDModelManager.Player.WebShopBuyedBundleIds.Clear();
			}
			if (tWDModelManager.Player.WebShopBuyedTradeFairBundleIds != null)
			{
				tWDModelManager.Player.WebShopBuyedTradeFairBundleIds.Clear();
			}
			if (tWDModelManager.Player.WebshopBuyedBundleSingularSyncDatas != null)
			{
				tWDModelManager.Player.WebshopBuyedBundleSingularSyncDatas.Clear();
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
