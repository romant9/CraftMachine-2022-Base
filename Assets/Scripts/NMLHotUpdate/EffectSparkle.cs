using System;
using System.Collections.Generic;
using Client.Constants;
using UnityEngine;

public class EffectSparkle : MonoBehaviour
{
	[Serializable]
	public class SparkleBase
	{
		public SparkleEffectType SparkleType;

		public float Duration = 3f;

		public float FadeInOut = 0.6f;

		[Tooltip("Render Queue Offset relative to ui widget")]
		public int QueueOffset;

		public bool UseWidgetAspect = true;

		public Color TintColor = Color.white;

		public float GlowScale = 1f;

		public float GlowNoiseSpeed = 1f;

		public float GlowNoiseAmount = 20f;

		public float GlowInitialRot;

		public Vector3 GlintScale = new Vector3(1f, 1f, 1f);

		public float GlintScrollSpeed = 0.5f;

		public float ParticleGravity;

		public float ParticleSpeed = 1f;

		public float ParticleScale = 0.02f;

		public Texture ParticleTexture;

		public float BurnSpeed = 0.8f;

		public Texture EdgeTexture;

		public float EdgeThickness = 0.05f;

		public float EdgeTextureScale = 0.5f;

		public float EdgeDualtexBlend = 0.5f;

		public Vector2 EdgeUvScrollSpeed = new Vector2(1f, 0f);

		public Vector2 EdgeUvScrollSpeed2 = new Vector2(1f, 0f);

		public Texture FlareTexture;

		public Vector3 FlareSize = new Vector3(0.05f, 0.05f, 0.05f);

		public float FlareSpeed = 0.5f;

		public float FlareRotSpeed = 0.5f;

		public float FlareStartRandomness;

		public bool FlareLoop;
	}

	public enum SparkleEffectType
	{
		Particle = 0,
		Glow = 1,
		Flash = 2,
		Swipe = 3,
		Glint = 4,
		Burn = 5,
		Edges = 6,
		Circumscribe = 7,
		Embers = 8
	}

	public enum LoopType
	{
		PlayUntilDisabled = 0,
		Respawn = 1,
		DeleteAfter = 2
	}

	public SparklePreset currentSparkle;

	public bool ResetOnWidgetChange;

	[HideInInspector]
	public int selectedPreset;

	private float startTime;

	private DateTime startDate;

	private static readonly string presetPath = "Effects/EffectSparklePresets";

	private static readonly string dataPath = "Effects/EffectSparkleData";

	private EffectSparklePresets presets;

	private UIWidget targetUIwidget;

	private float age;

	private bool playing = true;

	private bool initialized;

	private MeshRenderer meshRenderer;

	private List<GameObject> effectGOList;

	private EffectSparkleData data;

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

	public void Start()
	{
		if (OfflineManager.IsNoEffects) return;
		if (!initialized)
		{
			InitSparkle();
		}
	}

	public void InitSparkle()
	{
		meshRenderer = base.gameObject.GetComponent<MeshRenderer>();
		targetUIwidget = base.gameObject.GetComponent<UIWidget>();
		data = UnityUtils.LoadFromAssetBundle<EffectSparkleData>(dataPath, "scriptableobjects");
		initialized = true;
		if (Application.isPlaying)
		{
			playing = true;
		}
		else
		{
			playing = false;
		}
		int num = 0;
		GetPresets();
		if (num > 0)
		{
			LoadPreset(num);
		}
		else if (currentSparkle == null)
		{
			currentSparkle = new SparklePreset();
		}
		ResetTimer();
	}

	public bool IsPlaying()
	{
		return playing;
	}

	public void OnEnable()
	{
		if (OfflineManager.IsNoEffects) return;

		if (!initialized)
		{
			InitSparkle();
		}
		SpawnEffect();
		if (targetUIwidget != null)
		{
			UIWidget uIWidget = targetUIwidget;
			uIWidget.onChange = (UIWidget.OnDimensionsChanged)Delegate.Combine(uIWidget.onChange, new UIWidget.OnDimensionsChanged(OnWidgetChange));
			UIWidget uIWidget2 = targetUIwidget;
			uIWidget2.OnAnchorDimensionsChangedChange = (UIWidget.OnAnchorDimensionsChanged)Delegate.Combine(uIWidget2.OnAnchorDimensionsChangedChange, new UIWidget.OnAnchorDimensionsChanged(OnWidgetAnchorChange));
		}
	}

