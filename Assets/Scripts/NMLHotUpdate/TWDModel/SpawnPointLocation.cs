using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class SpawnPointLocation
	{
		public int MissionLevel;

		public string MissionId;

		public bool IsDeadly;

		public bool InitialSpawn;

		public List<SpawnPointLocation> SpawnPointsToUnlock;

		public int Id { get; set; }

		public float LocationX { get; set; }

		public float LocationY { get; set; }

		public SpawnPointGroup SpawnPointGroup { get; set; }

		public DropEventDefinition.DropEventTag LootTag { get; set; }

		[JsonIgnore]
		public bool IsExplicit
		{
			get
			{
				if (MissionId != null)
				{
					return MissionId.Length > 0;
				}
				return false;
			}
		}
	}
}
