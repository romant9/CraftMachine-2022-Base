using BaseModel;

namespace TWDModel
{
	public class StartPhoneCallCommand : ConsumeCurrencyCommand
	{
		public DropType DropType { get; set; }

		public int CallSlotNumber { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel playerModel = manager.GetPlayer() as PlayerModel;
			TWDModelResult result = TWDModelResult.Error;
			if (playerModel != null)
			{
				result = playerModel.PhoneCall.Call(DropType, CallSlotNumber);
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
