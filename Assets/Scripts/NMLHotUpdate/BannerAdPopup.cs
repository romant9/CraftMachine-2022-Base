using System.Collections;
using UnityEngine;

public class BannerAdPopup : HUDElement
{
	public UITexture bannerAdTexture;

	public int maxHeight = 150;

	[Tooltip("timeout <= 0 means banner stays forever")]
	public float autoCloseTimeout;

	private const float cantCloseBeforeSeconds = 1f;

	private bool canClose;

	public override void Open()
	{
		canClose = false;
		base.Open();
		Texture bannerTexture = GameManager.Instance.BannerManager.GetBannerTexture();
		bannerAdTexture.mainTexture = bannerTexture;
		float num = (float)maxHeight / (float)Mathf.Max(bannerTexture.height, maxHeight);
		int width = Mathf.RoundToInt((float)bannerTexture.width * num);
		int height = Mathf.RoundToInt((float)bannerTexture.height * num);
		bannerAdTexture.width = width;
		bannerAdTexture.height = height;
		GameManager.Instance.BannerManager.IncrementShowCount();
		StartCoroutine(WaitForCanClose());
		if (autoCloseTimeout > 0f)
		{
			StartCoroutine(AutoClose());
		}
	}

	private IEnumerator AutoClose()
	{
		yield return new WaitForSeconds(autoCloseTimeout);
		Close();
	}

	private IEnumerator WaitForCanClose()
	{
		yield return new WaitForSeconds(1f);
		canClose = true;
	}

	public override void OnClickClose()
	{
		if (canClose)
		{
			base.OnClickClose();
		}
	}

	public void OnClickBanner()
	{
		GameManager.Instance.BannerManager.OnBannerClicked();
		OnClickClose();
	}
}
