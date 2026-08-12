using System;
using UnityEngine;

[Serializable]
public struct ImpactConfiguration
{
	public ForceDirectionType forceDirectionType;

	public Vector3 forceDirection;

	public float forceMagnitude;

	public string bodyPartName;

	public Vector3 forceOffset;

	public EffectSpawnLocation effectSpawnLocation;

	public EffectSpawnDirection effectSpawnDirection;

	public PrefabResource effectPrefabResource;

	public DetachType detachBodyPartType;
}
