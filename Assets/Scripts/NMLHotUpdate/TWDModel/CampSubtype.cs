using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class CampSubtype
	{
		public int Level;

		public string Name;

		public string Background;

		public GridSize Size;

		public FixedVec2 GatePosition;

		public RectData[] ValidBuildingPositions;

		public List<InitialCampBuildingData> Buildings;
	}
}
