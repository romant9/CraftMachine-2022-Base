using System;

namespace TWDModel
{
	[Serializable]
	public class ReturnEndlessDealDefinition
	{
		public int Id;

		public int Class;

		public string Reward;

		public ReturnEndlessDealPackType Type;

		public int Quality;

		public string BundleIdentifier;

		public int CouncilLevelMin;

		public int CouncilLevelMax;
	}
}
