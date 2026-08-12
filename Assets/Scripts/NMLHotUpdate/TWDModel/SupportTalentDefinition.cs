using System;

namespace TWDModel
{
	[Serializable]
	public class SupportTalentDefinition
	{
		public int Id;

		public int SupportTalentId;

		public int Level;

		public SupportTalentType Type;

		public AttributeType TalentAttributeType;

		public int TalentAttributeValue;

		public string TalentTrait;

		public string TalentTraitDesc;

		public int PrimarySupportTalentTokenAmount;

		public int AdvancedSupportTalentTokenAmount;

		public int SupportTokenAmount;
	}
}
