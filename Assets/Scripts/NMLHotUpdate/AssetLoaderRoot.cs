using System.Collections;
using UnityEngine;

public class AssetLoaderRoot : SingularityMonoBehaviour<AssetLoaderRoot>
{
	[SerializeField]
	private Camera uiCamera;

	private IEnumerator mCoroutine;

	public bool IsFading { get; set; }

	public void Show(bool show)
	{
		if (base.gameObject.activeInHierarchy || show)
		{
			if (show)
			{
				base.gameObject.SetActive(value: true);
			}
			if (mCoroutine != null)
			{
				StopCoroutine(mCoroutine);
			}
			mCoroutine = FadeTo(show, show ? 0.225f : 0.45f);
			StartCoroutine(mCoroutine);
		}
	}

	private void OnDisable()
	{
		IsFading = false;
	}

	private IEnumerator FadeTo(bool show, float time)
	{
		IsFading = true;
		uiCamera.clearFlags = (show ? CameraClearFlags.Color : CameraClearFlags.Depth);
		UIPanel uiPanel = GetComponent<UIPanel>();
		if (uiPanel == null)
		{
			base.gameObject.SetActive(show);
		}
		else if (show)
		{
			base.gameObject.SetActive(value: true);
			uiPanel.alpha = 1f;
			yield return null;
		}
		else
		{
			float startAlpha = uiPanel.alpha;
			float targetAlpha = 0f;
			for (float cumulTime = 0f; cumulTime < time; cumulTime += Time.deltaTime)
			{
				uiPanel.alpha = Mathf.Lerp(startAlpha, targetAlpha, cumulTime / time);
				yield return null;
			}
			if (!show)
			{
				base.gameObject.SetActive(value: false);
			}
		}
		if (OfflineManager.IsLoadDataManager)
			uiCamera.clearFlags = CameraClearFlags.Depth;
		else
			uiCamera.clearFlags = CameraClearFlags.Color;
		IsFading = false;
	}
}
