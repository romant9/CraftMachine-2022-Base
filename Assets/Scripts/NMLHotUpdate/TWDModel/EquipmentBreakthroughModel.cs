using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class EquipmentBreakthroughModel : TWDModelObject
	{
		public int Level { get; set; }

		public bool UnlockedRandomTrait { get; set; }

		public TWDModelResult BreakthroughLevelUp(string equipIdentifier, int rarityLevel, List<EquipTokenItemModel> consumeEquipTokenModelList, int consumeApocalypticEquipTokenAmount)
		{
			if (consumeEquipTokenModelList == null || consumeEquipTokenModelList.Count == 0)
			{
				return TWDModelResult.Error;
			}
			if (consumeApocalypticEquipTokenAmount <= 0)
			{
				return TWDModelResult.Error;
			}
			int num = Level + 1;
			EquipBreakthroughDefinition equipBreakthroughDefinitionByRarityAndLevel = base.manager.GameEconomyData.GetEquipBreakthroughDefinitionByRarityAndLevel(rarityLevel, num);
			if (equipBreakthroughDefinitionByRarityAndLevel == null)
			{
				return TWDModelResult.Error;
			}
			EquipTokenDefinition equipTokenDefinitionByRelateEquipId = base.manager.GameEconomyData.GetEquipTokenDefinitionByRelateEquipId(equipIdentifier);
			Dictionary<EquipTokenItemModel, int> consumeEquipTokenModelDict = new Dictionary<EquipTokenItemModel, int>();
			foreach (EquipTokenItemModel consumeEquipTokenModel in consumeEquipTokenModelList)
			{
				if (consumeEquipTokenModelDict.ContainsKey(consumeEquipTokenModel))
				{
					consumeEquipTokenModelDict[consumeEquipTokenModel]++;
				}
				else
				{
					consumeEquipTokenModelDict[consumeEquipTokenModel] = 1;
				}
				if (equipTokenDefinitionByRelateEquipId.Category != consumeEquipTokenModel.Definition.Category)
				{
					return TWDModelResult.Error;
				}
				if (equipBreakthroughDefinitionByRarityAndLevel.WeaponDrawingType == WeaponDrawingType.SameClassWeapon)
				{
					if (equipTokenDefinitionByRelateEquipId.SurvivorClass != consumeEquipTokenModel.Definition.SurvivorClass)
					{
						return TWDModelResult.Error;
					}
					continue;
				}
				if (equipBreakthroughDefinitionByRarityAndLevel.WeaponDrawingType == WeaponDrawingType.SameNameWeapon)
				{
					if (equipTokenDefinitionByRelateEquipId.EquipmentBreakthroughsType != consumeEquipTokenModel.Definition.EquipmentBreakthroughsType)
					{
						return TWDModelResult.Error;
					}
					continue;
				}
				return TWDModelResult.Error;
			}
			Func<TWDModelResult> nonCurrencyTokenSubtractFunc = delegate
			{
				foreach (KeyValuePair<EquipTokenItemModel, int> item in consumeEquipTokenModelDict)
				{
					if (item.Key.OwnedTokensAmount < item.Value)
					{
						return TWDModelResult.NotEnoughEquipToken;
					}
				}
				foreach (KeyValuePair<EquipTokenItemModel, int> item2 in consumeEquipTokenModelDict)
				{
					item2.Key.AddEquipToken(-item2.Value);
				}
				return TWDModelResult.OK;
			};
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.EquipBreakthrough);
			cashierItem.SetCost(CurrencyType.ApocalypticEquipToken, consumeApocalypticEquipTokenAmount);
			cashier.AddItem(cashierItem);
			TWDModelResult tWDModelResult = cashier.Pay(equipBreakthroughDefinitionByRarityAndLevel, null, null, nonCurrencyTokenSubtractFunc);
			if (tWDModelResult != TWDModelResult.OK)
			{
				return tWDModelResult;
			}
			Metrics metrics = base.manager.Metrics;
			metrics.ResourceChangeUsedReason = "EquipmentBreakthrough";
			metrics.AddItemChange().AddResources(cashier).Send();
			string text = "";
			foreach (EquipTokenItemModel consumeEquipTokenModel2 in consumeEquipTokenModelList)
			{
				text += consumeEquipTokenModel2?.EquipTokenId;
			}
			base.manager.TdMetrics.SetEventType("equipment_breakthrough").AddProperty("breakthrough_equipment_id", equipIdentifier).AddProperty("breakthrough_lv_before", Level)
				.AddProperty("breakthrough_lv_after", num)
				.AddProperty("breakthrough_equiptoken_id_used", text)
				.Send();
			SetLevel(num);
			return TWDModelResult.OK;
		}

		public TWDModelResult BreakthroughRemoldLevelUp(string equipIdentifier, int rarityLevel, int consumeApocalypticEquipTokenAmount, SurvivorClass survivorClass)
		{
			if (consumeApocalypticEquipTokenAmount <= 0)
			{
				return TWDModelResult.Error;
			}
			int num = Level + 1;
			EquipBreakthroughDefinition remoldEquipBreakthroughDefinitionByRarityAndLevel = base.manager.GameEconomyData.GetRemoldEquipBreakthroughDefinitionByRarityAndLevel(rarityLevel, num);
			if (remoldEquipBreakthroughDefinitionByRarityAndLevel == null)
			{
				return TWDModelResult.Error;
			}
			base.manager.GameEconomyData.GetEquipTokenDefinitionByRelateEquipId(equipIdentifier);
			new Dictionary<EquipTokenItemModel, int>();
			CurrencyType survivorClassCurrencyType = GetSurvivorClassCurrencyType(survivorClass);
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.EquipBreakthrough);
			cashierItem.SetCost(survivorClassCurrencyType, remoldEquipBreakthroughDefinitionByRarityAndLevel.CommonBluePrintCost);
			cashierItem.SetCost(CurrencyType.ApocalypticEquipToken, consumeApocalypticEquipTokenAmount);
			cashier.AddItem(cashierItem);
			TWDModelResult tWDModelResult = cashier.Pay(remoldEquipBreakthroughDefinitionByRarityAndLevel);
			if (tWDModelResult != TWDModelResult.OK)
			{
				return tWDModelResult;
			}
			Metrics metrics = base.manager.Metrics;
			metrics.ResourceChangeUsedReason = "EquipmentBreakthrough";
			metrics.AddItemChange().AddResources(cashier).Send();
			base.manager.TdMetrics.SetEventType("equipment_breakthrough").AddProperty("breakthrough_equipment_id", equipIdentifier).AddProperty("breakthrough_lv_before", Level)
				.AddProperty("breakthrough_lv_after", num)
				.AddProperty("breakthrough_equiptoken_id_used", survivorClassCurrencyType)
				.AddProperty("breakthrough_equiptoken_id_used_cost", remoldEquipBreakthroughDefinitionByRarityAndLevel.CommonBluePrintCost)
				.Send();
			SetLevel(num);
			return TWDModelResult.OK;
		}

		public CurrencyType GetSurvivorClassCurrencyType(SurvivorClass survivorClass)
		{
			return survivorClass switch
			{
				SurvivorClass.Warrior => CurrencyType.CBPWarrior,
				SurvivorClass.Scout => CurrencyType.CBPScout,
				SurvivorClass.Bruiser => CurrencyType.CBPBruiser,
				SurvivorClass.Shooter => CurrencyType.CBPShooter,
				SurvivorClass.Hunter => CurrencyType.CBPHunter,
				SurvivorClass.Assault => CurrencyType.CBPAssault,
				_ => CurrencyType.None,
			};
		}

		public void SetLevel(int newLevel)
		{
			Level = newLevel;
		}

		public void UnlockRandomTrait()
		{
			UnlockedRandomTrait = true;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
