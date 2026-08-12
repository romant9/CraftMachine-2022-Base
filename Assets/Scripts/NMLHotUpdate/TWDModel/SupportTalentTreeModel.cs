using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class SupportTalentTreeModel : TWDModelObject
	{
		public int TreeId { get; private set; }

		public CurrencyType UpgradeTalentSupportTokenType { get; private set; }

		public ModelList<SupportTalentNodeTrunkModel> TrunkNodes { get; private set; }

		[JsonIgnore]
		public SupportTalentTreeMainDefinition Definition => base.manager.GameEconomyData.GetSupportTalentTreeMainDefinitionById(TreeId);

		public SupportTalentTreeModel(int treeId, CurrencyType upgradeTalentSupportTokenType)
		{
			TreeId = treeId;
			UpgradeTalentSupportTokenType = upgradeTalentSupportTokenType;
		}

		public override void Initialize()
		{
			base.Initialize();
			TrunkNodes = new ModelList<SupportTalentNodeTrunkModel>();
			TrunkNodes.SetManager(base.manager);
			TrunkNodes.Initialize();
			foreach (SupportTalentTreeTrunkDefinition item in base.manager.GameEconomyData.GetSupportTalentTreeTrunkDefinitionsByTreeId(TreeId))
			{
				SupportTalentNodeTrunkModel supportTalentNodeTrunkModel = new SupportTalentNodeTrunkModel(base.manager.GameEconomyData.GetSupportTalentDefinitionByTalentIdAndLevel(item.TrunkId, 0).Id, TreeId, item.TrunkId);
				supportTalentNodeTrunkModel.SetManager(base.manager);
				supportTalentNodeTrunkModel.Initialize();
				TrunkNodes.Add(supportTalentNodeTrunkModel);
			}
		}

		public SupportTalentNodeAbstract GetNodeModelByNodeId(int nodeId, bool limitTrunkNode = false)
		{
			SupportTalentNodeAbstract result = null;
			foreach (SupportTalentNodeTrunkModel trunkNode in TrunkNodes)
			{
				if (trunkNode.TrunkId == nodeId)
				{
					result = trunkNode;
					break;
				}
				if (!limitTrunkNode && trunkNode.SupportTalentNodeBranchModel != null && trunkNode.SupportTalentNodeBranchModel.BranchId == nodeId)
				{
					result = trunkNode.SupportTalentNodeBranchModel;
					break;
				}
			}
			return result;
		}

		public bool CanUpgradeNodeByNodeId(int nodeId)
		{
			SupportTalentNodeAbstract nodeModelByNodeId = GetNodeModelByNodeId(nodeId);
			if (nodeModelByNodeId == null)
			{
				return false;
			}
			int maxLevel = nodeModelByNodeId.GetMaxLevel();
			if (nodeModelByNodeId.Level >= maxLevel)
			{
				return false;
			}
			int requireTrunkId = nodeModelByNodeId.GetRequireTrunkId();
			int requireTrunkMinLevel = nodeModelByNodeId.GetRequireTrunkMinLevel();
			if (requireTrunkId == 0 || requireTrunkMinLevel == 0)
			{
				return true;
			}
			SupportTalentNodeAbstract nodeModelByNodeId2 = GetNodeModelByNodeId(requireTrunkId, limitTrunkNode: true);
			if (nodeModelByNodeId2 == null)
			{
				return false;
			}
			if (nodeModelByNodeId2.Level < requireTrunkMinLevel)
			{
				return false;
			}
			return true;
		}

		public Cashier GetUpgradeNodeCashierByNodeId(int nodeId)
		{
			Cashier cashier = new Cashier(base.manager);
			SupportTalentNodeAbstract nodeModelByNodeId = GetNodeModelByNodeId(nodeId);
			if (nodeModelByNodeId == null)
			{
				return null;
			}
			SupportTalentDefinition supportTalentDefinitionByTalentIdAndLevel = base.manager.GameEconomyData.GetSupportTalentDefinitionByTalentIdAndLevel(nodeModelByNodeId.GetCurrentTalentNodeId(), nodeModelByNodeId.Level);
			if (supportTalentDefinitionByTalentIdAndLevel == null)
			{
				return null;
			}
			if (supportTalentDefinitionByTalentIdAndLevel.SupportTokenAmount < 0)
			{
				return null;
			}
			CashierItem cashierItem = new CashierItem(PurchaseType.UpgradeSupportTalent);
			cashierItem.SetCost(UpgradeTalentSupportTokenType, supportTalentDefinitionByTalentIdAndLevel.SupportTokenAmount);
			if (supportTalentDefinitionByTalentIdAndLevel.PrimarySupportTalentTokenAmount < 0)
			{
				return null;
			}
			if (supportTalentDefinitionByTalentIdAndLevel.AdvancedSupportTalentTokenAmount < 0)
			{
				return null;
			}
			if (supportTalentDefinitionByTalentIdAndLevel.PrimarySupportTalentTokenAmount > 0)
			{
				cashierItem.SetCost(CurrencyType.PrimarySupportTalentToken, supportTalentDefinitionByTalentIdAndLevel.PrimarySupportTalentTokenAmount);
			}
			if (supportTalentDefinitionByTalentIdAndLevel.AdvancedSupportTalentTokenAmount > 0)
			{
				cashierItem.SetCost(CurrencyType.AdvancedSupportTalentToken, supportTalentDefinitionByTalentIdAndLevel.AdvancedSupportTalentTokenAmount);
			}
			cashier.AddItem(cashierItem);
			return cashier;
		}

		public TWDModelResult UpgradeNodeByNodeId(int nodeId)
		{
			SupportTalentNodeAbstract nodeModelByNodeId = GetNodeModelByNodeId(nodeId);
			if (nodeModelByNodeId == null)
			{
				return TWDModelResult.SupportTalentNodeNotFound;
			}
			int level = nodeModelByNodeId.Level;
			Cashier upgradeNodeCashierByNodeId = GetUpgradeNodeCashierByNodeId(nodeId);
			SupportTalentDefinition nextLevelTalentNodeDefinition = nodeModelByNodeId.GetNextLevelTalentNodeDefinition();
			if (nextLevelTalentNodeDefinition == null)
			{
				return TWDModelResult.Error;
			}
			TWDModelResult num = upgradeNodeCashierByNodeId.Pay(nodeModelByNodeId);
			if (num == TWDModelResult.OK)
			{
				nodeModelByNodeId.Level = nextLevelTalentNodeDefinition.Level;
				nodeModelByNodeId.TalentId = nextLevelTalentNodeDefinition.Id;
			}
			base.manager.Metrics.AddFind().AddSupportTalent(TreeId, nodeId, level).AddSupportTalentNextLevel(nextLevelTalentNodeDefinition.Level)
				.Send();
			return num;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
