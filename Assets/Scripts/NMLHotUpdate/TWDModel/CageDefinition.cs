using System;

namespace TWDModel
{
	[Serializable]
	public class CageDefinition
	{
		public string WalkerId;

		public int Level;

		public int CostLevelUpOutpost;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public int UpgradeTime;

		public int DependencyLevelRequired;

		public string EpisodeLock;

		public int UnlockPriceDiamonds;

		public int CostAmountOuptost;

		public int AmountDependencyLevelRequired;

		public bool Enabled;

		public bool Placeable;
	}
}
