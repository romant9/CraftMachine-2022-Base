using System;
using System.Collections.Generic;
using UnityEngine;

namespace NextGames.Sdk.AssetBundleManager
{
	[Serializable]
	public class AssetBundleDescription
	{
		[SerializeField]
		private uint _bundleCrc;

		[SerializeField]
		private string _versionHash;

		[SerializeField]
		private string _assetsHash;

		[SerializeField]
		private string _name;

		[SerializeField]
		private List<string> _objectIds;

		[SerializeField]
		private List<string> _assetNames;

		[SerializeField]
		private string _variant;

		[SerializeField]
		private List<string> _scenes;

		[SerializeField]
		private string[] _dependencies;

		[SerializeField]
		private int _priority;

		[SerializeField]
		private BundleStreamingAssets _streamingAssets;

		[SerializeField]
		private long _size;

		[SerializeField]
		private bool _inStreamingAssets;

		[SerializeField]
		private string _url;

		public uint BundleCrc
		{
			get
			{
				return _bundleCrc;
			}
			set
			{
				_bundleCrc = value;
			}
		}

		public string VersionHash
		{
			get
			{
				return _versionHash;
			}
			set
			{
				_versionHash = value;
			}
		}

		public string AssetsHash
		{
			get
			{
				return _assetsHash;
			}
			set
			{
				_assetsHash = value;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public List<string> ObjectIds
		{
			get
			{
				return _objectIds;
			}
			set
			{
				_objectIds = value;
			}
		}

		public List<string> AssetNames
		{
			get
			{
				return _assetNames;
			}
			set
			{
				_assetNames = value;
			}
		}

		public string Variant
		{
			get
			{
				return _variant;
			}
			set
			{
				_variant = value;
			}
		}

		public List<string> Scenes
		{
			get
			{
				return _scenes;
			}
			set
			{
				_scenes = value;
			}
		}

		public string[] Dependencies
		{
			get
			{
				return _dependencies;
			}
			set
			{
				_dependencies = value;
			}
		}

		public int Priority
		{
			get
			{
				return _priority;
			}
			set
			{
				_priority = value;
			}
		}

		public long Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
			}
		}

		public BundleStreamingAssets StreamingAssets
		{
			get
			{
				return _streamingAssets;
			}
			set
			{
				_streamingAssets = value;
			}
		}

		public string FullName
		{
			get
			{
				if (!string.IsNullOrEmpty(_variant))
				{
					return $"{_name}.{_variant}";
				}
				return _name;
			}
		}

		public bool InStreamingAssets
		{
			get
			{
				return _inStreamingAssets;
			}
			set
			{
				_inStreamingAssets = value;
			}
		}

		public string Url
		{
			get
			{
				return _url;
			}
			set
			{
				_url = value;
			}
		}

		public AssetBundleDescription()
		{
			_objectIds = new List<string>();
			_assetNames = new List<string>();
			_scenes = new List<string>();
			_name = string.Empty;
			_variant = string.Empty;
			_versionHash = string.Empty;
			_url = string.Empty;
		}

		private AssetBundleDescription(AssetBundleDescription source)
		{
			_bundleCrc = source._bundleCrc;
			_versionHash = source._versionHash;
			_assetsHash = source._assetsHash;
			_name = source._name;
			_objectIds = new List<string>(source._objectIds);
			_assetNames = new List<string>(source._assetNames);
			_variant = source._variant;
			_scenes = new List<string>(source._scenes);
			_priority = source._priority;
			_streamingAssets = source._streamingAssets;
			_url = source._url;
			if (source._dependencies != null)
			{
				_dependencies = new string[source._dependencies.Length];
				source._dependencies.CopyTo(_dependencies, 0);
			}
		}

		public AssetBundleDescription DeepCopy()
		{
			return new AssetBundleDescription(this);
		}

		public bool FindAssetName(string guid, out string assetName)
		{
			int count = _assetNames.Count;
			for (int i = 0; i < _objectIds.Count; i++)
			{
				if (guid == _objectIds[i] && i < count)
				{
					assetName = _assetNames[i];
					return true;
				}
			}
			assetName = null;
			return false;
		}
	}
}