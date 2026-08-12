using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class EquipmentTraitMockData : UtilsList.IDeepClonable<EquipmentTraitMockData>
	{
		public string Identifier { get; set; }

		public List<int> RemodeValues { get; set; }

		public List<int> RemodeIndexs { get; set; }

		[JsonIgnore]
		public int RarityLevel => UpgradeTraitsData.GetTraitLevelIdentifier(Identifier);

		[JsonIgnore]
		public bool IsTactical { get; set; }

		public EquipmentTraitMockData()
		{
		}

		public EquipmentTraitMockData(string id, List<int> remodeValues, List<int> remodeIndexs)
		{
			Identifier = id;
			RemodeValues = remodeValues;
			RemodeIndexs = remodeIndexs;
		}

		private EquipmentTraitMockData(EquipmentTraitMockData other)
		{
			Identifier = other.Identifier;
		}

		public EquipmentTraitMockData DeepClone()
		{
			return new EquipmentTraitMockData(this);
		}
	}
}
