using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ContentPackManager : MonoBehaviour
{
	private ContentPackManifest contentPackManifest;

	private bool bundlesInLocalResources;

	private Dictionary<string, AssetBundle> loadedAssetBundlesDict = new Dictionary<string, AssetBundle>();

	private AsyncLoadingHandle manifestLoadingHandle = new AsyncLoadingHandle();

	public ContentPackManifest ContentPackManifest => contentPackManifest;

	public List<AssetBundle> LoadedDynamicAssetBundles { get; private set; }

	public static ContentPackManager Instance { get; private set; }

	public static bool IsInitialContentAvailable
	{
		get
		{
			if (Instance == null)
			{
				return false;
			}
			return Instance.loadedAssetBundlesDict.ContainsKey("CP0");
		}
	}

	public AsyncLoadingHandle ManifestLoadingHandle => manifestLoadingHandle;

	public ContentPackManager()
	{
		LoadedDynamicAssetBundles = new List<AssetBundle>();
	}

	private void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	public AssetBundle GetContentPack(string contentPackName)
	{
		if (!loadedAssetBundlesDict.ContainsKey(contentPackName))
		{
			return null;
		}
		return loadedAssetBundlesDict[contentPackName];
	}

	public static string GetManifestName(RuntimePlatform targetPlatform, string assetVersion)
	{
		SHA1CryptoServiceProvider sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();
		string arg = targetPlatform.ToString();
		string text = $"TWD_{arg}_{GameManager.ClientVersion}_{assetVersion}";
		byte[] array = new byte[text.Length * 2];
		Buffer.BlockCopy(text.ToCharArray(), 0, array, 0, array.Length);
		sHA1CryptoServiceProvider.TransformFinalBlock(array, 0, array.Length);
		return BitConverter.ToString(sHA1CryptoServiceProvider.Hash).Replace("-", "");
	}

	public AsyncLoadingHandle DownloadContentPack(string contentPackName)
	{
		if (contentPackManifest == null)
		{
			return null;
		}
		AsyncLoadingHandle asyncLoadingHandle = new AsyncLoadingHandle();
		if (!loadedAssetBundlesDict.ContainsKey(contentPackName))
		{
			StartCoroutine(DownloadContentPackProcess(contentPackName, asyncLoadingHandle));
		}
		else
		{
			asyncLoadingHandle.SignalFinished(null);
		}
		return asyncLoadingHandle;
	}

	private IEnumerator DownloadContentPackProcess(string contentPackName, AsyncLoadingHandle loadingHandle)
	{
		ContentPack replacement = null;
		foreach (ContentPack contentPack in contentPackManifest.ContentPacks)
		{
			if (contentPack.OriginalName == contentPackName)
			{
				replacement = contentPack;
				break;
			}
		}
		if (replacement == null)
		{
			string errorMessage = $"The content pack with the name \"{contentPackName}\" is not available.";
			loadingHandle.SignalFinished(errorMessage);
			yield break;
		}
		if (bundlesInLocalResources)
		{
			TextAsset textAsset = (TextAsset)Resources.Load(replacement.ReplacementName);
			_ = textAsset == null;
			TextAsset obj = (TextAsset)Resources.Load(replacement.ReplacementName + "_scenes");
			_ = obj == null;
			AssetBundle assetBundle = AssetBundle.LoadFromMemory(textAsset.bytes);
			_ = assetBundle == null;
			AssetBundle assetBundle2 = AssetBundle.LoadFromMemory(obj.bytes);
			_ = assetBundle2 == null;
			loadedAssetBundlesDict.Add(contentPackName, assetBundle);
			loadedAssetBundlesDict.Add(contentPackName + "_scenes", assetBundle2);
			LoadedDynamicAssetBundles.Add(assetBundle);
			loadingHandle.SignalFinished(null);
			yield break;
		}
		string contentPackUrl = CdnUrlHelper.RewriteCdnUrl(GameManager.Instance.gameEconomyData.ConfigData.CDNBaseUrl + "/assets/" + replacement.ReplacementName);
		WWW contentPackWWW = WWW.LoadFromCacheOrDownload(contentPackUrl, 0, replacement.BundleCRC);
		while (!contentPackWWW.isDone)
		{
			yield return null;
			loadingHandle.ReportProgress(contentPackWWW.progress * 0.5f);
		}
		if (!string.IsNullOrEmpty(contentPackWWW.error))
		{
			string errorMessage2 = $"Failed to download content pack \"{contentPackUrl}\": {contentPackWWW.error}";
			loadingHandle.SignalFinished(errorMessage2);
			yield break;
		}
		WWW streamedContentPackWWW = WWW.LoadFromCacheOrDownload(contentPackUrl + "_scenes", 0, replacement.ScenesCRC);
		while (!streamedContentPackWWW.isDone)
		{
			yield return null;
			loadingHandle.ReportProgress(streamedContentPackWWW.progress * 0.5f + 0.5f);
		}
		if (!string.IsNullOrEmpty(streamedContentPackWWW.error))
		{
			string errorMessage3 = string.Format("Failed to download scene content pack \"{0}\": {1}", contentPackUrl + "_scenes", streamedContentPackWWW.error);
			loadingHandle.SignalFinished(errorMessage3);
			yield break;
		}
		AssetBundle assetBundle3 = contentPackWWW.assetBundle;
		AssetBundle assetBundle4 = streamedContentPackWWW.assetBundle;
		loadedAssetBundlesDict.Add(contentPackName, assetBundle3);
		loadedAssetBundlesDict.Add(contentPackName + "_scenes", assetBundle4);
		LoadedDynamicAssetBundles.Add(assetBundle3);
		loadingHandle.SignalFinished(null);
	}
}
