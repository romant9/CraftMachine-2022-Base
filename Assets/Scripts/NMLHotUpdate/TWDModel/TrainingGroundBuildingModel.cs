using Newtonsoft.Json;

namespace TWDModel
{
	public class TrainingGroundBuildingModel : ModelUpgraderBuildingModel
	{
		[JsonIgnore]
		public override bool UpgradeInside
		{
			get
			{
				if (!base.IsUpgrading && base.BuildingRepaired)
				{
					bool result = false;
					{
						foreach (SurvivorModel survivor in base.manager.Player.SurvivorContainer.Survivors)
						{
							if (survivor.IsUpgrading())
							{
								return false;
							}
							if (survivor.CanUpgrade && survivor.GetUpgradeCashier(instantUpgrade: false).CanAfford())
							{
								result = true;
							}
						}
						return result;
					}
				}
				return false;
			}
		}

		[JsonIgnore]
		public SurvivorModel UpgradingSurvivor => base.UpgradingModel as SurvivorModel;

		public override TWDModelResult CancelUpgrade()
		{
			if (base.UpgradingModel == null)
			{
				return base.CancelUpgrade();
			}
			TWDModelResult num = ((SurvivorModel)base.UpgradingModel).TimedActionModel.Cancel();
			if (num == TWDModelResult.OK)
			{
				Cashier cashier = UpgradingSurvivor.TimedActionModel.GetCashier();
				base.manager.Metrics.AddFind().AddResources(cashier.LastRefundAmounts).AddCancelUpgrade(this)
					.AddSurvivor(UpgradingSurvivor)
					.Send();
				ResetUpgradingModel();
				NotifyChange("UpgradingItemCancelled");
			}
			return num;
		}

		public override void MarkModelUpgradeAsSeen()
		{
			if (base.UpgradedUnseenModel is SurvivorModel survivorModel)
			{
				survivorModel.TriggerTrainedDailyQuestAction();
			}
			base.MarkModelUpgradeAsSeen();
		}
	}
}
