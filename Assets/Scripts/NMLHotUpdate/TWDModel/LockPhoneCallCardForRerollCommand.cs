using BaseModel;

namespace TWDModel
{
	public class LockPhoneCallCardForRerollCommand : ModelCommand
	{
		public int PhoneCallLootIndex { get; set; }

		public bool Locked { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel playerModel = manager.GetPlayer() as PlayerModel;
			TWDModelResult result = TWDModelResult.Error;
			if (playerModel != null)
			{
				if (playerModel.PhoneCall.NumRerolls > 0)
				{
					if (playerModel.PhoneCall.SetLootLockedForReroll(PhoneCallLootIndex, Locked))
					{
						result = TWDModelResult.OK;
					}
				}
				else
				{
					result = TWDModelResult.Error;
				}
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
