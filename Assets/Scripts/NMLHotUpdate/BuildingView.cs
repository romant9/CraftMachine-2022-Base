using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using TWDModel;
using UnityEngine;

public class BuildingView : ModelView<BuildingModel>
{
	public static string DefaultSelectionAudioEvent = "camp/select_basic";

	protected List<BuildingIndicator> indicators = new List<BuildingIndicator>();

	private GameObject outlineIndicator;

	private GridSize buildingSize;

	private int buildingLevel;

	private string buildingType;

	private GridPosition buildingPosition;

	private bool isMoveable;

	private GameObject upgradeAvailableGameObject;

	public bool HasBeenUpgraded { get; private set; }

	public GameObject BuildingGameObject { get; protected set; }

	public string SelectBuildingAudioEvent { get; private set; }

	public GridSize BuildingSize
	{
		get
		{
			if (base.Model != null)
			{
				return base.Model.Size;
			}
			return buildingSize;
		}
	}

	public int BuildingLevel
	{
		get
		{
			if (base.Model != null)
			{
				return base.Model.Level;
			}
			return buildingLevel;
		}
	}

	public string BuildingType
	{
		get
		{
			if (base.Model != null)
			{
				return base.Model.TypeName;
			}
			return buildingType;
		}
	}

	public GridPosition BuildingPosition
	{
		get
		{
			if (base.Model != null)
			{
				return base.Model.GridPosition;
			}
			return buildingPosition;
		}
	}

	public bool IsMoveable
	{
		get
		{
			if (base.Model != null)
			{
				return base.Model.IsMoveable;
			}
			return isMoveable;
		}
	}

	public bool IsTemporary => base.Model == null;

	public override void Initialize(ModelObject model)
	{
		if (model != null)
		{
			base.Initialize(model);
			ResetVisualization();
			base.Model.Changed += OnModelChange;
			GameManager.Instance.playerModel.Changed += OnPlayerChange;
		}
	}

	public void SetupVisualsWithoutModel(string abuildingType, int level, GridSize abuildingSize, GridPosition initialPosition, bool aisMoveable)
	{
		buildingType = abuildingType;
		buildingLevel = level;
		buildingSize = abuildingSize;
		buildingPosition = initialPosition;
		isMoveable = aisMoveable;
		SetupBuildingPrefabObject();
		DeleteIndicators();
	}

	private void Start()
	{
		TryShowCampMovedApparition();
	}

	private void TryShowCampMovedApparition()
	{
		if (base.Model != null && base.Model.CampMoved)
		{
			NGUITools.SetActiveChildren(base.gameObject, state: false);
			Invoke("ShowMoveCampApparition", Random.Range(1f, 2f));
		}
	}

	private void ShowMoveCampApparition()
	{
		ShowCompleteEffect();
		NGUITools.SetActiveChildren(base.gameObject, state: true);
		Helpers.ExecuteCommand(new MoveFinishedCommand(base.Model));
	}

	protected virtual void OnDestroy()
	{
		if (base.Model != null)
		{
			base.Model.Changed -= OnModelChange;
			GameManager.Instance.playerModel.Changed -= OnPlayerChange;
		}
		DeleteIndicators();
	}

	public void ShowOutline(GameObject prefab, Material buildingOutlineMaterial)
	{
		if (!(outlineIndicator != null))
		{
			outlineIndicator = Helpers.InstantiateToParent(prefab, base.gameObject);
			outlineIndicator.GetComponentInChildren<MeshRenderer>().material = buildingOutlineMaterial;
			Vector3 localScale = outlineIndicator.transform.localScale;
			localScale.x = (float)BuildingSize.X * (float)GameManager.Instance.playerModel.Camp.Grid.CellSize.X;
			localScale.z = (float)BuildingSize.Y * (float)GameManager.Instance.playerModel.Camp.Grid.CellSize.Y;
			outlineIndicator.transform.localScale = localScale;
		}
	}

	public void HideOutline()
	{
		if (!(outlineIndicator == null))
		{
			Object.Destroy(outlineIndicator);
			outlineIndicator = null;
		}
	}

	private void SetupBuildingPrefabObject()
	{
		BuildingResource buildingResourceFromStats = BuildingResource.GetBuildingResourceFromStats(BuildingType, BuildingLevel);
		if (BuildingGameObject != null)
		{
			Object.Destroy(BuildingGameObject);
			upgradeAvailableGameObject = null;
		}
		CampModel camp = GameManager.Instance.playerModel.Camp;
		base.transform.localRotation = Quaternion.identity;
		base.transform.localPosition = new Vector3((float)camp.Grid.CellSize.X * ((float)BuildingPosition.X + (float)(BuildingSize.X / 2)), 0f, (float)camp.Grid.CellSize.Y * ((float)BuildingPosition.Y + (float)(BuildingSize.Y / 2)));
		BuildingGameObject = Helpers.InstantiateToParent(buildingResourceFromStats.GetPrefab(), base.transform.gameObject);
		BoxCollider boxCollider = BuildingGameObject.GetComponentsInChildren<BoxCollider>(includeInactive: true).ToList().Find((BoxCollider x) => x.gameObject.tag == "Building");
		if (boxCollider == null)
		{
			if (!(base.Model is VegetationModel))
			{
				Debug.LogError("There is no collider in " + base.Model.BuildingType.Name);
			}
		}
		else
		{
			boxCollider.size *= 0.95f;
		}
		SelectBuildingAudioEvent = buildingResourceFromStats.SelectBuildingAudioEvent;
	}

