using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NextGames.Sdk.AssetBundleManager
{
	public class AssetBundleManager : MonoBehaviour
	{
		private class MultiAssetBundleDownloader
		{
			private static readonly Dictionary<AssetBundleHandler, int> s_downloaders = new Dictionary<AssetBundleHandler, int>();

			private readonly Dictionary<AssetBundleHandler, float> _progressDict;

			private readonly AssetBundleHandler[] _handlers;

			private readonly Action _success;

			private readonly Action<string> _error;

			private readonly Action<float> _progress;

			private readonly long _totalSize;

			private readonly float _timeoutDuration;

			private int _maxConcurrentDownloads;

			private int UpdateIndex
			{
				get
				{
					for (int num = _handlers.Length - 1; num >= 0; num--)
					{
						if (_handlers[num].AssetBundleState == AssetBundleState.Downloading)
						{
							return num;
						}
					}
					return _handlers.Length - 1;
				}
			}

			public MultiAssetBundleDownloader(AssetBundleHandler[] handlers, Action success, Action<string> error, Action<float> progress, int maxConcurrentDownloads = -1, float timeoutDuration = 10f)
			{
				_handlers = handlers;
				_success = success;
				_error = error;
				_progress = progress;
				_totalSize = 0L;
				_timeoutDuration = timeoutDuration;
				_maxConcurrentDownloads = maxConcurrentDownloads;
				if (progress != null)
				{
					_progressDict = new Dictionary<AssetBundleHandler, float>(handlers.Length);
				}
				AssignDownloaders();
				HookEvents();
				for (int i = 0; i < handlers.Length; i++)
				{
					if (progress != null)
					{
						_progressDict.Add(handlers[i], 0f);
					}
					_totalSize += handlers[i].AssetBundleSize;
				}
				if (_maxConcurrentDownloads == -1)
				{
					_maxConcurrentDownloads = handlers.Length;
				}
				if (handlers.Length < _maxConcurrentDownloads)
				{
					_maxConcurrentDownloads = handlers.Length;
				}
				for (int j = 0; j < _maxConcurrentDownloads; j++)
				{
					handlers[j].SetTimeoutDuration(_timeoutDuration);
					handlers[j].LoadAssetBundle();
				}
			}

			private void HookEvents()
			{
				for (int i = 0; i < _handlers.Length; i++)
				{
					if (_progress == null)
					{
						_handlers[i].HookEvents(OnSuccess, OnError, null);
					}
					else
					{
						_handlers[i].HookEvents(OnSuccess, OnError, OnProgress);
					}
				}
			}

			private void UnhookEvents()
			{
				for (int i = 0; i < _handlers.Length; i++)
				{
					if (_progress == null)
					{
						_handlers[i].UnhookEvents(OnSuccess, OnError, null);
					}
					else
					{
						_handlers[i].UnhookEvents(OnSuccess, OnError, OnProgress);
					}
				}
			}

			private void AssignDownloaders()
			{
				for (int i = 0; i < _handlers.Length; i++)
				{
					if (s_downloaders.ContainsKey(_handlers[i]))
					{
						s_downloaders[_handlers[i]]++;
					}
					else
					{
						s_downloaders.Add(_handlers[i], 1);
					}
				}
			}

			private void UnassignDownloaders()
			{
				for (int i = 0; i < _handlers.Length; i++)
				{
					s_downloaders[_handlers[i]]--;
				}
			}

			private void CancelUnassignedDownloadHandlers()
			{
				for (int i = 0; i < _handlers.Length; i++)
				{
					if (s_downloaders[_handlers[i]] == 0)
					{
						_handlers[i].CancelLoadAssetBundle();
						s_downloaders.Remove(_handlers[i]);
					}
				}
			}

			private void OnProgress(AssetBundleHandler handler, float progress)
			{
				_progressDict[handler] = progress;
				if (handler == _handlers[UpdateIndex] && _progress != null)
				{
					_progress(CalculateProgress());
				}
			}

			private void OnSuccess(AssetBundleHandler handler)
			{
				if (AllFinished())
				{
					UnassignDownloaders();
					UnhookEvents();
				}
				else
				{
					for (int i = 0; i < _handlers.Length; i++)
					{
						if (_handlers[i].AssetBundleState == AssetBundleState.Empty && CurrentDownloads() < _maxConcurrentDownloads)
						{
							_handlers[i].SetTimeoutDuration(_timeoutDuration);
							_handlers[i].LoadAssetBundle();
						}
					}
				}
				if (_success != null && AllFinished())
				{
					_success();
				}
			}

			private void OnError(AssetBundleHandler handler, string error)
			{
				UnassignDownloaders();
				UnhookEvents();
				CancelUnassignedDownloadHandlers();
				if (_error != null)
				{
					_error(error);
				}
			}

			private bool AllFinished()
			{
				return _handlers.All((AssetBundleHandler x) => x.AssetBundleState == AssetBundleState.Downloaded);
			}

			private int CurrentDownloads()
			{
				return _handlers.Count((AssetBundleHandler x) => x.AssetBundleState == AssetBundleState.Downloading);
			}

			private float CalculateProgress()
			{
				long num = 0L;
				for (int i = 0; i < _handlers.Length; i++)
				{
					num += DownloadAmountForAssetBundle(_handlers[i]);
				}
				return Convert.ToSingle((double)num / (double)_totalSize);
			}

			private long DownloadAmountForAssetBundle(AssetBundleHandler handler)
			{
				return Convert.ToInt64((double)_progressDict[handler] * (double)handler.AssetBundleSize);
			}
		}

		private class MultiAssetBundleUnloader
		{
			private readonly bool[] _handlerUnloaded;

			private readonly Action _unloaded;

			public MultiAssetBundleUnloader(AssetBundleHandler[] handlers, bool unloadAllAssets, Action unloaded)
			{
				_handlerUnloaded = new bool[handlers.Length];
				_unloaded = unloaded;
				for (int i = 0; i < handlers.Length; i++)
				{
					int index = i;
					handlers[i].UnloadAssetBundle(unloadAllAssets, delegate
					{
						UnloadedAssetBundleAtIndex(index);
					});
				}
			}

			private void UnloadedAssetBundleAtIndex(int index)
			{
				_handlerUnloaded[index] = true;
				if (_handlerUnloaded.All((bool value) => value) && _unloaded != null)
				{
					_unloaded();
				}
			}
		}

		private readonly Dictionary<string, AssetBundleHandler> _currentHandlers = new Dictionary<string, AssetBundleHandler>();

		private readonly Dictionary<string, Dictionary<string, AssetBundleHandler>> _availableVariants = new Dictionary<string, Dictionary<string, AssetBundleHandler>>();

		private readonly Dictionary<AssetBundleHandler, string[]> _dependencies = new Dictionary<AssetBundleHandler, string[]>();

		[SerializeField]
		public bool LoadAllAssetBundlesFromStreamingAssets;

		[Tooltip("Duration used to check if bundle download hasn't progressed further during the time (bad connection, in example).")]
		[SerializeField]
		private float _timeoutDuration = 10f;

		private static AssetBundleManager instance;

		public static AssetBundleManager Instance => instance;

		private void Awake()
		{
			if (instance != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			instance = this;
			if (base.transform == base.transform.root)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		public void LoadCollection(AssetBundleCollection collection, Dictionary<string, string> fileName2Md5Dict = null)
		{
			AssetBundleDescription[] descriptions = collection.Descriptions;
			foreach (AssetBundleDescription assetBundleDescription in descriptions)
			{
				if (LoadAllAssetBundlesFromStreamingAssets)
				{
					assetBundleDescription.InStreamingAssets = true;
				}
				AssetBundleHandler assetBundleHandler = IsMd5Bundles ? new AssetBundleHandler(assetBundleDescription, fileName2Md5Dict?["AssetBundles/" + assetBundleDescription.FullName], this)
				: new AssetBundleHandler(assetBundleDescription, this);
				if (!_availableVariants.ContainsKey(assetBundleDescription.Name))
				{
					_availableVariants.Add(assetBundleDescription.Name, new Dictionary<string, AssetBundleHandler>());
				}
				_availableVariants[assetBundleDescription.Name].Add(assetBundleDescription.Variant, assetBundleHandler);
				_dependencies.Add(assetBundleHandler, assetBundleDescription.Dependencies);
			}
			foreach (KeyValuePair<string, Dictionary<string, AssetBundleHandler>> availableVariant in _availableVariants)
			{
				AssetBundleHandler value = availableVariant.Value.ElementAt(0).Value;
				_currentHandlers.Add(availableVariant.Key, value);
			}
		}

		public bool IsAssetExistedInAssetBundle(string assetBundle, string assetName)
		{
			return _currentHandlers.ContainsKey(assetBundle) && _currentHandlers[assetBundle].IsAssetExisted(assetName);
		}

		public bool HasActiveHandlers()
		{
			return (from x in _currentHandlers.Values.Distinct()
				where x.AssetBundleState == AssetBundleState.Downloading || x.AssetBundleState == AssetBundleState.Aborting || x.AssetBundleState == AssetBundleState.Unloading
				select x).ToArray().Length > 0;
		}

		public void UnloadCollection()
		{
			if ((from x in _currentHandlers.Values.Distinct()
				where x.AssetBundleState != AssetBundleState.Empty
				where x.AssetBundleState != AssetBundleState.Error
				select x).ToArray().Length != 0)
			{
				throw new InvalidOperationException("Cannot unload collection, make sure that all AssetBundles has been unloaded and no active downloads or unloads are going on when calling this method");
			}
			_currentHandlers.Clear();
			_availableVariants.Clear();
			_dependencies.Clear();
		}

		public long GetDownloadSizeForAllAssets()
		{
			AssetBundleHandler[] array = _currentHandlers.Values.ToArray();
			return (from x in (from x in array.SelectMany((AssetBundleHandler x) => _dependencies[x])
					select _currentHandlers[x]).Concat(array).Distinct()
				where x.AssetBundleState != AssetBundleState.Downloaded
				where !x.Cached
				select x).Sum((AssetBundleHandler x) => x.AssetBundleSize);
		}

		public long GetDownloadSizeWhere(Func<AssetBundleHandler, bool> func)
		{
			return (from x in _currentHandlers.Values.Where(func)
				where x.AssetBundleState != AssetBundleState.Downloaded
				where !x.Cached
				select x).Sum((AssetBundleHandler x) => x.AssetBundleSize);
		}

		public void SetAssetBundleVariant(string assetBundle, string variant)
		{
			_currentHandlers[assetBundle] = _availableVariants[assetBundle][variant];
		}

		public bool HasCachedAssetBundlesWhere(Func<AssetBundleHandler, bool> func)
		{
			AssetBundleHandler[] array = _currentHandlers.Values.Where(func).ToArray();
			if (array.Length == 0)
			{
				Debug.LogWarning("Couldn't find any AssetBundle matching search function!");
				return false;
			}
			return (from x in array.SelectMany((AssetBundleHandler x) => _dependencies[x])
				select _currentHandlers[x]).Concat(array).Distinct().All((AssetBundleHandler x) => x.Cached);
		}

		public void DownloadAllAssetBundles(Action success, Action<string> error, Action<float> progress, int maxConcurrentDownload = -1)
		{
			AssetBundleHandler[] array = PrepHandlersToDownload();
			if (array.Length == 0)
			{
				success?.Invoke();
			}
			else
			{
				new MultiAssetBundleDownloader(array, success, error, progress, maxConcurrentDownload, _timeoutDuration);
			}
		}

		public AssetBundleHandler[] PrepHandlersToDownload()
		{
			return (from x in _currentHandlers.Values
				where x.AssetBundleState != AssetBundleState.Downloaded
				where !x.Cached
				select x).ToArray();
		}

		public List<string> LoadedAssetBundles()
		{
			return (from t in _currentHandlers.Values
				where t.IsAssetBundleLoaded
				select t.AssetBundleName).ToList();
		}

		public bool IsAssetBundleDownloading(string bundleName)
		{
			if (_currentHandlers.ContainsKey(bundleName) && _currentHandlers[bundleName].AssetBundleState == AssetBundleState.Downloading)
			{
				return true;
			}
			return false;
		}

		public void DownloadAssetBundlesWhere(Func<AssetBundleHandler, bool> func, Action success, Action<string> error, Action<float> progress, int maxConcurrentDownload = -1)
		{
			AssetBundleHandler[] array = _currentHandlers.Values.Where(func).ToArray();
			if (array.Length == 0)
			{
				Debug.LogWarning("Couldn't find any AssetBundle matching search function!");
				return;
			}
			array = (from x in (from x in array.SelectMany((AssetBundleHandler x) => _dependencies[x])
					select _currentHandlers[x]).Concat(array).Distinct()
				where x.AssetBundleState != AssetBundleState.Downloaded
				select x).ToArray();
			if (array.Length == 0)
			{
				success?.Invoke();
			}
			else
			{
				new MultiAssetBundleDownloader(array, success, error, progress, maxConcurrentDownload, _timeoutDuration);
			}
		}

		public void DownloadAssetBundle(string assetBundle, Action success, Action<string> error, Action<float> progress)
		{
			AssetBundleHandler[] array = (from x in _dependencies[_currentHandlers[assetBundle]].Select((string x) => _currentHandlers[x]).Concat(new AssetBundleHandler[1] { _currentHandlers[assetBundle] })
				where x.AssetBundleState != AssetBundleState.Downloaded
				select x).ToArray();
			if (array.Length == 0)
			{
				success?.Invoke();
			}
			else
			{
				new MultiAssetBundleDownloader(array, success, error, progress, -1, _timeoutDuration);
			}
		}

		public void LoadAssetBundle(string assetBundle, Action success, Action<string> error, Action<float> progress)
		{
			if (_currentHandlers[assetBundle].IsAssetBundleLoaded)
			{
				success?.Invoke();
				return;
			}
			if (_currentHandlers[assetBundle].AssetBundleState != AssetBundleState.Downloaded && !_currentHandlers[assetBundle].Cached)
			{
				error("Assetbundle " + assetBundle + " is not downloaded or cached");
				return;
			}
			AssetBundleHandler[] array = new AssetBundleHandler[1] { _currentHandlers[assetBundle] };
			if (array.Length == 0)
			{
				success?.Invoke();
			}
			else
			{
				new MultiAssetBundleDownloader(array, success, error, progress, -1, _timeoutDuration);
			}
		}

		public void LoadAssetBundles(List<string> assetBundles, Action success, Action<string> error, Action<float> progress)
		{
			AssetBundleHandler[] array = _currentHandlers.Values.Where((AssetBundleHandler x) => assetBundles.Contains(x.AssetBundleName)).ToArray();
			array = (from x in (from x in array.SelectMany((AssetBundleHandler x) => _dependencies[x])
					select _currentHandlers[x]).Concat(array).Distinct()
				where (x.AssetBundleState == AssetBundleState.Downloaded || x.Cached) && !x.IsAssetBundleLoaded
				select x).ToArray();
			if (array.Length == 0)
			{
				success?.Invoke();
			}
			else
			{
				new MultiAssetBundleDownloader(array, success, error, progress, -1, _timeoutDuration);
			}
		}
		public List<string> GetAllUnloadedBundlesAndDependencies(List<string> assetBundles)
		{
			AssetBundleHandler[] array = _currentHandlers.Values.Where((AssetBundleHandler x) => assetBundles.Contains(x.AssetBundleName)).ToArray();
			array = (from x in (from x in array.SelectMany((AssetBundleHandler x) => _dependencies[x])
					select _currentHandlers[x]).Concat(array).Distinct()
				where !x.IsAssetBundleLoaded && x.AssetBundleState != AssetBundleState.Downloading
				select x).ToArray();
			return array.Select((AssetBundleHandler x) => x.AssetBundleName).ToList();
		}

		public void CancelAllAssetBundleDownloads()
		{
			foreach (KeyValuePair<string, AssetBundleHandler> currentHandler in _currentHandlers)
			{
				currentHandler.Value.CancelLoadAssetBundle();
			}
		}

		public void UnloadAssetBundleWithRealDependencies(string assetBundle, bool unloadAllAssets, Action unloaded)
		{
			List<AssetBundleHandler> handlers = new List<AssetBundleHandler> { _currentHandlers[assetBundle] };
			foreach (string dependency in _dependencies[_currentHandlers[assetBundle]])
			{
				handlers.Add(_currentHandlers[dependency]);
			}
			new MultiAssetBundleUnloader(handlers.ToArray(), unloadAllAssets, unloaded);
		}

		public void UnloadAssetBundle(string assetBundle, bool unloadAllAssets, Action unloaded)
		{
			_currentHandlers[assetBundle].UnloadAssetBundle(unloadAllAssets, unloaded);
		}

		public void UnloadAllAssetBundles(bool unloadAllAssets, Action unloaded)
		{
			AssetBundleHandler[] array = _currentHandlers.Values.Where((AssetBundleHandler x) => x.AssetBundleState == AssetBundleState.Downloaded).ToArray();
			if (array.Length == 0)
			{
				unloaded?.Invoke();
			}
			else
			{
				new MultiAssetBundleUnloader(array, unloadAllAssets, unloaded);
			}
		}

		public UnityEngine.Object LoadAsset(string assetName, string bundleName)
		{
			return _currentHandlers[bundleName].AssetBundle.LoadAsset(assetName);
		}

		public T LoadAsset<T>(string assetName, string bundleName, bool IsCustom = false) where T : UnityEngine.Object
		{
			if (IsLoadFromResources || IsCustom)
			{
				if (assetName.Contains("_Apo1")) assetName = assetName.Replace("_Apo1", "");
				var obj = Resources.Load<T>(bundleName + '/' + assetName);

				if (!obj)
				{
					obj = Resources.Load<T>("AssetBundles/" + bundleName + '/' + assetName);
				}
				if (obj != null) return obj;
			}

			try
			{
				var handler = _currentHandlers[bundleName];
				if (handler.AssetBundle == null)
				{
					Debug.LogError(handler.ErrorMessage + " for asset " + assetName + " in bundle " + bundleName);
				}
				return handler.AssetBundle.LoadAsset<T>(assetName);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message + " for asset " + assetName + " in bundle " + bundleName);
				return null;
			}
		}

		public AssetRequest LoadAssetAsync(string assetName, string bundleName)
		{
			return new AssetRequest(_currentHandlers[bundleName].AssetBundle.LoadAssetAsync(assetName));
		}

		public AssetRequest LoadAssetAsync<T>(string assetName, string bundleName) where T : UnityEngine.Object
		{
			return new AssetRequest(_currentHandlers[bundleName].AssetBundle.LoadAssetAsync<T>(assetName));
		}

		public void LoadAssetAsync<T>(string assetName, string bundleName, Action<T> callback) where T : UnityEngine.Object
		{
			AssetRequest assetRequest = null;
			if (assetRequest == null)
			{
				assetRequest = new AssetRequest(_currentHandlers[bundleName].AssetBundle.LoadAssetAsync<T>(assetName));
			}
			StartCoroutine(CoLoadAssetAsync(assetRequest, callback));
		}

		private IEnumerator CoLoadAssetAsync<T>(AssetRequest assetRequest, Action<T> callback) where T : UnityEngine.Object
		{
			while (!assetRequest.IsDone)
			{
				yield return null;
			}
			callback?.Invoke(assetRequest.Asset as T);
		}

		public UnityEngine.Object[] LoadAllAssets(string bundleName)
		{
			return _currentHandlers[bundleName].AssetBundle.LoadAllAssets();
		}

		public T[] LoadAllAssets<T>(string bundleName) where T : UnityEngine.Object
		{
			return _currentHandlers[bundleName].AssetBundle.LoadAllAssets<T>();
		}

		public AssetRequest LoadAllAssetsAsync(string bundleName)
		{
			return new AssetRequest(_currentHandlers[bundleName].AssetBundle.LoadAllAssetsAsync());
		}

		public AssetRequest LoadAllAssetsAsync<T>(string bundleName) where T : UnityEngine.Object
		{
			return new AssetRequest(_currentHandlers[bundleName].AssetBundle.LoadAllAssetsAsync<T>());
		}

		public UnityEngine.Object[] LoadAssetWithSubAssets(string assetName, string bundleName)
		{
			return _currentHandlers[bundleName].AssetBundle.LoadAssetWithSubAssets(assetName);
		}

		public T[] LoadAssetWithSubAssets<T>(string assetName, string bundleName) where T : UnityEngine.Object
		{
			return _currentHandlers[bundleName].AssetBundle.LoadAssetWithSubAssets<T>(assetName);
		}

		public AssetRequest LoadAssetWithSubAssetsAsync(string assetName, string bundleName)
		{
			return new AssetRequest(_currentHandlers[bundleName].AssetBundle.LoadAssetWithSubAssetsAsync(assetName));
		}

		public AssetRequest LoadAssetWithSubAssetsAsync<T>(string assetName, string bundleName) where T : UnityEngine.Object
		{
			return new AssetRequest(_currentHandlers[bundleName].AssetBundle.LoadAssetWithSubAssetsAsync<T>(assetName));
		}

		public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
		{
			SceneManager.LoadScene(sceneName, mode);
		}

		public AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode)
		{
			return SceneManager.LoadSceneAsync(sceneName, mode);
		}


		#region myparams
		public static bool IsLoadDataManager = false; //не используем
		public static bool IsLoadModular = false; //не используем
		public static string StreamingAssetsPath { get { return customStreamingPath.ToLower().Contains("streamingassets") ? customStreamingPath : Application.streamingAssetsPath; } set { customStreamingPath = value; } }

		private static string customStreamingPath = Application.streamingAssetsPath;

		public static bool IsMd5Bundles = true; //линк OfflineManager
		public static bool IsLoadFromResources = false; //линк OfflineManager

        #endregion

        #region mycode
        public bool IsAssetBundleEmpty(string bundleName)
        {
            if (_currentHandlers.ContainsKey(bundleName) && _currentHandlers[bundleName].AssetBundleState == AssetBundleState.Empty)
            {
                return true;
            }

            return false;
        }

        public void LoadAssetBundlesOnly(List<string> assetBundles, Action success, Action<string> error, Action<float> progress)
        {
            AssetBundleHandler[] array = _currentHandlers.Values.Where((AssetBundleHandler x) => assetBundles.Contains(x.AssetBundleName)).ToArray();

            if (array.Length == 0)
            {
                success?.Invoke();
            }
            else
            {
                new MultiAssetBundleDownloader(array, success, error, progress, -1, _timeoutDuration);
            }
        }

        public UnityEngine.Object LoadAssetResponse(string assetName, string bundleName)
        {
            return _currentHandlers[bundleName].AssetBundle.LoadAsset(assetName);
        }

        public static void SetSettings(string gameMod)
		{
			switch (gameMod)
			{
				case "Pro":
					{
						IsLoadDataManager = true;
						IsLoadModular = true;
						IsLoadFromResources = false;
					}
					break;
				case "Game":
					{
						IsLoadDataManager = false;
						IsLoadModular = true;
						IsLoadFromResources = false;
					}
					break;
				default:
					{
						IsLoadDataManager = true;
						IsLoadModular = false;
						IsLoadFromResources = true;
					}
					break;
			}
		}
		#endregion
	}
}