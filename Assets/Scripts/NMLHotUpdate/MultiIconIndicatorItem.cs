using UnityEngine;
using UnityEngine.Events;

public class MultiIconIndicatorItem : MonoBehaviour
{
	[SerializeField]
	private UnityEvent OnReduceValue;

	[SerializeField]
	private UnityEvent OnValueReducedZero;

	[SerializeField]
	private UnityEvent OnValueReducedZeroThisSegmentOnly;

	private Transform container;

	private float currentValue = -1f;

	private float maxValue;

	private float initSize;

	public void Initialize(float maxValue)
	{
		container = ((base.transform.childCount > 0) ? base.transform.GetChild(0) : base.transform);
		initSize = container.transform.localScale.x;
		this.maxValue = maxValue;
	}

	public void SetCurrentValue(float value, bool otherIndicatorsReducedToZero)
	{
		if (value >= maxValue)
		{
			value = maxValue;
		}
		if (value < 0f)
		{
			value = 0f;
		}
		if (value != currentValue)
		{
			OnValueChange(currentValue, value, otherIndicatorsReducedToZero);
		}
		currentValue = value;
	}

	private void OnValueChange(float previousValue, float newValue, bool otherIndicatorsReducedToZero)
	{
		container.transform.localPosition = Vector3.one;
		container.transform.localScale = Vector3.one * initSize;
		if (newValue < previousValue && newValue != 0f)
		{
			OnReduceValue?.Invoke();
		}
		if (newValue <= 0f)
		{
			if (!otherIndicatorsReducedToZero && previousValue > 0f)
			{
				OnValueReducedZeroThisSegmentOnly?.Invoke();
			}
			else
			{
				OnValueReducedZero?.Invoke();
			}
		}
	}
}
