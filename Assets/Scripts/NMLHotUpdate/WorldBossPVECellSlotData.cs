using BaseModel;
using TWDModel;

public class WorldBossPVECellSlotData
{
	public WorldBossCellDefinition CellDefinition;

	public WorldBossCellStateSnapshot CellStateSnapshot;

	public bool HasValue => CellDefinition != null;
}
