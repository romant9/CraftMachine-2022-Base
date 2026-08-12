using TWDModel;

public class UpgradePathData
{
	public int StartLevel { get; set; }

	public int CurrentLevel { get; set; }

	public int MaxLevel { get; set; }

	public SurvivorModel Survivor { get; set; }

	public EquipmentItemModel Equipment { get; set; }

	public UpgradeTraitsData GetUpgradeData(int level)
	{
		if (Survivor != null)
		{
			return Survivor.GetUpgradeTraitsDataForLevel(level);
		}
		if (Equipment != null)
		{
			return Equipment.GetUpgradeTraitsDataForLevel(level);
		}
		return null;
	}
}
