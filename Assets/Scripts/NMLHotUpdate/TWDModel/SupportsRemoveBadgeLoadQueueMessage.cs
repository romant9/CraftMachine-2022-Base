using System.Collections.Generic;
using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class SupportsRemoveBadgeLoadQueueMessage : SupportLoadQueueMessage
	{
		public List<SupportRemoveSupportItemEntry> SupportRemoveBadgeEntries { get; set; }

		public SupportsRemoveBadgeLoadQueueMessage()
		{
		}

		public SupportsRemoveBadgeLoadQueueMessage(List<SupportRemoveSupportItemEntry> supportRemoveBadgeEntries)
		{
			SupportRemoveBadgeEntries = supportRemoveBadgeEntries;
		}

		public override bool Execute(TWDModelManager manager)
		{
			manager.Metrics.AddResetCombat(manager.Player.Combat != null).AddSupport(base.SupportGivenTimestamp, base.SupportEntityGUID).Send();
			if (manager.Player.Combat != null)
			{
				manager.Player.DeleteCombatModel(notify: false);
			}
			foreach (SupportRemoveSupportItemEntry badge in SupportRemoveBadgeEntries)
			{
				if (manager.Player != null && !string.IsNullOrEmpty(badge.Identifier) && badge.RemoveItem)
				{
					List<BadgeModel> list = new List<BadgeModel>();
					ModelList<BadgeModel> badges = manager.Player.Equipment.Badges;
					IEnumerable<BadgeModel> collection = manager.Player.SurvivorContainer.Survivors.Models.SelectMany((SurvivorModel x) => x.BadgeContainer.Badges);
					list.AddRange(badges);
					list.AddRange(collection);
					BadgeModel badgeModel = list.First((BadgeModel x) => x.GenerateName() == badge.Identifier);
					SurvivorModel survivorModel = manager.Player.SurvivorContainer.Survivors.Models.Find((SurvivorModel x) => x.BadgeContainer.Badges.Models.Contains(badgeModel));
					if (survivorModel != null)
					{
						survivorModel.ReclaimBadge(badgeModel, pay: false, returnBadgeInventory: false);
					}
					else
					{
						manager.Player.Equipment.RemoveBadge(badgeModel);
					}
					manager.Metrics.AddRemove().AddBadge(badgeModel).AddSupport(base.SupportGivenTimestamp, base.SupportEntityGUID)
						.Send();
				}
			}
			return true;
		}
	}
}
