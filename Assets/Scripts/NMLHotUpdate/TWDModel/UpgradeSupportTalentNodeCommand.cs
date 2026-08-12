using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class UpgradeSupportTalentNodeCommand : ModelCommand
	{
		public int SupportTalentTreeModelId { get; private set; }

		public int NodeId { get; private set; }

		public UpgradeSupportTalentNodeCommand()
		{
		}

		public UpgradeSupportTalentNodeCommand(int modelId, int supportTalentTreeModelId, int nodeId)
			: base(modelId)
		{
			SupportTalentTreeModelId = supportTalentTreeModelId;
			NodeId = nodeId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			SupportModel model = manager.GetModel<SupportModel>(base.ModelId);
			SupportTalentTreeModel model2 = manager.GetModel<SupportTalentTreeModel>(SupportTalentTreeModelId);
			SupportTalentNodeAbstract nodeModelByNodeId = model2.GetNodeModelByNodeId(NodeId);
			if (nodeModelByNodeId == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.SupportTalentNodeNotFound);
			}
			if (!model2.CanUpgradeNodeByNodeId(NodeId))
			{
				return new NGModelCommandRespond(this, TWDModelResult.SupportTalentNodeCanNotUpgrade);
			}
			int num = -1;
			if (model.SlotAssembledTalentIds != null && model.SlotAssembledTalentIds.Count > 0)
			{
				foreach (KeyValuePair<int, int> slotAssembledTalentId in model.SlotAssembledTalentIds)
				{
					if (slotAssembledTalentId.Value == nodeModelByNodeId.TalentId)
					{
						num = slotAssembledTalentId.Key;
						break;
					}
				}
			}
			TWDModelResult tWDModelResult = model2.UpgradeNodeByNodeId(NodeId);
			if (tWDModelResult != TWDModelResult.OK)
			{
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			SupportTalentNodeAbstract nodeModelByNodeId2 = model2.GetNodeModelByNodeId(NodeId);
			if (model.SlotAssembledTalentIds != null && num >= 0)
			{
				model.SlotAssembledTalentIds[num] = nodeModelByNodeId2.TalentId;
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
