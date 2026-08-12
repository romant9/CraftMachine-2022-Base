using TWDModel;

public class StunVisualizationTask : ActorVisualizationTask
{
	private int turnCount;

	private int maxDuration;

	private bool isDead;

	private ActorModel SourceActor { get; set; }

	private AbilityDefinition Ability { get; set; }

	public StunVisualizationTask(StunAction action)
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
		if (exclusiveTimedEffect != null && exclusiveTimedEffect.Type == TimedEffectType.Stun)
		{
			maxDuration = exclusiveTimedEffect.Duration;
			turnCount = maxDuration - exclusiveTimedEffect.Counter;
		}
		isDead = base.Actor.IsDead;
	}

	public override bool Update(float deltaTime)
	{
		if (base.Action is StunAction stunAction)
		{
			bool flag = ((SourceActor.SelectedAbility.PushEffect != null) ? base.Actor.IsDead : isDead);
			if (stunAction.Avoided)
			{
				base.ActorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.StunAvoided"), "Ui_Icon_StatusEffect_Stunned", NotificationSound.None, ActorNotificationType.TimedEffectNotification), dueLuck: false, null, null, TimedEffectType.None, stackMultiple: false, wipeAllPreviousOfSameType: true);
			}
			else if (!flag && turnCount > 0)
			{
				if (base.Actor.IsStunned)
				{
					base.ActorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.Stunned"), "Ui_Icon_StatusEffect_Stunned", NotificationSound.None, ActorNotificationType.TimedEffectNotification), dueLuck: false, null, null, TimedEffectType.Stun);
				}
				base.ActorView.Stun(turnCount, maxDuration);
			}
		}
		return false;
	}
}
