using System.Collections.Generic;
using NextGames.Sdk.AssetBundleManager;
using TWDModel;
using UnityEngine;

public class ModularCharacter : ScriptableObject
{
	public CharacterGender Gender;

	public CharacterWeight Weight;

	public string PortraitHeadPartName;

	public string HeadPartName;

	public string TorsoPartName;

	public string LegsPartName;

	public Color TorsoColor = Color.white;

	public int TorsoColorPreset;

	public Color LegsColor = Color.white;

	public int LegsColorPreset;

	public Color SkinColor = Color.white;

	public int SkinColorPreset;

	public ActorProperties.PortraitSetupType PortraitSetup;

	public Vector3 HeadBoneRotation;

	public List<HeadAttachment> HeadAttachments;

	public ModularCharacter OutfitOverride;

	public bool Mirrored;

	private GameObject portraitHeadPartPrefab;

	private GameObject headPartPrefab;

	private GameObject torsoPartPrefab;

	private GameObject legsPartPrefab;

	public static string BundleName => "modularcharacter";

	public ActorGender GetActorGender()
	{
		if (Gender != CharacterGender.Male)
		{
			return ActorGender.Female;
		}
		return ActorGender.Male;
	}

	public GameObject GetPortraitHeadPart()
	{
		if (portraitHeadPartPrefab == null)
		{
			portraitHeadPartPrefab = AssetBundleManager.Instance.LoadAsset<GameObject>(PortraitHeadPartName, BundleName);
		}
		return portraitHeadPartPrefab;
	}

	public GameObject GetHeadPart()
	{
		if (headPartPrefab == null)
		{
			headPartPrefab = AssetBundleManager.Instance.LoadAsset<GameObject>(HeadPartName, BundleName);
		}
		return headPartPrefab;
	}

	public GameObject GetTorsoPart()
	{
		if (torsoPartPrefab == null)
		{
			torsoPartPrefab = AssetBundleManager.Instance.LoadAsset<GameObject>(TorsoPartName, BundleName);
		}
		return torsoPartPrefab;
	}

	public GameObject GetLegsPart()
	{
		if (legsPartPrefab == null)
		{
			legsPartPrefab = AssetBundleManager.Instance.LoadAsset<GameObject>(LegsPartName, BundleName);
		}
		return legsPartPrefab;
	}
}
