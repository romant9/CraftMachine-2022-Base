using BaseModel;
using TWDModel;

public class TimedQueueItemModel
{
	[IgnoreModelProperty]
	public TWDModelObject Item { get; set; }

	public long MillisecondsTillCompletion { get; set; }

	public long OriginalActionTime { get; set; }
}
