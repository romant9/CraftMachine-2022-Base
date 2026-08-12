using System;

namespace TWDModel
{
	[Serializable]
	public class SupportTalentTreeTrunkDefinition
	{
		public int TrunkId;

		public int TreeId;

		public int RequireTrunkId;

		public int RequireTrunkMinLevel;

		public string Icon;

		public string TalentName;

		public int MaxLevel;
	}
}
