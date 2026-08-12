using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ActorSpawnPointView : ModelView<ActorSpawnPointModel>
{
	[SerializeField]
	[Tooltip("Spawn start locations. If any specified they are used to initialize spawned actor position and actor will move to the actual spawn game play location from here. If multiple are specified it will use closest to the actual game play location.")]
	public List<GameObject> SpawnStartLocations;

	[SerializeField]
	[Tooltip("Animation that is used while spawning the actor.")]
	public SpawnAnimationType AnimationType;

	public override bool AutoGenerateViewID => true;
}
