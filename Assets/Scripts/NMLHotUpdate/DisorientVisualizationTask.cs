using TWDModel;

public class DisorientVisualizationTask : ActorVisualizationTask
{
	private bool isDead;

	private ActorModel SourceActor { get; set; }

	private AbilityDefinition Ability { get; set; }

	public DisorientVisualizationTask(DisorientAction action)
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
		isDead = base.Actor.IsDead;
	}

	public override bool Update(float deltaTime)
	{
		if (base.Action is DisorientAction { Avoided: not false })
		{
			base.ActorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.DisorientAvoided"), "Ui_Icon_StatusEffect_Disorient", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
		}
		return false;
	}
}
