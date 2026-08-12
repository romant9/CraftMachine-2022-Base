using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class RecyleWeaponCommand : ConsumeCurrencyCommand
	{
		public List<int> ModelIdsList { get; set; }

		public int RecyleWeaponModelId { get; set; }

		public RecyleWeaponCommand()
		{
		}

		public RecyleWeaponCommand(List<int> modelIdsList, int recyleWeaponModelId)
		{
			ModelIdsList = modelIdsList;
			RecyleWeaponModelId = recyleWeaponModelId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager { Player: not null } tWDModelManager) || tWDModelManager.Player.gameEconomyData == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			RecycleWeaponActivityModel model = manager.GetModel<RecycleWeaponActivityModel>(RecyleWeaponModelId);
			if (model == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			List<EquipmentItemModel> list = new List<EquipmentItemModel>();
			foreach (int modelIds in ModelIdsList)
			{
				EquipmentItemModel model2 = manager.GetModel<EquipmentItemModel>(modelIds);
				if (model2 == null)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				list.Add(model2);
			}
			bool flag = model.RecycleWeapons(list);
			return new NGModelCommandRespond(this, (!flag) ? TWDModelResult.Error : TWDModelResult.OK);
		}
	}
}
