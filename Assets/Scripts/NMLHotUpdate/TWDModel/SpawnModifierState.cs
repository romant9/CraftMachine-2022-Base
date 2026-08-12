using System.Collections.Generic;

namespace TWDModel
{
	public class SpawnModifierState
	{
		public int PromoteMeleeRaiderCount;

		public int PromoteRangedRaiderCount;

		public int PromoteWalkerCount;

		public int UpgradePromotedWalkerCount;

		public int PromoteThreatWalkerCount;

		public int UpgradeWalkerCount;

		public int UpgradeRaiderCount;

		public int WalkerMoveRange;

		public List<WalkerType> PromoteWalkerType = new List<WalkerType>();

		public List<WalkerType> PromoteThreatWalkerType = new List<WalkerType>();
	}
}
