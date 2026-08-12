using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RumblePreset
{
	public string name = "Preset Name";

	public List<EffectRumble.RumbleCurve> RumbleCurves = new List<EffectRumble.RumbleCurve>();

	[HideInInspector]
	public bool HasPosCurve;

	[HideInInspector]
	public bool HasRotCurve;

	[HideInInspector]
	public bool HasScaleCurve;

	[HideInInspector]
	public bool HasNguiAlphaCurve;

	[HideInInspector]
	public bool HasMaterialAlphaCurve;
}
