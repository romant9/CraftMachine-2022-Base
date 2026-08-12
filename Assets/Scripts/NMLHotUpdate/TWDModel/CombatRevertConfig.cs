using System;

namespace TWDModel
{
	[Serializable]
	public class CombatRevertConfig
	{
		public string missionID;

		public bool isActive;

		public int turnsLimit;

		public int timesLimit;
	}
}
