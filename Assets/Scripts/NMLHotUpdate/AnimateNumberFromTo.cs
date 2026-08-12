using System.Collections;
using TWDModel;
using UnityEngine;

public class AnimateNumberFromTo : MonoBehaviour
{
	[SerializeField]
	private float delayToStart;

	[SerializeField]
	private float animationLenght;

	[SerializeField]
	private UILabel label;

	[SerializeField]
	private bool ignoreTimeScale;

	public void Animate(int from, int to)
	{
		StartCoroutine(AnimateCoRoutine(from, to));
	}

	private IEnumerator AnimateCoRoutine(int from, int to, CurrencyType? currencyType = null)
	{
		if (ignoreTimeScale)
		{
			yield return new WaitForSecondsRealtime(delayToStart);
		}
		else
		{
			yield return new WaitForSeconds(delayToStart);
		}
		float updateSpeed = (float)Mathf.Abs(to - from) / animationLenght;
		float currentValue = from;
		while (Mathf.FloorToInt(currentValue) != to)
		{
			currentValue = Mathf.Min(currentValue + updateSpeed * (ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime), to);
			label.text = (currencyType.HasValue ? HelpersLocalization.GetComponentRewardName(currencyType.Value, Mathf.FloorToInt(currentValue)) : Mathf.FloorToInt(currentValue).ToString());
			yield return null;
		}
	}

	public void AnimateComponentCurrency(int from, int to, CurrencyType currencyType)
	{
		StartCoroutine(AnimateCoRoutine(from, to, currencyType));
	}

	public void SetLabel(UILabel label)
	{
		if (label != null)
		{
			this.label = label;
		}
	}

	public void AddDelayToStart(float delay)
	{
		delayToStart += delay;
	}

	public void SetIgnoreTimeScale(bool ignoreTimeScale)
	{
		this.ignoreTimeScale = ignoreTimeScale;
	}
}
