using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScoreDataProvider
{
	public enum FetchState
	{
		Done = 0,
		Fetching = 1
	}

	private List<ScoreDataEntry> cachedData;

	protected bool useCachedOnly;

	protected float LastRequestedTime;

	protected float LastUpdatedTime;

	protected FetchState State;

	public event Action<ScoreDataProvider, List<ScoreDataEntry>> OnDataReceived;

	public void RequestData(bool forceFetch = false)
	{
		bool flag = State == FetchState.Done && Time.time > LastUpdatedTime + (float)GetCacheDurationSeconds();
		bool flag2 = State == FetchState.Fetching && Time.time > LastRequestedTime + (float)GetRequestTimeoutSeconds();
		bool num = cachedData == null || flag || flag2;
		bool flag3 = useCachedOnly && cachedData != null;
		if ((num || forceFetch) && !flag3)
		{
			State = FetchState.Fetching;
			LastRequestedTime = Time.time;
			if (!RequestInternal() && this.OnDataReceived != null)
			{
				this.OnDataReceived(this, null);
			}
		}
		else if (this.OnDataReceived != null)
		{
			this.OnDataReceived(this, cachedData);
		}
	}

	protected IEnumerator TESTREQUEST()
	{
		yield return new WaitForSeconds(5f);
	}

	protected virtual ScoreDataEntry CreateEntry()
	{
		return new ScoreDataEntry();
	}

	protected virtual void AddCurrentPlayerData(List<ScoreDataEntry> data)
	{
	}

	protected virtual void NotifyDataReceived(List<ScoreDataEntry> data)
	{
		State = FetchState.Done;
		LastUpdatedTime = Time.time;
		AddCurrentPlayerData(data);
		cachedData = data;
		if (this.OnDataReceived != null)
		{
			this.OnDataReceived(this, data);
		}
	}

	public virtual int GetCacheDurationSeconds()
	{
		return 300;
	}

	public virtual int GetRequestTimeoutSeconds()
	{
		return 60;
	}

	protected abstract bool RequestInternal();
}
