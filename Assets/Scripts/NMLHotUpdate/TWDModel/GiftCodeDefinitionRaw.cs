using System;

namespace TWDModel
{
	[Serializable]
	public class GiftCodeDefinitionRaw
	{
		public string Identifier;

		public string Code;

		public string Rewards;

		public int MinCouncil;

		public int MaxCouncil;

		public string StartTimestamp;

		public string EndTimestamp;
	}
}
