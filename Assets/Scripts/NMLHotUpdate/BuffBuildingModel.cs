using Newtonsoft.Json;
using TWDModel;

public class BuffBuildingModel : BuildingModel
{
	[JsonIgnore]
	public string EffectTypeIdentifier
	{
		get
		{
			if (GetCurrentUpgradeLevel() != null)
			{
				return "Buff" + GetCurrentUpgradeLevel().BuffEffectType;
			}
			return "";
		}
	}

	[JsonIgnore]
	public TraitDefinition TraitDefinition => base.gameEconomyData.GetTraitDefinition(EffectTypeIdentifier);

	public override void Initialize()
	{
		base.Initialize();
	}
}
