using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NextGames.Sdk.AssetBundleManager;
using UnityEngine;

public class AssetBundleController : SingularityMonoBehaviour<AssetBundleController>
{
	private bool collectionLoaded;

	private int loadingRequestsCount;

	private AssetBundlesResourcesMap assetBundleConfig;

	private List<string> loadedBundles = new List<string>();

	private List<string> queueToUnload = new List<string>();

	public static Dictionary<string, string> fileName2Md5Dict;

	private bool _ShowUI = true;

	public bool AssetBundlesInitializedAndLoaded { get; private set; }

	public bool AdditiveSceneLoaded { get; set; }

	public bool LoadingAssetBundles => loadingRequestsCount > 0;

	private void Log(string message)
	{
		if (GameManager.ActiveBranch.Contains("develop") || DebugTWD.IsDebugBuild)
		{
			Debug.LogError(message);
		}
	}

	protected override void AwakeInternal()
	{
		if (OfflineManager.IsLoadDataManager)
		{
			OfflineManager.Instance.On_StreamingChanged += OnPathChanged;
		}
		LoadAssetBundleCollection();
	}

	private void LoadAssetBundleCollection()
	{
		if (TWDPlayerPrefs.HasKey(UserPrefsKeys.Key_StreamingAssetsPath))
		{
			AssetBundleManager.StreamingAssetsPath = TWDPlayerPrefs.GetString(UserPrefsKeys.Key_StreamingAssetsPath);
		}
		string text = "AssetBundles/AssetBundleData.json";
		string url;
		if (AssetBundleManager.IsMd5Bundles)
		{
			fileName2Md5Dict ??= MyTools.GetFileName2Md5Dict();
			string empty = string.Empty;
			string text2 = fileName2Md5Dict[text];
			string text3 = ((text.LastIndexOf(".") != -1) ? text.Insert(text.LastIndexOf("."), "." + text2) : text.Insert(text.LastIndexOf("\\") + 1, text2 + "."));
			url = ((!File.Exists(Application.persistentDataPath + "/GameAssets/" + text3)) ? ("file://" + AssetBundleManager.StreamingAssetsPath + "/" + text3) : ("file://" + Application.persistentDataPath + "/GameAssets/" + text3));
		}
		else
		{
			url = string.Format("file://{0}/{1}/{2}", AssetBundleManager.StreamingAssetsPath, "AssetBundles", "AssetBundleData.json").Replace('\\', '/');
		}
		StartCoroutine(AssetBundleCollection.DownloadCollection(url, OnCollectionDownloaded, OnError));
	}

	private void OnCollectionDownloaded(AssetBundleCollection collection)
	{
		AssetBundleManager.Instance.LoadCollection(collection, fileName2Md5Dict);

		if (OfflineManager.IsLoadDataManager)
		{
			List<string> bundles = OfflineManager.IsLoadBundleModular ? bundlesListPro : bundlesListLight;
			if (!AssetBundleManager.IsLoadFromResources) StartCoroutine(LoadAssetBundleResourceLight(bundles, OnLoaded));
		}
		else
		{
			StartCoroutine(LoadAssetBundleResource());
		}
	}

	private IEnumerator LoadAssetBundleResourceLight(List<string> bundles, Callback call = null)
	{
		SingularityMonoBehaviour<AssetBundleController>.Instance.LoadAssetBundle(bundles);
		while (SingularityMonoBehaviour<AssetBundleController>.Instance.LoadingAssetBundles)
		{
			yield return null;
		}
		assetBundleConfig = AssetBundleManager.Instance.LoadAsset<AssetBundlesResourcesMap>("AssetBundleResource", "abresource");
		collectionLoaded = true;
		call?.Invoke();
	}

	private void OnLoaded()
	{
		OfflineManager.Instance.ActivateMethods();
	}

	private IEnumerator LoadAssetBundleResource()
	{
		SingularityMonoBehaviour<AssetBundleController>.Instance.LoadAssetBundle("abresource");
		SingularityMonoBehaviour<AssetBundleController>.Instance.LoadAssetBundle("scriptableobjects");
		while (SingularityMonoBehaviour<AssetBundleController>.Instance.LoadingAssetBundles)
		{
			yield return null;
		}
		assetBundleConfig = AssetBundleManager.Instance.LoadAsset<AssetBundlesResourcesMap>("AssetBundleResource", "abresource");
		collectionLoaded = true;
	}

	public bool IsAssetExistedInAssetBundle(string assetBundle, string assetName)
	{
		return AssetBundleManager.Instance.IsAssetExistedInAssetBundle(assetBundle, assetName);
	}

	public IEnumerator DownloadAssets(bool showUI = true)
	{
		_ShowUI = showUI;
		while (!collectionLoaded)
		{
			yield return null;
		}
		if (_ShowUI)
		{
			LoadingScreenHUD.SetLoadingMessage(LoadingMessageType.AssetLoading);
		}
		AssetBundleHandler[] array = AssetBundleManager.Instance.PrepHandlersToDownload();
		long num = 0L;
		AssetBundleHandler[] array2 = array;
		foreach (AssetBundleHandler assetBundleHandler in array2)
		{
			num += assetBundleHandler.AssetBundleSize;
		}
		Action<float> progress = null;
		if (_ShowUI && num > 0)
		{
			LoadingScreenHUD.BeginHotUpdateKeepAwake();
			LoadingScreenHUD.SetLoadingMessage(LoadingMessageType.DownloadAssets);
			progress = OnDownloadProgress;
		}
		AssetBundleManager.Instance.DownloadAllAssetBundles(OnBundlesDownloaded, OnError, progress);
	}

