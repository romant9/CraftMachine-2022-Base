using System;
using UnityEngine;

public class CacheableObject : MonoBehaviour, ICacheableObject
{
	public delegate void ChangeCallback();

	private ChangeCallback AddedCallback;

	private ChangeCallback RetrievedCallback;

	public void Destroy()
	{
		if ((bool)SingularityMonoBehaviour<ObjectPoolManager>.Instance)
		{
			SingularityMonoBehaviour<ObjectPoolManager>.Instance.ReturnObjectToPool(base.gameObject);
			return;
		}
		Clear();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void AddAddedCallback(ChangeCallback callback)
	{
		RemoveAddedCallback(callback);
		AddedCallback = (ChangeCallback)Delegate.Combine(AddedCallback, callback);
	}

	public void RemoveAddedCallback(ChangeCallback callback)
	{
		AddedCallback = (ChangeCallback)Delegate.Remove(AddedCallback, callback);
	}

	public void AddRetrievedCallback(ChangeCallback callback)
	{
		RemoveRetrievedCallback(callback);
		RetrievedCallback = (ChangeCallback)Delegate.Combine(RetrievedCallback, callback);
	}

	public void RemoveRetrievedCallback(ChangeCallback callback)
	{
		RetrievedCallback = (ChangeCallback)Delegate.Remove(RetrievedCallback, callback);
	}

	public virtual void OnPoolReturn()
	{
		if (AddedCallback != null)
		{
			AddedCallback();
		}
	}

	public virtual void OnPoolRetrieve()
	{
		if (RetrievedCallback != null)
		{
			RetrievedCallback();
		}
	}

	public virtual void Clear()
	{
		AddedCallback = null;
		RetrievedCallback = null;
	}
}
