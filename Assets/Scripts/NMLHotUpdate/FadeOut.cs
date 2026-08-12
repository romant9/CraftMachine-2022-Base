using UnityEngine;

public class FadeOut : MonoBehaviour
{
	private Color rendererColor = Color.white;

	private Color fadeColor = Color.white;

	[Tooltip("How fast it fade out")]
	[Range(0.01f, 10f)]
	public float fadeTime = 1f;

	private float fadeStart;

	private Renderer renderer;

	private void Start()
	{
		renderer = GetComponent<Renderer>();
		if ((bool)renderer)
		{
			rendererColor = renderer.material.color;
		}
		fadeColor = rendererColor;
		fadeColor.a = 0f;
	}

	private void Update()
	{
		Color color = Color.Lerp(rendererColor, fadeColor, fadeStart);
		renderer.material.color = color;
		if (fadeStart < 1f)
		{
			fadeStart += Time.deltaTime / fadeTime;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
