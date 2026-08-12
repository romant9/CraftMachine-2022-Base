using System;

namespace TWDModel
{
	[Serializable]
	public class DeepLinkDefinitionsRaw
	{
		public string Identifier;

		public string Deeplink;

		public string Rewards;

		public string DeepLinkAction;

		public int MinCouncil;

		public int MaxCouncil;

		public string StartTimestamp;

		public string EndTimestamp;

		public string SpenderTier;
	}
}
