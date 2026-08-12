using System;
using TWDModel;
using UnityEngine;

[Serializable]
public class FactionColorEntry
{
	public Faction Faction;

	public Color UIColor;

	public Color ShaderNormalColor;

	public Color ShaderSelectedColor;

	public Color ShaderInactiveColor;
}
