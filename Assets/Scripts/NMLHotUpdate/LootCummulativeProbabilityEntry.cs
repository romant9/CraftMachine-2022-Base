using TWDModel;

public class LootCummulativeProbabilityEntry
{
	public DropEventDefinition.DropEventType EventType { get; set; }

	public FixedPoint GoldDropCummulativeProbability { get; set; }

	public FixedPoint SilverDropCummulativeProbability { get; set; }
}
