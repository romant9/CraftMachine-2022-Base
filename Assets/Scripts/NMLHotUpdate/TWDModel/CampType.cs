using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class CampType
	{
		public string Name;

		public int Level;

		public int MaxCouncilLevel;

		public int MoveCostGas;

		public List<InitialCampBuildingData> Buildings;

		public List<CampSubtype> CampSubtypes;

		public CampType()
		{
			Buildings = new List<InitialCampBuildingData>();
			CampSubtypes = new List<CampSubtype>();
		}
	}
}
