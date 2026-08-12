using System;

namespace TWDModel
{
	[Serializable]
	public enum IncrementalDifficultyEffect
	{
		None = 0,
		ReduceThreatCounter = 1,
		AddThreatWalker = 2,
		PromoteThreatWalker = 3,
		UpgradeRaider = 4,
		PromoteMeleeRaider = 5,
		PromoteRangedRaider = 6,
		UpgradeWalker = 7,
		PromoteWalker = 8,
		UpgradePromotedWalker = 9,
		PromoteWalkerArmored = 10,
		PromoteWalkerTank = 11,
		PromoteWalkerSpiked = 12,
		PromoteThreatWalkerArmored = 13,
		PromoteThreatWalkerTank = 14,
		PromoteThreatWalkerSpiked = 15,
		PromoteWalkerCommonWealth = 16,
		PromoteThreatWalkerCommonWealth = 17,
		WalkerMoveRange = 18
	}
}
