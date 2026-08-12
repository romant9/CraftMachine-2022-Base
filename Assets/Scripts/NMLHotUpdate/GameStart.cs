using System.Collections;
using System.Collections.Generic;
using NextGames.Sdk.AssetBundleManager;
using UnityEngine;

public class GameStart
{
	private static MonoBehaviour m_Mb;

	public static string hotfixVersion;

	public static void Execute(MonoBehaviour mb, Dictionary<string, string> fileName2Md5Dict, string hotfixVersion)
	{
		if (hotfixVersion != null)
		{
			GameStart.hotfixVersion = hotfixVersion.Replace(hotfixVersion[..hotfixVersion.LastIndexOf('.')], OfflineManager.ShortVersion);
		}
		else
		{
			GameStart.hotfixVersion = OfflineManager.ShortVersion;
		}
		new GameObject("AssetBundleManager").AddComponent<AssetBundleManager>().LoadAllAssetBundlesFromStreamingAssets = true;
		AssetBundleController.fileName2Md5Dict = fileName2Md5Dict;
		new GameObject("AssetBundleController").AddComponent<AssetBundleController>();
		m_Mb = mb;
		IEnumerator container = null;
		Helpers.StartCoroutine(m_Mb, LoadPreloadAssetBundles(), ref container);
	}

	private static IEnumerator LoadPreloadAssetBundles()
	{
		yield return new WaitForSeconds(1f);
		IEnumerator coroutine = null;
		while (SingularityMonoBehaviour<AssetBundleController>.Instance.LoadingAssetBundles)
		{
			yield return null;
		}
		Helpers.StartCoroutine(m_Mb, DelayToSwitchScene(), ref coroutine);
	}

	private static IEnumerator DelayToSwitchScene()
	{
		yield return new WaitForSeconds(1f);
		string assetbundleName = "scene_gameloader";
		SingularityMonoBehaviour<AssetBundleController>.Instance.LoadAssetBundle(assetbundleName);
		while (SingularityMonoBehaviour<AssetBundleController>.Instance.LoadingAssetBundles)
		{
			yield return null;
		}
		yield return null;
		string sceneName = "GameLoader";
		AssetBundleManager.Instance.LoadScene(sceneName);
	}
}
