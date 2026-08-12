using System.Collections.Generic;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class StartInteractiveObjectVisualizationTask : ActorVisualizationTask
{
	public enum VisualizationState
	{
		Preparing = 0,
		StartInteraction = 1,
		LoopInteraction = 2,
		EndInteraction = 3
	}

	private VisualizationState State;

	private bool forceStartPositionSet;

	private bool forceEndPositionSet;

	private EnvironmentAnimation environmentAnimation;

	private Vector3 startLocation;

	private Quaternion startRotation;

	private Vector3 actorLocation;

	private SurvivorAnimationController SurvivorAnimationController
	{
		get
		{
			if (!(base.ActorView != null))
			{
				return null;
			}
			return base.ActorView.CharacterAnimationController as SurvivorAnimationController;
		}
	}

	private InteractiveObjectView InteractiveObjectView { get; set; }

	public StartInteractiveObjectVisualizationTask(StartInteractiveObjectAction action)
		: base(action)
	{
		base.Actor = GameManager.Instance.modelManager.GetModel<ActorModel>(action.ModelId);
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		InteractiveObjectView = GameManager.Instance.GetViewForModel(action.Target) as InteractiveObjectView;
		AddFactionDependency(base.Actor.Faction);
		AddActorDependency(base.Actor);
		State = VisualizationState.Preparing;
	}

	public StartInteractiveObjectVisualizationTask(ActorModel actor, InteractiveObjectModel target)
		: base(null)
	{
		base.Actor = actor;
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		InteractiveObjectView = GameManager.Instance.GetViewForModel(target) as InteractiveObjectView;
		AddFactionDependency(base.Actor.Faction);
		AddActorDependency(base.Actor);
		State = VisualizationState.Preparing;
	}

	public override List<VisualizationTask> TasksToQueue()
	{
		List<VisualizationTask> list = new List<VisualizationTask>();
		if (!InteractiveObjectView.SkipUseAnimation)
		{
			FixedVec3 position = GridView.Instance.GetPosition(base.Actor.GridCoordinate);
			int edge = InteractiveObjectView.Model.Location.Edge;
			GridCoordinate coordinate;
			if (edge >= 0)
			{
				GridView.Instance.Model.GetCoordinatesFromEdge(edge, out var a, out var b);
				FixedVec3 position2 = GridView.Instance.GetPosition(a);
				FixedVec3 position3 = GridView.Instance.GetPosition(b);
				FixedVec3 fixedVec = FixedVec3.Normalize(position3 - position2);
				FixedVec3 fixedVec2 = (position2 + position3) * 0.5;
				FixedVec3 position4 = GridView.Instance.GetPosition(base.Actor.GridCoordinate);
				if (FixedVec3.Dot(FixedVec3.Normalize(fixedVec2 - position4), fixedVec) < 0.0)
				{
					fixedVec = FixedVec3.Negative(fixedVec);
				}
				FixedVec3 position5 = position4 + fixedVec * GridView.Instance.Grid.CellSize.X;
				coordinate = GridView.Instance.Grid.GetCoordinate(position5);
			}
			else
			{
				coordinate = InteractiveObjectView.Model.Location.Coordinate;
			}
			FixedVec3 position6 = GridView.Instance.GetPosition(coordinate);
			list.Add(new TurnToTargetVisualizationTask(base.Actor, position.ToVector3(), position6.ToVector3(), ignoreTimedEffect: true));
		}
		list.Add(this);
		return list;
	}

	public override void Start()
	{
		base.Start();
		if (!InteractiveObjectView.SkipUseAnimation)
		{
			SurvivorAnimationController.EnsureIdle();
			forceStartPositionSet = false;
			forceEndPositionSet = false;
			environmentAnimation = InteractiveObjectView.gameObject.GetComponent<EnvironmentAnimation>();
			SurvivorAnimationController.InteractionCompleted += OnInteractionCompleted;
		}
	}

	private void OnInteractionCompleted()
	{
		ReleaseAllDependencies();
	}

	public override void Finished()
	{
		SurvivorAnimationController.InteractionCompleted -= OnInteractionCompleted;
	}

	public override bool Update(float deltaTime)
	{
		if (base.ActorView == null || InteractiveObjectView.SkipUseAnimation)
		{
			return false;
		}
		switch (State)
		{
		case VisualizationState.Preparing:
			if (!SurvivorAnimationController.IsIdle)
			{
				break;
			}
			actorLocation = GridView.Instance.GetPosition(base.Actor.GridCoordinate).ToVector3();
			startLocation = actorLocation;
			startRotation = base.ActorView.transform.rotation;
			if (environmentAnimation != null)
			{
				EnvironmentAnimationLocation closestLocation = environmentAnimation.GetClosestLocation(actorLocation);
				if (closestLocation != null)
				{
					Vector3 worldPosition = closestLocation.GetWorldPosition(InteractiveObjectView.transform);
					if (GridView.Instance.Model.GetCoordinate(worldPosition.ToFixedVec3()) == base.Actor.GridCoordinate)
					{
						startLocation = worldPosition;
						startRotation = closestLocation.GetWorldRotation(InteractiveObjectView.transform);
					}
				}
			}
			_ = startLocation.x;
			_ = startRotation.x;
			SurvivorAnimationController.StartEnvironmentAnimation(environmentAnimation);
			State = VisualizationState.StartInteraction;
			base.ActorView.SetWeaponActive(active: false);
			break;
		case VisualizationState.StartInteraction:
			if (SurvivorAnimationController.IsInStartInteraction && !forceStartPositionSet)
			{
				SurvivorAnimationController.ForceEndPosition(startLocation, startRotation, 0.75f);
				forceStartPositionSet = true;
			}
			if (!SurvivorAnimationController.IsInStartInteraction && forceStartPositionSet)
			{
				State = VisualizationState.LoopInteraction;
			}
			break;
		case VisualizationState.LoopInteraction:
			if (InteractiveObjectView.Model.TurnsToComplete > 1)
			{
				return false;
			}
			SurvivorAnimationController.EndEnvironmentAnimation(completed: true);
			State = VisualizationState.EndInteraction;
			break;
		case VisualizationState.EndInteraction:
			if (SurvivorAnimationController.IsInEndInteraction && !forceEndPositionSet)
			{
				ReleaseAllDependencies();
				SurvivorAnimationController.ForceEndPosition(actorLocation, base.ActorView.transform.rotation, 0.75f);
				forceEndPositionSet = true;
			}
			if (SurvivorAnimationController.IsIdle)
			{
				base.ActorView.SetWeaponActive(active: true);
				return false;
			}
			break;
		}
		base.ActorView.transform.position += SurvivorAnimationController.LastDeltaMovement;
		base.ActorView.transform.rotation *= SurvivorAnimationController.LastDeltaRotation;
		return true;
	}
}
