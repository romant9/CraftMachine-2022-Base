using System;
using TWDModel;
using UnityEngine;

[Serializable]
public struct TimedEffectEntry
{
	public TimedEffectType TimedEffectType;

	public string Sprite;

	public Color GradientTop;

	public Color GradientBottom;

	public byte TweenGroupId;
}
