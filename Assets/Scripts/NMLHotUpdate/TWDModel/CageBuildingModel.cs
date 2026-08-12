using Newtonsoft.Json;

namespace TWDModel
{
	public class CageBuildingModel : ModelUpgraderBuildingModel
	{
		[JsonIgnore]
		public override bool UpgradeInside => false;

		[JsonIgnore]
		public OutpostWalkerModel UpgradingWalker => base.UpgradingModel as OutpostWalkerModel;

		public override TWDModelResult CancelUpgrade()
		{
			if (base.UpgradingModel == null)
			{
				return base.CancelUpgrade();
			}
			TWDModelResult num = ((OutpostWalkerModel)base.UpgradingModel).TimedActionModel.Cancel();
			if (num == TWDModelResult.OK)
			{
				Cashier cashier = UpgradingWalker.TimedActionModel.GetCashier();
				base.manager.Metrics.AddFind().AddResources(cashier.LastRefundAmounts).AddCancelUpgrade(this)
					.AddOupostWalker(UpgradingWalker)
					.Send();
				ResetUpgradingModel();
				NotifyChange("UpgradingItemCancelled");
			}
			return num;
		}
	}
}
