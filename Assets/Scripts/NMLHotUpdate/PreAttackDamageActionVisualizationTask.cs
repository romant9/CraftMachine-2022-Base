using TWDModel;

public class PreAttackDamageActionVisualizationTask : ActorVisualizationTask
{
	private ActorModel DamagerActor { get; set; }

	private ActorView DamagerView { get; set; }

	public PreAttackDamageActionVisualizationTask(PreAttackAction preAttackAction)
		: base(preAttackAction)
	{
		base.Actor = GameManager.Instance.modelManager.GetModel<ActorModel>(preAttackAction.ModelId);
		base.ActorView = CombatView.Instance.GetActorViewFromModel(base.Actor);
		DamagerActor = preAttackAction.DamagerActor;
		if (DamagerActor != null)
		{
			AddDependency(DamagerActor, reserve: false);
			DamagerView = CombatView.Instance.GetActorViewFromModel(DamagerActor);
			if (!DamagerActor.IsDead && base.Actor == preAttackAction.DamagerActor && (base.Actor.HasAnyLevelTrait("Interruptor") || base.Actor.HasAnyLevelTrait("Equipment_Active_Interruptor")))
			{
				string textId = (preAttackAction.Interrupted ? "ActorNotification.Interrupted" : "ActorNotification.Interrupt.Avoided");
				DamagerView?.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(textId)));
			}
		}
	}

	public override void Start()
	{
		base.Start();
		ReleaseDependency(DamagerActor);
	}
}
