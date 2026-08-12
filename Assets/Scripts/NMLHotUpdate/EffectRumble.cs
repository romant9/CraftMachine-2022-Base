using System;
using System.Collections.Generic;
using Client.Constants;
using UnityEngine;

public class EffectRumble : MonoBehaviour
{
	[Serializable]
	public class RumbleCurve
	{
		[Tooltip("Type of curve")]
		public CurveType Curvetype;

		[Tooltip("How to modulate the outcome of the curve")]
		public ModulatorType Modulator = ModulatorType.Sin;

		[Tooltip("Where to apply the output")]
		public ChannelType Channel;

		[Tooltip("Value to multiply the curve with")]
		public Vector3 ChannelMultiplier;

		[Tooltip("Value to add to the final curve")]
		public Vector3 ChannelOffset;

		[Tooltip("Delay in the start of this curve")]
		public float StartDelay;

		[Tooltip("Sine wave frequency")]
		public float SinFrequency = 30f;

		[Tooltip("Value at the beginning")]
		public float StartLevel;

		[Tooltip("Attack phase duration in seconds")]
		public float AttackDuration;

		[Tooltip("Attack strength level")]
		public float AttackLevel;

		[Tooltip("Decay phase duration in seconds")]
		public float DecayDuration;

		[Tooltip("Sustain phase duration in seconds")]
		public float SustainDuration;

		[Tooltip("Sustain phase strength level")]
		public float SustainLevel;

		[Tooltip("Release phase duration in seconds")]
		public float ReleaseDuration;

		[Tooltip("Acceleration constant for ballistic curve")]
		public float BallisticG;

		[Tooltip("Initial Velocity for ballistic curve")]
		public float BallisticV;
	}

	public enum ModulatorType
	{
		Straight = 0,
		Sin = 1,
		Cos = 2
	}

	public enum CurveType
	{
		ADSR = 0,
		Ballistic = 1
	}

	public enum ChannelType
	{
		Scale = 0,
		Position = 1,
		Rotation = 2,
		NguiAlpha = 3,
		MaterialAlpha = 4
	}

	public enum LoopType
	{
		None = 0,
		Enabled = 1,
		DisableOnEnd = 2
	}

	[HideInInspector]
	public LoopType Loop;

	[HideInInspector]
	public float LoopInterval = 1f;

	public RumblePreset currentRumble;

	[HideInInspector]
	public int selectedPreset;

	private Vector3 BasePosition;

	private Vector3 BaseRotation;

	private Vector3 BaseScale;

	private float startTime;

	private DateTime startDate;

	private static readonly string presetPath = "Effects/EffectRumblePresets";

	private EffectRumblePresets presets;

	private UIWidget targetUIwidget;

	private float age;

	private bool playing = true;

	private bool initialized;

	private MeshRenderer meshRenderer;

	private float[] curveValue;

	private Material TargetMaterial
	{
		get
		{
			if (meshRenderer != null)
			{
				return meshRenderer.material;
			}
			return null;
		}
	}

	public event EffectRumbleFinished EffectFinished;

	public void Start()
	{
		meshRenderer = base.gameObject.GetComponent<MeshRenderer>();
		initialized = true;
		startTime = Time.time;
		StoreBasePosition();
		if (Application.isPlaying)
		{
			playing = true;
		}
		else
		{
			playing = false;
		}
		GetPresets();
		if (selectedPreset > 0)
		{
			LoadPreset(selectedPreset);
		}
		else if (currentRumble == null)
		{
			currentRumble = new RumblePreset();
		}
		targetUIwidget = base.gameObject.GetComponent<UIWidget>();
		curveValue = new float[currentRumble.RumbleCurves.Count];
	}

	public bool IsPlaying()
	{
		return playing;
	}

	public void OnEnable()
	{
		ResetTimer();
	}

	public void OnDisable()
	{
		if (initialized)
		{
			LoadBasePosition();
		}
	}

