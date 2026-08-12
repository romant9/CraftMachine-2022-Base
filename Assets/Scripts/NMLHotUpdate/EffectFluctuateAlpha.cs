using UnityEngine;

public class EffectFluctuateAlpha : MonoBehaviour
{
	public float alphaSpeed = 2f;

	public Vector2 alphaMinMax = new Vector2(0.1f, 0.9f);

	private float alpha;

	private Color origColor = Color.white;

	private bool NGUImode;

	private Material mat;

	private UITexture guiTex;

	private void Start()
	{
		Renderer component = GetComponent<Renderer>();
		guiTex = GetComponent<UITexture>();
		if (component != null)
		{
			mat = component.material;
			origColor = mat.color;
		}
		else if (guiTex != null)
		{
			NGUImode = true;
			origColor = guiTex.color;
		}
	}

	private void Update()
	{
		alpha = 1f * Mathf.PerlinNoise(11.77f, alphaSpeed * Time.time);
		alpha = Mathf.Lerp(alphaMinMax.x, alphaMinMax.y, alpha);
		if (NGUImode)
		{
			guiTex.color = new Color(origColor.r, origColor.g, origColor.b, alpha);
		}
		else
		{
			mat.color = new Color(origColor.r, origColor.g, origColor.b, alpha);
		}
	}
}
