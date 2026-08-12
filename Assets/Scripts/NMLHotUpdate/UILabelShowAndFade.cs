using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UILabel))]
public class UILabelShowAndFade : MonoBehaviour
{
	[Header("渐隐渐显设置")]
	public float fadeInDuration = 1f;

	public float stayDuration = 2f;

	public float fadeOutDuration = 1f;

	public bool playOnStart = true;

	public bool disableObjectAfterFinish = true;

	private UILabel label;

	private Color originalColor;

	private bool isPlaying;

	private void Awake()
	{
		label = GetComponent<UILabel>();
		originalColor = label.color;
	}

	private void Start()
	{
		if (playOnStart)
		{
			StartFade();
		}
	}

	public void StartFade()
	{
		if (!isPlaying)
		{
			if (disableObjectAfterFinish && !base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			StartCoroutine(FadeRoutine());
		}
	}

	public void StopFade()
	{
		StopAllCoroutines();
		isPlaying = false;
		label.color = originalColor;
	}

	public void RestartFade()
	{
		StopFade();
		StartFade();
	}

	private IEnumerator FadeRoutine()
	{
		isPlaying = true;
		Color color = originalColor;
		color.a = 0f;
		label.color = color;
		float timer = 0f;
		while (timer < fadeInDuration)
		{
			timer += Time.deltaTime;
			color.a = Mathf.Lerp(0f, originalColor.a, timer / fadeInDuration);
			label.color = color;
			yield return null;
		}
		color.a = originalColor.a;
		label.color = color;
		yield return new WaitForSeconds(stayDuration);
		timer = 0f;
		while (timer < fadeOutDuration)
		{
			timer += Time.deltaTime;
			color.a = Mathf.Lerp(originalColor.a, 0f, timer / fadeOutDuration);
			label.color = color;
			yield return null;
		}
		color.a = 0f;
		label.color = color;
		isPlaying = false;
		if (disableObjectAfterFinish)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void SetTextAndPlay(string newText)
	{
		if (disableObjectAfterFinish && !base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		label.text = newText;
		RestartFade();
	}

	public void SetColor(Color newColor)
	{
		float a = label.color.a;
		originalColor = newColor;
		originalColor.a = a;
		label.color = originalColor;
	}

	public void EnableObject()
	{
		base.gameObject.SetActive(value: true);
		label.color = originalColor;
	}

	public void DisableObject()
	{
		base.gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}

	private void OnEnable()
	{
		if (isPlaying)
		{
			RestartFade();
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		isPlaying = false;
	}
}
