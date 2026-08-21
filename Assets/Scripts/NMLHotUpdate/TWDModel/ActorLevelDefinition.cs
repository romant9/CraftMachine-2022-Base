using System;

namespace TWDModel
{
	[Serializable]
	public class ActorLevelDefinition
	{
		public string ActorDefinitionID;

		public int Level;

		public int Health;

		public int Damage;

		public int SPGain;

		public int EventSPGain;

		public int ApocalypticChallengeHealth;

		public int ApocalypticChallengeDamage;

		public int EndlessNormalHealth;

		public int EndlessNormalDamage;

		public int EndlessExpertHealth;

		public int EndlessExpertDamage;

		public bool SegmentedHP;

		public int HPBarQuantity;

		public int GuildBossHP;

		public int GuildBossDamage;

		public int GuildBossDefense;
	}
}
