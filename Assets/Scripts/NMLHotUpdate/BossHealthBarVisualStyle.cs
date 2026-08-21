using System;
using UnityEngine;

[Serializable]
public struct BossHealthBarVisualStyle
{
	[Tooltip("Main bar foreground sprite. Leave empty to keep the prefab default.")]
	public string foregroundSprite;

	[Tooltip("Main bar background sprite. Leave empty to keep the prefab default.")]
	public string backgroundSprite;

	[Tooltip("Delay bar foreground sprite. Leave empty to keep the prefab delay bar sprite.")]
	public string delayForegroundSprite;

	[Tooltip("Delay bar background sprite. Leave empty to keep the prefab delay bar background.")]
	public string delayBackgroundSprite;

	public bool overrideForegroundColor;

	public Color foregroundColor;

	public bool overrideDelayForegroundColor;

	public Color delayForegroundColor;
}