	public void OnDisable()
	{
		if (OfflineManager.IsNoEffects) return;

		if (targetUIwidget != null)
		{
			UIWidget uIWidget = targetUIwidget;
			uIWidget.onChange = (UIWidget.OnDimensionsChanged)Delegate.Remove(uIWidget.onChange, new UIWidget.OnDimensionsChanged(OnWidgetChange));
			UIWidget uIWidget2 = targetUIwidget;
			uIWidget2.OnAnchorDimensionsChangedChange = (UIWidget.OnAnchorDimensionsChanged)Delegate.Remove(uIWidget2.OnAnchorDimensionsChangedChange, new UIWidget.OnAnchorDimensionsChanged(OnWidgetAnchorChange));
		}
		if (!initialized || effectGOList == null)
		{
			return;
		}
		if (Application.isPlaying)
		{
			for (int i = 0; i < effectGOList.Count; i++)
			{
				UnityEngine.Object.Destroy(effectGOList[i]);
			}
		}
		else
		{
			for (int j = 0; j < effectGOList.Count; j++)
			{
				UnityEngine.Object.DestroyImmediate(effectGOList[j]);
			}
		}
		effectGOList.Clear();
	}

	private void OnWidgetChange()
	{
		if (ResetOnWidgetChange)
		{
			OnDisable();
			OnEnable();
		}
	}

