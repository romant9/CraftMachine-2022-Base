using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class EquipTokenContainerModel : TWDModelObject
	{
		public const string ArmorItemListKey = "ArmorItemListKey";

		public const string WeaponItemListKey = "WeaponItemListKey";

		public const string EquipmentTokenTypeUnlockEvent = "EquipmentTokenTypeUnlockEvent";

		public const string EquipmentTokenTypeUpdateEvent = "EquipmentTokenTypeUpdateEvent";

		public ModelList<EquipTokenItemModel> EquipTokenItems { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public bool CanAssemble()
		{
			if (EquipTokenItems == null)
			{
				return false;
			}
			for (int i = 0; i < EquipTokenItems.Count; i++)
			{
				if (EquipTokenItems[i].CanUnlock())
				{
					return true;
				}
			}
			return false;
		}

		public int GetEquipTokenCountBySurvivorClass(SurvivorClass survivorClass)
		{
			int num = 0;
			if (EquipTokenItems == null)
			{
				return num;
			}
			foreach (EquipTokenItemModel equipTokenItem in EquipTokenItems)
			{
				if (equipTokenItem.EquipmentDefinition.SurvivorClass == survivorClass && equipTokenItem.CanUnlock())
				{
					num++;
				}
			}
			return num;
		}

		public Dictionary<string, List<EquipTokenItemModel>> GetEquipTokenItems(SurvivorClass survivorClass)
		{
			Dictionary<string, List<EquipTokenItemModel>> dictionary = new Dictionary<string, List<EquipTokenItemModel>>
			{
				["ArmorItemListKey"] = new List<EquipTokenItemModel>(),
				["WeaponItemListKey"] = new List<EquipTokenItemModel>()
			};
			if (EquipTokenItems != null)
			{
				foreach (EquipTokenItemModel equipTokenItem in EquipTokenItems)
				{
					if (equipTokenItem.OwnedTokensAmount > 0 && equipTokenItem.EquipmentDefinition.SurvivorClass == survivorClass)
					{
						if (equipTokenItem.EquipmentDefinition.Type == EquipmentType.Armor)
						{
							dictionary["ArmorItemListKey"].Add(equipTokenItem);
						}
						else
						{
							dictionary["WeaponItemListKey"].Add(equipTokenItem);
						}
					}
				}
			}
			dictionary["ArmorItemListKey"].Sort((EquipTokenItemModel x, EquipTokenItemModel y) => -x.CompareTo(y));
			dictionary["WeaponItemListKey"].Sort((EquipTokenItemModel x, EquipTokenItemModel y) => -x.CompareTo(y));
			return dictionary;
		}

		public EquipTokenItemModel AddEquipToken(string equipTokenId, int amount, bool isMigrate = false)
		{
			if (EquipTokenItems == null)
			{
				EquipTokenItems = new ModelList<EquipTokenItemModel>();
			}
			if (base.manager.GameEconomyData.GetEquipTokenDefinition(equipTokenId) == null)
			{
				return null;
			}
			EquipTokenItemModel equipTokenItemModel = EquipTokenItems.Find((EquipTokenItemModel x) => x.EquipTokenId == equipTokenId);
			EquipTokenItemModel result;
			if (equipTokenItemModel == null)
			{
				equipTokenItemModel = new EquipTokenItemModel(equipTokenId, amount);
				equipTokenItemModel.SetManager(base.manager);
				equipTokenItemModel.Initialize();
				if (!isMigrate)
				{
					equipTokenItemModel.Start();
				}
				EquipTokenItems.Add(equipTokenItemModel);
				result = equipTokenItemModel;
			}
			else
			{
				EquipTokenItemModel equipTokenItemModel2 = new EquipTokenItemModel(equipTokenId, amount);
				equipTokenItemModel2.SetManager(base.manager);
				equipTokenItemModel2.Initialize();
				equipTokenItemModel.AddEquipToken(equipTokenItemModel2);
				result = equipTokenItemModel2;
			}
			NotifyChange("EquipmentTokenTypeUpdateEvent", this);
			return result;
		}
	}
}