	public IEnumerator DelayerInit()
	{
		float t = 0f;
		while (t < 10f)
		{
			t += Time.deltaTime;
			Debug.LogError("Waiting");
			yield return null;
		}
		ResetVisualization();
	}

	protected virtual void ResetVisualization(bool updateBuildingGraphics = true)
	{
		if (updateBuildingGraphics)
		{
			SetupBuildingPrefabObject();
		}
		DeleteIndicators();
		if (base.Model == null)
		{
			return;
		}
		if (base.Model.Producer != null && !base.Model.IsUpgrading)
		{
			CollectIndicator collectIndicator = CampView.Instance.BuildingsHud.CreateCollectIndicator(this);
			collectIndicator.name = base.name + " - " + base.Model.ModelId;
			indicators.Add(collectIndicator);
		}
		SetIndicatorUpgradeAvailable();
		SetIndicatorInsideBuildingUpgradeAvailable();
		if (base.Model.IsUpgrading)
		{
			BuildingResource buildingResourceFromStats = BuildingResource.GetBuildingResourceFromStats(BuildingType, BuildingLevel);
			if (!string.IsNullOrEmpty(buildingResourceFromStats.ConstructionScaffoldingPrefabName))
			{
				Helpers.InstantiateToParent(buildingResourceFromStats.GetScaffoldingPrefab(), BuildingGameObject);
			}
			BuildingUpgradeIndicator buildingUpgradeIndicator = CampView.Instance.BuildingsHud.CreateUpgradeIndicator(this);
			buildingUpgradeIndicator.FollowTarget(BuildingGameObject);
			buildingUpgradeIndicator.SetType(UpgradeType.Building);
			indicators.Add(buildingUpgradeIndicator);
		}
	}

	public void UpdateIndicators()
	{
		if (indicators == null)
		{
			return;
		}
		for (int i = 0; i < indicators.Count; i++)
		{
			if (indicators[i] != null)
			{
				indicators[i].UpdateFollowTarget();
			}
		}
	}

	public void ShowCompleteEffect()
	{
		Helpers.InstantiateToParent(CampView.Instance.BuildingsHud.upgradeCompleteEffect, base.gameObject).SetActive(value: true);
	}

