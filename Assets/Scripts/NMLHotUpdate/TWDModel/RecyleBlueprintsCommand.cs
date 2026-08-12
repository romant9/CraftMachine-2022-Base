using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class RecyleBlueprintsCommand : ConsumeCurrencyCommand
	{
		public List<string> EquipmentToeknIdsList { get; set; }

		public int RecyleWeaponModelId { get; set; }

		public RecyleBlueprintsCommand()
		{
		}

		public RecyleBlueprintsCommand(List<string> equipmentToeknIdsList, int recyleWeaponModelId)
		{
			EquipmentToeknIdsList = equipmentToeknIdsList;
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
			bool flag = model.RecycleBlueprints(EquipmentToeknIdsList);
			return new NGModelCommandRespond(this, (!flag) ? TWDModelResult.Error : TWDModelResult.OK);
		}
	}
}
