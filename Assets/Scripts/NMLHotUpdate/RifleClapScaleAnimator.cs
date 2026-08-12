using System.Collections;
using UnityEngine;

public class RifleClapScaleAnimator : MonoBehaviour
{
	private const float DefaultDuration = 0.18f;

	private Coroutine scaleRoutine;

	private bool isAnimatingOut;

	public void PlayAppear(Vector3 targetScale, float duration = 0.18f)
	{
		isAnimatingOut = false;
		StartScaleRoutine(Vector3.zero, targetScale, duration, destroyOnComplete: false);
	}

	public void PlayDisappearAndDestroy(float duration = 0.18f)
	{
		if (!isAnimatingOut)
		{
			isAnimatingOut = true;
			StartScaleRoutine(base.transform.localScale, Vector3.zero, duration, destroyOnComplete: true);
		}
	}

	private void StartScaleRoutine(Vector3 startScale, Vector3 endScale, float duration, bool destroyOnComplete)
	{
		if (scaleRoutine != null)
		{
			StopCoroutine(scaleRoutine);
		}
		scaleRoutine = StartCoroutine(AnimateScale(startScale, endScale, duration, destroyOnComplete));
	}

	private IEnumerator AnimateScale(Vector3 startScale, Vector3 endScale, float duration, bool destroyOnComplete)
	{
		base.transform.localScale = startScale;
		float elapsed = 0f;
		float safeDuration = Mathf.Max(0.01f, duration);
		while (elapsed < safeDuration)
		{
			elapsed += Time.deltaTime;
			float t = Mathf.Clamp01(elapsed / safeDuration);
			base.transform.localScale = Vector3.Lerp(startScale, endScale, t);
			yield return null;
		}
		base.transform.localScale = endScale;
		scaleRoutine = null;
		if (destroyOnComplete)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
