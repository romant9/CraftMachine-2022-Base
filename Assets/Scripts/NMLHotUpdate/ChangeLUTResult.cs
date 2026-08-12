using UnityEngine;

public class ChangeLUTResult : ConditionalResult
{
	public bool ChangeLUTBlendOnTrue = true;

	public float LUTBlendValueOnTrue = 1f;

	public bool ChangeLUTBlendOnFalse = true;

	public float LUTBlendValueOnFalse;

	private void SetLUTBlend(float blendValue)
	{
		AmplifyColorEffect component = Camera.main.GetComponent<AmplifyColorEffect>();
		if (component != null)
		{
			component.BlendAmount = blendValue;
		}
		else
		{
			Debug.LogError("Cannot change LUT blend value, no AmplifyColorEffect component found in camera.");
		}
	}

	public override void OnConditionTrue()
	{
		if (ChangeLUTBlendOnTrue)
		{
			SetLUTBlend(LUTBlendValueOnTrue);
		}
	}

	public override void OnConditionFalse()
	{
		if (ChangeLUTBlendOnFalse)
		{
			SetLUTBlend(LUTBlendValueOnFalse);
		}
	}
}
