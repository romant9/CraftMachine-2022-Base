using TWDModel;

namespace Client.Redeem
{
	public class GiftCodeRedeemManager : IRedeemManager
	{
		private readonly GameEconomyData gameEconomyData;

		private readonly PlayerModel playerModel;

		public GiftCodeRedeemManager()
		{
			gameEconomyData = GameManager.Instance.gameEconomyData;
			playerModel = GameManager.Instance.playerModel;
		}

		public RedeemValidity RedeemCode(string code, out IRedeemDefinition redeemDefinition)
		{
			if (gameEconomyData.TryGetGiftCodeDefinition(code, out var giftCodeDefinition))
			{
				RedeemValidity num = giftCodeDefinition.CheckValidity(playerModel);
				redeemDefinition = giftCodeDefinition;
				if (num == RedeemValidity.Valid)
				{
					Helpers.ExecuteCommand(new RedeemCodeCommand(code));
				}
				return num;
			}
			redeemDefinition = null;
			return RedeemValidity.Invalid;
		}
	}
}