	private void OnBundlesDownloaded()
	{
		Log("Downloaded all asset bundles");
		LoadAssetBundle((from t in assetBundleConfig.resources
			where t.ShouldLoadOnStart
			select t.Identifier).ToList(), delegate
		{
			Log("AssetBundles loaded");
			AssetBundlesInitializedAndLoaded = true;
		}, delegate(float value)
		{
			if (_ShowUI)
			{
				OnDownloadProgress(value);
			}
		});
	}

	public void LoadAssetBundleAndDependencies(List<string> assetBundles)
	{
		LoadAssetBundle(AssetBundleManager.Instance.GetAllUnloadedBundlesAndDependencies(assetBundles));
	}

	public void LoadAssetBundle(string assetbundleName)
	{
		LoadAssetBundle(new List<string> { assetbundleName });
	}

	public void LoadAssetBundle(List<string> assetBundles, Action success = null, Action<float> loadingProgressCallback = null)
	{
		queueToUnload = queueToUnload.Except(assetBundles).ToList();
		List<string> nonLoadedBundles = assetBundles.Except(loadedBundles).ToList();
		if (assetBundles.Count > nonLoadedBundles.Count)
		{
			List<string> values = assetBundles.Except(nonLoadedBundles).ToList();
			Log("AssetBundles " + string.Join(",", values) + " already loaded!");
		}
		if (nonLoadedBundles.Count > 0)
		{
			loadingRequestsCount++;
			AssetBundleManager.Instance.LoadAssetBundles(nonLoadedBundles, delegate
			{
				loadedBundles = AssetBundleManager.Instance.LoadedAssetBundles();
				success?.Invoke();
				Log("AssetBundle(s) " + string.Join(", ", nonLoadedBundles) + " loaded");
				loadingRequestsCount--;
				UnloadQueued();
			}, OnError, loadingProgressCallback);
		}
		else
		{
			success?.Invoke();
		}
	}

	public void LoadSceneBundleAndDependencies(string scenario)
	{
		string text = "scene_" + scenario.Split('_')[0].ToLower(CultureInfo.InvariantCulture);
		List<string> list = new List<string> { text };
		text += "_dependencies";
		if (!IsBundleLoaded(text))
		{
			list.Add(text);
		}
		LoadAssetBundle(list);
	}

	public void UnloadSceneBundleDependencies(string scene)
	{
		UnloadAssetBundle("scene_" + scene.Split('_')[0].ToLower(CultureInfo.InvariantCulture) + "_dependencies");
	}

	public void UnloadSceneBundle(string scene)
	{
		UnloadAssetBundle("scene_" + scene.Split('_')[0].ToLower(CultureInfo.InvariantCulture));
	}

	public bool IsBundleLoaded(string bundle)
	{
		return loadedBundles.Contains(bundle);
	}

	public void UnloadQueued()
	{
		foreach (string item in queueToUnload)
		{
			UnloadAssetBundle(item);
		}
	}

	public void UnloadAssetBundle(List<string> assetBundles)
	{
		foreach (string assetBundle in assetBundles)
		{
			UnloadAssetBundle(assetBundle);
		}
	}

	public void UnloadAssetBundleWithRealDependencies(string assetBundle)
	{
		AssetBundleManager.Instance.UnloadAssetBundleWithRealDependencies(assetBundle, unloadAllAssets: false, delegate
		{
			loadedBundles.Remove(assetBundle);
			queueToUnload.Remove(assetBundle);
			Log(assetBundle + " asset bundle unloaded");
		});
	}

	public void UnloadAssetBundle(string assetBundle)
	{
		if (LoadingAssetBundles)
		{
			if (!queueToUnload.Contains(assetBundle))
			{
				queueToUnload.Add(assetBundle);
			}
			return;
		}
		AssetBundleManager.Instance.UnloadAssetBundle(assetBundle, unloadAllAssets: true, delegate
		{
			loadedBundles.Remove(assetBundle);
			queueToUnload.Remove(assetBundle);
			Log(assetBundle + " asset bundle unloaded");
		});
	}

	public void UnloadAssetBundleWithDependencies(string assetBundle)
	{
		UnloadAssetBundle(assetBundle);
		UnloadAssetBundle(assetBundle + "_dependencies");
	}

	public void UnloadCampOnlyAssetBundles()
	{
		UnloadAssetBundle((from t in assetBundleConfig.resources
			where t.UnloadInCombat
			select t.Identifier).ToList());
	}

	public void LoadCampOnlyAssetBundles()
	{
		List<string> allUnloadedBundlesAndDependencies = AssetBundleManager.Instance.GetAllUnloadedBundlesAndDependencies((from t in assetBundleConfig.resources
			where t.UnloadInCombat
			select t.Identifier).ToList());
		LoadAssetBundle(allUnloadedBundlesAndDependencies);
	}

	private void OnError(string obj)
	{
		Debug.LogError("AssetBundleController: " + obj);
		OfflineManager.Instance?.SetStreamingPath();
	}

	private void OnDownloadProgress(float value)
	{
		SingularityMonoBehaviour<LoadingScreenHUD>.Instance.AssetsDownloading(value);
	}


	#region myparams
	private static readonly List<string> bundlesListPro = new() { "abresource", "scriptableobjects", "prefabresources", "itemgraphics", "hudelements", "uilistitems", "modularcharacter" };
	private static readonly List<string> bundlesListLight = new() { "abresource", "scriptableobjects", "prefabresources" };
	#endregion

	#region mycode
	private void OnPathChanged()
	{
		AssetBundleManager.Instance.UnloadAllAssetBundles(true, delegate
		{
			AssetBundleManager.Instance.UnloadCollection();
			LoadAssetBundleCollection();
		});
	}
	#endregion
}
