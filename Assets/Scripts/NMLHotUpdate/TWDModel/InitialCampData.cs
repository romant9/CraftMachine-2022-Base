using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class InitialCampData
	{
		public List<InitialCampBuildingData> Buildings;

		public MissionCarData MissionCar;

		public int GridWidth;

		public int GridHeight;

		public int CampLevel;

		public int CampSubtypeId;
	}
}
