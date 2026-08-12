using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class WalkerRandomizerWeight
	{
		public string MissionType;

		public int MinLevel;

		public int MaxLevel;

		public int WalkerArmored;

		public int WalkerTank;

		public int WalkerSpiked;

		public int WalkerFast;

		public int WalkerGoo;

		public int WalkerExplosive;

		public int WalkerSlim;

		public int WalkerMetalhead;

		public int WalkerWhisperer;

		public int WalkerWhispererMelee;

		public int ExplosiveBarrel;

		public int WalkerCommonWealth;

		[JsonIgnore]
		public WalkerType[] WalkerTypes;

		[JsonIgnore]
		public FixedPoint[] WalkerWeights;

		[JsonIgnore]
		public int TotalWeight;

		public void Start()
		{
			WalkerTypes = new WalkerType[12]
			{
				WalkerType.WalkerArmored,
				WalkerType.WalkerTank,
				WalkerType.WalkerSpiked,
				WalkerType.WalkerFast,
				WalkerType.WalkerGoo,
				WalkerType.WalkerExplosive,
				WalkerType.WalkerSlim,
				WalkerType.WalkerMetalhead,
				WalkerType.WalkerWhisperer,
				WalkerType.WalkerWhispererMelee,
				WalkerType.ExplosiveBarrel,
				WalkerType.WalkerCommonWealth
			};
			WalkerWeights = new FixedPoint[12]
			{
				WalkerArmored, WalkerTank, WalkerSpiked, WalkerFast, WalkerGoo, WalkerExplosive, WalkerSlim, WalkerMetalhead, WalkerWhisperer, WalkerWhispererMelee,
				ExplosiveBarrel, WalkerCommonWealth
			};
		}
	}
}
