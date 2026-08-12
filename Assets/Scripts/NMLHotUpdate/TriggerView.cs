using BaseModel;
using TWDModel;

public class TriggerView : ModelView<TriggerModel>, TriggerReceiver
{
	public override bool AutoGenerateViewID => true;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		(model as TriggerModel).AddNonModelReceiver(this);
	}

	public void OnTriggered(ActorModel instigator)
	{
	}
}
