using Newtonsoft.Json;

namespace TWDModel
{
	public class BounsModel : TWDModelObject
	{
		public int ItemID;

		public int Level;

		[JsonIgnore]
		public SurvivorModel UsingSurvivor => base.manager.Player.SurvivorContainer.Survivors.Models.Find((SurvivorModel x) => x.UsingBounsModel == this);

		[JsonIgnore]
		public BounsLevelDefinition LevelDefinition => base.manager.GameEconomyData.GetBounsLevelDefinition(ItemID, Level);

		[JsonIgnore]
		public string Owner => base.manager.GameEconomyData.GetBounsInfo(ItemID)?.Owner;

		public BounsModel(int itemId)
		{
			ItemID = itemId;
		}

		public override bool IsValid()
		{
			return true;
		}

		public void UpgradeLevel()
		{
			if (UsingSurvivor != null)
			{
				UsingSurvivor.RemoveTrait(LevelDefinition.TraitsLevel);
				UsingSurvivor.RemoveTrait(LevelDefinition.QualityLevel);
			}
			Level++;
			if (UsingSurvivor != null)
			{
				UsingSurvivor.AddTrait(LevelDefinition.TraitsLevel);
				UsingSurvivor.AddTrait(LevelDefinition.QualityLevel);
			}
		}

		public void SetLevel(int level)
		{
			if (UsingSurvivor != null)
			{
				UsingSurvivor.RemoveTrait(LevelDefinition.TraitsLevel);
				UsingSurvivor.RemoveTrait(LevelDefinition.QualityLevel);
			}
			Level = level;
			if (UsingSurvivor != null)
			{
				UsingSurvivor.AddTrait(LevelDefinition.TraitsLevel);
				UsingSurvivor.AddTrait(LevelDefinition.QualityLevel);
			}
		}
	}
}
