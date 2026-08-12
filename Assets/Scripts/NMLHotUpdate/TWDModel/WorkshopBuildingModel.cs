using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class WorkshopBuildingModel : ModelUpgraderBuildingModel
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
						foreach (EquipmentItemModel allEquipment in base.manager.Player.Equipment.GetAllEquipments())
						{
							if (allEquipment.IsUpgrading())
							{
								return false;
							}
							if (allEquipment.CanUpgrade && allEquipment.GetUpgradeCashier(instantUpgrade: false).CanAfford())
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
		public EquipmentItemModel UpgradingEquipment => base.UpgradingModel as EquipmentItemModel;

		public override TWDModelResult CancelUpgrade()
		{
			if (base.UpgradingModel == null)
			{
				return base.CancelUpgrade();
			}
			TWDModelResult num = UpgradingEquipment.TimedActionModel.Cancel(new List<CurrencyType> { CurrencyType.EquipmentUpgradeToken });
			if (num == TWDModelResult.OK)
			{
				Cashier cashier = UpgradingEquipment.TimedActionModel.GetCashier();
				base.manager.Metrics.AddFind().AddResources(cashier.LastRefundAmounts).AddCancelUpgrade(this)
					.AddEquipment(UpgradingEquipment)
					.Send();
				ResetUpgradingModel();
				NotifyChange("UpgradingItemCancelled");
			}
			return num;
		}

		public int GetMaxEquipmentLevel(int workshopLevel)
		{
			int num = 0;
			for (int i = 0; i < base.gameEconomyData.EquipmentLevelDefinitions.Length; i++)
			{
				EquipmentLevelDefinition equipmentLevelDefinition = base.gameEconomyData.EquipmentLevelDefinitions[i];
				if (workshopLevel >= equipmentLevelDefinition.WorkshopLevelRequired)
				{
					num = Math.Max(num, equipmentLevelDefinition.Level);
				}
			}
			return num;
		}

		public override void MarkModelUpgradeAsSeen()
		{
			base.MarkModelUpgradeAsSeen();
		}

		public override void Start()
		{
			base.Start();
			if (UpgradingEquipment?.TimedActionModel != null && base.UpgradedUnseenModel == null && UpgradingEquipment.TimedActionModel.MillisecondsTillCompletion <= 0)
			{
				UpgradingEquipment.TimedActionModel.MillisecondsTillCompletion = 1L;
			}
		}
	}
}
