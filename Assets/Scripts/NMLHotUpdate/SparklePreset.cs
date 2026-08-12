using System;
using System.Collections.Generic;

[Serializable]
public class SparklePreset
{
	public string name = "Preset Name";

	public float PathRounding;

	public float PathOffset;

	public List<EffectSparkle.SparkleBase> Sparkles = new List<EffectSparkle.SparkleBase>();
}