	public void ResetTimer()
	{
		if (Application.isPlaying)
		{
			startTime = Time.time;
		}
		else
		{
			startDate = DateTime.Now;
		}
		age = 0f;
		playing = true;
	}

	public void StoreBasePosition()
	{
		BasePosition = base.transform.localPosition;
		BaseRotation = base.transform.localEulerAngles;
		BaseScale = base.transform.localScale;
	}

	public void LoadBasePosition()
	{
		base.transform.ConditionalSetPosition(localCoord: true, BasePosition, currentRumble.HasPosCurve);
		base.transform.ConditionalSetRotation(localCoord: true, BaseRotation, currentRumble.HasRotCurve);
		base.transform.ConditionalSetScale(localCoord: true, BaseScale, currentRumble.HasScaleCurve);
	}

	public void Update()
	{
		if (Application.isPlaying)
		{
			age = Time.time - startTime;
		}
		else
		{
			TimeSpan timeSpan = DateTime.Now - startDate;
			age = 60f * (float)timeSpan.Minutes + (float)timeSpan.Seconds + 0.001f * (float)timeSpan.Milliseconds;
		}
		if (age > LoopInterval && !playing)
		{
			return;
		}
		if (age > LoopInterval)
		{
			if (Application.isPlaying)
			{
				switch (Loop)
				{
				case LoopType.Enabled:
					base.transform.ConditionalSetPosition(localCoord: true, BasePosition, currentRumble.HasPosCurve);
					base.transform.ConditionalSetRotation(localCoord: true, BaseRotation, currentRumble.HasRotCurve);
					base.transform.ConditionalSetScale(localCoord: true, BaseScale, currentRumble.HasScaleCurve);
					startTime = Time.time;
					break;
				case LoopType.DisableOnEnd:
					base.transform.ConditionalSetPosition(localCoord: true, BasePosition, currentRumble.HasPosCurve);
					base.transform.ConditionalSetRotation(localCoord: true, BaseRotation, currentRumble.HasRotCurve);
					base.transform.ConditionalSetScale(localCoord: true, BaseScale, currentRumble.HasScaleCurve);
					playing = false;
					base.gameObject.SetActive(value: false);
					break;
				default:
					playing = false;
					break;
				}
			}
			else if (Loop == LoopType.Enabled)
			{
				base.transform.ConditionalSetPosition(localCoord: true, BasePosition, currentRumble.HasPosCurve);
				base.transform.ConditionalSetRotation(localCoord: true, BaseRotation, currentRumble.HasRotCurve);
				base.transform.ConditionalSetScale(localCoord: true, BaseScale, currentRumble.HasScaleCurve);
				startDate = DateTime.Now;
				playing = false;
			}
			else
			{
				base.transform.ConditionalSetPosition(localCoord: true, BasePosition, currentRumble.HasPosCurve);
				base.transform.ConditionalSetRotation(localCoord: true, BaseRotation, currentRumble.HasRotCurve);
				base.transform.ConditionalSetScale(localCoord: true, BaseScale, currentRumble.HasScaleCurve);
				playing = false;
			}
			if (Loop != LoopType.Enabled)
			{
				NotifyEffectFinished();
			}
		}
		else
		{
			Evaluate(age, doTransform: true);
		}
	}

