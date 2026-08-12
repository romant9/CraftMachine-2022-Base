using System;

namespace TWDModel
{
	[Serializable]
	public class MapData
	{
		public MapSpawnPointModel MapSpawnPoints;

		public string Name;

		public int MoveCampMissionCarLevel;

		public int SizeX;

		public int SizeY;

		public int InitResourceRun;

		public int InitEquipmentRun;

		public int InitRescueSurvivor;

		public int InitSecureLocationGas;

		public int InitSecureLocationSupplies;

		public int InitExplorableResourceRun;

		public int InitExplorableEquipmentRun;

		public int InitExplorableRescueSurvivor;

		public int InitExplorableSecureLocationGas;

		public int InitExplorableSecureLocationSupplies;

		public MapData()
		{
			MapSpawnPoints = new MapSpawnPointModel();
		}
	}
}
