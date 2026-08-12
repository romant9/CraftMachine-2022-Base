using TWDModel;

namespace Client.Redeem
{
	public class DeepLinkRedeemManager : IRedeemManager
	{
		private readonly GameEconomyData gameEconomyData;

		private readonly PlayerModel playerModel;

		public DeepLinkRedeemManager()
		{
			gameEconomyData = GameManager.Instance.gameEconomyData;
			playerModel = GameManager.Instance.playerModel;
		}

		public RedeemValidity RedeemCode(string deepLink, out IRedeemDefinition redeemDefinition)
		{
			if (gameEconomyData.TryGetDeepLinkDefinition(deepLink, out var deepLinkDefinition))
			{
				RedeemValidity num = deepLinkDefinition.CheckValidity(playerModel);
				redeemDefinition = deepLinkDefinition;
				if (num == RedeemValidity.Valid)
				{
					Helpers.ExecuteCommand(new ConsumeDeepLinkCommand(deepLink));
				}
				return num;
			}
			redeemDefinition = null;
			return RedeemValidity.Invalid;
		}
	}
}
