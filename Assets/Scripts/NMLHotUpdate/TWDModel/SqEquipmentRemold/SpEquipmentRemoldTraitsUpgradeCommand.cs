using System.Collections.Generic;
using BaseModel;

namespace TWDModel.SqEquipmentRemold
{
	public class SpEquipmentRemoldTraitsUpgradeCommand : ConsumeCurrencyCommand
	{
		public new int ModelId { get; set; }

		public SpEquipmentRemoldTraitsUpgradeCommand()
		{
		}

		public SpEquipmentRemoldTraitsUpgradeCommand(int modelId)
		{
			ModelId = modelId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager { Player: not null } tWDModelManager) || tWDModelManager.Player.gameEconomyData == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			EquipmentItemModel model = manager.GetModel<EquipmentItemModel>(ModelId);
			if (model == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			string randomUpgradeableTraitId = model.SpEquipmentRemoldModel.GetRandomUpgradeableTraitId();
			if (string.IsNullOrEmpty(randomUpgradeableTraitId))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			Dictionary<CurrencyType, int> dictionary = model.SpEquipmentRemoldModel.CalculateUpgradeCost(randomUpgradeableTraitId);
			Cashier cashier = new Cashier(tWDModelManager);
			CashierItem cashierItem = new CashierItem(PurchaseType.SPEquipmentRemoldTraits);
			foreach (KeyValuePair<CurrencyType, int> item in dictionary)
			{
				cashierItem.SetCost(item.Key, item.Value);
			}
			cashier.AddItem(cashierItem);
			if (cashier.Pay(model) == TWDModelResult.OK)
			{
				string text = model.SpEquipmentRemoldModel.UpgradeTrait(randomUpgradeableTraitId);
				if (text != null)
				{
					SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = tWDModelManager.Player.gameEconomyData.GetSPTraitsRemodeDefinition(randomUpgradeableTraitId);
					if (sPTraitsRemodeDefinition.PassiveTraits != null && sPTraitsRemodeDefinition.PassiveTraits.Count > 0)
					{
						foreach (string passiveTrait in sPTraitsRemodeDefinition.PassiveTraits)
						{
							model.RemoveModSkillPassiveTrait(passiveTrait);
						}
					}
					SPTraitsRemoldDefinitions sPTraitsRemodeDefinition2 = tWDModelManager.Player.gameEconomyData.GetSPTraitsRemodeDefinition(text);
					if (sPTraitsRemodeDefinition2.PassiveTraits != null && sPTraitsRemodeDefinition2.PassiveTraits.Count > 0)
					{
						if (sPTraitsRemodeDefinition2?.EquipType != null && sPTraitsRemodeDefinition2.EquipType.Count > 0)
						{
							if (sPTraitsRemodeDefinition2.EquipType.Contains(model.Definition.Type.ToString()))
							{
								foreach (string passiveTrait2 in sPTraitsRemodeDefinition2.PassiveTraits)
								{
									model.ApplyModSkillPassiveTraitToOwner(passiveTrait2);
								}
							}
						}
						else
						{
							foreach (string passiveTrait3 in sPTraitsRemodeDefinition2.PassiveTraits)
							{
								model.ApplyModSkillPassiveTraitToOwner(passiveTrait3);
							}
						}
					}
					return new NGModelCommandRespond(this, TWDModelResult.OK);
				}
			}
			else
			{
				manager.Debug.LogError("SpEquipmentRemoldTraitsCommand Execute failed  EquipmentId : " + model.Definition.ID);
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}
	}
}
