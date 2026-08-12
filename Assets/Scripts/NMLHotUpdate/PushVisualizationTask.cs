using System.Collections.Generic;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class PushVisualizationTask : ActorVisualizationTask
{
	private PushState State;

	private PolylinePathIterator PathIterator;

	private MovableView MovableView { get; set; }

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

	public PushVisualizationTask(ActorModel pushingActor, MovableView movableView)
		: base(null, affectsCovers: true)
	{
		base.Actor = pushingActor;
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		MovableView = movableView;
		AddFactionDependency(base.Actor.Faction);
		AddActorDependency(base.Actor);
		PolylinePath polylinePath = new PolylinePath();
		Vector3 inStart = GridView.Instance.GetPosition(base.Actor.GridCoordinate).ToVector3();
		Vector3 inEnd = GridView.Instance.GetPosition(MovableView.Model.GetTargetCoordinate(base.Actor.GridCoordinate)).ToVector3();
		polylinePath.AddSegment(new LineSegment(inStart, inEnd, new Vector3(0f, 1f, 0f)));
		PathIterator = new PolylinePathIterator(polylinePath);
	}

	public override List<VisualizationTask> TasksToQueue()
	{
		return new List<VisualizationTask> { this };
	}

	public override void Start()
	{
		base.Start();
		if (!SurvivorAnimationController.IsIdle)
		{
			SurvivorAnimationController.EnsureIdle();
			State = PushState.WaitingForIdle;
		}
		else
		{
			SurvivorAnimationController.StartPush();
			State = PushState.Moving;
		}
	}

	public override bool Update(float deltaTime)
	{
		if (base.ActorView == null)
		{
			return false;
		}
		switch (State)
		{
		case PushState.WaitingForIdle:
			if (SurvivorAnimationController.IsIdle)
			{
				SurvivorAnimationController.StartPush();
				State = PushState.Moving;
			}
			break;
		case PushState.Moving:
		{
			base.ActorView.SetWeaponActive(active: false);
			Vector3 position = PathIterator.Position;
			float lastDeltaMovementMagnitude = SurvivorAnimationController.LastDeltaMovementMagnitude;
			PathIterator.Advance(lastDeltaMovementMagnitude);
			Vector3 vector = PathIterator.Position - position;
			base.ActorView.transform.position = PathIterator.Position;
			Vector3 vector2 = new Vector3(0f, 1f, 0f);
			Vector3 direction = PathIterator.Direction;
			float angle = new Vector3(0f, 0f, 1f).SignedAngle(direction, vector2);
			base.ActorView.transform.rotation = Quaternion.AngleAxis(angle, vector2);
			if (MovableView != null)
			{
				MovableView.transform.position += vector;
			}
			if (PathIterator.AtEnd)
			{
				SurvivorAnimationController.StopPush();
				base.ActorView.transform.position = GridView.Instance.GetPosition(base.Actor.GridCoordinate).ToVector3();
				return false;
			}
			break;
		}
		}
		return true;
	}

	public override void Finished()
	{
		base.Finished();
		base.ActorView.SetWeaponActive(active: true);
	}
}
