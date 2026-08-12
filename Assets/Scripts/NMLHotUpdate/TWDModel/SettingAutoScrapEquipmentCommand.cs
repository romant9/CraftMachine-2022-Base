using BaseModel;

namespace TWDModel
{
	public class SettingAutoScrapEquipmentCommand : ModelCommand
	{
		public AutoScrapEquipmentType IsEquipmentAutoScrap { get; set; }

		public SettingAutoScrapEquipmentCommand(AutoScrapEquipmentType isEquipmentAutoScrap)
		{
			IsEquipmentAutoScrap = isEquipmentAutoScrap;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.OK;
			(manager.GetPlayer() as PlayerModel).IsEquipmentAutoScrap = IsEquipmentAutoScrap;
			return new NGModelCommandRespond(this, result);
		}
	}
}
