using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace NextGames.Sdk.AssetBundleManager
{
	public class AssetBundleCollection
	{
		[SerializeField]
		private AssetBundleDescription[] _descriptions;

		[SerializeField]
		private RuntimePlatform _platform;

		[SerializeField]
		private string _commitHash;

		[SerializeField]
		private string _timestamap;

		public AssetBundleDescription[] Descriptions => _descriptions;

		public RuntimePlatform Platform
		{
			get
			{
				return _platform;
			}
			set
			{
				_platform = value;
			}
		}

		public string CommitHash
		{
			get
			{
				return _commitHash;
			}
			set
			{
				_commitHash = value;
			}
		}

		public string Timestamap
		{
			get
			{
				return _timestamap;
			}
			set
			{
				_timestamap = value;
			}
		}

		public AssetBundleCollection(AssetBundleDescription[] descriptions)
		{
			_descriptions = descriptions;
			_commitHash = string.Empty;
			_timestamap = string.Empty;
		}

		private AssetBundleCollection(AssetBundleCollection source)
		{
			_descriptions = source._descriptions.Select((AssetBundleDescription x) => x.DeepCopy()).ToArray();
			_platform = source._platform;
			_commitHash = source._commitHash;
			_timestamap = source._timestamap;
		}

		public AssetBundleCollection DeepCopy()
		{
			return new AssetBundleCollection(this);
		}

		public static IEnumerator DownloadCollection(string url, Action<AssetBundleCollection> success, Action<string> error, Action<float> progress = null)
		{
			UnityWebRequest request = UnityWebRequest.Get(url);
			if (progress == null)
			{
				yield return request.SendWebRequest();
			}
			else
			{
				request.SendWebRequest();
				progress(0f);
				while (!request.isDone)
				{
					yield return null;
					progress(request.downloadProgress);
				}
			}
			if (request.isHttpError || request.isNetworkError)
			{
				error?.Invoke($"Failed to download AssetBundleCollection from '{request.url}' with error '{request.error}')");
			}
			else
			{
				success?.Invoke(JsonUtility.FromJson<AssetBundleCollection>(request.downloadHandler.text));
			}
		}
	}
}