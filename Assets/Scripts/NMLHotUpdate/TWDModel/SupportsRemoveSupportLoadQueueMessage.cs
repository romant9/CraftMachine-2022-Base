using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class SupportsRemoveSupportLoadQueueMessage : SupportLoadQueueMessage
	{
		public List<SupportRemoveSupportItemEntry> SupportRemoveSupportEntries { get; set; }

		public SupportsRemoveSupportLoadQueueMessage()
		{
		}

		public SupportsRemoveSupportLoadQueueMessage(List<SupportRemoveSupportItemEntry> supportRemoveSupportItemEntries)
		{
			SupportRemoveSupportEntries = supportRemoveSupportItemEntries;
		}

		public override bool Execute(TWDModelManager manager)
		{
			manager.Metrics.AddResetCombat(manager.Player.Combat != null).AddSupport(base.SupportGivenTimestamp, base.SupportEntityGUID).Send();
			if (manager.Player.Combat != null)
			{
				manager.Player.DeleteCombatModel(notify: false);
			}
			foreach (SupportRemoveSupportItemEntry support in SupportRemoveSupportEntries)
			{
				if (manager.Player != null && !string.IsNullOrEmpty(support.Identifier) && support.RemoveItem)
				{
					SupportModel supportModel = null;
					supportModel = manager.Player.SupportModels.First((SupportModel x) => x.SupportId == support.Identifier);
					manager.Metrics.AddRemove().AddResources(new Dictionary<CurrencyType, OverflowableAmount> { 
					{
						manager.Player.GetCurrency(supportModel.Currency).Type,
						new OverflowableAmount
						{
							Amount = -manager.Player.GetCurrency(supportModel.Currency).Value
						}
					} }).AddSupportUnit(supportModel)
						.AddSupport(base.SupportGivenTimestamp, base.SupportEntityGUID)
						.Send();
					manager.Player.GetCurrency(supportModel.Currency).SetValue(0);
					supportModel.Level = 0;
				}
			}
			return true;
		}
	}
}
