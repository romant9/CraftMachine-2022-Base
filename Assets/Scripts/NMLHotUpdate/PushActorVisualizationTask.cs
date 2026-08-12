using Client.Utils;
using TWDModel;
using UnityEngine;

public class PushActorVisualizationTask : ActorVisualizationTask
{
	private CombatModel combat;

	private PushActorAction _pushActorAction;

	private static readonly float duration = 0.5f;

	private PolylinePathIterator PathIterator;

	private ActorModel attacker;

	private float currentTime;

	private float pathLength;

	public bool SetPushCoordinateAsDestination;

	public PushActorVisualizationTask(PushActorAction action)
		: base(action, affectsCovers: true)
	{
		_pushActorAction = action;
		base.Actor = action.PushEffect.DamageAction.TargetActor;
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		combat = GameManager.Instance.playerModel.Combat;
		AddFactionDependency(base.Actor.Faction);
		attacker = action.PushEffect.DamageAction.DamagerActor;
		if (attacker != null)
		{
			AddDependency(attacker, reserve: false);
		}
		if (VisualizationQueue.Instance.GetMostRecentlyAddedActorTask<FireWeaponVisualizationTask>(base.Actor) != null)
		{
			VisualizationQueue.Instance.AddTaskBlocker();
		}
		PushActorVisualizationTask mostRecentlyAddedActorTask = VisualizationQueue.Instance.GetMostRecentlyAddedActorTask<PushActorVisualizationTask>(base.Actor);
		if (mostRecentlyAddedActorTask != null)
		{
			mostRecentlyAddedActorTask.SetPushCoordinateAsDestination = true;
		}
		PolylinePath polylinePath = new PolylinePath();
		Vector3 inStart = GridView.Instance.GetPosition(action.PushEffect.OriginalCoordinate).ToVector3();
		Vector3 inEnd = GridView.Instance.GetPosition(action.PushEffect.PushCoordinate).ToVector3();
		polylinePath.AddSegment(new LineSegment(inStart, inEnd, new Vector3(0f, 1f, 0f)));
		PathIterator = new PolylinePathIterator(polylinePath);
		pathLength = PathIterator.TotalLength;
	}

	public void ReleaseDependenciesToAttacker()
	{
		ReleaseDependency(attacker, reservationOnly: false);
	}

	public override bool Update(float deltaTime)
	{
		if (base.ActorView == null || base.Actor.IsDead || SetPushCoordinateAsDestination)
		{
			return false;
		}
		Vector3 position = PathIterator.Position;
		float num = (currentTime + deltaTime) / duration;
		float num2 = 1f - Mathf.Pow(num - 1f, 4f);
		if (num >= 1f)
		{
			num2 = 1f;
		}
		float num3 = num2 * pathLength;
		float num4 = pathLength - PathIterator.RemainingDistance;
		float distance = num3 - num4;
		PathIterator.Advance(distance);
		currentTime += deltaTime;
		_ = PathIterator.Position - position;
		base.ActorView.transform.position = PathIterator.Position;
		if (num >= 1f)
		{
			return false;
		}
		return true;
	}
}
