using System;
using System.Collections;
using UnityEngine;

namespace NextGames.Sdk.AssetBundleManager
{
	public class AssetRequest : IEnumerator
	{
		private readonly AssetBundleRequest _request;

		public UnityEngine.Object Asset => _request.asset;

		public UnityEngine.Object[] AllAssets => _request.allAssets;

		public bool IsDone => _request.isDone;

		public float Progress => _request.progress;

		public object Current { get; private set; }

		public AssetRequest(AssetBundleRequest request)
		{
			_request = request;
		}

		public bool MoveNext()
		{
			return !_request.isDone;
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}
	}
}