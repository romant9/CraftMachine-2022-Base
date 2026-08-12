using BaseModel;

namespace TWDModel
{
	public class EquipBadgeCommand : ConsumeCurrencyCommand
	{
		public int BadgeId { get; protected set; }

		public bool SaveExisting { get; set; }

		public EquipBadgeCommand()
		{
		}

		public EquipBadgeCommand(SurvivorModel survivorModel, BadgeModel badgeModel)
			: base(survivorModel)
		{
			BadgeId = badgeModel.ModelId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			SurvivorModel model = manager.GetModel<SurvivorModel>(base.ModelId);
			BadgeModel model2 = manager.GetModel<BadgeModel>(BadgeId);
			TWDModelResult tWDModelResult = model.EquipBadge(model2, SaveExisting);
			if (tWDModelResult == TWDModelResult.OK)
			{
				((TWDModelManager)manager).Metrics.AddEquip().AddBadge(model2).AddSurvivor(model)
					.Send();
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
