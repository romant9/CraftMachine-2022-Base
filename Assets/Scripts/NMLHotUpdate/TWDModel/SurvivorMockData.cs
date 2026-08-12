using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class SurvivorMockData : UtilsList.IDeepClonable<SurvivorMockData>
	{
		[JsonIgnore]
		private List<TraitMockData> upgradeTraits;

		public string UpgradeTraitsList;

		[JsonIgnore]
		public string OwnerHashedPlayerId;

		[JsonIgnore]
		public int AssignedCounter;

		public int RarityLevel { get; set; }

		public string ActorDefinitionId { get; set; }

		public string CharacterPrefabName { get; set; }

		public string Name { get; set; }

		public SurvivorClass SurvivorClass { get; set; }

		public int AdjustedLevel { get; set; }

		public int TotalDamage { get; set; }

		public int Level { get; set; }

		public string AnalyticsId { get; set; }

		[JsonIgnore]
		public List<TraitMockData> UpgradeTraits
		{
			get
			{
				if (upgradeTraits == null && !string.IsNullOrEmpty(UpgradeTraitsList))
				{
					upgradeTraits = new List<TraitMockData>();
					string[] array = UpgradeTraitsList.Split(',');
					foreach (string text in array)
					{
						if (!string.IsNullOrEmpty(text))
						{
							upgradeTraits.Add(new TraitMockData(text));
						}
					}
				}
				return upgradeTraits;
			}
		}

		public EquipmentItemMockData MockWeapon { get; set; }

		public EquipmentItemMockData MockArmor { get; set; }

		[JsonIgnore]
		public bool IsHero => SurvivorModel.IsHeroFormActorDefinition(ActorDefinitionId);

		[JsonIgnore]
		public ActorGender Gender => SurvivorModel.GetAssetGender(CharacterPrefabName);

		[JsonIgnore]
		public ActorAge Age => ActorAge.Adult;

		public SurvivorMockData()
		{
		}

		private SurvivorMockData(SurvivorMockData otherSurvivor)
		{
			RarityLevel = otherSurvivor.RarityLevel;
			ActorDefinitionId = otherSurvivor.ActorDefinitionId;
			CharacterPrefabName = otherSurvivor.CharacterPrefabName;
			Name = otherSurvivor.Name;
			SurvivorClass = otherSurvivor.SurvivorClass;
			AdjustedLevel = otherSurvivor.AdjustedLevel;
			TotalDamage = otherSurvivor.TotalDamage;
			MockWeapon = otherSurvivor.MockWeapon;
			MockArmor = otherSurvivor.MockArmor;
			AnalyticsId = otherSurvivor.AnalyticsId;
			upgradeTraits = null;
		}

		public SurvivorMockData DeepClone()
		{
			SurvivorMockData survivorMockData = new SurvivorMockData(this);
			survivorMockData.Start();
			return survivorMockData;
		}

		public string GetLeaderTraitId()
		{
			if (!IsHero)
			{
				return string.Empty;
			}
			for (int i = 0; i < UpgradeTraits.Count; i++)
			{
				TraitMockData traitMockData = UpgradeTraits[i];
				if (traitMockData.IsLeaderBuff)
				{
					return traitMockData.Identifier;
				}
			}
			return string.Empty;
		}

		public void Start()
		{
			if (AnalyticsId == "be93128b39559be8d06b9d79f396f4aa")
			{

			}
			MockWeapon?.Start();
			MockArmor?.Start();
			if (UpgradeTraits != null && UpgradeTraits.Count > 0)
			{
				UpgradeTraits[0].IsTactical = true;
			}
		}



		#region myparams
		public int AdjustedLevelAdd { get; set; }
		#endregion
	}
}
