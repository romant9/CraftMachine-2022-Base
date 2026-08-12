using System;
using UnityEngine;

[Serializable]
public class EquipmentResourceEntry : ResourceEntry
{
	public string PrefabName;

	public string OtherHandPrefabName;

	public string IconSprite;

	public string AnimationId;

	public string TypeSoundOverride;

	public bool useOtherHandOnCharged;

	public Material specialMaterial;
}
