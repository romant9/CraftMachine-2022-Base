using System;
using System.Collections;
using UnityEngine;

public class ScreenshotShare : MonoBehaviour
{
	[SerializeField]
	private Texture2D Logo;

	[SerializeField]
	private Color screenshotBorderColor;

	[SerializeField]
	private int screenShotBorder = 20;

	[SerializeField]
	private Material appleMaterial;

	[SerializeField]
	private Material googleMaterial;

	[SerializeField]
	private Material epicMaterial;

	[SerializeField]
	private UIRect screenshotArea;

	private byte[] screenshot;

	private UIButton shareButton;

	private Callback shareCompleteCallback;

	private bool shareCompleteCallbackInvoked;

	private string shareURL = "";

	private string shareText = "";

	public bool ScreenShotReady { get; protected set; }

	public string ShareType { get; protected set; }

	public IEnumerator TakeScreenshot(string shareType, UIButton shareButton, UITexture storeBadge, Action<bool> showUiCallback, Callback shareComplete = null, bool skipPreview = false, string shareURL = "", string shareText = "")
	{
		ShareType = shareType;
		this.shareButton = shareButton;
		if (shareButton != null)
		{
			shareButton.gameObject.SetActive(value: false);
		}
		storeBadge.material = epicMaterial;
		showUiCallback(obj: true);
		yield return new WaitForEndOfFrame();
		screenshot = GetScreenShot(screenShotBorder, screenshotBorderColor, screenshotArea);
		showUiCallback(obj: false);
		if (!skipPreview)
		{
			SharePreview obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SharePreview) as SharePreview;
			obj.ScreenshotShare = this;
			obj.SetPreview(screenshot);
			obj.Open();
		}
		else
		{
			Share();
		}
		shareCompleteCallback = shareComplete;
		shareCompleteCallbackInvoked = false;
		this.shareURL = shareURL;
		this.shareText = shareText;
	}

	public void Share()
	{
		StartCoroutine(ShareCoRoutine());
	}

	private IEnumerator ShareCoRoutine()
	{
		ScreenShotReady = false;
		ScreenShotReady = true;
		while (!ScreenShotReady)
		{
			yield return null;
		}
		if (shareCompleteCallback != null)
		{
			shareCompleteCallback();
			shareCompleteCallbackInvoked = true;
			shareCompleteCallback = null;
		}
	}

	public void OnSharePreviewClosing()
	{
		if (shareButton != null)
		{
			shareButton.gameObject.SetActive(value: true);
		}
		if (shareCompleteCallback != null && !ScreenShotReady && shareCompleteCallbackInvoked)
		{
			shareCompleteCallback = null;
		}
	}

	public byte[] GetScreenShot(int screenshotBorder, Color screenshotBorderColor, UIRect size = null)
	{
		int num = Screen.width;
		int num2 = Screen.height;
		int num3 = 0;
		int num4 = 0;
		if (size != null)
		{
			Vector3 vector = UICamera.currentCamera.WorldToScreenPoint(size.worldCorners[1]);
			Vector3 vector2 = UICamera.currentCamera.WorldToScreenPoint(size.worldCorners[3]);
			num = (int)vector2.x - (int)vector.x;
			num2 = (int)vector.y - (int)vector2.y;
			num3 = (int)vector.x;
			num4 = Screen.height - (int)vector.y;
		}
		Texture2D texture2D = new Texture2D(num + screenshotBorder * 2, num2 + screenshotBorder * 2, TextureFormat.RGB24, mipChain: false);
		Color[] pixels = texture2D.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
		{
			pixels[i] = screenshotBorderColor;
		}
		texture2D.SetPixels(pixels);
		texture2D.ReadPixels(new Rect(num3, num4, num, num2), screenshotBorder, screenshotBorder);
		texture2D.Apply();
		float num5 = (float)Screen.height / 640f;
		if (num5 > 1f)
		{
			texture2D = ScaleTexture(texture2D, (int)((float)texture2D.width / num5), (int)((float)texture2D.height / num5));
		}
		if (Logo != null)
		{
			Color[] pixels2 = Logo.GetPixels();
			Color[] pixels3 = texture2D.GetPixels(screenshotBorder * 2, texture2D.height - screenshotBorder * 2 - Logo.height, Logo.width, Logo.height);
			for (int j = 0; j < pixels2.Length; j++)
			{
				pixels2[j] = Color.Lerp(pixels3[j], pixels2[j], pixels2[j].a);
			}
			texture2D.SetPixels(screenshotBorder * 2, texture2D.height - screenshotBorder * 2 - Logo.height, Logo.width, Logo.height, pixels2);
			texture2D.Apply();
		}
		return texture2D.EncodeToPNG();
	}

	public Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
	{
		Texture2D texture2D = new Texture2D(targetWidth, targetHeight, source.format, mipChain: false);
		for (int i = 0; i < texture2D.height; i++)
		{
			for (int j = 0; j < texture2D.width; j++)
			{
				texture2D.SetPixel(j, i, source.GetPixelBilinear((float)j / (float)texture2D.width, (float)i / (float)texture2D.height));
			}
		}
		texture2D.Apply();
		return texture2D;
	}

	private void ShareNative(byte[] image)
	{
		ScreenShotReady = false;
		string url = ((!string.IsNullOrEmpty(shareURL)) ? shareURL : "");
		string text = ((!string.IsNullOrEmpty(shareText)) ? shareText : LocalizationManager.GetText("Popup.Share.DefaultMessage"));
		new NativeShare().ShareScreenshotWithText(text, image, url);
		ScreenShotReady = true;
		AnalyticsManager.instance.CreateEvent("Share").AddProperty("ShareType", ShareType).Send();
	}
}
