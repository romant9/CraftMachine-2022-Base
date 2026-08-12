using UnityEngine;

public class CitadelLeaderMan : MonoBehaviour
{
	private GameObject effectPrefab;

	private static string effectPrefabPath = "Effects/CitadelLeaderMan";

	private bool visible = true;

	private GameObject parentHips;

	private bool inited;

	private GameObject effectHips;

	public void Start()
	{
		if (!inited)
		{
			Init();
		}
	}

	public void OnEnable()
	{
		PlayEffect();
	}

	public void PlayEffect()
	{
		if (!inited)
		{
			Init();
		}
		if (!IsEffectAlive())
		{
			RemoveEffect();
			AddEffect();
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
			parentHips = base.transform.FindInChildren("Bind_LeftFoot")?.gameObject;
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
			effectHips = null;
		}
		_ = SingularityMonoBehaviour<AudioManager>.Instance != null;
	}

	private bool IsEffectAlive()
	{
		if (effectHips == null)
		{
			return false;
		}
		ParticleSystem[] componentsInChildren = effectHips.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i] != null && componentsInChildren[i].IsAlive(withChildren: true))
			{
				return true;
			}
		}
		return false;
	}
}
