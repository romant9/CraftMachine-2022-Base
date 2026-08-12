using Newtonsoft.Json;

namespace TWDModel
{
	public class SupportTalentNodeBranchModel : SupportTalentNodeAbstract
	{
		public int BranchId { get; private set; }

		[JsonIgnore]
		public SupportTalentTreeBranchDefinition Definition => base.manager.GameEconomyData.GetSupportTalentTreeBranchDefinitionByBranchId(BranchId);

		public SupportTalentNodeBranchModel(int talentId, int treeId, int branchId)
			: base(talentId, treeId, 0)
		{
			BranchId = branchId;
		}

		public SupportTalentTreeBranchDirection GetDirection()
		{
			return base.manager.GameEconomyData.GetSupportTalentTreeBranchDefinitionByBranchId(BranchId)?.Direction ?? SupportTalentTreeBranchDirection.None;
		}

		public override int GetCurrentTalentNodeId()
		{
			return BranchId;
		}

		public override SupportTalentDefinition GetNextLevelTalentNodeDefinition()
		{
			return base.manager.GameEconomyData.GetSupportTalentDefinitionByTalentIdAndLevel(BranchId, base.Level + 1);
		}

		public override int GetRequireTrunkId()
		{
			return base.manager.GameEconomyData.GetSupportTalentTreeBranchDefinitionByBranchId(BranchId)?.TrunkId ?? 0;
		}

		public override int GetRequireTrunkMinLevel()
		{
			return base.manager.GameEconomyData.GetSupportTalentTreeBranchDefinitionByBranchId(BranchId)?.RequireTrunkMinLevel ?? 0;
		}

		public override int GetMaxLevel()
		{
			return base.manager.GameEconomyData.GetSupportTalentTreeBranchDefinitionByBranchId(BranchId)?.MaxLevel ?? 0;
		}

		public override bool IsTrunkNode()
		{
			return false;
		}

		public override string GetTalentName()
		{
			return Definition.TalentName;
		}

		public override string GetTalentIcon()
		{
			return Definition.Icon;
		}
	}
}
