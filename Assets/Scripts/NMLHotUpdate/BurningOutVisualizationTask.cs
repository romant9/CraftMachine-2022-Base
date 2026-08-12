using TWDModel;

public class BurningOutVisualizationTask : ActorVisualizationTask
{
	public BurningOutVisualizationTask(BurningOutAction action)
		: base(action)
	{
		base.Actor = GameManager.Instance.modelManager.GetModel<ActorModel>(action.ModelId);
		base.ActorView = GameManager.Instance.GetViewForModel(action.TargetActor) as ActorView;
		AddFactionDependency(action.TargetActor.Faction);
		AddActorDependency(action.TargetActor);
	}

	public override bool Update(float deltaTime)
	{
		if (base.ActorView == null)
		{
			return false;
		}
		if (base.Action is BurningOutAction { Avoided: not false })
		{
			base.ActorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.BurningAvoided"), "Ui_Icon_StatusEffect_Burning", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
		}
		return false;
	}
}
