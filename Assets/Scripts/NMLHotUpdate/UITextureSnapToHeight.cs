using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class UITextureSnapToHeight : MonoBehaviour
{
	private UITexture uiTexture;

	private int aspect;

	private void Awake()
	{
		uiTexture = GetComponent<UITexture>();
		SetCustomAspect(100);
	}

	public void SetCustomAspect(int value)
	{
		aspect = value;
	}

	private void Update()
	{
		if (!(uiTexture == null) && !(uiTexture.mainTexture == null))
		{
			int num = 100;
			uiTexture.width = (int)((float)(num * ((aspect <= 0) ? 100 : aspect)) * 1f / 100f);
			uiTexture.height = num;
		}
	}
}
