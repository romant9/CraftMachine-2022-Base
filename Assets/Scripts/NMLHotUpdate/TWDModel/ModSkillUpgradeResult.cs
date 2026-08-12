namespace TWDModel
{
	public class ModSkillUpgradeResult
	{
		public string OldId { get; set; }

		public SPTraitsRemoldDefinitions NextTraitDef { get; set; }

		public ModSkillUpgradeResult(string oldId, SPTraitsRemoldDefinitions nextTraitDef)
		{
			OldId = oldId;
			NextTraitDef = nextTraitDef;
		}
	}
}
