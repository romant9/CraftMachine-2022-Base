using System;
using UnityEngine;

public class ThreatMeterOverlay : MonoBehaviour
{
	[Tooltip("References to all sprites that should be animated.")]
	public UISprite[] CornerSpritesBase;

	[Tooltip("References to all sprites that should be animated.")]
	public UISprite[] CornerSpritesOver;

	[Tooltip("Defines threat levels interpolation points.")]
	public float[] ThreatLevelInterpolationPoints;

	[Tooltip("Threat level back sprite color interpolation start points.")]
	public Color[] BackSpriteStartColors;

	[Tooltip("Threat level back sprite color interpolation end points.")]
	public Color[] BackSpriteEndColors;

	[Tooltip("Threat level top sprite color interpolation start points.")]
	public Color[] TopSpriteStartColors;

	[Tooltip("Threat level top sprite interpolation points end colors.")]
	public Color[] TopSpriteEndColors;

	[Tooltip("Screen borders color.")]
	public Color BordersColor;

	[Tooltip("Threat level interpolation points frequency.")]
	public int[] ThreatLevelBPMs;

	private float animationAngle;

	private float currentFrequency;

	private Color currentStartTintColor;

	private Color currentEndTintColor;

	private Color currentBackgroundStartTintColor;

	private Color currentBackgroundEndTintColor;

	private float threatLevel;

	private Texture2D rectangleTexture;

	private Color currentBordersColor;

	public float ThreatLevel
	{
		get
		{
			return threatLevel;
		}
		set
		{
			threatLevel = Mathf.Min(value, 1f);
			bool active = threatLevel > 0f;
			base.gameObject.SetActive(active);
			base.gameObject.SetActive(value: true);
			int num = 0;
			for (int i = 0; i < ThreatLevelInterpolationPoints.Length; i++)
			{
				if (threatLevel <= ThreatLevelInterpolationPoints[i])
				{
					num = i;
					break;
				}
			}
			float num2 = (float)ThreatLevelBPMs[num] / 60f;
			currentFrequency = MathF.PI * num2;
			currentStartTintColor = TopSpriteStartColors[num];
			currentEndTintColor = TopSpriteEndColors[num];
			currentBackgroundStartTintColor = BackSpriteStartColors[num];
			currentBackgroundEndTintColor = BackSpriteEndColors[num];
			Update();
		}
	}

	private void Start()
	{
		animationAngle = 0f;
		rectangleTexture = new Texture2D(1, 1);
		rectangleTexture.SetPixel(0, 0, Color.white);
	}

	private void Update()
	{
		animationAngle += Time.deltaTime * currentFrequency;
		animationAngle %= MathF.PI;
		float num = Mathf.Sin(animationAngle);
		Color color = currentStartTintColor + (currentEndTintColor - currentStartTintColor) * num;
		TintElements(CornerSpritesOver, color);
		Color color2 = currentBackgroundStartTintColor + (currentBackgroundEndTintColor - currentBackgroundStartTintColor) * num;
		currentBordersColor = BordersColor * color2;
		TintElements(CornerSpritesBase, color2);
	}

	private void TintElements(UISprite[] elements, Color color)
	{
		for (int i = 0; i < elements.Length; i++)
		{
			elements[i].color = color;
		}
	}

	private void OnGUI()
	{
		GUI.color = currentBordersColor;
		int num = 3;
		GUI.DrawTexture(new Rect(0f, 0f, Screen.width, num), rectangleTexture, ScaleMode.StretchToFill);
		GUI.DrawTexture(new Rect(0f, 0f, num, Screen.height), rectangleTexture, ScaleMode.StretchToFill);
		GUI.DrawTexture(new Rect(Screen.width - num, 0f, num, Screen.height), rectangleTexture, ScaleMode.StretchToFill);
		GUI.DrawTexture(new Rect(0f, Screen.height - num, Screen.width, num), rectangleTexture, ScaleMode.StretchToFill);
	}
}
