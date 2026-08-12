using TWDModel;

public class BuildingConstructionData
{
	public string BuildingType { get; set; }

	public bool IsAvailable { get; set; }

	public int ConstructionPrice { get; set; }

	public CurrencyType ConstructionPriceCurrency { get; set; }

	public int RequiredCouncilLevel { get; set; }

	public string RequiredBuilding { get; set; }

	public int Amount { get; set; }
}
