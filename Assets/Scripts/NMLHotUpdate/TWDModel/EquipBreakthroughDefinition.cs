using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class EquipBreakthroughDefinition
	{
		public int Level;

		public int WeaponFragmentsNumber;

		public int WeaponStar;

		public int NeedTokenStar;

		public WeaponDrawingType WeaponDrawingType;

		public int WeaponDrawingNumber;

		public int Traits1QualityLevel;

		public int Traits2QualityLevel;

		public int Traits3QualityLevel;

		public int Traits4QualityLevel;

		public int AttackPercentage;

		public int AttackNumber;

		public int DefensePercentage;

		public int DefenseNumber;

		public int ApocalypticTraitLevel;

		public int Hit;

		public string Describe;

		public string MaterialsDescribe;

		public int CommonBluePrintCost;

		public string WeaponMode;

		public List<int> ScrapResources;

		public int ScrapSkillToken;

		[JsonIgnore]
		public Rewards RewardEntries { get; set; }
	}
}
