using BaseModel;

namespace TWDModel
{
	public class EquipPrizeWheelCommand : ConsumeCurrencyCommand
	{
		public EquipPrizeType TimeType { get; set; }

		public string Identifier { get; set; }

		public EquipPrizeWheelCommand()
		{
		}

		public EquipPrizeWheelCommand(EquipPrizeType timeType, string identifier)
		{
			TimeType = timeType;
			Identifier = identifier;
		}

		public static Cashier GetCashier(TWDModelManager manager, string identifier, EquipPrizeType equipPrizeType)
		{
			EquipPrizeWheelDefinition equipPrizeWheelDefinition = manager.GameEconomyData.GetEquipPrizeWheelDefinition(identifier);
			CurrencyType currency = ((equipPrizeWheelDefinition.RadioType == RadioType.GoldRadio) ? CurrencyType.GoldRadio : CurrencyType.Phone);
			Cashier cashier = Cashier.CreateOneItemCashier(manager, PurchaseType.EquipPrize, currency, (equipPrizeType == EquipPrizeType.Ten) ? equipPrizeWheelDefinition.TenTimesPrice : equipPrizeWheelDefinition.OncePrice);
			cashier.UseDiamondsAmount = -2;
			return cashier;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager tWDModelManager)
			{
				EquipPrizeWheelModel equipPrizeWheelModel = tWDModelManager.Player.EquipPrizeWheelModel;
				if (equipPrizeWheelModel == null)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				EquipPrizeWheelDefinition equipPrizeWheelDefinition = tWDModelManager.GameEconomyData.GetEquipPrizeWheelDefinition(Identifier);
				if (equipPrizeWheelDefinition == null || !equipPrizeWheelDefinition.IsOpen(tWDModelManager.Player.UtcTimeStamp))
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				Cashier cashier = GetCashier(tWDModelManager, Identifier, TimeType);
				TWDModelResult tWDModelResult = cashier.Pay();
				if (tWDModelResult != TWDModelResult.OK)
				{
					return new NGModelCommandRespond(this, tWDModelResult);
				}
				equipPrizeWheelModel.CurrentEquipPrizeType = TimeType;
				equipPrizeWheelModel.CurrentEquipPrizeWheelDefinition = equipPrizeWheelDefinition;
				equipPrizeWheelModel.AddReward(TimeType, equipPrizeWheelDefinition.SlotNumber);
				Metrics metrics = tWDModelManager.Metrics;
				metrics.ResourceChangeUsedReason = "EquipmentCall";
				metrics.AddItemChange().AddResources(cashier).Send();
				tWDModelManager.TdMetrics.SetEventType("equipment_call").AddProperty("equip_prize_phone_used", (TimeType == EquipPrizeType.Ten) ? equipPrizeWheelDefinition.TenTimesPrice : equipPrizeWheelDefinition.OncePrice).AddProperty("equip_prize_choose_type", TimeType)
					.AddProperty("equip_prize_call_slot", equipPrizeWheelDefinition.SlotNumber)
					.AddProperty("equip_prize_acceptance", equipPrizeWheelDefinition.SlotNumber)
					.Send();
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}
	}
}
