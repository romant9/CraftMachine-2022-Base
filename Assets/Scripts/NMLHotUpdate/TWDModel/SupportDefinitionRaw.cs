using System;

namespace TWDModel
{
	[Serializable]
	public class SupportDefinitionRaw
	{
		public string Identifier;

		public int Index;

		public int Level;

		public int TokensToUnlock;

		public int Cooldown;

		public string Parameters;

		public string SupportTalentTree;

		public int SupportTalentSlot;

		public int Category;

		public int ChallengeCooldown;

		public int DistanceCooldown;

		public int GVGCooldown;

		public string UpgradeCost;

		public int InnerCooldown;
	}
}
