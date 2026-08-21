using System.Collections.Generic;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class MoveVisualizationTask : ActorVisualizationTask
{
	private MoveAction _moveAction;

	private CombatModel combat;

	private PolylinePathIterator PathIterator;

	private MoveSpeed MoveSpeed;

	private bool stopped;

	private MoveVisualizationState State;

	private bool SlowdownAtEnd;

	private bool forcePositionSet;

	private bool inited;

	private bool globallyBlocking;

	private static int RunDistanceThreshold = 2;

	private CharacterAnimationController CharacterAnimationController;

	private GridCoordinate OldGrid = new GridCoordinate(0, 0);

	private Vector3 OldVector3 = Vector3.zero;

	public override bool IsGlobalBlocker => globallyBlocking;

	private bool HasEnemyTarget { get; set; }

	public MoveVisualizationTask(MoveAction action)
		: base(action, affectsCovers: true)
	{
		globallyBlocking = action.GloballyBlocking;
		_moveAction = action;
		PolylinePath polylinePath = new PolylinePath();
		float num = 0.63f;
		List<Line> list = new List<Line>();
		for (int i = 0; i < action.Path.Count - 1; i++)
		{
			GridCoordinate coordinate = action.Path[i];
			GridCoordinate coordinate2 = action.Path[i + 1];
			Vector3 inStart = GridView.Instance.GetPosition(coordinate).ToVector3();
			Vector3 inEnd = GridView.Instance.GetPosition(coordinate2).ToVector3();
			list.Add(new Line(inStart, inEnd));
		}
		Vector3 vector = new Vector3(0f, 1f, 0f);
		for (int j = 0; j < list.Count; j++)
		{
			Line line = list[j];
			Line line2 = ((j + 1 < list.Count) ? list[j + 1] : null);
			if (line2 == null || Vector3.Dot(Vector3.Normalize(line2.end - line2.start), Vector3.Normalize(line.end - line.start)) > 0.95f)
			{
				if (!polylinePath.EndsAtCurve)
				{
					polylinePath.AddSegment(new LineSegment(line.start, line.end, vector));
				}
				else
				{
					polylinePath.AddSegment(new LineSegment(line.center, line.end, vector));
				}
				continue;
			}
			Vector3 startTangent = (line.end - line.center) * (1f - num * 0.75f);
			Vector3 endTangent = (line2.end - line2.center) * (1f - num * 0.75f);
			if (!polylinePath.EndsAtCurve)
			{
				polylinePath.AddSegment(new LineSegment(line.start, line.center, vector));
			}
			polylinePath.AddSegment(new CurveSegment(line.center, line2.center, startTangent, endTangent, vector));
		}
		ActorModel model = GameManager.Instance.modelManager.GetModel<ActorModel>(action.ModelId);
		HasEnemyTarget = action.Path.HasTargetCoordinate && GameManager.Instance.playerModel.Combat.GetOccupier(action.Path.TargetCoordinate) != null;
		MoveSpeed moveSpeed = MoveSpeed.Walk;
		moveSpeed = ((!model.IsWalker) ? ((action.Path.Count > RunDistanceThreshold || HasEnemyTarget) ? MoveSpeed.Jog : MoveSpeed.Walk) : ((model.AIController.AIDataModel.Alertness > AIAlertness.Wandering) ? MoveSpeed.Jog : MoveSpeed.Walk));
		SlowdownAtEnd = !HasEnemyTarget;
		Init(model, polylinePath, moveSpeed);
	}

	public MoveVisualizationTask(ActorModel actor, PolylinePath polylinePath, MoveSpeed moveSpeed = MoveSpeed.Jog)
		: base(null, affectsCovers: true)
	{
		Init(actor, polylinePath, moveSpeed);
	}

	private void Init(ActorModel actor, PolylinePath polylinePath, MoveSpeed moveSpeed)
	{
		base.Actor = actor;
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		CharacterAnimationController = ((base.ActorView != null) ? base.ActorView.GetComponent<CharacterAnimationController>() : null);
		MoveSpeed = moveSpeed;
		AddFactionDependency(base.Actor.Faction);
		AddActorDependency(base.Actor);
		combat = GameManager.Instance.playerModel.Combat;
		if (combat != null)
		{
			AddSpatialDependency(base.Actor.GridCoordinate.X, base.Actor.GridCoordinate.Y);
		}
		PathIterator = new PolylinePathIterator(polylinePath);
		if (base.Actor.Faction == Faction.Walker && !base.Actor.IsVisibleToSurvivors)
		{
			HiddenMovementEffect component = base.ActorView.GetComponent<HiddenMovementEffect>();
			if (component != null && component.effectPrefab != null)
			{
				Object.Instantiate(component.effectPrefab, base.ActorView.transform.position + new Vector3(0f, 2f, 0f), Quaternion.identity);
			}
		}
		State = MoveVisualizationState.StartingToMove;
		inited = true;
	}

	public override List<VisualizationTask> TasksToQueue()
	{
		List<VisualizationTask> list = new List<VisualizationTask>();
		EquipmentItemModel selectedEquipment = base.Actor.SelectedEquipment;
		if (HasEnemyTarget && base.ActorView.CurrentWeapon != null && base.ActorView.CurrentWeapon != selectedEquipment && !selectedEquipment.IsChargeEquipment)
		{
			list.Add(new EquipWeaponVisualizationTask(base.Actor, selectedEquipment));
		}
		list.Add(this);
		return list;
	}

	public override void Start()
	{
		base.Start();
		if (CharacterAnimationController != null)
		{
			CharacterAnimationController.EnsureIdle();
			CharacterAnimationController.SetIdleStance(IdleStance.Stand);
		}
		forcePositionSet = false;
	}

	public override bool Update(float deltaTime)
	{
		if (!inited)
		{
			Debug.LogError("MoveVisualizationTask has not been initialized yet, this should NOT happen!");
			return true;
		}
		if (base.Actor == null)
		{
			Debug.LogError("Actor is null in MoveVisualizationTask!");
			return false;
		}
		if (base.ActorView == null)
		{
			Debug.LogError("ActorView is null in MoveVisualizationTask for Actor = " + base.Actor.Name + "!");
			return false;
		}
		if (CharacterAnimationController == null)
		{
			Debug.LogError("CharacterAnimationController is null in MoveVisualizationTask for Actor = " + base.Actor.Name + "!");
			return false;
		}
		if (stopped)
		{
			Debug.LogWarning("MoveVisualizationTask requested to stop!");
			return false;
		}
		if (CharacterAnimationController.IsInDeath)
		{
			return false;
		}
		if (State == MoveVisualizationState.StartingToMove)
		{
			if (CharacterAnimationController.IsIdle || base.Actor is CampDefenseWalkerModel)
			{
				CharacterAnimationController.StartMove((MoveSpeed == MoveSpeed.Jog) ? 1f : 0f);
				if (CharacterAnimationController.IsMoveRequested)
				{
					State = MoveVisualizationState.Moving;
				}
				base.ActorView.SetCoverIconState(CoverIconState.None);
			}
			return true;
		}
		if (State == MoveVisualizationState.Moving)
		{
			float num = ((CharacterAnimationController.LastDeltaMovementMagnitude > 0f) ? CharacterAnimationController.LastDeltaMovementMagnitude : (base.ActorView.GetMoveSpeed(MoveSpeed) * deltaTime));
			if (base.Actor.IsInvisible)
			{
				num *= 2f;
			}
			PathIterator.Advance(num);
			base.ActorView.transform.position = PathIterator.Position;
			Vector3 direction = PathIterator.Direction;
			base.ActorView.transform.rotation = Quaternion.LookRotation(direction);
			float num2 = 1f;
			if (!base.Actor.IsWalker && !base.Actor.IsInvisible)
			{
				if (SlowdownAtEnd && PathIterator.RemainingDistance < num2)
				{
					CharacterAnimationController.SetTargetMoveSpeed(0f);
				}
				if (PathIterator.RemainingDistance <= 1f && !CharacterAnimationController.IsStopping)
				{
					CharacterAnimationController.StopMove();
				}
				if (CharacterAnimationController.IsInStopMove && !CharacterAnimationController.HasForceEndPosition && !forcePositionSet)
				{
					forcePositionSet = true;
					CharacterAnimationController.ForceEndPosition(PathIterator.End, Quaternion.LookRotation(PathIterator.EndDirection));
				}
			}
		}
		if (PathIterator.AtEnd)
		{
			CharacterAnimationController.StopMove(useStopMove: false);
			return false;
		}
		return true;
	}

	public override void Stop()
	{
		if (CharacterAnimationController != null && CharacterAnimationController.IsValid)
		{
			CharacterAnimationController.StopMove();
			stopped = true;
		}
	}

	public override void Finished()
	{
		base.Finished();
		if (CharacterAnimationController != null && CharacterAnimationController.IsValid)
		{
			CharacterAnimationController.StopMove();
			stopped = true;
		}
		if (base.Actor is CampDefenseWalkerModel && PathIterator != null && base.ActorView != null)
		{
			CampDefenseModel campDefenseModel = GameManager.Instance?.playerModel?.Camp?.CampDefenseModel;
			if (campDefenseModel != null && campDefenseModel.Walkers.IndexOf(base.Actor as CampDefenseWalkerModel) >= 0)
			{
				base.ActorView.transform.position = PathIterator.End;
				base.ActorView.transform.rotation = Quaternion.LookRotation(PathIterator.EndDirection);
			}
		}
		else if (CharacterAnimationController != null && !CharacterAnimationController.HasForceEndPosition && _moveAction?.Path != null && _moveAction.Path.End.IsValid && combat?.Grid != null && base.ActorView != null)
		{
			base.ActorView.transform.position = combat.Grid.GetPosition(_moveAction.Path.End).ToVector3();
		}
	}
}
