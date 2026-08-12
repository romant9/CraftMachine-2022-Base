using System;

namespace TWDModel
{
	[Serializable]
	public class WorldMapItem
	{
		public FixedVec2 Position;

		public string PrefabName;

		public int DetailMapId;

		public WorldMapItem(int detailmapId, FixedVec2 position, string prefabName)
		{
			DetailMapId = detailmapId;
			Position = position;
			PrefabName = prefabName;
		}
	}
}
