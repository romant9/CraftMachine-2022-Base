using System;
using UnityEngine;

[Serializable]
public class IndicatorInstanceInfo
{
	public IndicatorType IndicatorType;

	public bool InstanceOnRightSide;

	public GameObject IndicatorInstance;

	public IndicatorInstanceInfo(IndicatorType inType)
	{
		IndicatorType = inType;
		InstanceOnRightSide = false;
		IndicatorInstance = null;
	}
}
