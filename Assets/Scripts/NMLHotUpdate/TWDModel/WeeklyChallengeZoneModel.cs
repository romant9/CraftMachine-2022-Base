using System.Collections.Generic;

namespace TWDModel
{
	public class WeeklyChallengeZoneModel
	{
		public bool FeatureEnabled { get; set; }

		public Dictionary<int, int> Id2ZoneIdDict { get; set; }

		public int GetZoneIdById(int id)
		{
			if (Id2ZoneIdDict.ContainsKey(id))
			{
				return Id2ZoneIdDict[id];
			}
			return 0;
		}
	}
}
