using TWDModel;

public class HerdVisualizationTask : ActorVisualizationTask
{
	private int turnCount;

	private int maxDuration;

	private bool isDead;

	private ActorModel SourceActor { get; set; }

	public HerdVisualizationTask(HerdAction action)
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
		if (exclusiveTimedEffect != null && exclusiveTimedEffect.Type == TimedEffectType.Herd)
		{
			maxDuration = exclusiveTimedEffect.Duration;
			turnCount = maxDuration - exclusiveTimedEffect.Counter;
		}
		isDead = base.Actor.IsDead;
	}

	public override bool Update(float deltaTime)
	{
		if (base.Action is HerdAction herdAction)
		{
			if (herdAction.Avoided)
			{
				base.ActorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.HerdAvoided"), "Ui_Icon_StatusEffect_Herd", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
			}
			else if (!isDead && turnCount > 0)
			{
				base.ActorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.Herded"), "Ui_Icon_StatusEffect_Herd", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
				base.ActorView.Herd(turnCount, maxDuration);
			}
		}
		return false;
	}
}
