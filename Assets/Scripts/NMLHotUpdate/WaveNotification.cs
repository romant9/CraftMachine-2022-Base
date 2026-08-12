using UnityEngine;

public class WaveNotification : HUDElement
{
	[Tooltip("Label for the wave heading text.")]
	public UILabel WaveHeading;

	[Tooltip("Label for the wave body text.")]
	public UILabel WaveBody;

	public void Reset()
	{
		Animator[] componentsInChildren = base.gameObject.GetComponentsInChildren<Animator>();
		foreach (Animator obj in componentsInChildren)
		{
			obj.Play(obj.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
		}
		base.gameObject.SetActive(value: true);
	}

	public void SetMessage(string heading, string body)
	{
		WaveHeading.text = heading;
		WaveBody.text = body;
	}

	public void SetColor(Color headingColor, Color waveBodyColor)
	{
		WaveHeading.color = headingColor;
		WaveBody.color = waveBodyColor;
	}
}
