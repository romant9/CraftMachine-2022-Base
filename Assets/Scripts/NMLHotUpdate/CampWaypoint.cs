using UnityEngine;

public class CampWaypoint : MonoBehaviour
{
	[HideInInspector]
	public bool Occupied;

	[Header("Z-axis direction determines which way the actor will look when on this waypoint.")]
	[SerializeField]
	[Tooltip("Animation to inject to character at this waypoint.")]
	private AnimationClip animationClip;

	[SerializeField]
	[Tooltip("Type of waypoint trigger.")]
	private WaypointType type;

	[SerializeField]
	[Tooltip("Parameter for time in seconds if applicable for waypoint type.")]
	private float timeParameter;

	[SerializeField]
	[Tooltip("If checked, survivors will be spawned at this waypoint and never moved.")]
	private bool staticWaypoint;

	[SerializeField]
	[Tooltip("Move speed for actors at this waypoint. Only valid for certain WaypointTypes.")]
	private MoveSpeed moveSpeed;

	public GameObject GameObjectCached { get; private set; }

	public Vector2 GetVector2Position => new Vector2(base.transform.position.x, base.transform.position.z);

	public Vector3 GetVector3Position => base.transform.position;

	public WaypointType Type => type;

	public MoveSpeed MoveSpeed => moveSpeed;

	public AnimationClip Animation => animationClip;

	public float GetTimeParameter => timeParameter;

	private void Awake()
	{
		GameObjectCached = base.gameObject;
	}

	public bool IsStatic()
	{
		return staticWaypoint;
	}
}
