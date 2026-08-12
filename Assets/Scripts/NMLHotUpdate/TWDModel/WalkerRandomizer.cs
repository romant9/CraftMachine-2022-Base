using BaseModel;

namespace TWDModel
{
	public class WalkerRandomizer
	{
		private TWDModelManager manager;

		private int missionLevel;

		private MapCategory category;

		private bool isHardScavenge;

		private int swapsDone;

		private int maxSwaps;

		private WalkerRandomizerSwap swapData;

		private WalkerRandomizerWeight weightData;

		public bool IsDisabled;

		public bool IsEnabled()
		{
			if (swapData != null && weightData != null && swapsDone < maxSwaps)
			{
				return !IsDisabled;
			}
			return false;
		}

		public WalkerRandomizer(TWDModelManager manager, int missionLevel, MapCategory category, bool isHardScavenge)
		{
			this.manager = manager;
			this.missionLevel = missionLevel;
			this.category = category;
			this.isHardScavenge = isHardScavenge;
			swapData = manager.GameEconomyData.GetWalkerRandomizerSwap(category, missionLevel);
			weightData = manager.GameEconomyData.GetWalkerRandomizerWeight(category, missionLevel);
			if (swapData != null)
			{
				maxSwaps = swapData.MaxSwaps + (isHardScavenge ? swapData.HardScavengeExtra : 0);
			}
		}

		public WalkerType RandomizeWalker(GridCoordinate coordinate, WalkerType walkerType)
		{
			if (MinDistanceToSurvivors(coordinate) >= swapData.MinSpawnDistance && (manager.Player.PlayerRandom.Next() < swapData.SwapChance || swapsDone < swapData.MinSwaps))
			{
				walkerType = weightData.WalkerTypes[manager.Player.PlayerRandom.WeightedRandom(weightData.WalkerWeights)];
				swapsDone++;
			}
			return walkerType;
		}

		private FixedPoint MinDistanceToSurvivors(GridCoordinate from)
		{
			FixedPoint fixedPoint = FixedPoint.MaxValue;
			ModelList<ActorModel> survivors = manager.Player.Combat.Survivors;
			for (int i = 0; i < survivors.Count; i++)
			{
				FixedPoint fixedPoint2 = from.DistanceTo(survivors[i].GridCoordinate);
				if (fixedPoint2 < fixedPoint)
				{
					fixedPoint = fixedPoint2;
				}
			}
			return fixedPoint;
		}
	}
}
