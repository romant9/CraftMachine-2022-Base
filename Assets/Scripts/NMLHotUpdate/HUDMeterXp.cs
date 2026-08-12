using System;
using UnityEngine;

public class HUDMeterXp : MonoBehaviour
{
	[SerializeField]
	private GameObject doubleXpContainer;

	private HUDMeter hudMeter;

	private void OnEnable()
	{
		hudMeter = GetComponent<HUDMeter>();
		HUDMeter hUDMeter = hudMeter;
		hUDMeter.OnProgressBarAnimationStart = (Callback)Delegate.Combine(hUDMeter.OnProgressBarAnimationStart, new Callback(OnProgressBarAnimationStart));
	}

	private void OnDisable()
	{
		HUDMeter hUDMeter = hudMeter;
		hUDMeter.OnProgressBarAnimationDone = (Callback)Delegate.Remove(hUDMeter.OnProgressBarAnimationDone, new Callback(OnProgressBarAnimationStart));
	}

	private void OnProgressBarAnimationStart()
	{
		TweenManager.PlayTweenGroup(doubleXpContainer, 2);
	}
}
