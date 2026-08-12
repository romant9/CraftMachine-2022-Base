using System;
using System.Collections.Generic;
using BaseModel;
using TWDModel.ResponsClass;

namespace TWDModel
{
	public class ScrapEquipmentItemsCommand : ModelCommand
	{
		public List<int> modelIds { get; set; }

		public Rewards Rewards { get; set; }

		public ScrapEquipmentItemsCommand()
		{
			modelIds = new List<int>();
		}

		public ScrapEquipmentItemsCommand(List<EquipmentItemModel> equipmentItems)
		{
			modelIds = new List<int>();
			for (int i = 0; i < equipmentItems.Count; i++)
			{
				modelIds.Add(equipmentItems[i].ModelId);
			}
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel playerModel = manager.GetPlayer() as PlayerModel;
			List<EquipmentItemModel> list = new List<EquipmentItemModel>();
			for (int i = 0; i < modelIds.Count; i++)
			{
				EquipmentItemModel model = manager.GetModel<EquipmentItemModel>(modelIds[i]);
				if (model.Owner == null)
				{
					list.Add(model);
				}
			}
			Rewards rewards = new Rewards();
			int num = 0;
			int num2 = 0;
			Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
			Dictionary<CurrencyType, int> dictionary2 = new Dictionary<CurrencyType, int>();
			Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
			bool flag = true;
			for (int j = 0; j < list.Count; j++)
			{
				EquipmentItemModel equipmentItemModel = list[j];
				ResponsScrapEquipmentItem responsScrapEquipmentItem = playerModel.Equipment.ScrapEquipmentItem(equipmentItemModel);
				flag = flag && responsScrapEquipmentItem.Result == TWDModelResult.OK;
				if (responsScrapEquipmentItem.Rewards == null)
				{
					continue;
				}
				if (responsScrapEquipmentItem.Rewards.SurvivorClassRew != null)
				{
					foreach (CurrencyType key in responsScrapEquipmentItem.Rewards.SurvivorClassRew.Keys)
					{
						if (!dictionary.ContainsKey(key))
						{
							dictionary[key] = 0;
						}
						dictionary[key] += Math.Abs(responsScrapEquipmentItem.Rewards.SurvivorClassRew[key]);
					}
				}
				if (responsScrapEquipmentItem.Rewards.ScrapSpTokenRewards != null)
				{
					foreach (CurrencyType key2 in responsScrapEquipmentItem.Rewards.ScrapSpTokenRewards.Keys)
					{
						if (!dictionary2.ContainsKey(key2))
						{
							dictionary2[key2] = 0;
						}
						dictionary2[key2] += Math.Abs(responsScrapEquipmentItem.Rewards.ScrapSpTokenRewards[key2]);
					}
				}
				if (responsScrapEquipmentItem.Rewards.EquiTokenRewards != null)
				{
					foreach (string key3 in responsScrapEquipmentItem.Rewards.EquiTokenRewards.Keys)
					{
						if (!dictionary3.ContainsKey(key3))
						{
							dictionary3[key3] = 0;
						}
						dictionary3[key3] += responsScrapEquipmentItem.Rewards.EquiTokenRewards[key3];
					}
				}
				num += responsScrapEquipmentItem.Rewards.ScrapAmount;
				num2 += responsScrapEquipmentItem.Rewards.apocalypticEquipTokencount;
			}
			if (num > 0)
			{
				rewards.AddRewardCurrency(CurrencyType.SurvivalPoints, num, isDiamondExchange: false, canOverflowMax: false);
			}
			if (num2 > 0)
			{
				rewards.AddRewardCurrency(CurrencyType.ApocalypticEquipToken, num2, isDiamondExchange: false, canOverflowMax: false);
			}
			if (dictionary.Count > 0)
			{
				foreach (KeyValuePair<CurrencyType, int> item in dictionary)
				{
					if (item.Value > 0)
					{
						rewards.AddRewardCurrency(item.Key, item.Value, isDiamondExchange: false, canOverflowMax: false);
					}
				}
			}
			if (dictionary2.Count > 0)
			{
				foreach (KeyValuePair<CurrencyType, int> item2 in dictionary2)
				{
					if (item2.Value > 0)
					{
						rewards.AddRewardCurrency(item2.Key, item2.Value, isDiamondExchange: false, canOverflowMax: false);
					}
				}
			}
			if (dictionary3.Count > 0)
			{
				foreach (KeyValuePair<string, int> item3 in dictionary3)
				{
					if (item3.Value > 0)
					{
						Rewards rewards2 = new Rewards("EquipToken(" + item3.Key + "," + item3.Value + ")");
						rewards.RewardsList.AddRange(rewards2.RewardsList);
					}
				}
			}
			Rewards = rewards;
			TWDModelResult tWDModelResult = ((!flag) ? TWDModelResult.Error : TWDModelResult.OK);
			if (tWDModelResult == TWDModelResult.OK)
			{
				TWDModelManager obj = manager as TWDModelManager;
				obj.Player.DailyQuestManager.StartAction("Scrap").TargetType = "Equipment";
				obj.Player.DailyQuestManager.CommitAction();
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
