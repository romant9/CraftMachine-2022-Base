using System;
using System.Collections.Generic;
using UnityEngine;

public class PoisonMan : MonoBehaviour
{
	[Serializable]
	public class Replacements
	{
		public Shader FromShader;

		public Shader ToShader;
	}

	private GameObject FirePrefab;

	public bool Hips;

	public bool Legs = true;

	public bool Arms;

	public List<Replacements> FireShaderReplacements;

	private static string firePrefabPath = "PoisonWalker";

	private bool visible = true;

	private GameObject parentHips;

	private GameObject parentLeftArm;

	private GameObject parentRightArm;

	private GameObject parentLeftLeg;

	private GameObject parentRightLeg;

	private Transform thisTransform;

	private bool inited;

	private GameObject fireHips;

	private GameObject fireLArm;

	private GameObject fireRArm;

	private GameObject fireLLeg;

	private GameObject fireRLeg;

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
		AddFire();
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
				RemoveFire();
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
		thisTransform = base.gameObject.transform;
		parentHips = thisTransform.FindInChildren("Bind_Hips")?.gameObject;
		parentLeftArm = thisTransform.FindInChildren("Bind_LeftHand")?.gameObject;
		parentRightArm = thisTransform.FindInChildren("Bind_RightHand")?.gameObject;
		parentLeftLeg = thisTransform.FindInChildren("Bind_LeftFoot")?.gameObject;
		parentRightLeg = thisTransform.FindInChildren("Bind_RightFoot")?.gameObject;
		FirePrefab = UnityUtils.LoadFromAssetBundle<PrefabResource>(firePrefabPath, "scriptableobjects").GetPrefab();
		inited = true;
	}

	private void AddFire()
	{
		fireHips = Helpers.InstantiateToParent(FirePrefab, parentHips);
		_ = SingularityMonoBehaviour<AudioManager>.Instance != null;
	}

	public void OnDisable()
	{
		if (!inited)
		{
			Init();
		}
		RemoveFire();
	}

	private void RemoveFire()
	{
		if (fireHips != null)
		{
			UnityEngine.Object.Destroy(fireHips);
		}
		if (fireLArm != null)
		{
			UnityEngine.Object.Destroy(fireLArm);
		}
		if (fireRArm != null)
		{
			UnityEngine.Object.Destroy(fireRArm);
		}
		if (fireLLeg != null)
		{
			UnityEngine.Object.Destroy(fireLLeg);
		}
		if (fireRLeg != null)
		{
			UnityEngine.Object.Destroy(fireRLeg);
		}
		_ = SingularityMonoBehaviour<AudioManager>.Instance != null;
	}
}
