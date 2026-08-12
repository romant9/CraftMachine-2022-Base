using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class EquipmentItemMockData : UtilsList.IDeepClonable<EquipmentItemMockData>
	{
		[JsonIgnore]
		private List<EquipmentTraitMockData> upgradeTraits;

		public string UpgradeTraitsList;

		public string EquipmentDefinitionId { get; set; }

		public SpEquipmentRemoldModel SpEquipmentRemoldModel { get; set; }

		public int RarityLevel { get; set; }

		public string AnalyticsId { get; set; }

		public ModSkillSlot[] ModSkillSlots { get; set; }

		[JsonIgnore]
		public List<EquipmentTraitMockData> UpgradeTraits
		{
			get
			{
				if (upgradeTraits == null && !string.IsNullOrEmpty(UpgradeTraitsList))
				{
					upgradeTraits = new List<EquipmentTraitMockData>();
					string[] array = UpgradeTraitsList.Split(',');
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split('_');
						string text = array2[0];
						List<int> remodeValues = null;
						List<int> remodeIndexs = null;
						if (array2.Length >= 3)
						{
							remodeValues = (from x in array2[1].Split('|')
								select int.Parse(x)).ToList();
							remodeIndexs = (from x in array2[2].Split('|')
								select int.Parse(x)).ToList();
						}
						if (!string.IsNullOrEmpty(text))
						{
							upgradeTraits.Add(new EquipmentTraitMockData(text, remodeValues, remodeIndexs));
						}
					}
				}
				return upgradeTraits;
			}
		}

		public EquipmentItemMockData()
		{
		}

		private EquipmentItemMockData(EquipmentItemMockData other)
		{
			EquipmentDefinitionId = other.EquipmentDefinitionId;
			RarityLevel = other.RarityLevel;
			UpgradeTraitsList = other.UpgradeTraitsList;
			upgradeTraits = null;
			AnalyticsId = other.AnalyticsId;
			ModSkillSlots = new ModSkillSlot[other.ModSkillSlots.Length];
			for (int i = 0; i < other.ModSkillSlots.Length; i++)
			{
				ModSkillSlot modSkillSlot = other.ModSkillSlots[i];
				ModSkillMode mode = null;
				if (modSkillSlot.ModSkillMode != null)
				{
					mode = new ModSkillMode(modSkillSlot.ModSkillMode.ID, modSkillSlot.ModSkillMode.Type, modSkillSlot.ModSkillMode.SurvivorClass, modSkillSlot.ModSkillMode.ModSkillState, null, modSkillSlot.ModSkillMode.ModSkillLockState)
					{
						SlotIndex = modSkillSlot.ModSkillMode.SlotIndex
					};
				}
				ModSkillSlots[i] = new ModSkillSlot(modSkillSlot.Index, mode);
			}
		}

		public EquipmentItemMockData DeepClone()
		{
			return new EquipmentItemMockData(this);
		}

		public void Start()
		{
			if (UpgradeTraits != null && UpgradeTraits.Count > 0)
			{
				UpgradeTraits[0].IsTactical = true;
			}
		}
	}
}
