using System;
using System.Linq;

namespace TWDModel
{
	public class DeepLinkDefinition : IRedeemDefinition
	{
		public Rewards Rewards { get; }

		public string Identifier { get; }

		public string DeepLink { get; }

		public string DeepLinkAction { get; }

		public int MinCouncil { get; }

		public int MaxCouncil { get; }

		public string SpenderTier { get; }

		public DateTime StartTimestamp { get; }

		public DateTime EndTimestamp { get; }

		public DeepLinkDefinition(DeepLinkDefinitionsRaw definitionRaw)
		{
			Rewards = new Rewards(definitionRaw.Rewards, null, 0, EquipmentSource.GiftCode);
			Identifier = definitionRaw.Identifier;
			DeepLink = definitionRaw.Deeplink;
			DeepLinkAction = definitionRaw.DeepLinkAction;
			MinCouncil = definitionRaw.MinCouncil;
			MaxCouncil = ((definitionRaw.MaxCouncil < 0) ? int.MaxValue : definitionRaw.MaxCouncil);
			StartTimestamp = GameEconomyData.ParseDateTime(definitionRaw.StartTimestamp);
			EndTimestamp = GameEconomyData.ParseDateTime(definitionRaw.EndTimestamp);
			SpenderTier = definitionRaw.SpenderTier;
		}

		public RedeemValidity CheckValidity(PlayerModel playerModel)
		{
			GameEconomyData gameEconomyData = playerModel.gameEconomyData;
			if (!gameEconomyData.IsInSpenderTier(playerModel, SpenderTier, playerModel.TotalUSDSpent, (int)playerModel.LifeTimeInDays, playerModel.GetTotalPurchases(), playerModel.BundleManager.GetSecondsSinceLastPurchaseThatCostMoney(), playerModel.CreationTimeStamp, playerModel.CouncilLevel))
			{
				return RedeemValidity.Invalid;
			}
			if (gameEconomyData.DeepLinkDefinitions.Any((DeepLinkDefinitionsRaw x) => x.Deeplink != DeepLink))
			{
				return RedeemValidity.Invalid;
			}
			if (playerModel.RedeemedDeeplinks.Contains(Identifier))
			{
				return RedeemValidity.AlreadyClaimed;
			}
			int councilLevel = playerModel.CouncilLevel;
			if (councilLevel < MinCouncil || councilLevel > MaxCouncil)
			{
				return RedeemValidity.LevelOffRange;
			}
			DateTime utcTime = playerModel.UtcTime;
			if (utcTime > EndTimestamp)
			{
				return RedeemValidity.Expired;
			}
			if (utcTime < StartTimestamp)
			{
				return RedeemValidity.Invalid;
			}
			return RedeemValidity.Valid;
		}
	}
}
