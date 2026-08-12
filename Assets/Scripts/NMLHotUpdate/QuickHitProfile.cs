using System;
using TWDModel;

[Serializable]
public class QuickHitProfile
{
	[Tooltip("Should use this profile as default if proper impact profile cannot be found.")]
	public bool useAsDefault;

	[Tooltip("Equipment type for this profile.")]
	public EquipmentType EquipmentType;

	public string SubCategory;

	public string bodyPartName;

	public EffectSpawnLocation effectSpawnLocation;

	public EffectSpawnDirection effectSpawnDirection;

	public PrefabResource effectPrefabResource;

	[Tooltip("Ricochet prefab is instantiated when an enemy with Impenetrable trait is hit.")]
	public PrefabResource ricochetPrefabResource;

	public bool spawnEffectsOnlyOnce;

	public bool spawnEffectsOnlyOnCharge;
}
