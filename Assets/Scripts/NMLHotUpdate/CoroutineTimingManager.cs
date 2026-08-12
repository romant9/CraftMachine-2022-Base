using System;
using System.Collections;
using UnityEngine;

public class CoroutineTimingManager : ITimingManager
{
	private class CancelObject : IDisposable
	{
		private readonly Coroutine coroutine;

		private readonly MonoBehaviour runnerObject;

		public CancelObject(Coroutine coroutine, MonoBehaviour runnerObject)
		{
			this.coroutine = coroutine;
			this.runnerObject = runnerObject;
		}

		public void Dispose()
		{
			if ((bool)runnerObject)
			{
				runnerObject.StopCoroutine(coroutine);
			}
		}
	}

	private readonly MonoBehaviour runner;

	public CoroutineTimingManager()
	{
		runner = new GameObject("CoroutineRunner").AddComponent<CoroutineRunner>();
		UnityEngine.Object.DontDestroyOnLoad(runner.gameObject);
	}

	public IDisposable Timer(TimeSpan timeSpan, Action action)
	{
		return new CancelObject(runner.StartCoroutine(TimerCoroutine((float)timeSpan.TotalSeconds, action, loops: false)), runner);
	}

	public IDisposable Interval(TimeSpan timeSpan, Action action)
	{
		return new CancelObject(runner.StartCoroutine(TimerCoroutine((float)timeSpan.TotalSeconds, action, loops: true)), runner);
	}

	private IEnumerator TimerCoroutine(float timeSeconds, Action action, bool loops)
	{
		do
		{
			yield return new WaitForSeconds(timeSeconds);
			action();
		}
		while (loops);
	}
}
