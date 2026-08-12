using UnityEngine;

[RequireComponent(typeof(UISprite))]
public class LoadingProgressBarShimmer : MonoBehaviour
{
	[SerializeField]
	private UIProgressBar targetBar;

	[SerializeField]
	private float glintAlpha = 0.45f;

	private UISprite glintSprite;

	private void Awake()
	{
		glintSprite = GetComponent<UISprite>();
		if (targetBar == null)
		{
			targetBar = GetComponentInParent<UIProgressBar>();
		}
	}

	private void LateUpdate()
	{
		if (targetBar == null || glintSprite == null)
		{
			return;
		}
		UISprite uISprite = targetBar.foregroundWidget as UISprite;
		if (uISprite == null)
		{
			return;
		}
		if (Mathf.Clamp01(targetBar.value) < 0.001f)
		{
			glintSprite.alpha = 0f;
			return;
		}
		glintSprite.alpha = glintAlpha;
		glintSprite.depth = uISprite.depth + 1;
		Transform cachedTransform = uISprite.cachedTransform;
		Transform cachedTransform2 = glintSprite.cachedTransform;
		if (cachedTransform2.parent == cachedTransform)
		{
			glintSprite.drawRegion = uISprite.drawRegion;
			return;
		}
		glintSprite.width = uISprite.width;
		glintSprite.height = uISprite.height;
		glintSprite.pivot = uISprite.pivot;
		glintSprite.drawRegion = uISprite.drawRegion;
		cachedTransform2.localPosition = cachedTransform.localPosition;
		cachedTransform2.localRotation = cachedTransform.localRotation;
		cachedTransform2.localScale = cachedTransform.localScale;
	}
}
