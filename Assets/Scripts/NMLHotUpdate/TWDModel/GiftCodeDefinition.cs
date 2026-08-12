using System;

namespace TWDModel
{
	public class GiftCodeDefinition : IRedeemDefinition
	{
		public Rewards Rewards { get; }

		public string Identifier { get; }

		public string Code { get; }

		public int MinCouncil { get; }

		public int MaxCouncil { get; }

		public DateTime StartTimestamp { get; }

		public DateTime EndTimestamp { get; }

		public GiftCodeDefinition(GiftCodeDefinitionRaw definitionRaw)
		{
			Rewards = new Rewards(definitionRaw.Rewards, null, 0, EquipmentSource.GiftCode);
			Identifier = definitionRaw.Identifier;
			Code = definitionRaw.Code;
			MinCouncil = definitionRaw.MinCouncil;
			MaxCouncil = ((definitionRaw.MaxCouncil < 0) ? int.MaxValue : definitionRaw.MaxCouncil);
			StartTimestamp = GameEconomyData.ParseDateTime(definitionRaw.StartTimestamp);
			EndTimestamp = GameEconomyData.ParseDateTime(definitionRaw.EndTimestamp);
		}

		public RedeemValidity CheckValidity(PlayerModel playerModel)
		{
			if (playerModel.RedeemedCodes.Contains(Identifier))
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
