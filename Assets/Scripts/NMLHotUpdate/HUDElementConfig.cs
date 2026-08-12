using System;
using NextGames.Sdk.AssetBundleManager;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "DefaultPopup", menuName = "Popup/Create HUDElementConfig")]
public class HUDElementConfig : ScriptableObject
{
	public enum ElementTypeEnum
	{
		HUD = 0,
		Popup = 1,
		Dialog = 2,
		ContextMenuBox = 3
	}

	public string PrefabName;

	private GameObject assetBundlePrefab;

	public ElementTypeEnum ElementType;

	public GameState LoadOnState;

	public bool DeleteOnExitState;

	public bool DeleteOnClose;

	public bool ShowOnTopCameras;

	public bool ShowInPerspectiveCamera;

	public bool BlockTutorialDialogs;

	public bool colorClear;

	public MusicState MusicType;

	public string UsesFeature;

	[Header("On Open")]
	public DisableCampMode DisableCampMode;

	public static string BundleName => "hudelements";

	public GameObject GetPrefab()
	{
		if (assetBundlePrefabOverride)
		{
			return assetBundlePrefabOverride;
		}
		else
		{
			if (assetBundlePrefab == null)
			{
				assetBundlePrefab = AssetBundleManager.Instance.LoadAsset<GameObject>(PrefabName, BundleName);
			}
			return assetBundlePrefab;
		}
	}

	#region myparams
	public GameObject assetBundlePrefabOverride;
	#endregion
}