	public TWDModelResult Collect()
	{
		if (base.Model != null)
		{
			TWDModelResult tWDModelResult = Helpers.ExecuteCommand(new CollectBuildingCommand(base.Model));
			if (tWDModelResult != TWDModelResult.OK)
			{
				if (SingularityMonoBehaviour<AudioManager>.Instance != null)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/invalid_action");
				}
				return tWDModelResult;
			}
			EventManager.NotifyClick("CollectedIndicator");
			return TWDModelResult.OK;
		}
		return TWDModelResult.Error;
	}

	public bool IsCollectIndicatorVisible()
	{
		for (int i = 0; i < indicators.Count; i++)
		{
			if (indicators[i] != null && indicators[i] is CollectIndicator && (indicators[i] as CollectIndicator).IsVisible)
			{
				return true;
			}
		}
		return false;
	}

	public virtual bool OnSelected(bool forcedSelection)
	{
		if (base.Model != null)
		{
			if (base.Model.Producer != null && base.Model.Producer.HasEnoughToCollect && IsCollectIndicatorVisible())
			{
				if (base.Model.Producer.GetAmountCollectable > 0)
				{
					TWDModelResult result = Collect();
					CollectIndicator.ShowCollectError(base.Model.Producer, result);
					EventManager.NotifyClick("Collect_" + base.Model.Producer.CurrencyType.ToString() + "_Indicator");
					return true;
				}
				CollectIndicator.ShowCollectError(base.Model.Producer, TWDModelResult.Error);
			}
			HideIndicatorInsideBuildingUpgradeAvailable();
		}
		return false;
	}

	public virtual void OnUnselected()
	{
		SetIndicatorInsideBuildingUpgradeAvailable();
	}

	public virtual void OnSelected()
	{
		HideIndicatorInsideBuildingUpgradeAvailable();
	}

	private void CreateBpReceivedAnimation(int amount)
	{
		CollectAnimation collectAnimation = CampView.Instance.BuildingsHud.CreateBpReceivedAnim();
		collectAnimation.FollowTarget(base.gameObject);
		collectAnimation.StartAnimationXp(amount);
	}

	private void DeleteIndicators()
	{
		if (indicators == null)
		{
			return;
		}
		for (int i = 0; i < indicators.Count; i++)
		{
			if (!(indicators[i] == null) && indicators[i].gameObject != null)
			{
				Object.Destroy(indicators[i].gameObject);
			}
		}
		indicators.Clear();
	}

	private void DeleteIndicator(BuildingIndicator buildingIndicator)
	{
		if (buildingIndicator.gameObject != null)
		{
			Object.Destroy(buildingIndicator.gameObject);
		}
		indicators.Remove(buildingIndicator);
	}

	private BuildingIndicator GetIndicator(string type)
	{
		return indicators.Find((BuildingIndicator s) => s.Type == type);
	}

	protected void SetIndicatorUpgradeAvailable()
	{
		if (base.Model == null)
		{
			return;
		}
		if (!base.Model.IsUpgrading && base.Model.CanPayUpgrade && !TutorialView.Instance.Running)
		{
			if (upgradeAvailableGameObject == null)
			{
				upgradeAvailableGameObject = CampView.Instance.BuildingsHud.CreateBuildingUpgradeAvailableIndicator(this);
			}
		}
		else if (upgradeAvailableGameObject != null)
		{
			Object.Destroy(upgradeAvailableGameObject);
		}
	}

	protected void SetIndicatorInsideBuildingUpgradeAvailable()
	{
		if (base.Model == null || (!(base.Model is WorkshopBuildingModel) && !(base.Model is TrainingGroundBuildingModel) && !(base.Model is MedicTentModel) && !(base.Model.TypeName == "RadioTent")))
		{
			return;
		}
		BuildingIndicator indicator = GetIndicator("Upgrade_Inside_Available");
		if ((base.Model.UpgradeInside || base.Model.TypeName == "RadioTent") && !TutorialView.Instance.Running)
		{
			if (indicator == null && CampView.Instance != null && CampView.Instance.BuildingsHud != null)
			{
				BuildingIndicator buildingIndicator = null;
				buildingIndicator = ((!(base.Model.TypeName == "RadioTent")) ? CampView.Instance.BuildingsHud.CreateBuildingUpgradeInsideAvailableIndicator(this) : CampView.Instance.BuildingsHud.CreateBuildingFreeCallIndicator(this));
				if (buildingIndicator != null)
				{
					indicators.Add(buildingIndicator);
				}
			}
		}
		else if (indicator != null)
		{
			DeleteIndicator(indicator);
		}
	}

	protected void HideIndicatorInsideBuildingUpgradeAvailable()
	{
		if (base.Model != null && (base.Model is WorkshopBuildingModel || base.Model is TrainingGroundBuildingModel || base.Model is MedicTentModel || base.Model.TypeName == "RadioTent"))
		{
			BuildingIndicator indicator = GetIndicator("Upgrade_Inside_Available");
			if (indicator != null)
			{
				DeleteIndicator(indicator);
			}
		}
	}

	private void OnPlayerChange(ModelObject m, string changed, object args)
	{
		if (changed == "currencyChangedEvent")
		{
			SetIndicatorUpgradeAvailable();
			SetIndicatorInsideBuildingUpgradeAvailable();
		}
	}

	protected virtual void OnModelChange(ModelObject model, string changed, object args)
	{
		switch (changed)
		{
		case "position":
			EventManager.NotifyEvent(EventManager.EventType.CampVisualizationChanged);
			break;
		case "build":
			ResetVisualization(updateBuildingGraphics: false);
			EventManager.NotifyEvent(EventManager.EventType.CampVisualizationChanged);
			break;
		case "level":
		{
			BuildingModel buildingModel2 = model as BuildingModel;
			HasBeenUpgraded = true;
			ResetVisualization();
			ShowCompleteEffect();
			CreateBpReceivedAnimation(buildingModel2.GetCurrentUpgradeLevel().AwardedXp);
			EventManager.NotifyEvent(EventManager.EventType.CampVisualizationChanged);
			if (buildingModel2 != null)
			{
				if (buildingModel2.Level == 1)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/building_build_ready");
				}
				else
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/building_upgrade_ready");
				}
			}
			break;
		}
		case "cancelUpgrade":
			ResetVisualization();
			EventManager.NotifyEvent(EventManager.EventType.CampVisualizationChanged);
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/building_upgrade_stop");
			break;
		case "collected":
			if (!(model is BuildingModel buildingModel))
			{
				break;
			}
			if (buildingModel.Producer.CurrencyType == CurrencyType.SurvivalPoints && GameManager.Instance.playerModel.IsTimedBonusActive(TimedBonusType.DoubleXp))
			{
				CurrencyModel currency = GameManager.Instance.playerModel.GetCurrency(buildingModel.Producer.CurrencyType);
				if (currency != null)
				{
					CampView.Instance.BuildingsHud.CreateCollectAnim(buildingModel.Producer.CurrencyType, base.gameObject, (int)(buildingModel.Producer.LastCollectedAmount * currency.AddMultiplier));
				}
			}
			else
			{
				CampView.Instance.BuildingsHud.CreateCollectAnim(buildingModel.Producer.CurrencyType, base.gameObject, buildingModel.Producer.LastCollectedAmount);
			}
			CampView.Instance.BuildingsHud.CreateCollectEffect(buildingModel.Producer.CurrencyType, base.gameObject);
			break;
		}
	}
}
