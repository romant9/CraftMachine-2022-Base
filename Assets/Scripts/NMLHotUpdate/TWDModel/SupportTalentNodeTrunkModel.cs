using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class SupportTalentNodeTrunkModel : SupportTalentNodeAbstract
	{
		public int TrunkId { get; private set; }

		public SupportTalentNodeBranchModel SupportTalentNodeBranchModel { get; private set; }

		[JsonIgnore]
		public SupportTalentTreeTrunkDefinition Definition => base.manager.GameEconomyData.GetSupportTalentTreeTrunkDefinitionByTrunkId(TrunkId);

		public SupportTalentNodeTrunkModel(int talentId, int treeId, int trunkId)
			: base(talentId, treeId, 0)
		{
			TrunkId = trunkId;
		}

		public override void Initialize()
		{
			base.Initialize();
			List<SupportTalentTreeBranchDefinition> supportTalentTreeBranchDefinitionsByTreeId = base.manager.GameEconomyData.GetSupportTalentTreeBranchDefinitionsByTreeId(base.TreeId);
			if (supportTalentTreeBranchDefinitionsByTreeId == null || supportTalentTreeBranchDefinitionsByTreeId.Count == 0)
			{
				return;
			}
			foreach (SupportTalentTreeBranchDefinition item in supportTalentTreeBranchDefinitionsByTreeId)
			{
				if (item.TrunkId == TrunkId)
				{
					SupportTalentDefinition supportTalentDefinitionByTalentIdAndLevel = base.manager.GameEconomyData.GetSupportTalentDefinitionByTalentIdAndLevel(item.BranchId, 0);
					SupportTalentNodeBranchModel = new SupportTalentNodeBranchModel(supportTalentDefinitionByTalentIdAndLevel.Id, base.TreeId, item.BranchId);
					SupportTalentNodeBranchModel.SetManager(base.manager);
					SupportTalentNodeBranchModel.Initialize();
					break;
				}
			}
		}

		public override int GetCurrentTalentNodeId()
		{
			return TrunkId;
		}

		public override SupportTalentDefinition GetNextLevelTalentNodeDefinition()
		{
			return base.manager.GameEconomyData.GetSupportTalentDefinitionByTalentIdAndLevel(TrunkId, base.Level + 1);
		}

		public override int GetRequireTrunkId()
		{
			return base.manager.GameEconomyData.GetSupportTalentTreeTrunkDefinitionByTrunkId(TrunkId)?.RequireTrunkId ?? 0;
		}

		public override int GetRequireTrunkMinLevel()
		{
			return base.manager.GameEconomyData.GetSupportTalentTreeTrunkDefinitionByTrunkId(TrunkId)?.RequireTrunkMinLevel ?? 0;
		}

		public override int GetMaxLevel()
		{
			return base.manager.GameEconomyData.GetSupportTalentTreeTrunkDefinitionByTrunkId(TrunkId)?.MaxLevel ?? 0;
		}

		public override bool IsTrunkNode()
		{
			return true;
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
