using System.Collections.Generic;
using BaseModel;
using TWDModel.ResponsClass;

namespace TWDModel
{
	public class ScrapEquipmentItemCommand : ModelCommand
	{
		public Rewards Rewards;

		public ScrapEquipmentItemCommand()
		{
		}

		public ScrapEquipmentItemCommand(EquipmentItemModel equipmentItemModel)
			: base(equipmentItemModel)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			EquipmentItemModel equipmentItemModel = (EquipmentItemModel)manager.GetModel(base.ModelId);
			PlayerModel playerModel = manager.GetPlayer() as PlayerModel;
			if (equipmentItemModel == null || playerModel == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			ResponsScrapEquipmentItem responsScrapEquipmentItem = playerModel.Equipment.ScrapEquipmentItem(equipmentItemModel);
			if (responsScrapEquipmentItem.Result == TWDModelResult.OK)
			{
				TWDModelManager obj = manager as TWDModelManager;
				obj.Player.DailyQuestManager.StartAction("Scrap").TargetType = "Equipment";
				obj.Player.DailyQuestManager.CommitAction();
				Rewards rewards = new Rewards();
				if (responsScrapEquipmentItem.Rewards != null)
				{
					if (responsScrapEquipmentItem.Rewards.ScrapAmount > 0)
					{
						rewards.AddRewardCurrency(CurrencyType.SurvivalPoints, responsScrapEquipmentItem.Rewards.ScrapAmount, isDiamondExchange: false, canOverflowMax: false);
					}
					if (responsScrapEquipmentItem.Rewards.apocalypticEquipTokencount > 0)
					{
						rewards.AddRewardCurrency(CurrencyType.ApocalypticEquipToken, responsScrapEquipmentItem.Rewards.apocalypticEquipTokencount, isDiamondExchange: false, canOverflowMax: false);
					}
					if (responsScrapEquipmentItem.Rewards.SurvivorClassRew != null && responsScrapEquipmentItem.Rewards.SurvivorClassRew.Count > 0)
					{
						foreach (KeyValuePair<CurrencyType, int> item in responsScrapEquipmentItem.Rewards.SurvivorClassRew)
						{
							if (item.Value > 0)
							{
								rewards.AddRewardCurrency(item.Key, item.Value, isDiamondExchange: false, canOverflowMax: false);
							}
						}
					}
					if (responsScrapEquipmentItem.Rewards.ScrapSpTokenRewards != null && responsScrapEquipmentItem.Rewards.ScrapSpTokenRewards.Count > 0)
					{
						foreach (KeyValuePair<CurrencyType, int> scrapSpTokenReward in responsScrapEquipmentItem.Rewards.ScrapSpTokenRewards)
						{
							if (scrapSpTokenReward.Value > 0)
							{
								rewards.AddRewardCurrency(scrapSpTokenReward.Key, scrapSpTokenReward.Value, isDiamondExchange: false, canOverflowMax: false);
							}
						}
					}
					if (responsScrapEquipmentItem.Rewards.EquiTokenRewards != null && responsScrapEquipmentItem.Rewards.EquiTokenRewards.Count > 0)
					{
						foreach (KeyValuePair<string, int> equiTokenReward in responsScrapEquipmentItem.Rewards.EquiTokenRewards)
						{
							if (equiTokenReward.Value > 0)
							{
								Rewards rewards2 = new Rewards("EquipToken(" + equiTokenReward.Key + "," + equiTokenReward.Value + ")");
								rewards.RewardsList.AddRange(rewards2.RewardsList);
							}
						}
					}
				}
				Rewards = rewards;
			}
			if (responsScrapEquipmentItem.Result != TWDModelResult.OK)
			{
				Rewards = null;
			}
			return new NGModelCommandRespond(this, responsScrapEquipmentItem.Result);
		}
	}
}