	private void OnWidgetAnchorChange()
	{
		if (ResetOnWidgetChange)
		{
			OnDisable();
			OnEnable();
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
	}

	public void Update()
	{
		if (Application.isPlaying)
		{
			age = Time.time - startTime;
			return;
		}
		TimeSpan timeSpan = DateTime.Now - startDate;
		age = 60f * (float)timeSpan.Minutes + (float)timeSpan.Seconds + 0.001f * (float)timeSpan.Milliseconds;
	}

	public void SpawnEffect()
	{
		ResetTimer();
		bool flag = true;
		if (effectGOList == null)
		{
			effectGOList = new List<GameObject>();
		}
		effectGOList.Clear();
		foreach (SparkleBase sparkle in currentSparkle.Sparkles)
		{
			Vector3 vector = targetUIwidget.worldCorners[2] - targetUIwidget.worldCorners[0];
			if (!sparkle.UseWidgetAspect)
			{
				float num = (vector.x + vector.y + vector.z) * 0.5f;
				vector = new Vector3(num, num, num);
			}
			GameObject gameObject = null;
			switch (sparkle.SparkleType)
			{
			case SparkleEffectType.Particle:
			{
				gameObject = UnityEngine.Object.Instantiate(data.ParticleEffectPrefab);
				gameObject.transform.localScale = vector;
				gameObject.transform.position = targetUIwidget.worldCenter;
				gameObject.transform.parent = base.gameObject.transform;
				flag = false;
				ParticleSystem[] componentsInChildren = gameObject.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particleSystem in componentsInChildren)
				{
					ParticleSystem.MainModule main = particleSystem.main;
					main.startLifetime = sparkle.Duration;
					main.gravityModifier = sparkle.ParticleGravity;
					main.startSpeed = sparkle.ParticleSpeed;
					main.startColor = sparkle.TintColor;
					if (sparkle.ParticleTexture != null)
					{
						particleSystem.GetComponent<Renderer>().material.mainTexture = sparkle.ParticleTexture;
					}
				}
				break;
			}
			case SparkleEffectType.Embers:
			{
				gameObject = UnityEngine.Object.Instantiate(data.EmbersEffectPrefab);
				gameObject.transform.localScale = vector * sparkle.GlowScale;
				gameObject.transform.position = targetUIwidget.worldCenter;
				gameObject.transform.parent = base.gameObject.transform;
				ParticleSystem[] componentsInChildren2 = gameObject.GetComponentsInChildren<ParticleSystem>();
				flag = false;
				ParticleSystem[] componentsInChildren = componentsInChildren2;
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					ParticleSystem.MainModule main2 = componentsInChildren[i].main;
					main2.startLifetime = sparkle.Duration;
					main2.gravityModifier = sparkle.ParticleGravity;
					main2.startColor = sparkle.TintColor;
				}
				break;
			}
			case SparkleEffectType.Glint:
			{
				gameObject = UnityEngine.Object.Instantiate(data.GlintEffectPrefab);
				gameObject.transform.localScale = Vector3.Scale(vector, sparkle.GlintScale);
				gameObject.transform.position = targetUIwidget.worldCenter;
				gameObject.transform.parent = base.gameObject.transform;
				MeshRenderer componentInChildren3 = gameObject.GetComponentInChildren<MeshRenderer>();
				if (componentInChildren3 != null && targetUIwidget.drawCall != null)
				{
					componentInChildren3.material.renderQueue = targetUIwidget.drawCall.finalRenderQueue + sparkle.QueueOffset;
				}
				componentInChildren3.material.SetColor(MaterialParameters.TintColor, sparkle.TintColor);
				UvScroll componentInChildren4 = gameObject.GetComponentInChildren<UvScroll>();
				if (componentInChildren4 != null)
				{
					componentInChildren4.uvScrollSpeed = new Vector2(sparkle.GlintScrollSpeed, 0f);
				}
				break;
			}
			case SparkleEffectType.Glow:
			{
				gameObject = UnityEngine.Object.Instantiate(data.GlowEffectPrefab);
				gameObject.transform.position = targetUIwidget.worldCenter;
				gameObject.transform.localScale = vector * sparkle.GlowScale;
				gameObject.transform.localEulerAngles = new Vector3(0f, 0f, sparkle.GlowInitialRot);
				gameObject.transform.parent = base.gameObject.transform;
				EffectRotationNoise componentInChildren = gameObject.GetComponentInChildren<EffectRotationNoise>();
				if (componentInChildren != null)
				{
					componentInChildren.Speed = sparkle.GlowNoiseSpeed;
					componentInChildren.Amount = sparkle.GlowNoiseAmount;
					componentInChildren.Offset = sparkle.GlowNoiseAmount * UnityEngine.Random.value;
				}
				MeshRenderer componentInChildren2 = gameObject.GetComponentInChildren<MeshRenderer>();
				componentInChildren2.material.SetColor(MaterialParameters.TintColor, sparkle.TintColor);
				if (componentInChildren2 != null && targetUIwidget.drawCall != null)
				{
					componentInChildren2.material.renderQueue = targetUIwidget.drawCall.finalRenderQueue + sparkle.QueueOffset;
				}
				break;
			}
			case SparkleEffectType.Burn:
				targetUIwidget.material = data.BurnMaterial;
				targetUIwidget.material.SetFloat(MaterialParameters.WipeSpeed, sparkle.BurnSpeed);
				targetUIwidget.onRender = OnRenderBurn;
				break;
			case SparkleEffectType.Edges:
			{
				gameObject = new GameObject();
				gameObject.AddComponent<MeshFilter>();
				MeshRenderer obj = gameObject.AddComponent<MeshRenderer>();
				gameObject.name = "EffectEdge";
				ThickLineRenderer thickLineRenderer = gameObject.AddComponent<ThickLineRenderer>();
				thickLineRenderer.LineThickness = sparkle.EdgeThickness;
				thickLineRenderer.StartFadeOutDistance = 1f;
				thickLineRenderer.EndFadeOutDistance = 1f;
				thickLineRenderer.extrudeMode = MeshGenerator.ExtrudeMode.Outwards;
				thickLineRenderer.TextureScale = sparkle.EdgeTextureScale;
				thickLineRenderer.SetPoints(CornerLine(targetUIwidget.worldCorners), new Vector3(0f, 0f, 1f));
				gameObject.transform.parent = base.gameObject.transform;
				UvScroll uvScroll = gameObject.AddComponent<UvScroll>();
				uvScroll.uvScrollSpeed = sparkle.EdgeUvScrollSpeed;
				uvScroll.uvScrollSpeed2 = sparkle.EdgeUvScrollSpeed2;
				gameObject.GetComponent<MeshRenderer>().sharedMaterial = data.EdgeMaterial;
				obj.material.SetColor(MaterialParameters.TintColor, sparkle.TintColor);
				obj.material.SetFloat(MaterialParameters.SecondBlend, sparkle.EdgeDualtexBlend);
				if (sparkle.EdgeTexture != null)
				{
					gameObject.GetComponent<MeshRenderer>().material.mainTexture = sparkle.EdgeTexture;
					gameObject.GetComponent<MeshRenderer>().material.SetTexture("_SecondTex", sparkle.EdgeTexture);
				}
				if (targetUIwidget.drawCall != null)
				{
					gameObject.GetComponent<Renderer>().material.renderQueue = targetUIwidget.drawCall.finalRenderQueue + sparkle.QueueOffset;
				}
				gameObject.AddComponent<EffectEditModeUpdate>().updateComponents = true;
				break;
			}
			case SparkleEffectType.Circumscribe:
			{
				gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
				gameObject.name = "EffectFlare";
				UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<MeshCollider>());
				gameObject.transform.localScale = sparkle.FlareSize;
				PolylinePath path = CornerPath(CornerLine(targetUIwidget.worldCorners));
				EffectMoveAlongPolyPath effectMoveAlongPolyPath = gameObject.AddComponent<EffectMoveAlongPolyPath>();
				if (targetUIwidget.drawCall != null)
				{
					gameObject.GetComponent<Renderer>().material.renderQueue = targetUIwidget.drawCall.finalRenderQueue + sparkle.QueueOffset;
				}
				effectMoveAlongPolyPath.Path = path;
				effectMoveAlongPolyPath.Speed = sparkle.FlareSpeed;
				effectMoveAlongPolyPath.RotSpeed = sparkle.FlareRotSpeed;
				effectMoveAlongPolyPath.StartOffset = UnityEngine.Random.value * sparkle.FlareStartRandomness;
				effectMoveAlongPolyPath.Loop = sparkle.FlareLoop;
				effectMoveAlongPolyPath.Offset = new Vector3(0f, 0f, -0.01f);
				MeshRenderer component = effectMoveAlongPolyPath.GetComponent<MeshRenderer>();
				component.material = data.CircumscribeFlareMaterial;
				component.material.SetColor(MaterialParameters.TintColor, sparkle.TintColor);
				if (sparkle.FlareTexture != null)
				{
					gameObject.GetComponent<MeshRenderer>().material.mainTexture = sparkle.FlareTexture;
				}
				gameObject.AddComponent<EffectEditModeUpdate>().updateComponents = true;
				gameObject.transform.parent = base.gameObject.transform;
				break;
			}
			}
			if (gameObject != null)
			{
				gameObject.layer = base.gameObject.layer;
				EffectEditorDelayedDestroy effectEditorDelayedDestroy = gameObject.GetComponent<EffectEditorDelayedDestroy>();
				if (effectEditorDelayedDestroy == null)
				{
					effectEditorDelayedDestroy = gameObject.AddComponent<EffectEditorDelayedDestroy>();
				}
				effectEditorDelayedDestroy.DestroyAfterDelay = true;
				effectEditorDelayedDestroy.Delay = sparkle.Duration;
				if (flag)
				{
					EffectFadeInOut effectFadeInOut = gameObject.GetComponent<EffectFadeInOut>();
					if (effectFadeInOut == null)
					{
						effectFadeInOut = gameObject.AddComponent<EffectFadeInOut>();
					}
					effectFadeInOut.Duration = sparkle.Duration;
					effectFadeInOut.Fade = sparkle.FadeInOut;
				}
			}
			if (gameObject != null)
			{
				effectGOList.Add(gameObject);
			}
		}
	}

	private PolylinePath CornerPath(List<Vector3> points)
	{
		Vector3 vector = new Vector3(0f, 0f, -1f);
		PolylinePath polylinePath = new PolylinePath();
		List<Line> list = new List<Line>();
		for (int i = 0; i < points.Count - 1; i++)
		{
			list.Add(new Line(points[i], points[i + 1]));
		}
		for (int j = 0; j < list.Count; j++)
		{
			Line line = list[j];
			Line line2 = ((j + 1 < list.Count) ? list[j + 1] : null);
			if (line2 == null || Vector3.Dot(Vector3.Normalize(line2.end - line2.start), Vector3.Normalize(line.end - line.start)) > 0.95f)
			{
				if (!polylinePath.EndsAtCurve)
				{
					polylinePath.AddSegment(new LineSegment(line.start, line.end, vector));
				}
				else
				{
					polylinePath.AddSegment(new LineSegment(line.center, line.end, vector));
				}
				continue;
			}
			Vector3 startTangent = (line.end - line.center) * 0.5f;
			Vector3 endTangent = (line2.end - line2.center) * 0.5f;
			if (!polylinePath.EndsAtCurve)
			{
				polylinePath.AddSegment(new LineSegment(line.start, line.center, vector));
			}
			polylinePath.AddSegment(new CurveSegment(line.center, line2.center, startTangent, endTangent, vector));
		}
		return polylinePath;
	}

	private List<Vector3> CornerLine(Vector3[] corners, bool flip = false)
	{
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < 4; i++)
		{
			int num = ((i - 1 < 0) ? (i - 1 + 4) : (i - 1));
			int num2 = ((i + 1 <= 3) ? (i + 1) : 0);
			Vector3 normalized = (corners[num] - corners[i]).normalized;
			Vector3 normalized2 = (corners[num2] - corners[i]).normalized;
			Vector3 vector = -(normalized + normalized2).normalized;
			list.Add(corners[i] + currentSparkle.PathRounding * normalized - currentSparkle.PathOffset * normalized2);
			list.Add(targetUIwidget.worldCorners[i] + currentSparkle.PathOffset * vector);
			list.Add(corners[i] + currentSparkle.PathRounding * normalized2 - currentSparkle.PathOffset * normalized);
		}
		list.Add(list[0]);
		if (flip)
		{
			list.Reverse();
		}
		return list;
	}

	public void OnRenderBurn(Material mat)
	{
		float num = mat.GetFloat("_WipeSpeed");
		mat.SetFloat("_Cutoff", age * num);
	}

	public string[] GetPresets()
	{
		presets = UnityUtils.LoadFromAssetBundle<EffectSparklePresets>(presetPath, "scriptableobjects");
		if (presets == null)
		{
			Debug.LogError("Failed to load Sparkle presets at " + presetPath);
			return new string[0];
		}
		List<string> list = new List<string>();
		list.Add("No Preset Connection");
		foreach (SparklePreset sparklePreset in presets.SparklePresets)
		{
			list.Add(sparklePreset.name);
		}
		return list.ToArray();
	}

	public void LoadPreset(int presetIndex)
	{
		if (presetIndex > 0 && presetIndex <= presets.SparklePresets.Count)
		{
			currentSparkle = presets.SparklePresets[presetIndex - 1];
			return;
		}
		if (currentSparkle == null)
		{
			currentSparkle = new SparklePreset();
		}
		else
		{
			currentSparkle = SparkleDeepCopy(currentSparkle);
		}
		currentSparkle.name = "New Preset";
	}

	public void CreatePreset()
	{
		presets.SparklePresets.Add(SparkleDeepCopy(currentSparkle));
	}

	private SparklePreset SparkleDeepCopy(SparklePreset source)
	{
		SparklePreset sparklePreset = new SparklePreset();
		sparklePreset.name = source.name;
		sparklePreset.PathOffset = source.PathOffset;
		sparklePreset.PathRounding = source.PathRounding;
		foreach (SparkleBase sparkle in source.Sparkles)
		{
			SparkleBase sparkleBase = new SparkleBase();
			sparkleBase.BurnSpeed = sparkle.BurnSpeed;
			sparkleBase.Duration = sparkle.Duration;
			sparkleBase.EdgeDualtexBlend = sparkle.EdgeDualtexBlend;
			sparkleBase.EdgeTexture = sparkle.EdgeTexture;
			sparkleBase.EdgeTextureScale = sparkle.EdgeTextureScale;
			sparkleBase.EdgeThickness = sparkle.EdgeThickness;
			sparkleBase.EdgeUvScrollSpeed = sparkle.EdgeUvScrollSpeed;
			sparkleBase.EdgeUvScrollSpeed2 = sparkle.EdgeUvScrollSpeed2;
			sparkleBase.FadeInOut = sparkle.FadeInOut;
			sparkleBase.FlareLoop = sparkle.FlareLoop;
			sparkleBase.FlareRotSpeed = sparkle.FlareRotSpeed;
			sparkleBase.FlareSize = sparkle.FlareSize;
			sparkleBase.FlareSpeed = sparkle.FlareSpeed;
			sparkleBase.FlareStartRandomness = sparkle.FlareStartRandomness;
			sparkleBase.FlareTexture = sparkle.FlareTexture;
			sparkleBase.GlowInitialRot = sparkle.GlowInitialRot;
			sparkleBase.GlowNoiseAmount = sparkle.GlowNoiseAmount;
			sparkleBase.GlowNoiseSpeed = sparkle.GlowNoiseSpeed;
			sparkleBase.GlowScale = sparkle.GlowScale;
			sparkleBase.ParticleGravity = sparkle.ParticleGravity;
			sparkleBase.ParticleScale = sparkle.ParticleScale;
			sparkleBase.ParticleSpeed = sparkle.ParticleSpeed;
			sparkleBase.ParticleTexture = sparkle.ParticleTexture;
			sparkleBase.QueueOffset = sparkle.QueueOffset;
			sparkleBase.SparkleType = sparkle.SparkleType;
			sparkleBase.TintColor = sparkle.TintColor;
			sparkleBase.UseWidgetAspect = sparkle.UseWidgetAspect;
			sparklePreset.Sparkles.Add(sparkleBase);
		}
		return sparklePreset;
	}
}
