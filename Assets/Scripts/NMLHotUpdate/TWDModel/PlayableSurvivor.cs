using System;

namespace TWDModel
{
	[Serializable]
	public class PlayableSurvivor
	{
		public string ActorID;

		public int MinLevel;

		public int MaxLevel;

		public int Rarity;

		public string WeaponID;

		public string ArmorID;

		public int EqLevel;

		public int EqRarity;

		public int RosterIndex;
	}
}
