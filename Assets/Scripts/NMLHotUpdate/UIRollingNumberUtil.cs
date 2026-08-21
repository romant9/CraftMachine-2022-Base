using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIRollingNumberUtil
{
	private static readonly Dictionary<int, Coroutine> activeCoroutines = new Dictionary<int, Coroutine>();

	public static void SetValue(UILabel[] digitLabels, int value)
	{
		if (digitLabels != null)
		{
			StopActiveAnimation(digitLabels);
			ApplyValue(digitLabels, Mathf.Max(0, value));
		}
	}

	public static void AnimateTo(UILabel[] digitLabels, int fromValue, int toValue, float duration = 0.6f, Action onComplete = null)
	{
		if (digitLabels == null || digitLabels.Length == 0)
		{
			onComplete?.Invoke();
			return;
		}
		fromValue = Mathf.Max(0, fromValue);
		toValue = Mathf.Max(0, toValue);
		StopActiveAnimation(digitLabels);
		ApplyValue(digitLabels, fromValue);
		if (fromValue == toValue)
		{
			onComplete?.Invoke();
			return;
		}
		if (GameManager.Instance == null)
		{
			onComplete?.Invoke();
			return;
		}
		int key = GetKey(digitLabels);
		Coroutine value = GameManager.Instance.StartCoroutine(CountCoroutine(digitLabels, fromValue, toValue, duration, onComplete, key));
		activeCoroutines[key] = value;
	}

	private static IEnumerator CountCoroutine(UILabel[] digitLabels, int fromValue, int toValue, float duration, Action onComplete, int key)
	{
		int stepCount = Mathf.Abs(toValue - fromValue);
		int direction = ((toValue > fromValue) ? 1 : (-1));
		duration = Mathf.Max(duration, 0f);
		if (duration <= 0f)
		{
			ApplyValue(digitLabels, toValue);
			activeCoroutines.Remove(key);
			onComplete?.Invoke();
			yield break;
		}
		int displayed = fromValue;
		float elapsed = 0f;
		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			int num = Mathf.RoundToInt(Mathf.Clamp01(elapsed / duration) * (float)stepCount);
			int a = fromValue + num * direction;
			a = ((direction <= 0) ? Mathf.Max(a, toValue) : Mathf.Min(a, toValue));
			if (a != displayed)
			{
				displayed = a;
				ApplyValue(digitLabels, displayed);
			}
			yield return null;
		}
		ApplyValue(digitLabels, toValue);
		activeCoroutines.Remove(key);
		onComplete?.Invoke();
	}

	private static void ApplyValue(UILabel[] digitLabels, int value)
	{
		for (int i = 0; i < digitLabels.Length; i++)
		{
			UILabel uILabel = digitLabels[i];
			if (uILabel != null)
			{
				uILabel.text = GetDigitAt(value, i).ToString();
			}
		}
	}

	private static void StopActiveAnimation(UILabel[] digitLabels)
	{
		int key = GetKey(digitLabels);
		if (key != 0 && activeCoroutines.TryGetValue(key, out var value) && value != null)
		{
			GameManager.Instance.StopCoroutine(value);
			activeCoroutines.Remove(key);
		}
	}

	private static int GetKey(UILabel[] digitLabels)
	{
		if (digitLabels == null)
		{
			return 0;
		}
		for (int i = 0; i < digitLabels.Length; i++)
		{
			if (digitLabels[i] != null)
			{
				return digitLabels[i].GetInstanceID();
			}
		}
		return 0;
	}

	private static int GetDigitAt(int value, int place)
	{
		int num = (int)Mathf.Pow(10f, place);
		return value / num % 10;
	}
}
