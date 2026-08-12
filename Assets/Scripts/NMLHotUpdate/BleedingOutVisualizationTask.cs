using TWDModel;

public class BleedingOutVisualizationTask : ActorVisualizationTask
{
	private ActorModel Target { get; set; }

	private ActorView TargetView { get; set; }

	public BleedingOutVisualizationTask(BleedingOutAction action)
		: base(action)
	{
		base.Actor = GameManager.Instance.modelManager.GetModel<ActorModel>(action.ModelId);
		Target = action.Target;
		TargetView = GameManager.Instance.GetViewForModel(action.Target) as ActorView;
		AddFactionDependency(Target.Faction);
		AddActorDependency(Target);
	}

	public BleedingOutVisualizationTask(ActorModel source, ActorModel target)
		: base(null)
	{
		base.Actor = source;
		Target = target;
		TargetView = GameManager.Instance.GetViewForModel(Target) as ActorView;
		AddFactionDependency(Target.Faction);
		AddActorDependency(Target);
	}

	public override void Start()
	{
	}

	public override bool Update(float deltaTime)
	{
		if (TargetView == null)
		{
			return false;
		}
		if (base.Action is BleedingOutAction { Avoided: not false })
		{
			TargetView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.BleedingAvoided"), "Ui_Icon_StatusEffect_Bleeding", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
			return false;
		}
		SurvivorAnimationController survivorAnimationController = TargetView.CharacterAnimationController as SurvivorAnimationController;
		if (survivorAnimationController != null)
		{
			survivorAnimationController.IsBleedingOutRequested = true;
		}
		return false;
	}
}
