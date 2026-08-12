using TWDModel;

public class NotificationVisualizationTask : ActorVisualizationTask
{
	private ActorNotificationMessage Message { get; set; }

	public NotificationVisualizationTask(ActorModel actor, ActorNotificationMessage message)
		: base(null)
	{
		base.Actor = actor;
		base.ActorView = GameManager.Instance.GetViewForModel(actor) as ActorView;
		AddDependency(actor, reserve: false);
		Message = message;
	}

	public override bool Update(float deltaTime)
	{
		base.ActorView.AddNotification(Message);
		return false;
	}
}
