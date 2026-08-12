using TWDModel;

public class EndInteractiveObjectVisualizationTask : ActorVisualizationTask
{
	private bool forceEndPositionSet;

	private SurvivorAnimationController SurvivorAnimationController
	{
		get
		{
			if (!(base.ActorView != null))
			{
				return null;
			}
			return base.ActorView.GetComponent<SurvivorAnimationController>();
		}
	}

	private bool Completed { get; set; }

	private InteractiveObjectView InteractiveObjectView { get; set; }

	public EndInteractiveObjectVisualizationTask(ActorModel actor, InteractiveObjectModel target, bool completed)
		: base(null)
	{
		base.Actor = actor;
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		InteractiveObjectView = GameManager.Instance.GetViewForModel(target) as InteractiveObjectView;
		Completed = completed;
		AddFactionDependency(base.Actor.Faction);
		AddActorDependency(base.Actor);
	}

	public override void Start()
	{
		base.Start();
		if (!InteractiveObjectView.SkipUseAnimation)
		{
			SurvivorAnimationController.EndEnvironmentAnimation(Completed);
			SurvivorAnimationController.InteractionCompleted += OnInteractionCompleted;
		}
		forceEndPositionSet = false;
	}

	private void OnInteractionCompleted()
	{
		ReleaseAllDependencies();
	}

	public override void Finished()
	{
		base.ActorView.SetWeaponActive(active: true);
		SurvivorAnimationController.InteractionCompleted -= OnInteractionCompleted;
	}

	public override bool Update(float deltaTime)
	{
		if (base.ActorView == null || InteractiveObjectView.SkipUseAnimation)
		{
			return false;
		}
		if (SurvivorAnimationController.IsInEndInteraction && !forceEndPositionSet)
		{
			ReleaseAllDependencies();
			SurvivorAnimationController.ForceEndPosition(base.ActorView.transform.position, base.ActorView.transform.rotation);
			forceEndPositionSet = true;
		}
		base.ActorView.transform.position += SurvivorAnimationController.LastDeltaMovement;
		base.ActorView.transform.rotation *= SurvivorAnimationController.LastDeltaRotation;
		return !SurvivorAnimationController.IsIdle && !SurvivorAnimationController.IsDeathRequested && !SurvivorAnimationController.IsInDeath;
	}
}
