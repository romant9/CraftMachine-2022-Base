using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class EquipmentSetupData
	{
		public string ID;

		public int RarityLevel;

		public int MinTier;

		public int MaxTier;

		public List<string> Specializations;
	}
}
