using TWDModel;
using UnityEngine;

public class SurvivalGameMan : MonoBehaviour
{
	private GameObject effectPrefabLeader;

	private GameObject effectPrefabEnemy;

	private static string effectPrefabPathLeader = "BattleEffect_Croat_SurvivalGame_Loop";

	private static string effectPrefabPathEnemy = "BattleEffect_Croat_SurvivalGame";

	private bool visible = true;

	private GameObject parentHipsEnemy;

	private GameObject parentHipsLeader;

	private bool inited;

	private GameObject effectHipsLeader;

	private GameObject effectHipsEnemy;

	private float delayedDestroyTimeoutSeconds;

	private bool delayedDestroy;

	private float timer;

	private ActorModel attachedActor;

	public void BindData(ActorModel actor)
	{
		attachedActor = actor;
	}

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
			parentHipsEnemy = base.transform.FindInChildren("Bind_Spine")?.gameObject;
			parentHipsLeader = base.transform.FindInChildren("Rootbone")?.gameObject;
			effectPrefabLeader = UnityUtils.LoadFromAssetBundle<PrefabResource>(effectPrefabPathLeader, "scriptableobjects").GetPrefab();
			effectPrefabEnemy = UnityUtils.LoadFromAssetBundle<PrefabResource>(effectPrefabPathEnemy, "scriptableobjects").GetPrefab();
			inited = true;
		}
	}

	private void AddEffect()
	{
		if (attachedActor != null && !attachedActor.IsDead)
		{
			if (attachedActor.IsSurvivalGameLeader())
			{
				effectHipsLeader = Helpers.InstantiateToParent(effectPrefabLeader, parentHipsLeader);
			}
			if (attachedActor.IsSurvivalGameEnemy())
			{
				effectHipsEnemy = Helpers.InstantiateToParent(effectPrefabEnemy, parentHipsEnemy);
				effectHipsLeader = Helpers.InstantiateToParent(effectPrefabLeader, parentHipsLeader);
			}
		}
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
		if (effectHipsLeader != null)
		{
			Object.Destroy(effectHipsLeader);
		}
		if (effectHipsEnemy != null)
		{
			Object.Destroy(effectHipsEnemy);
		}
		_ = SingularityMonoBehaviour<AudioManager>.Instance != null;
	}
}
