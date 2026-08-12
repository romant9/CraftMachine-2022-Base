using BaseModel;
using TWDModel;

public class MissionEventView : ModelView<MissionEventModel>
{
	public MissionEventType EventType;

	public override bool AutoGenerateViewID => true;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
	}
}
