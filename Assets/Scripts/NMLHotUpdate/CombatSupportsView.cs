using System.Collections.Generic;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class CombatSupportsView : ModelView<CombatSupportManager>
{
	private const int TargetEffectPoolSize = 10;

	private IDictionary<SupportModel, CombatSupportView> views;

	private IDictionary<SupportModel, GameObject> targetEffectPrefabs;

	private IDictionary<SupportModel, CombatSupportGroundTargetView> groundTragetEffects;

	public static CombatSupportsView Instance { get; private set; }

	private void Start()
	{
		if (GameManager.Instance.modelManager == null || GameManager.Instance.modelManager.CombatModel == null)
		{
			return;
		}
		Initialize(GameManager.Instance.modelManager.CombatModel.SupportManager);
		Instance = this;
		SupportViewResources supportViewResources = UnityUtils.LoadFromAssetBundle<SupportViewResources>("SupportViewResources", "scriptableobjects");
		views = new Dictionary<SupportModel, CombatSupportView>();
		targetEffectPrefabs = new Dictionary<SupportModel, GameObject>();
		groundTragetEffects = new Dictionary<SupportModel, CombatSupportGroundTargetView>();
		Transform uIParent = SingularityMonoBehaviour<HUDManager>.Instance.UIParent;
		foreach (CombatSupportModel support in base.Model.Supports)
		{
			SupportViewResourceEntry resources = supportViewResources.GetResources(support.SupportModel.SupportId);
			if (resources != null)
			{
				if (!string.IsNullOrEmpty(resources.MainEffectResourceAddress))
				{
					CombatSupportView component = Object.Instantiate(UnityUtils.LoadFromAssetBundle<PrefabResource>(resources.MainEffectResourceAddress, "scriptableobjects").GetPrefab(), uIParent, worldPositionStays: false).GetComponent<CombatSupportView>();
					views[support.SupportModel] = component;
				}
				if (!string.IsNullOrEmpty(resources.TargetEffectResourceAddress))
				{
					PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>(resources.TargetEffectResourceAddress, "scriptableobjects");
					SingularityMonoBehaviour<ObjectPoolManager>.Instance.SetupCacheForObject(prefabResource.GetPrefab(), 10);
					targetEffectPrefabs[support.SupportModel] = prefabResource.GetPrefab();
				}
				if (!string.IsNullOrEmpty(resources.GroundTargetEffectResourceAddress))
				{
					CombatSupportGroundTargetView component2 = Object.Instantiate(UnityUtils.LoadFromAssetBundle<PrefabResource>(resources.GroundTargetEffectResourceAddress, "scriptableobjects").GetPrefab()).GetComponent<CombatSupportGroundTargetView>();
					component2.Initialize(support);
					component2.gameObject.SetActive(value: false);
					groundTragetEffects[support.SupportModel] = component2;
				}
			}
		}
	}

	public void SupportExecuted(SupportModel supportModel, GridCoordinate targetCenter, IEnumerable<ActorModel> targets)
	{
		if (views.TryGetValue(supportModel, out var value))
		{
			value.Execute();
		}
		if (targetEffectPrefabs.TryGetValue(supportModel, out var value2))
		{
			foreach (ActorModel target in targets)
			{
				ActorView actorViewFromModel = CombatView.Instance.GetActorViewFromModel(target);
				if ((bool)actorViewFromModel)
				{
					SingularityMonoBehaviour<ObjectPoolManager>.Instance.FetchObject(value2, actorViewFromModel.transform).transform.Reset();
				}
			}
		}
		if (groundTragetEffects.TryGetValue(supportModel, out var value3))
		{
			Vector3 position = supportModel.manager.CombatModel.Grid.GetPosition(targetCenter).ToVector3();
			value3.gameObject.SetActive(value: true);
			value3.Execute(position);
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_support/" + supportModel.SupportId.ToLower());
	}

	private void OnDestroy()
	{
		foreach (KeyValuePair<SupportModel, CombatSupportView> view in views)
		{
			if ((bool)view.Value)
			{
				Object.Destroy(view.Value.gameObject);
			}
		}
		views.Clear();
	}
}
