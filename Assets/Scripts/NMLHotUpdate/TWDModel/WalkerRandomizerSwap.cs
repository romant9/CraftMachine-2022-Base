using System;

namespace TWDModel
{
	[Serializable]
	public class WalkerRandomizerSwap
	{
		public string MissionType;

		public int MinLevel;

		public int MaxLevel;

		public FixedPoint SwapChance;

		public int MinSwaps;

		public int MaxSwaps;

		public int HardScavengeExtra;

		public int MinSpawnDistance;
	}
}
