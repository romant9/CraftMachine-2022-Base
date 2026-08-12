using UnityEngine;

public class ChangeVignetteResult : ConditionalResult
{
	public bool ChangeVignetteColorOnTrue = true;

	public Color VignetteColorOnTrue = Color.white;

	public bool ChangeVignetteColorOnFalse = true;

	public Color VignetteColorOnFalse = Color.white;

	private void SetVignetteColor(Color color)
	{
		if (Camera.main.TryGetComponent<AmplifyColorEffect>(out var component))
		{
			Debug.LogWarning("Ignore SetVignetteColor");
			//component.VignetteColor = color;
		}
		else
		{
			Debug.LogError("Cannot change vignetted color value, no AmplifyColorEffect component found in camera.");
		}
	}

	public override void OnConditionTrue()
	{
		if (ChangeVignetteColorOnTrue)
		{
			SetVignetteColor(VignetteColorOnTrue);
		}
	}

	public override void OnConditionFalse()
	{
		if (ChangeVignetteColorOnFalse)
		{
			SetVignetteColor(VignetteColorOnFalse);
		}
	}
}
