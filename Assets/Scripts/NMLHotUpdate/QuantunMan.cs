using UnityEngine;

public class QuantunMan : MonoBehaviour
{
	private GameObject effectPrefab;

	private static string effectPrefabPath = "FxInfo_Debuff_Bleed";

	private bool visible = true;

	private GameObject parentHips;

	private bool inited;

	private GameObject effectHips;

	private float delayedDestroyTimeoutSeconds;

	private bool delayedDestroy;

	private float timer;

	public void Start()
	{
		if (!inited)
		{
			Init();
		}
	}

	public void OnEnable()
	{
		if (!inited)
		{
			Init();
		}
		AddEffect();
	}

	public void SetDelayedDestroyDelay(float inSeconds)
	{
		delayedDestroyTimeoutSeconds = inSeconds;
		delayedDestroy = true;
		timer = 0f;
	}

	public void Update()
	{
		if (delayedDestroy)
		{
			timer += Time.deltaTime;
			if (timer >= delayedDestroyTimeoutSeconds)
			{
				RemoveEffect();
				delayedDestroy = false;
			}
		}
	}

	public void SetVisibility(bool setVisible)
	{
		if (visible && !setVisible && base.enabled)
		{
			OnDisable();
			visible = setVisible;
		}
		if (!visible && setVisible && base.enabled)
		{
			OnEnable();
			visible = setVisible;
		}
	}

	private void Init()
	{
		if (base.transform != null)
		{
			parentHips = base.transform.FindInChildren("Bind_Spine")?.gameObject;
			effectPrefab = UnityUtils.LoadFromAssetBundle<PrefabResource>(effectPrefabPath, "scriptableobjects").GetPrefab();
			inited = true;
		}
	}

	private void AddEffect()
	{
		effectHips = Helpers.InstantiateToParent(effectPrefab, parentHips);
	}

	public void OnDisable()
	{
		if (!inited)
		{
			Init();
		}
		RemoveEffect();
	}

	private void RemoveEffect()
	{
		if (effectHips != null)
		{
			Object.Destroy(effectHips);
		}
		_ = SingularityMonoBehaviour<AudioManager>.Instance != null;
	}
}
