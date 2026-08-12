using System.Collections.Generic;

namespace TWDModel
{
	public class MapSpawnPointModel : TWDModelObject
	{
		public List<SpawnPointLocation> SpawnPointLocations { get; protected set; }

		public List<SpawnPointGroup> SpawnPointGroups { get; protected set; }

		public FixedVec2 MoveCampLocation { get; set; }

		public MapSpawnPointModel()
		{
			SpawnPointLocations = new List<SpawnPointLocation>();
			SpawnPointGroups = new List<SpawnPointGroup>();
			MoveCampLocation = default(FixedVec2);
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
