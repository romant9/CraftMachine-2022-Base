using TWDModel;

public class RootVisualizationTask : ActorVisualizationTask
{
	private int turnCount;

	private int maxDuration;

	private ActorModel SourceActor { get; set; }

	public RootVisualizationTask(RootAction action)
		: base(action, affectsCovers: true)
	{
		base.Actor = GameManager.Instance.modelManager.GetModel<ActorModel>(action.TargetActor.ModelId);
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		SourceActor = GameManager.Instance.modelManager.GetModel<ActorModel>(action.SourceActor.ModelId);
		AddFactionDependency(base.Actor.Faction);
		AddDependency(base.Actor, reserve: false);
		if (SourceActor != null)
		{
			AddDependency(SourceActor, reserve: false);
		}
		TimedEffect exclusiveTimedEffect = base.Actor.ExclusiveTimedEffect;
		if (exclusiveTimedEffect != null && exclusiveTimedEffect.Type == TimedEffectType.Root)
		{
			maxDuration = exclusiveTimedEffect.Duration;
			turnCount = maxDuration - exclusiveTimedEffect.Counter;
		}
	}

	public override bool Update(float deltaTime)
	{
		RootAction rootAction = base.Action as RootAction;
		if (rootAction.TargetActor.IsDead)
		{
			return false;
		}
		if (rootAction != null)
		{
			if (rootAction.Avoided)
			{
				base.ActorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.RootAvoided"), "Ui_Icon_StatusEffect_Rooted", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
			}
			else if (turnCount > 0)
			{
				base.ActorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.Rooted"), "Ui_Icon_StatusEffect_Rooted", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
			}
		}
		return false;
	}
}
