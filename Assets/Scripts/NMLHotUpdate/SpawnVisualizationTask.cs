using Client.Utils;
using TWDModel;
using UnityEngine;

public class SpawnVisualizationTask : ActorVisualizationTask
{
	private float currentNormalizedTime;

	private GridCoordinate spawnLocation { get; set; }

	private ActorSpawnPointView actorSpawnPointView { get; set; }

	private CharacterAnimationController animationController
	{
		get
		{
			if (!(base.ActorView != null))
			{
				return null;
			}
			return base.ActorView.CharacterAnimationController;
		}
	}

	public SpawnVisualizationTask(SpawnAction action)
		: base(action, affectsCovers: true)
	{
		base.Actor = GameManager.Instance.modelManager.GetModel<ActorModel>(action.ModelId);
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		AddFactionDependency(base.Actor.Faction);
		AddActorDependency(base.Actor);
		if (action.Instigator != null)
		{
			AddDependency(action.Instigator, reserve: false);
		}
		spawnLocation = action.SpawnLocation;
		if (action.ActorSpawnPoint != null)
		{
			ActorSpawnPointView[] array = Object.FindObjectsOfType<ActorSpawnPointView>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].ViewId == action.ActorSpawnPoint.ViewId)
				{
					actorSpawnPointView = array[i];
					break;
				}
			}
		}
		if (base.ActorView != null)
		{
			base.ActorView.CanUpdateVisibility = true;
			base.ActorView.SetVisible(visible: false);
			base.ActorView.CanUpdateVisibility = false;
		}
	}

	private void BeginAnimation(SpawnAnimationType spawnAnimationType)
	{
		if (base.Actor.Faction == Faction.Walker)
		{
			switch (spawnAnimationType)
			{
			case SpawnAnimationType.Crawl:
				(animationController as WalkerAnimationController).StartCrawl();
				break;
			case SpawnAnimationType.Walk:
				animationController?.StartMove(1f);
				break;
			}
		}
		else
		{
			animationController?.StartMove(1f);
		}
		Helpers.ExecuteCommand(new UpdateActorVisibilityCommand(base.Actor));
		base.ActorView.CanUpdateVisibility = true;
		base.ActorView.SetVisible(base.ActorView.Model.IsVisibleToSurvivors);
	}

	private void EndAnimation(SpawnAnimationType spawnAnimationType)
	{
		if (base.Actor.Faction == Faction.Walker)
		{
			switch (spawnAnimationType)
			{
			case SpawnAnimationType.Crawl:
				(animationController as WalkerAnimationController).StandUp();
				break;
			case SpawnAnimationType.Walk:
				animationController?.StopMove();
				break;
			}
		}
		else
		{
			animationController?.StopMove();
		}
		Helpers.ExecuteCommand(new UpdateActorVisibilityCommand(base.Actor));
	}

	public override void Start()
	{
		if (!(base.ActorView == null) && !(actorSpawnPointView == null))
		{
			if (actorSpawnPointView != null)
			{
				BeginAnimation(actorSpawnPointView.AnimationType);
			}
			else
			{
				BeginAnimation(SpawnAnimationType.Crawl);
			}
		}
	}

	public override bool Update(float deltaTime)
	{
		if (base.ActorView == null || actorSpawnPointView == null || animationController == null)
		{
			return false;
		}
		GridCoordinate coordinate = spawnLocation;
		Vector3 position = actorSpawnPointView.transform.position;
		Vector3 vector = GridView.Instance.GetPosition(coordinate).ToVector3();
		if (actorSpawnPointView != null && actorSpawnPointView.SpawnStartLocations != null && actorSpawnPointView.SpawnStartLocations.Count > 0)
		{
			float num = float.MaxValue;
			for (int i = 0; i < actorSpawnPointView.SpawnStartLocations.Count; i++)
			{
				float num2 = Vector3.Distance(actorSpawnPointView.SpawnStartLocations[i].transform.position, vector);
				if (num2 < num)
				{
					num = num2;
					position = actorSpawnPointView.SpawnStartLocations[i].transform.position;
				}
			}
		}
		float num3 = Vector3.Distance(position, vector);
		float num4 = ((animationController.LastDeltaMovementMagnitude > 0f) ? animationController.LastDeltaMovementMagnitude : (base.ActorView.GetMoveSpeed(MoveSpeed.Jog) * deltaTime));
		float num5 = Mathf.Clamp((num3 > 0f) ? (num4 / num3) : 1f, 0f, 1f);
		currentNormalizedTime = Mathf.Clamp(currentNormalizedTime + num5, 0f, 1f);
		Vector3 position2 = Vector3.Lerp(position, vector, currentNormalizedTime);
		base.ActorView.transform.position = position2;
		Vector3 vector2 = new Vector3(0f, 1f, 0f);
		Vector3 normalized = (vector - position).normalized;
		float angle = new Vector3(0f, 0f, 1f).SignedAngle(normalized, vector2);
		base.ActorView.transform.rotation = Quaternion.AngleAxis(angle, vector2);
		if (currentNormalizedTime >= 1f)
		{
			base.ActorView.transform.position = vector;
			if (actorSpawnPointView != null)
			{
				EndAnimation(actorSpawnPointView.AnimationType);
			}
			else
			{
				EndAnimation(SpawnAnimationType.Crawl);
			}
			return false;
		}
		return true;
	}
}
