using System.Collections;
using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class CampDefenseView : ModelView<CampDefenseModel>
{
	private static CampDefenseView instance;

	private CampDefenseKillWalkerIndicator killIndicator;

	public static CampDefenseView Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType<CampDefenseView>();
			}
			return instance;
		}
	}

	public void SetEnabled(bool enabled)
	{
		base.gameObject.SetActive(enabled);
		if (enabled)
		{
			for (int i = 0; i < base.Model.Walkers.Count; i++)
			{
				ActorView actorView = GameManager.Instance.GetViewForModel((ActorModel)base.Model.Walkers[i]) as ActorView;
				if (actorView == null)
				{
					CreateWalkerView(base.Model.Walkers[i]);
					continue;
				}
				actorView.gameObject.SetActive(value: true);
				if (killIndicator != null && !killIndicator.gameObject.activeSelf)
				{
					StartCoroutine(CreateKillIndicator(base.Model.Walkers[i], actorView));
				}
			}
			return;
		}
		for (int j = 0; j < base.Model.Walkers.Count; j++)
		{
			ActorView actorView2 = GameManager.Instance.GetViewForModel((ActorModel)base.Model.Walkers[j]) as ActorView;
			if (actorView2 != null)
			{
				actorView2.gameObject.SetActive(value: false);
			}
		}
	}

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		base.Model.Changed += OnModelChange;
	}

	protected void OnModelChange(ModelObject m, string changed, object args)
	{
		if (changed == "CampDefenseWalkerAdded" && base.gameObject.activeSelf)
		{
			if (args is WalkerModel walker)
			{
				CreateWalkerView(walker);
			}
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.OnCampDefenseAdded(base.Model.Walkers.Count);
			}
		}
		else
		{
			if (!(changed == "CampDefenseWalkerKilled"))
			{
				return;
			}
			List<object> obj = args as List<object>;
			WalkerModel model = obj[0] as WalkerModel;
			LootEntry lootEntry = obj[1] as LootEntry;
			ActorView actorView = GameManager.Instance.GetViewForModel((ActorModel)model) as ActorView;
			VisualizationQueue.Instance.StopDependentTasks(actorView.Model);
			if (lootEntry.RewardedCurrency != CurrencyType.None && lootEntry.RewardedAmount > 0)
			{
				int lastAdded = GameManager.Instance.playerModel.GetCurrency(lootEntry.RewardedCurrency).LastAdded;
				if (lastAdded > 0)
				{
					CampView.Instance.BuildingsHud.CreateCollectAnim(lootEntry.RewardedCurrency, actorView.gameObject, lootEntry.RewardedAmount, null, BuildingsHUD.CollectSoundTrigger.OnFinished);
				}
				if (lastAdded < lootEntry.RewardedAmount && !CampView.Instance.Model.HasMaximumStorages(lootEntry.RewardedCurrency))
				{
					HUDNotification.Error(LocalizationManager.GetText("Error.NoStorage." + lootEntry.RewardedCurrency));
				}
			}
			ActorHitEffects component = actorView.gameObject.GetComponent<ActorHitEffects>();
			if (component != null)
			{
				component.SpawnHitEffects(null, 6f);
			}
			Object.Destroy(actorView.gameObject);
			killIndicator.Walker = null;
			killIndicator.gameObject.SetActive(value: false);
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.OnCampDefenseKilled(base.Model.Walkers.Count);
			}
			for (int i = 0; i < base.Model.Walkers.Count; i++)
			{
				ActorModel actorModel = base.Model.Walkers[i];
				if (actorModel != null)
				{
					ActorView actorView2 = GameManager.Instance.GetViewForModel(actorModel) as ActorView;
					if (actorView2 != null && (bool)actorView2.gameObject)
					{
						StartCoroutine(CreateKillIndicator(actorModel, actorView2));
						break;
					}
				}
			}
		}
	}

	private void OnDestroy()
	{
		if (base.Model != null)
		{
			base.Model.Changed -= OnModelChange;
		}
		instance = null;
	}

	private void CreateWalkerView(WalkerModel walker)
	{
		if (walker == null || walker.Definition == null || !base.isActiveAndEnabled || GameManager.Instance == null)
		{
			return;
		}
		ActorResourceEntry resources = GameManager.Instance.GetResources<ActorResourceEntry>("WalkerCamp");
		if (resources == null)
		{
			return;
		}
		List<string> prefabResourceList = resources.PrefabResourceList;
		if (prefabResourceList == null || prefabResourceList.Count == 0)
		{
			return;
		}
		GameObject[] defensePaths = GameObject.FindGameObjectsWithTag("DefensePath");
		if (defensePaths == null || defensePaths.Length == 0)
		{
			return;
		}
		resources.GetRandomPrefabAsync(delegate(GameObject asset)
		{
			if (!(this == null) && base.isActiveAndEnabled && walker != null && walker.Definition != null && base.Model != null && base.Model.Walkers.Contains(walker as CampDefenseWalkerModel) && !(asset == null))
			{
				GameObject gameObject = Object.Instantiate(asset);
				if (base.transform == null)
				{
					Object.Destroy(gameObject);
				}
				else
				{
					gameObject.transform.parent = base.transform;
					UnityUtils.StripPhysicsFromHierarchy(gameObject);
					ActorView component = gameObject.GetComponent<ActorView>();
					if (component == null)
					{
						Object.Destroy(gameObject);
					}
					else
					{
						component.UseModelForInitialPosition = false;
						component.Initialize(walker);
						CampWaypointPath defenseWaypointPath = GetDefenseWaypointPath(walker, defensePaths);
						if (defenseWaypointPath == null || defenseWaypointPath.Waypoints == null || defenseWaypointPath.Waypoints.Count == 0)
						{
							Object.Destroy(gameObject);
						}
						else
						{
							GameObject gameObject2 = defenseWaypointPath.Waypoints[0];
							if (gameObject2 == null)
							{
								Object.Destroy(gameObject);
							}
							else
							{
								CampWaypoint component2 = gameObject2.GetComponent<CampWaypoint>();
								if (component2 == null)
								{
									Object.Destroy(gameObject);
								}
								else
								{
									component.transform.position = component2.transform.position;
									gameObject.AddComponent<CampActorController>().ForceMovement(defenseWaypointPath);
									StartCoroutine(CreateKillIndicator(walker, component));
								}
							}
						}
					}
				}
			}
		});
	}

	private CampWaypointPath GetDefenseWaypointPath(WalkerModel walker, GameObject[] defensePaths)
	{
		if (TutorialView.Instance != null && TutorialView.Instance.Running)
		{
			GameObject gameObject = GameObject.Find("DefensePath_Tutorial_" + base.Model.Walkers.IndexOf(walker as CampDefenseWalkerModel));
			if (gameObject == null)
			{
				gameObject = GameObject.Find("DefensePath_Tutorial_0");
			}
			if (gameObject == null)
			{
				return null;
			}
			return gameObject.GetComponent<CampWaypointPath>();
		}
		int num = Random.Range(0, defensePaths.Length);
		if (defensePaths[num] == null)
		{
			return null;
		}
		return defensePaths[num].GetComponent<CampWaypointPath>();
	}

	private IEnumerator CreateKillIndicator(ActorModel model, ActorView view)
	{
		yield return null;
		if (!base.isActiveAndEnabled || view == null || CampView.Instance == null)
		{
			yield break;
		}
		if (killIndicator == null)
		{
			killIndicator = CampView.Instance.ActorHUD.CreateKillWalkerIndicator(view);
			if (killIndicator != null)
			{
				killIndicator.name = "CampDefenseKillWalkerIndicator";
				killIndicator.Walker = model;
			}
		}
		else if (!killIndicator.gameObject.activeSelf)
		{
			killIndicator.Walker = model;
			killIndicator.FollowTarget(view.gameObject);
			killIndicator.gameObject.SetActive(value: true);
		}
	}

	private void Update()
	{
		if (killIndicator != null && killIndicator.gameObject.activeSelf && killIndicator.HasTarget())
		{
			killIndicator.UpdateFollowTarget();
		}
	}
}
