using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class EquipTraitsDefinition
	{
		public SurvivorClass SurvivorClass;

		public EquipmentCategory EquipmentType;

		public int TraitsSlot;

		public string TraitsGroup;

		public int TraitsQualityLevel;

		public string EquipTraitsLevelID;

		public List<int> ConstructionParametersNumber;

		public List<int> MinConstructionParameters;

		public List<int> MaxConstructionParameters;
	}
}
