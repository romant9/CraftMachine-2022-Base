using System.Collections.Generic;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class DashVisualizationTask : VisualizationTask
{
	private static float moveSpeed = 10f;

	private float totalAnimationDuration;

	private float animationTime = -1f;

	private Vector3 finalPosition;

	private Vector3 initialPosition;

	private ActorModel Actor { get; set; }

	private AbilityDefinition Ability { get; set; }

	private ActorView View { get; set; }

	public DashVisualizationTask(DashAction action)
		: base(action)
	{
		Actor = GameManager.Instance.modelManager.GetModel<ActorModel>(action.ModelId);
		View = GameManager.Instance.GetViewForModel(Actor) as ActorView;
		Ability = action.Ability.Definition;
		AddFactionDependency(Actor.Faction);
		AddActorDependency(Actor);
		FixedVec3 position = GridView.Instance.GetPosition(action.OriginalCoordinate);
		FixedVec3 position2 = GridView.Instance.GetPosition(Actor.GridCoordinate);
		initialPosition = position.ToVector3();
		finalPosition = position2.ToVector3();
		float magnitude = (finalPosition - initialPosition).magnitude;
		totalAnimationDuration = magnitude / moveSpeed;
		View.transform.position = initialPosition;
		animationTime = 0f;
	}

	public override List<VisualizationTask> TasksToQueue()
	{
		return new List<VisualizationTask>
		{
			new TurnToTargetVisualizationTask(Actor, initialPosition, finalPosition),
			this
		};
	}

	public override bool Update(float deltaTime)
	{
		animationTime += deltaTime;
		if (animationTime >= totalAnimationDuration)
		{
			View.transform.position = finalPosition;
			return false;
		}
		float num = animationTime / totalAnimationDuration;
		Vector3 vector = finalPosition - initialPosition;
		float magnitude = vector.magnitude;
		Vector3 position = initialPosition + vector.normalized * (magnitude * num);
		View.transform.position = position;
		return true;
	}
}
