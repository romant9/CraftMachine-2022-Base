using System;

namespace TWDModel
{
	[Serializable]
	public class SupportTalentTreeBranchDefinition
	{
		public int BranchId;

		public int TreeId;

		public int TrunkId;

		public int RequireTrunkMinLevel;

		public string TalentName;

		public string Icon;

		public SupportTalentTreeBranchDirection Direction;

		public int MaxLevel;
	}
}
