using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class DebugOneButtonUpgradeSupportTalentCommand : ModelCommand
	{
		public string SupportId { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			SupportModel supportModel = (manager as TWDModelManager).Player.GetSupportModel(SupportId);
			if (supportModel == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (!supportModel.InitializedTalent)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			foreach (SupportTalentTreeModel supportTalentTreeModel in supportModel.SupportTalentTreeModels)
			{
				if (supportModel.Level >= supportTalentTreeModel.Definition.UnlockLevel)
				{
					UpgradeTreeNodeToMaxLevel(supportModel, supportTalentTreeModel);
				}
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}

		private void UpgradeTreeNodeToMaxLevel(SupportModel supportModel, SupportTalentTreeModel supportTalentTreeModel)
		{
			foreach (SupportTalentNodeTrunkModel trunkNode in supportTalentTreeModel.TrunkNodes)
			{
				int maxLevel = trunkNode.GetMaxLevel();
				if (trunkNode.Level >= maxLevel)
				{
					continue;
				}
				while (trunkNode.Level < maxLevel)
				{
					SupportTalentDefinition nextLevelTalentNodeDefinition = trunkNode.GetNextLevelTalentNodeDefinition();
					if (nextLevelTalentNodeDefinition != null)
					{
						trunkNode.Level = nextLevelTalentNodeDefinition.Level;
						trunkNode.TalentId = nextLevelTalentNodeDefinition.Id;
					}
				}
				if (trunkNode.SupportTalentNodeBranchModel == null)
				{
					continue;
				}
				int maxLevel2 = trunkNode.SupportTalentNodeBranchModel.GetMaxLevel();
				if (trunkNode.SupportTalentNodeBranchModel.Level >= maxLevel2)
				{
					continue;
				}
				int num = -1;
				if (supportModel.SlotAssembledTalentIds != null && supportModel.SlotAssembledTalentIds.Count > 0)
				{
					foreach (KeyValuePair<int, int> slotAssembledTalentId in supportModel.SlotAssembledTalentIds)
					{
						if (slotAssembledTalentId.Value == trunkNode.SupportTalentNodeBranchModel.TalentId)
						{
							num = slotAssembledTalentId.Key;
							break;
						}
					}
				}
				while (trunkNode.SupportTalentNodeBranchModel.Level < maxLevel2)
				{
					SupportTalentDefinition nextLevelTalentNodeDefinition2 = trunkNode.SupportTalentNodeBranchModel.GetNextLevelTalentNodeDefinition();
					if (nextLevelTalentNodeDefinition2 != null)
					{
						trunkNode.SupportTalentNodeBranchModel.Level = nextLevelTalentNodeDefinition2.Level;
						trunkNode.SupportTalentNodeBranchModel.TalentId = nextLevelTalentNodeDefinition2.Id;
						if (supportModel.SlotAssembledTalentIds != null && num >= 0)
						{
							supportModel.SlotAssembledTalentIds[num] = trunkNode.SupportTalentNodeBranchModel.TalentId;
						}
					}
				}
			}
		}
	}
}
