namespace TWDModel
{
	public abstract class SupportTalentNodeAbstract : TWDModelObject
	{
		public int TalentId { get; set; }

		public int TreeId { get; set; }

		public int Level { get; set; }

		public SupportTalentNodeAbstract(int talentId, int treeId, int level)
		{
			TalentId = talentId;
			TreeId = treeId;
			Level = level;
		}

		public abstract int GetCurrentTalentNodeId();

		public SupportTalentDefinition GetCurrentTalentNodeDefinition()
		{
			return base.manager.GameEconomyData.GetSupportTalentDefinitionById(TalentId);
		}

		public abstract SupportTalentDefinition GetNextLevelTalentNodeDefinition();

		public abstract int GetRequireTrunkId();

		public abstract int GetRequireTrunkMinLevel();

		public abstract int GetMaxLevel();

		public abstract bool IsTrunkNode();

		public abstract string GetTalentName();

		public abstract string GetTalentIcon();

		public override bool IsValid()
		{
			return true;
		}
	}
}
