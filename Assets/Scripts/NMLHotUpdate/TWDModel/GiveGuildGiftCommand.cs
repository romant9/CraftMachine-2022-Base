using BaseModel;

namespace TWDModel
{
	public class GiveGuildGiftCommand : TWDSocialModelCommand
	{
		public string GiftSenderName { get; set; }

		public string GiftSenderId { get; set; }

		public DropType GiftType { get; set; }

		public long ExpirationTimeMs { get; set; }

		public string Message { get; set; }

		public bool UsePerk { get; set; }

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new GiveGuildGiftGroupCommand
			{
				GiftSenderId = GiftSenderId,
				GiftSenderName = GiftSenderName,
				Message = Message,
				GiftType = GiftType,
				ExpirationTimeMs = ExpirationTimeMs
			};
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			NGModelCommandRespond nGModelCommandRespond = null;
			if (modelManager is TWDModelManager tWDModelManager && tWDModelManager.Player.GetCashierForGuildGift(UsePerk).CanAfford() && tWDModelManager.Player.CanGiveGuildGift() && tWDModelManager.Player.GuildModel != null && tWDModelManager.Player.GuildModel.CanGiveGift())
			{
				nGModelCommandRespond = base.Execute(modelManager) as NGModelCommandRespond;
				if (nGModelCommandRespond != null && nGModelCommandRespond.GetModelResult() == TWDModelResult.OK && tWDModelManager.Player.PayForGuildGift(UsePerk) == TWDModelResult.OK)
				{
					tWDModelManager.Player.ResetGuildGiftCooldownTimer();
				}
			}
			return nGModelCommandRespond;
		}
	}
}
