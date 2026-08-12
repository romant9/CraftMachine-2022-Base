using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class InhabitantMoveVisualizationTask : ActorVisualizationTask
{
	private PolylinePathIterator PathIterator;

	private MoveSpeed MoveSpeed;

	private InhabitantMoveVisualizationState State;

	private GameObject inhabitantHostContainer;

	private bool inited;

	private InhabitantLegacyAnimationController InhabitantLegacyAnimationController;

	public override bool IsGlobalBlocker => false;

	public InhabitantMoveVisualizationTask(ActorModel actor, GameObject hostActor, PolylinePath polylinePath, MoveSpeed moveSpeed = MoveSpeed.Jog)
		: base(null, affectsCovers: true)
	{
		inhabitantHostContainer = hostActor;
		Init(actor, polylinePath, moveSpeed);
	}

	private void Init(ActorModel actor, PolylinePath polylinePath, MoveSpeed moveSpeed)
	{
		base.Actor = actor;
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		InhabitantLegacyAnimationController = ((base.ActorView != null) ? base.ActorView.GetComponent<InhabitantLegacyAnimationController>() : null);
		MoveSpeed = moveSpeed;
		AddFactionDependency(base.Actor.Faction);
		AddActorDependency(base.Actor);
		PathIterator = new PolylinePathIterator(polylinePath);
		State = InhabitantMoveVisualizationState.StartingToMove;
		inited = true;
	}

	public override List<VisualizationTask> TasksToQueue()
	{
		return new List<VisualizationTask> { this };
	}

	public override bool Update(float deltaTime)
	{
		if (!inited)
		{
			Debug.LogError("InhabitantMoveVisualizationTask has not been initialized yet, this should NOT happen!");
			return true;
		}
		if (base.Actor == null)
		{
			Debug.LogError("Actor is null in InhabitantMoveVisualizationTask!");
			return false;
		}
		if (base.ActorView == null)
		{
			Debug.LogError("ActorView is null in InhabitantMoveVisualizationTask for Actor = " + base.Actor.Name + "!");
			return false;
		}
		if (InhabitantLegacyAnimationController == null)
		{
			Debug.LogWarning("InhabitantLegacyAnimationController is null in InhabitantMoveVisualizationTask for Actor = " + base.Actor.Name + "!");
		}
		if (State == InhabitantMoveVisualizationState.StartingToMove)
		{
			if (InhabitantLegacyAnimationController != null)
			{
				InhabitantLegacyAnimationController.StartMove();
			}
			State = InhabitantMoveVisualizationState.Moving;
			base.ActorView.SetCoverIconState(CoverIconState.None);
			return true;
		}
		if (State == InhabitantMoveVisualizationState.Moving)
		{
			float distance = ((InhabitantLegacyAnimationController.LastDeltaMovementMagnitude > 0f) ? InhabitantLegacyAnimationController.LastDeltaMovementMagnitude : (base.ActorView.GetMoveSpeed(MoveSpeed) * deltaTime));
			PathIterator.Advance(distance);
			if (inhabitantHostContainer != null)
			{
				inhabitantHostContainer.transform.position = PathIterator.Position;
			}
			base.ActorView.transform.position = PathIterator.Position;
			Vector3 direction = PathIterator.Direction;
			base.ActorView.transform.rotation = Quaternion.LookRotation(direction);
		}
		if (PathIterator.AtEnd)
		{
			InhabitantLegacyAnimationController.StopMove();
			return false;
		}
		return true;
	}
}
