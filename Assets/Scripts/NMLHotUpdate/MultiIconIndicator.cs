using System.Collections.Generic;
using UnityEngine;

public class MultiIconIndicator : MonoBehaviour
{
	[SerializeField]
	private GameObject iconPrefab;

	private float currentBarValue;

	private float totalBarValue;

	private float valuePerSegment;

	private Transform container;

	private List<MultiIconIndicatorItem> incicators = new List<MultiIconIndicatorItem>();

	public int CurrentActiveIndicatorCount => SetIconData(currentBarValue, setData: false).Length;

	public void Initialize()
	{
		container = ((base.transform.childCount > 0) ? base.transform.GetChild(0) : base.transform);
		incicators.Clear();
		container.DestroyChildren();
	}

	public void RefreshUI()
	{
		if (currentBarValue >= totalBarValue)
		{
			currentBarValue = totalBarValue;
		}
		if (currentBarValue <= 0f)
		{
			currentBarValue = 0f;
		}
		SetIconData();
		SetCurrentValue();
	}

	private void SetIconData()
	{
		float[] array = SetIconData(totalBarValue, setData: false);
		if (incicators.Count == array.Length)
		{
			return;
		}
		Initialize();
		for (int i = 0; i < array.Length; i++)
		{
			GameObject obj = Object.Instantiate(iconPrefab);
			obj.transform.SetParent(container);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localEulerAngles = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			if (obj.TryGetComponent<MultiIconIndicatorItem>(out var component))
			{
				component.Initialize(valuePerSegment);
				incicators.Add(component);
			}
		}
		if (TryGetComponent<UIGrid>(out var component2))
		{
			component2.Reposition();
		}
		if (TryGetComponent<UITable>(out var component3))
		{
			component3.Reposition();
		}
	}

	private void SetCurrentValue()
	{
		if (currentBarValue > totalBarValue || currentBarValue < 0f || incicators.Count == 0)
		{
			return;
		}
		float[] array = SetIconData(currentBarValue, setData: true);
		bool otherIndicatorsReducedToZero = false;
		if (array.Length > incicators.Count)
		{
			return;
		}
		for (int i = 0; i < incicators.Count; i++)
		{
			if (i < array.Length)
			{
				incicators[i].SetCurrentValue(array[i], otherIndicatorsReducedToZero);
				continue;
			}
			incicators[i].SetCurrentValue(0f, otherIndicatorsReducedToZero);
			otherIndicatorsReducedToZero = true;
		}
	}

	private float[] SetIconData(float value, bool setData)
	{
		int num = ((value >= valuePerSegment) ? ((int)(value / valuePerSegment)) : 0);
		float num2 = value % valuePerSegment;
		if (num2 > 0f)
		{
			num++;
		}
		float[] array = new float[num];
		if (setData)
		{
			for (int i = 0; i < num; i++)
			{
				if (num2 <= 0f)
				{
					array[i] = valuePerSegment;
				}
				else if (i == num - 1)
				{
					array[i] = num2;
				}
				else
				{
					array[i] = valuePerSegment;
				}
			}
		}
		return array;
	}

	public void SetCurrentValue(float value)
	{
		currentBarValue = value;
		if (currentBarValue >= totalBarValue)
		{
			currentBarValue = totalBarValue;
		}
		if (currentBarValue <= 0f)
		{
			currentBarValue = 0f;
		}
		RefreshUI();
	}

	public void SetMaxValue(float value)
	{
		totalBarValue = value;
		if (totalBarValue >= value)
		{
			totalBarValue = value;
		}
		if (totalBarValue <= 0f)
		{
			totalBarValue = 0f;
		}
	}

	public void SetValuePerSegment(float value)
	{
		valuePerSegment = value;
	}
}
