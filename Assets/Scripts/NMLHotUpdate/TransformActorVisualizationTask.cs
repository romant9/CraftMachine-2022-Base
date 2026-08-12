using TWDModel;

public class TransformActorVisualizationTask : ActorVisualizationTask
{
	private ActorModel sourceActor;

	private ActorModel targetActor;

	private ActorView sourceView;

	private ActorView targetView;

	private WalkerAnimationController controller;

	private bool isComplete;

	private bool transformStarted;

	public TransformActorVisualizationTask(TransformActorAction action)
		: base(action)
	{
		sourceActor = action.SourceActor;
		sourceView = GameManager.Instance.GetViewForModel(sourceActor) as ActorView;
		targetActor = action.TargetActor;
		targetView = GameManager.Instance.GetViewForModel(targetActor) as ActorView;
		AddActorDependency(sourceActor);
		AddActorDependency(targetActor);
		if (action.Instigator != null)
		{
			AddActorDependency(action.Instigator);
		}
		AddFactionDependency(sourceActor.Faction);
		controller = sourceView.CharacterAnimationController as WalkerAnimationController;
	}

	public override void Start()
	{
		base.Start();
		controller.OnTransformEffectHandler += OnTransformEffect;
		controller.OnTransformHandler += OnTransform;
	}

	public override bool Update(float deltaTime)
	{
		if (controller == null)
		{
			return false;
		}
		if (controller.IsIdle && !transformStarted)
		{
			controller.Transform();
			transformStarted = true;
		}
		if (RunTime > 10f)
		{
			return false;
		}
		return !isComplete;
	}

	private void OnTransformEffect()
	{
		TransformConcealmentEffect component = sourceView.GetComponent<TransformConcealmentEffect>();
		if (component != null)
		{
			Helpers.InstantiateToParent(component.effectPrefab, targetView.gameObject);
		}
		controller.OnTransformEffectHandler -= OnTransformEffect;
	}

	private void OnTransform()
	{
		CombatView.Instance.RemoveActorView(sourceView);
		targetView.CanUpdateVisibility = true;
		targetView.SetVisible(targetActor.IsVisibleToSurvivors);
		controller.OnTransformHandler -= OnTransform;
		isComplete = true;
	}
}
