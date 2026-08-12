namespace TWDModel
{
	public class SupportGiveConsumablesEntry
	{
		public EquipmentModel.ConsumableType ConsumableType { get; set; }

		public int AddValue { get; set; }

		public SupportGiveConsumablesEntry(EquipmentModel.ConsumableType consumableType, int addValue)
		{
			ConsumableType = consumableType;
			AddValue = addValue;
		}
	}
}