	public float[] Evaluate(float age, bool doTransform)
	{
		float num = 0f;
		Vector3 vector = new Vector3(0f, 0f, 0f);
		Vector3 vector2 = new Vector3(0f, 0f, 0f);
		Vector3 vector3 = new Vector3(0f, 0f, 0f);
		float num2 = 0f;
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < currentRumble.RumbleCurves.Count; i++)
		{
			curveValue[i] = CurveEval(currentRumble.RumbleCurves[i], age);
			switch (currentRumble.RumbleCurves[i].Modulator)
			{
			case ModulatorType.Sin:
				curveValue[i] *= Mathf.Sin(currentRumble.RumbleCurves[i].SinFrequency * age);
				break;
			case ModulatorType.Cos:
				curveValue[i] *= Mathf.Cos(currentRumble.RumbleCurves[i].SinFrequency * age);
				break;
			}
			if (doTransform)
			{
				switch (currentRumble.RumbleCurves[i].Channel)
				{
				case ChannelType.Position:
					vector += curveValue[i] * currentRumble.RumbleCurves[i].ChannelMultiplier;
					vector += currentRumble.RumbleCurves[i].ChannelOffset;
					break;
				case ChannelType.Rotation:
					vector2 += curveValue[i] * currentRumble.RumbleCurves[i].ChannelMultiplier;
					vector2 += currentRumble.RumbleCurves[i].ChannelOffset;
					break;
				case ChannelType.Scale:
					vector3 += curveValue[i] * currentRumble.RumbleCurves[i].ChannelMultiplier;
					vector3 += currentRumble.RumbleCurves[i].ChannelOffset;
					break;
				case ChannelType.NguiAlpha:
					num2 += curveValue[i] * currentRumble.RumbleCurves[i].ChannelMultiplier.x;
					num2 += currentRumble.RumbleCurves[i].ChannelOffset.x;
					flag = true;
					break;
				case ChannelType.MaterialAlpha:
					num2 += curveValue[i] * currentRumble.RumbleCurves[i].ChannelMultiplier.x;
					num2 += currentRumble.RumbleCurves[i].ChannelOffset.x;
					flag2 = true;
					break;
				}
			}
			num += curveValue[i];
		}
		if (doTransform)
		{
			base.transform.ConditionalSetPosition(localCoord: true, BasePosition + vector, currentRumble.HasPosCurve);
			base.transform.ConditionalSetRotation(localCoord: true, BaseRotation + vector2, currentRumble.HasRotCurve);
			base.transform.ConditionalSetScale(localCoord: true, BaseScale + vector3, currentRumble.HasScaleCurve);
			if (targetUIwidget != null && flag)
			{
				targetUIwidget.alpha = num2;
			}
			if (TargetMaterial != null && flag2 && Application.isPlaying)
			{
				if (!TargetMaterial.HasProperty(MaterialParameters.Color) && !TargetMaterial.HasProperty(MaterialParameters.TintColor))
				{
					Debug.LogWarning("Object " + base.name + " material " + TargetMaterial.name + " does not have color property");
				}
				else
				{
					if (TargetMaterial.HasProperty(MaterialParameters.Color))
					{
						TargetMaterial.color = new Color(TargetMaterial.color.r, TargetMaterial.color.g, TargetMaterial.color.b, num2);
					}
					if (TargetMaterial.HasProperty(MaterialParameters.TintColor))
					{
						Color color = TargetMaterial.GetColor(MaterialParameters.TintColor);
						TargetMaterial.SetColor(MaterialParameters.TintColor, new Color(color.r, color.g, color.b, num2));
					}
				}
			}
		}
		return curveValue;
	}

	private float CurveEval(RumbleCurve curve, float time)
	{
		float result = 0f;
		switch (curve.Curvetype)
		{
		case CurveType.ADSR:
			result = ADSRCurve(curve, time);
			break;
		case CurveType.Ballistic:
			result = ParabolicCurve(curve, time);
			break;
		}
		return result;
	}

	private float ParabolicCurve(RumbleCurve c, float time)
	{
		time = Mathf.Max(0f, time - c.StartDelay);
		float num = 0.4f;
		float num2 = c.BallisticV / c.BallisticG;
		float result = (0f - c.BallisticG) * time * time + c.BallisticV * time;
		if (time > num2)
		{
			time -= num2;
			result = (0f - c.BallisticG) * time * time + num * c.BallisticV * time;
			float num3 = num * c.BallisticV / c.BallisticG;
			if (time > num3)
			{
				result = 0f;
			}
		}
		return result;
	}

	private float ADSRCurve(RumbleCurve c, float time)
	{
		float result = c.StartLevel;
		float startDelay = c.StartDelay;
		float num = c.StartDelay + c.AttackDuration;
		float num2 = c.StartDelay + c.AttackDuration + c.DecayDuration;
		float num3 = c.StartDelay + c.AttackDuration + c.DecayDuration + c.SustainDuration;
		float b = c.StartDelay + c.AttackDuration + c.DecayDuration + c.SustainDuration + c.ReleaseDuration;
		if (time >= startDelay && time < num)
		{
			float num4 = Mathf.InverseLerp(startDelay, num, time);
			float num5 = Mathf.Pow(1f - num4, 2f);
			result = c.AttackLevel * (1f - num5) + c.StartLevel * num5;
		}
		if (time >= num && time < num2)
		{
			float t = Mathf.InverseLerp(num, num2, time);
			result = Mathf.SmoothStep(c.AttackLevel, c.SustainLevel, t);
		}
		if (time >= num2 && time < num3)
		{
			result = c.SustainLevel;
		}
		if (time >= num3)
		{
			float t2 = Mathf.InverseLerp(num3, b, time);
			result = Mathf.SmoothStep(c.SustainLevel, 0f, t2);
		}
		return result;
	}

	public string[] GetPresets()
	{
		presets = UnityUtils.LoadFromAssetBundle<EffectRumblePresets>(presetPath, "scriptableobjects");
		if (presets == null)
		{
			Debug.LogError("Failed to load rumble presets at " + presetPath);
			return new string[0];
		}
		List<string> list = new List<string>();
		list.Add("No Preset Connection");
		foreach (RumblePreset rumblePreset in presets.RumblePresets)
		{
			list.Add(rumblePreset.name);
		}
		return list.ToArray();
	}

	public void SavePreset(string presetName)
	{
		RumblePreset rumblePreset = new RumblePreset();
		rumblePreset.name = presetName;
		presets.RumblePresets.Add(rumblePreset);
	}

	public void LoadPreset(int presetIndex)
	{
		if (presetIndex > 0 && presetIndex < presets.RumblePresets.Count)
		{
			currentRumble = presets.RumblePresets[presetIndex - 1];
			return;
		}
		if (currentRumble == null)
		{
			currentRumble = new RumblePreset();
		}
		else
		{
			currentRumble = RumbleDeepCopy(currentRumble);
		}
		currentRumble.name = "New Preset";
	}

	public void CreatePreset()
	{
		presets.RumblePresets.Add(RumbleDeepCopy(currentRumble));
	}

	private RumblePreset RumbleDeepCopy(RumblePreset source)
	{
		RumblePreset rumblePreset = new RumblePreset();
		rumblePreset.name = source.name;
		foreach (RumbleCurve rumbleCurf in source.RumbleCurves)
		{
			RumbleCurve rumbleCurve = new RumbleCurve();
			rumbleCurve.Curvetype = rumbleCurf.Curvetype;
			rumbleCurve.Modulator = rumbleCurf.Modulator;
			rumbleCurve.Channel = rumbleCurf.Channel;
			rumbleCurve.ChannelMultiplier = rumbleCurf.ChannelMultiplier;
			rumbleCurve.StartDelay = rumbleCurf.StartDelay;
			rumbleCurve.SinFrequency = rumbleCurf.SinFrequency;
			rumbleCurve.AttackDuration = rumbleCurf.AttackDuration;
			rumbleCurve.AttackLevel = rumbleCurf.AttackLevel;
			rumbleCurve.DecayDuration = rumbleCurf.DecayDuration;
			rumbleCurve.SustainDuration = rumbleCurf.SustainDuration;
			rumbleCurve.SustainLevel = rumbleCurf.SustainLevel;
			rumbleCurve.ReleaseDuration = rumbleCurf.ReleaseDuration;
			rumbleCurve.BallisticG = rumbleCurf.BallisticG;
			rumbleCurve.BallisticV = rumbleCurf.BallisticV;
			rumblePreset.RumbleCurves.Add(rumbleCurve);
		}
		return rumblePreset;
	}

	private void NotifyEffectFinished()
	{
		this.EffectFinished?.Invoke(base.gameObject);
	}
}
