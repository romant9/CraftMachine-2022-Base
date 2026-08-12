using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class CombatAreasView : CombatModelView
{
	[SerializeField]
	private string effectPath;

	[SerializeField]
	private string activenessTrait;

	private const int PoolSize = 20;

	private GameObject actorEffectPrefab;

	private IDictionary<ActorModel, CombatAreaActorEffectView> deployedActorEffects;

	private CombatModel combatModel;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		((CombatAreasManager)model).ActorAreaRefreshed += OnActorAreasRefresh;
		deployedActorEffects = new Dictionary<ActorModel, CombatAreaActorEffectView>();
		actorEffectPrefab = UnityUtils.LoadFromAssetBundle<PrefabResource>(effectPath, "scriptableobjects").GetPrefab();
		SingularityMonoBehaviour<ObjectPoolManager>.Instance.SetupCacheForObject(actorEffectPrefab, 20);
		combatModel = GameManager.Instance.modelManager.CombatModel;
		combatModel.Changed += CombatModelOnChanged;
		OnActorAreasRefresh();
	}

	protected override void OnDestroy()
	{
		if (base.Model is CombatAreasManager combatAreasManager)
		{
			combatAreasManager.ActorAreaRefreshed -= OnActorAreasRefresh;
		}
		if (combatModel != null)
		{
			combatModel.Changed -= CombatModelOnChanged;
		}
		base.OnDestroy();
	}

	private void OnActorAreasRefresh(ActorModel actor = null)
	{
		CombatModel combatModel = base.Model.manager.CombatModel;
		if (actor == null)
		{
			RefreshActorEffects(combatModel.Survivors);
			RefreshActorEffects(combatModel.Raiders);
			RefreshActorEffects(combatModel.Walkers);
		}
		else
		{
			RefreshActorEffects(actor);
		}
	}

	private void CombatModelOnChanged(ModelObject model, string changed, object args)
	{
		if (changed == "actorBecameVisible" || changed == "actorBecameHidden")
		{
			RefreshActorEffects((ActorModel)args);
		}
	}

	private void RefreshActorEffects(IList<ActorModel> actors)
	{
		if (actors == null)
		{
			return;
		}
		foreach (ActorModel actor in actors)
		{
			RefreshActorEffects(actor);
		}
	}

	private void RefreshActorEffects(ActorModel actor)
	{
		bool num = actor.HasTrait(activenessTrait);
		bool flag = deployedActorEffects.ContainsKey(actor);
		bool flag2 = num && actor.IsVisibleToSurvivors;
		if (flag2 && !flag)
		{
			SpawnActorEffect(actor);
		}
		else if (!flag2 && flag)
		{
			DespawnActorEffect(actor);
		}
	}

	private void SpawnActorEffect(ActorModel actor)
	{
		if (deployedActorEffects.Count < 20)
		{
			ModelView<ActorModel> viewForModel = GameManager.Instance.GetViewForModel(actor);
			if ((bool)viewForModel)
			{
				CombatAreaActorEffectView component = SingularityMonoBehaviour<ObjectPoolManager>.Instance.FetchObject(actorEffectPrefab, viewForModel.transform).GetComponent<CombatAreaActorEffectView>();
				component.transform.Reset();
				component.Show(actor.Faction);
				deployedActorEffects.Add(actor, component);
			}
		}
	}

	private void DespawnActorEffect(ActorModel actor)
	{
		CombatAreaActorEffectView effect = deployedActorEffects[actor];
		effect.StartKill(delegate
		{
			SingularityMonoBehaviour<ObjectPoolManager>.Instance.ReturnObjectToPool(effect.gameObject);
			deployedActorEffects.Remove(actor);
		});
	}
}
