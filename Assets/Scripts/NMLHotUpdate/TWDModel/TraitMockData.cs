using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class TraitMockData : UtilsList.IDeepClonable<TraitMockData>
	{
		public string Identifier { get; set; }

		[JsonIgnore]
		public bool IsTactical { get; set; }

		[JsonIgnore]
		public bool IsLeaderBuff => Identifier.ToLower().Contains("leaderbuff");

		[JsonIgnore]
		public int RarityLevel => UpgradeTraitsData.GetTraitLevelIdentifier(Identifier);

		public TraitMockData()
		{
		}

		public TraitMockData(string id)
		{
			Identifier = id;
		}

		private TraitMockData(TraitMockData other)
		{
			Identifier = other.Identifier;
		}

		public TraitMockData DeepClone()
		{
			return new TraitMockData(this);
		}
	}
}
