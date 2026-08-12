using TWDModel;
using UnityEngine;

public class TurnToTargetVisualizationTask : ActorVisualizationTask
{
	private Quaternion startActorRotation;

	private Quaternion endActorRotation;

	private const float actorFullTurnTime = 0.5f;

	private float turnDuration;

	private float actorTurnElapsedTime;

	private bool isGlobalBlocker;

	public override bool IsGlobalBlocker => isGlobalBlocker;

	public bool IgnoreTimedEffect { get; private set; }

	public TurnToTargetVisualizationTask(ActorModel actor, Vector3 sourcePosition, Vector3 targetPosition, bool ignoreTimedEffect = false)
		: base(null)
	{
		base.Actor = actor;
		base.ActorView = GameManager.Instance.GetViewForModel(actor) as ActorView;
		AddFactionDependency(actor.Faction);
		AddActorDependency(actor);
		IgnoreTimedEffect = ignoreTimedEffect;
		Vector3 normalized = (targetPosition - sourcePosition).normalized;
		endActorRotation = Quaternion.LookRotation(normalized, new Vector3(0f, 1f, 0f));
		IsTaskValid = !base.Actor.IsDead && !base.Actor.IsEnvironmental && (base.Actor.ExclusiveTimedEffect == null || base.Actor.IsRooted || base.Actor.IsPitfalled || base.Actor.IsInvisible || base.Actor.IsDisoriented || base.Actor.IsABTesterAed || base.Actor.IsTaunted || IgnoreTimedEffect) && base.Actor.Faction != Faction.Lure;
	}

	public void SetGlobalBlocker(bool blocking)
	{
		isGlobalBlocker = blocking;
	}

	public override void Start()
	{
		base.Start();
		actorTurnElapsedTime = 0f;
		turnDuration = Quaternion.Angle(startActorRotation, endActorRotation) / 360f * 0.5f;
		startActorRotation = base.ActorView.transform.rotation;
	}

	public override bool Update(float deltaTime)
	{
		if (!IsTaskValid)
		{
			return false;
		}
		actorTurnElapsedTime += deltaTime;
		float num = Mathf.Clamp((turnDuration > 0f) ? (actorTurnElapsedTime / turnDuration) : 1f, 0f, 1f);
		base.ActorView.transform.rotation = Quaternion.Slerp(startActorRotation, endActorRotation, num);
		if (num >= 1f)
		{
			return false;
		}
		return true;
	}
}
