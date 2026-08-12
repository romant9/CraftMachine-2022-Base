using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace NextGames.Sdk.AssetBundleManager
{
	public class AssetBundleHandler
	{
		private const int c_maxRetryCount = 3;

		private readonly MonoBehaviour _coroutineHandler;

		private readonly AssetBundleDescription _description;

		private string _Md5;

		private AssetBundle _assetBundle;

		private AssetBundleState _state;

		private int _currentRetry;

		private StringBuilder _errorBuilder;

		private long _timestamp;

		private float _timeoutDuration;

		private ulong _downloadedBytes;

		public AssetBundleState AssetBundleState => _state;

		public bool IsAssetExisted(string assetName)
		{
			return _description.AssetNames.Contains(assetName);
		}

		public AssetBundle AssetBundle
		{
			get
			{
				if (_assetBundle == null)
				{
					return null;
					//throw new NullReferenceException("Assetbundle " + ((_description != null) ? _description.Name : "(NULL)") + " is not yet available. AssetBundle state: " + _state);
				}
				return _assetBundle;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return "Assetbundle " + ((_description != null) ? _description.Name : "(NULL)") + " is not yet available. AssetBundle state: " + _state;
			}
		}

		public long AssetBundleSize => _description.Size;

		public int AssetBundlePriority => _description.Priority;

		public string AssetBundleName => _description.Name;

		public bool IsAssetBundleLoaded => _assetBundle != null;

		public bool Cached
		{
			get
			{
				if (_description.InStreamingAssets)
				{
					return true;
				}

				if (string.IsNullOrEmpty(_description.Url))
				{
					throw new Exception("Non-local assetbundle " + _description.FullName + "must have valid url.");
				}

				return Caching.IsVersionCached(_description.Url, Hash128.Parse(_description.VersionHash));
			}
		}

		private event Action<AssetBundleHandler> _success;

		private event Action<AssetBundleHandler, string> _error;

		private event Action<AssetBundleHandler, float> _progress;

		private event Action<AssetBundleHandler> _internalUnloaded;

		public void SetTimeoutDuration(float duration)
		{
			if (AssetBundleState == AssetBundleState.Empty)
			{
				_timeoutDuration = duration;
			}
		}

		public AssetBundleHandler(AssetBundleDescription description, MonoBehaviour coroutineHandler)
		{
			_description = description;
			_coroutineHandler = coroutineHandler;
		}

		public AssetBundleHandler(AssetBundleDescription description, string md5, MonoBehaviour coroutineHandler)
		{
			_description = description;
			_Md5 = md5;
			_coroutineHandler = coroutineHandler;
		}

		private string CreateUri()
		{
			if (_description.InStreamingAssets)
			{
				if (!AssetBundleManager.IsMd5Bundles)
				{
					return string.Format("file://{0}/{1}/{2}", AssetBundleManager.StreamingAssetsPath, "AssetBundles", _description.FullName).Replace('\\', '/');
				}
				else
				{
					string text = "AssetBundles/" + _description.FullName;
					string text2 = ((text.LastIndexOf(".") != -1) ? text.Insert(text.LastIndexOf("."), "." + _Md5) : text.Insert(text.LastIndexOf("/") + 1, _Md5 + "."));
					if (File.Exists(Application.persistentDataPath + "/GameAssets/" + text2))
					{
						return "file://" + Application.persistentDataPath + "/GameAssets/" + text2;
					}
					return "file://" + Application.streamingAssetsPath + "/" + text2;
				}
			}
			return _description.Url;
		}

		public void LoadAssetBundle()
		{
			if (_state == AssetBundleState.Empty || _state == AssetBundleState.Error)
			{
				_currentRetry = 0;
				_errorBuilder = new StringBuilder();
				_state = AssetBundleState.Downloading;
				_coroutineHandler.StartCoroutine(LoadAssetBundleInternal());
			}
			else if (_state == AssetBundleState.Aborting)
			{
				_state = AssetBundleState.Downloading;
			}
			else if (_state == AssetBundleState.Unloading)
			{
				Debug.LogWarningFormat("Starting to download AssetBundle '{0}' immediately after it has been unloaded", _description.Name);
				_internalUnloaded += ReDownloadAfterUnload;
			}
			else if (_state == AssetBundleState.Downloaded)
			{
				Debug.LogWarningFormat("Requested loading of AssetBundle '{0}' but it was alread loaded", _description.Name);
			}
		}

		public void HookEvents(Action<AssetBundleHandler> success, Action<AssetBundleHandler, string> error, Action<AssetBundleHandler, float> progress)
		{
			if (success != null)
			{
				_success += success;
			}

			if (error != null)
			{
				_error += error;
			}

			if (progress != null)
			{
				_progress += progress;
			}
		}

		public void UnhookEvents(Action<AssetBundleHandler> success, Action<AssetBundleHandler, string> error, Action<AssetBundleHandler, float> progress)
		{
			if (success != null)
			{
				_success -= success;
			}

			if (error != null)
			{
				_error -= error;
			}

			if (progress != null)
			{
				_progress -= progress;
			}
		}

		public void CancelLoadAssetBundle()
		{
			if (_state == AssetBundleState.Downloading)
			{
				_state = AssetBundleState.Aborting;
			}
		}

		private IEnumerator LoadAssetBundleInternal()
		{
			_state = AssetBundleState.Downloading;
			using UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(CreateUri());
			request.SendWebRequest();
			if (_progress != null)
			{
				_progress(this, 0f);
			}

			_timestamp = DateTime.UtcNow.Ticks;
			while (!request.isDone)
			{
				yield return null;
				if (_downloadedBytes != request.downloadedBytes)
				{
					_downloadedBytes = request.downloadedBytes;
					_timestamp = DateTime.UtcNow.Ticks;
				}
				if (_state == AssetBundleState.Aborting)
				{
					request.Abort();
					_state = AssetBundleState.Empty;
					yield break;
				}

				if (_progress != null && request.downloadHandler != null)
				{
					_progress(this, Mathf.Clamp01(request.downloadProgress));
				}
			}
			if (request.isHttpError || request.isNetworkError)
			{
				yield return RetryRequest(request);
				yield break;
			}

			_assetBundle = DownloadHandlerAssetBundle.GetContent(request);
			if (_assetBundle == null)
			{
				_state = AssetBundleState.Error;
				if (_error != null)
				{
					_error(this, $"Failed to parse AssetBundle from download '{request.url}'");
				}
			}
			else
			{
				_state = AssetBundleState.Downloaded;
				if (_success != null)
				{
					_success(this);
				}
			}
		}

		private IEnumerator RetryRequest(UnityWebRequest request)
		{
			request.Abort();
			_errorBuilder.AppendLine("retry " + _currentRetry + ", error: " + request.error);
			if (_currentRetry >= 3)
			{
				_state = AssetBundleState.Error;
				if (_error != null)
				{
					_error(this, $"Failed to download AssetBundle from '{request.url}' with error(s) {_errorBuilder}");
				}
			}
			else
			{
				_currentRetry++;
				yield return null;
				yield return LoadAssetBundleInternal();
			}
		}

		public void UnloadAssetBundle(bool unloadAllAssets, Action unloaded)
		{
			if (_assetBundle != null)
			{
				_coroutineHandler.StartCoroutine(UnloadAssetBundleInternal(unloadAllAssets, unloaded));
			}
		}

		private IEnumerator UnloadAssetBundleInternal(bool unloadAllAssets, Action unloaded)
		{
			_state = AssetBundleState.Unloading;
			_assetBundle.Unload(unloadAllAssets);
			if (unloadAllAssets)
			{
				yield return Resources.UnloadUnusedAssets();
			}

			_state = AssetBundleState.Empty;
			if (_internalUnloaded != null)
			{
				_internalUnloaded(this);
			}

			unloaded?.Invoke();
		}

		private void ReDownloadAfterUnload(AssetBundleHandler handler)
		{
			_internalUnloaded -= ReDownloadAfterUnload;
			LoadAssetBundle();
		}
	}
}