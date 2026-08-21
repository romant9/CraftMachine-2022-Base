using BaseModel;
using TWDModel;

public class WorldBossPVPCellSlotData
{
	public WorldBossCellDefinition CellDefinition;

	public WorldBossCellStateSnapshot CellStateSnapshot;

	public string MyColorFlag;

	public bool HasValue => CellDefinition != null;
}
