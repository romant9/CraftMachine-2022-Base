using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ModSkillMode : TWDModelObject
	{
		public string ID { get; set; }

		public string Type { get; set; }

		public SurvivorClass SurvivorClass { get; set; }

		public ModSkillState ModSkillState { get; set; }

		[JsonIgnore]
		public ModSkillLockState ModSkillLockState { get; set; } = ModSkillLockState.Locked;

		[JsonIgnore]
		public ModSkillUpState ModSkillUpState { get; set; } = ModSkillUpState.UnUpgradable;

		[IgnoreModelProperty]
		[JsonIgnore]
		public EquipmentItemModel EquipmentItemModel { get; set; }

		public int SlotIndex { get; set; } = -1;

		[JsonIgnore]
		public bool CanEquip { get; set; } = true;

		public ModSkillMode()
		{
		}

		public ModSkillMode(string type, SurvivorClass survivorClass)
		{
			Type = type;
			SurvivorClass = survivorClass;
		}

		public ModSkillMode(string id, string type, SurvivorClass survivorClass, ModSkillState modSkillState, EquipmentItemModel equippedItemModel, ModSkillLockState modSkillLockState)
		{
			ID = id;
			Type = type;
			SurvivorClass = survivorClass;
			ModSkillState = modSkillState;
			EquipmentItemModel = equippedItemModel;
			ModSkillLockState = modSkillLockState;
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public override void Start()
		{
			base.Start();
		}

		public bool IsMaxLevel()
		{
			if (base.gameEconomyData == null)
			{
				return false;
			}
			SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(ID);
			if (sPTraitsRemodeDefinition == null)
			{
				return false;
			}
			return sPTraitsRemodeDefinition.Level >= sPTraitsRemodeDefinition.MaxLevel;
		}

		public SPTraitsRemoldDefinitions GetSpTraitsDefaultTrait()
		{
			if (base.gameEconomyData == null)
			{
				return null;
			}
			SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(ID);
			if (sPTraitsRemodeDefinition == null)
			{
				return null;
			}
			return sPTraitsRemodeDefinition;
		}

		public ModSkillUpgradeResult Upgrade()
		{
			SPTraitsRemoldDefinitions spTraitsDefaultTrait = GetSpTraitsDefaultTrait();
			if (spTraitsDefaultTrait == null)
			{
				return null;
			}
			if (IsMaxLevel())
			{
				return null;
			}
			string iD = ID;
			SPTraitsRemoldDefinitions nextLevelTraitId = GetNextLevelTraitId(ID, spTraitsDefaultTrait.Level);
			if (nextLevelTraitId == null)
			{
				return null;
			}
			if (nextLevelTraitId.Level != spTraitsDefaultTrait.Level + 1)
			{
				return null;
			}
			if (nextLevelTraitId.Level > nextLevelTraitId.MaxLevel)
			{
				return null;
			}
			if (nextLevelTraitId.Type != spTraitsDefaultTrait.Type)
			{
				return null;
			}
			ID = nextLevelTraitId.ID;
			Type = nextLevelTraitId.Type;
			return new ModSkillUpgradeResult(iD, nextLevelTraitId);
		}

		private SPTraitsRemoldDefinitions GetNextLevelTraitId(string currentTraitId, int currentLevel)
		{
			SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(currentTraitId);
			if (sPTraitsRemodeDefinition == null)
			{
				return null;
			}
			SPTraitsRemoldDefinitions[] sPTraitsRemodeDefinition2 = base.gameEconomyData.SPTraitsRemodeDefinition;
			if (sPTraitsRemodeDefinition2 == null)
			{
				return null;
			}
			SPTraitsRemoldDefinitions[] array = sPTraitsRemodeDefinition2;
			foreach (SPTraitsRemoldDefinitions sPTraitsRemoldDefinitions in array)
			{
				if (sPTraitsRemoldDefinitions != null && !(sPTraitsRemoldDefinitions.ID == currentTraitId) && !string.IsNullOrEmpty(sPTraitsRemoldDefinitions.Type) && !(sPTraitsRemoldDefinitions.Type != sPTraitsRemodeDefinition.Type) && sPTraitsRemoldDefinitions.Level == currentLevel + 1)
				{
					return sPTraitsRemoldDefinitions;
				}
			}
			return null;
		}

		public void Reset()
		{
			EquipmentItemModel = null;
			ModSkillState = ModSkillState.Unequipped;
			SlotIndex = -1;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
