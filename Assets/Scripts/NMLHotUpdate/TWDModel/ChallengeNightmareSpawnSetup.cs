using System;
using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	[Serializable]
	public class ChallengeNightmareSpawnSetup
	{
		public int Round;

		public string BaseSpawn;

		public string ThreatSpawn;

		public int ThreatCounterAtStart;

		public int InitialThreatTurns;

		public List<WalkerType> GetChallengeNightmareSpawnCompositions(bool isThreatSpawn)
		{
			List<WalkerType> list = new List<WalkerType>();
			List<string> list2 = new List<string>();
			list2 = ((!isThreatSpawn) ? BaseSpawn.Split(',').ToList() : ThreatSpawn.Split(',').ToList());
			foreach (string item in list2)
			{
				list.Add(GameEconomyData.GetTypeEnum<WalkerType>(item));
			}
			return list;
		}
	}
}
