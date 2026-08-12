using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class SupportsRemoveEquipmentLoadQueueMessage : SupportLoadQueueMessage
	{
		public List<SupportRemoveSupportItemEntry> SupportRemoveEquipmentEntries { get; set; }

		public override bool Execute(TWDModelManager manager)
		{
			manager.Metrics.AddResetCombat(manager.Player.Combat != null).AddSupport(base.SupportGivenTimestamp, base.SupportEntityGUID).Send();
			if (manager.Player.Combat != null)
			{
				manager.Player.DeleteCombatModel(notify: false);
			}
			PlayerModel player = manager.Player;
			List<EquipmentItemModel> list = player.Equipment.Armors.Union(player.Equipment.RangeWeapons).Union(player.Equipment.MeleeWeapons).ToList();
			foreach (SupportRemoveSupportItemEntry equipment in SupportRemoveEquipmentEntries)
			{
				if (manager.Player != null && !string.IsNullOrEmpty(equipment.Identifier) && equipment.RemoveItem)
				{
					EquipmentItemModel equipmentItemModel = null;
					equipmentItemModel = list.Find((EquipmentItemModel x) => x.GenerateName == equipment.Identifier || x.Definition.ID == equipment.Identifier);
					if (equipmentItemModel != null)
					{
						manager.Metrics.AddRemove().AddEquipment(equipmentItemModel).AddSupport(base.SupportGivenTimestamp, base.SupportEntityGUID)
							.Send();
						player.Equipment.ScrapEquipmentItem(equipmentItemModel, deletedBySupport: true);
					}
				}
			}
			return true;
		}
	}
}
