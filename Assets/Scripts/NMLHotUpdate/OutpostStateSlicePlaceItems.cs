using System;
using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class OutpostStateSlicePlaceItems : OutpostStateBase
{
	[Header("Generic")]
	public GameObject SlicePrefab;

	public float SliceScale = 0.5f;

	public float SliceMarginScale;

	public GameObject SliceContainer;

	[SerializeField]
	private UIButton ClearHotspotButton;

	public GameObject SurvivorSettingsContainer;

	public UIButton SurvivorModeStationary;

	public UISprite SurvivorModeStationarySelected;

	public UIButton SurvivorModeDefensive;

	public UISprite SurvivorModeDefensiveSelected;

	public UIButton SurvivorModeOffensive;

	public UISprite SurvivorModeOffensiveSelected;

	public UIButton EditTeamButton;

	[Header("Walker settings")]
	public GameObject WalkerSettingsContainer;

	public UIButton WalkerMinus;

	public UIButton WalkerPlus;

	public UISprite WalkerClass;

	public UILabel WalkerCount;

	public UISprite WalkerPortrait;

	[Header("Survivor menu")]
	public GameObject SurvivorMainParent;

	[Tooltip("Where the Survivor card prefab will be placed")]
	[SerializeField]
	private GameObject survivoCardParent;

	[Tooltip("Survivor card prefab")]
	[SerializeField]
	private GameObject survivorCardPrefab;

	[SerializeField]
	public GameObject StateStationarySelected;

	[SerializeField]
	public GameObject StateDefensiveSelected;

	[SerializeField]
	public GameObject StateOffensiveSelected;

	[SerializeField]
	public UILabel StateAISelectedLabel;

	private SurvivorCard survivorCard;

	[Header("Walker Menu")]
	public GameObject WalkerMenu;

	public UISprite WalkerClassIcon;

	[SerializeField]
	private UILabel WalkerNameLabel;

	[SerializeField]
	private UILabel WalkerLevelDescLabel;

	[SerializeField]
	private UILabel WalkerLevelLabel;

	[SerializeField]
	private UILabel DeploymentCostDescLabel;

	[SerializeField]
	private UILabel DeploymentCostLabel;

	[Header("BoxOrFlag Menu")]
	[SerializeField]
	private GameObject BoxOrFlagMenu;

	[Header("No Selection Menu")]
	public GameObject NoSelectionMenu;

	[Header("Deployment Points Label")]
	public UILabel DeploymentPointsLabel;

	[Header("Deployment Items")]
	public UIGrid DeploymentItemGrid;

	public GameObject DeployItemPrefab;

	[SerializeField]
	public UIScrollView DeployItemList;

	[SerializeField]
	private OutpostHelpPopup outpostHelpPopup;

	private OutpostSliceHotspot selectedHotspot;

	private Dictionary<WalkerType, int> AvailableWalkerCountList = new Dictionary<WalkerType, int>();

	private Dictionary<WalkerType, int> WalkerDeploymentsActivePerType = new Dictionary<WalkerType, int>();

	private Dictionary<SlicePosition, SlicePanel> Slices = new Dictionary<SlicePosition, SlicePanel>();

	public List<OutpostDeploymentMarker> DeploymentItems = new List<OutpostDeploymentMarker>();

	public SlicePosition SelectedSlice { get; set; }

	public override bool ShowHeader => false;

	public override GameObject GetTutorialPanel => outpostHelpPopup.gameObject;

	private void Awake()
	{
		outpostHelpPopup.gameObject.SetActive(value: false);
	}

	public void CreateSlices()
	{
		CreateChosenSlice(SlicePosition.First);
		CreateChosenSlice(SlicePosition.Second);
		CreateChosenSlice(SlicePosition.Third);
	}

	public void OnPreviousStepClicked()
	{
		RequestStateChange(StateChangeDirection.Prev);
	}

	public void OnEnable()
	{
		UpdateSlices();
		UpdateDeploymentItems();
		UpdateHotspotProperties(null);
		UpdateDeploymentPoints();
		TutorialView.Instance.StartPart("OutpostEditExplain");
	}

	public void OnDisable()
	{
		AvailableWalkerCountList = null;
		WalkerDeploymentsActivePerType = null;
		DestroySlices();
	}

	private void OnDeploymentInteraction(OutpostDeploymentItem item)
	{
		if (!(selectedHotspot != null))
		{
			return;
		}
		HotspotState hotspotState = item.State;
		WalkerType walkerType = item.WalkerType;
		if (hotspotState == HotspotState.DefenderSpawn_0 || hotspotState == HotspotState.DefenderSpawn_1 || hotspotState == HotspotState.DefenderSpawn_2)
		{
			HotspotState firstFreeDefenderState = base.OutpostLevelModel.GetFirstFreeDefenderState();
			if (firstFreeDefenderState != HotspotState.None)
			{
				hotspotState = firstFreeDefenderState;
			}
		}
		if (OutpostEditManager.CallModifyOutpostCommand(base.OutpostLevelModel.GetChosenSliceViewId(SelectedSlice), selectedHotspot.HotspotModel.ViewId, hotspotState, walkerType, 1))
		{
			switch (hotspotState)
			{
			case HotspotState.DefenderSpawn_0:
			case HotspotState.DefenderSpawn_1:
			case HotspotState.DefenderSpawn_2:
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_add_survivor");
				break;
			case HotspotState.Walker:
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_add_walker");
				break;
			case HotspotState.Flag:
			case HotspotState.ResourceContainer:
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_add_item");
				break;
			case HotspotState.None:
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_remove_item");
				break;
			}
		}
		else
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/invalid_action");
		}
		HotspotInfo hotspotInfo = GetHotspotInfo(selectedHotspot);
		if (hotspotInfo != null)
		{
			selectedHotspot.SetState(hotspotInfo.State, hotspotInfo.WalkerType, hotspotInfo.Count);
		}
		else
		{
			selectedHotspot.SetState(HotspotState.None, WalkerType.WalkerNormal, 0);
		}
		UpdateDeploymentItems();
		UpdateHotspotProperties(selectedHotspot.HotspotModel);
		UpdateDeploymentPoints();
	}

	private void UpdateDeploymentPoints()
	{
		if (DeploymentPointsLabel != null && base.OutpostLevelModel != null && GameManager.Instance.playerModel.SelectedOutpostTemplateDefinitionId != "")
		{
			string selectedOutpostTemplateDefinitionId = GameManager.Instance.playerModel.SelectedOutpostTemplateDefinitionId;
			OutpostTemplateDefinition outpostTemplateDefinition = GameManager.Instance.gameEconomyData.GetOutpostTemplateDefinition(selectedOutpostTemplateDefinitionId);
			string chosenSliceViewId = base.OutpostLevelModel.GetChosenSliceViewId(SelectedSlice);
			int maxDeploymentForSlice = base.OutpostLevelModel.GetMaxDeploymentForSlice(SelectedSlice, outpostTemplateDefinition);
			int totalUsedDeploymentForSlice = base.OutpostLevelModel.GetTotalUsedDeploymentForSlice(chosenSliceViewId);
			int num = UtilsMath.Max(0, maxDeploymentForSlice - totalUsedDeploymentForSlice);
			DeploymentPointsLabel.text = num.ToString();
		}
	}

	private void OnMainSliceInteraction(SlicePanel slicePanel, OutpostSliceHotspot hotspot, string eventId)
	{
		slicePanel.DeselectAllHotspots();
		selectedHotspot = null;
		if (slicePanel != null && slicePanel.Position == SelectedSlice && hotspot != null)
		{
			hotspot.SetSelected(selected: true);
			selectedHotspot = hotspot;
		}
		else if (slicePanel != null && slicePanel.Position != SelectedSlice)
		{
			SelectedSlice = slicePanel.Position;
			OnDisable();
			OnEnable();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_change_slice");
		}
		UpdateDeploymentItems();
		UpdateHotspotProperties((hotspot != null) ? hotspot.HotspotModel : null);
	}

	private HotspotInfo GetHotspotInfo(OutpostSliceHotspot sliceHotspot)
	{
		OutpostHotspotModel hotspotModel = sliceHotspot.HotspotModel;
		OutpostLevelModel editLevelModel = GameManager.Instance.playerModel.OutpostModel.EditLevelModel;
		if (editLevelModel == null || hotspotModel == null)
		{
			return null;
		}
		return editLevelModel.FindHotspotInfo(hotspotModel.ViewId);
	}

	private void CalculateWalkersAvailable()
	{
		if (AvailableWalkerCountList == null)
		{
			AvailableWalkerCountList = new Dictionary<WalkerType, int>();
		}
		int length = Enum.GetValues(typeof(WalkerType)).Length;
		for (int i = 0; i < length; i++)
		{
			WalkerType walkerType = (WalkerType)i;
			int value = 0;
			int availableCount = GetAvailableCount(walkerType);
			if (AvailableWalkerCountList.TryGetValue(walkerType, out value))
			{
				AvailableWalkerCountList[walkerType] = availableCount;
			}
			else
			{
				AvailableWalkerCountList.Add(walkerType, availableCount);
			}
		}
	}

	private int GetAvailableForType(WalkerType walkerType)
	{
		if (AvailableWalkerCountList == null)
		{
			AvailableWalkerCountList = new Dictionary<WalkerType, int>();
		}
		int value = 0;
		AvailableWalkerCountList.TryGetValue(walkerType, out value);
		return value;
	}

	private int GetActiveForType(WalkerType walkerType)
	{
		if (WalkerDeploymentsActivePerType == null)
		{
			WalkerDeploymentsActivePerType = new Dictionary<WalkerType, int>();
		}
		int value = 0;
		WalkerDeploymentsActivePerType.TryGetValue(walkerType, out value);
		return value;
	}

	private void AddActiveForType(WalkerType walkerType, int amount = 1)
	{
		if (WalkerDeploymentsActivePerType == null)
		{
			WalkerDeploymentsActivePerType = new Dictionary<WalkerType, int>();
		}
		int value = 0;
		if (WalkerDeploymentsActivePerType.TryGetValue(walkerType, out value))
		{
			WalkerDeploymentsActivePerType[walkerType] += amount;
		}
		else
		{
			WalkerDeploymentsActivePerType.Add(walkerType, amount);
		}
	}

	private bool IsWalkerAvailableToDeploy(OutpostDeploymentMarker deploymentItem)
	{
		if (deploymentItem != null && deploymentItem.CurrentState == HotspotState.Walker)
		{
			int activeForType = GetActiveForType(deploymentItem.CurrentWalkerType);
			if (GetAvailableForType(deploymentItem.CurrentWalkerType) > activeForType)
			{
				AddActiveForType(deploymentItem.CurrentWalkerType);
				return true;
			}
		}
		return false;
	}

	private void UpdateDeploymentItems()
	{
		float y = DeployItemPrefab.GetComponent<BoxCollider>().size.y;
		int num = 0;
		CalculateWalkersAvailable();
		WalkerDeploymentsActivePerType = null;
		for (int num2 = DeploymentItems.Count - 1; num2 >= 0; num2--)
		{
			OutpostDeploymentMarker outpostDeploymentMarker = DeploymentItems[num2];
			if (outpostDeploymentMarker != null)
			{
				if (CanSelectedSliceContain(outpostDeploymentMarker.CurrentState))
				{
					if (outpostDeploymentMarker.CurrentState == HotspotState.Flag)
					{
						if (!HasFlag())
						{
							outpostDeploymentMarker.Active();
						}
						else
						{
							outpostDeploymentMarker.Deactive();
						}
					}
					if (outpostDeploymentMarker.CurrentState == HotspotState.ResourceContainer)
					{
						if (!HasResourceContainer())
						{
							outpostDeploymentMarker.Active();
						}
						else
						{
							outpostDeploymentMarker.Deactive();
						}
					}
					if (outpostDeploymentMarker.CurrentState == HotspotState.DefenderSpawn_0)
					{
						if (!HasDefender(HotspotState.DefenderSpawn_0))
						{
							outpostDeploymentMarker.Active();
						}
						else
						{
							outpostDeploymentMarker.Deactive();
						}
					}
					if (outpostDeploymentMarker.CurrentState == HotspotState.DefenderSpawn_1)
					{
						if (!HasDefender(HotspotState.DefenderSpawn_1))
						{
							outpostDeploymentMarker.Active();
						}
						else
						{
							outpostDeploymentMarker.Deactive();
						}
					}
					if (outpostDeploymentMarker.CurrentState == HotspotState.DefenderSpawn_2)
					{
						if (!HasDefender(HotspotState.DefenderSpawn_2))
						{
							outpostDeploymentMarker.Active();
						}
						else
						{
							outpostDeploymentMarker.Deactive();
						}
					}
					if (outpostDeploymentMarker.CurrentState == HotspotState.Walker)
					{
						if (IsWalkerAvailableToDeploy(outpostDeploymentMarker))
						{
							outpostDeploymentMarker.Active();
						}
						else
						{
							outpostDeploymentMarker.Deactive();
						}
					}
				}
				else
				{
					outpostDeploymentMarker.Deactive();
				}
				if (outpostDeploymentMarker.gameObject.activeSelf)
				{
					outpostDeploymentMarker.transform.localPosition = new Vector3(0f, 0f - y * (float)num, 0f);
					num++;
				}
			}
		}
		DeploymentItemGrid.Reposition();
		DeployItemList.ResetPosition();
	}

	private void CreateChosenSlice(SlicePosition slicePosition)
	{
		if (SliceContainer != null && SlicePrefab != null && base.OutpostLevelModel != null)
		{
			string text = base.OutpostLevelModel.GetChosenSliceViewId(slicePosition);
			if (text == null)
			{
				text = base.OutpostTemplateModel.GetSliceViewIds(slicePosition)[0];
			}
			OutpostLevelModel editLevelModel = GameManager.Instance.playerModel.OutpostModel.EditLevelModel;
			SlicePanel slicePanel = SlicePanel.CreateSlicePanel(GameManager.Instance.playerModel.GetOutpostTemplate(editLevelModel.BaseRunLocationID), SlicePrefab, SliceContainer, SliceScale, SliceMarginScale, slicePosition - (SelectedSlice - 1), 3, slicePosition, text, editLevelModel, slicePosition == SelectedSlice);
			if (slicePanel != null)
			{
				Slices.Add(slicePosition, slicePanel);
				slicePanel.SetLabel("");
				slicePanel.ShowArrows(enabled: false);
				slicePanel.ShowSidewaysArrows(slicePosition == SelectedSlice);
				slicePanel.EnableSliceSelectClick = true;
				slicePanel.OnSliceInteraction += OnMainSliceInteraction;
				slicePanel.GetComponent<UIButton>().tweenTarget = null;
				slicePanel.SetHotspotInteraction(slicePosition == SelectedSlice);
			}
		}
	}

	private int GetAvailableCount(WalkerType walkerType)
	{
		OutpostModel outpostModel = GameManager.Instance.playerModel.OutpostModel;
		if (!outpostModel.IsCageEnabled(walkerType.ToString()) || !outpostModel.IsPlaceableEnabled(walkerType.ToString()))
		{
			return 0;
		}
		int num = 0;
		OutpostWalkerModel walkerModel = outpostModel.GetWalkerModel(walkerType.ToString());
		if (walkerModel != null && !walkerModel.IsLocked)
		{
			num = walkerModel.Amount;
		}
		OutpostLevelModel editLevelModel = outpostModel.EditLevelModel;
		if (editLevelModel != null)
		{
			return num - editLevelModel.GetTotalWalkersAssigned(walkerType);
		}
		return 0;
	}

	private int GetOwnedCount(WalkerType walkerType)
	{
		OutpostModel outpostModel = GameManager.Instance.playerModel.OutpostModel;
		if (!outpostModel.IsCageEnabled(walkerType.ToString()))
		{
			return 0;
		}
		int result = 0;
		OutpostWalkerModel walkerModel = outpostModel.GetWalkerModel(walkerType.ToString());
		if (walkerModel != null && !walkerModel.IsLocked)
		{
			result = walkerModel.Amount;
		}
		return result;
	}

	private bool HasFlag()
	{
		return GameManager.Instance.playerModel.OutpostModel.EditLevelModel?.HasFlag ?? false;
	}

	private bool HasResourceContainer()
	{
		return GameManager.Instance.playerModel.OutpostModel.EditLevelModel?.HasResourceContainer ?? false;
	}

	private bool HasDefender(HotspotState state)
	{
		return GameManager.Instance.playerModel.OutpostModel.EditLevelModel?.HasDefender(state) ?? false;
	}

	private bool CanSelectedSliceContain(HotspotState hotspotState)
	{
		if (GameManager.Instance.playerModel.OutpostModel.EditLevelModel != null)
		{
			string chosenSliceViewId = GameManager.Instance.playerModel.OutpostModel.EditLevelModel.GetChosenSliceViewId(SelectedSlice);
			List<OutpostHotspotModel> hotspotModels = GetCurrentSlicePanel().OutpostTemplateModel.GetSliceModel(chosenSliceViewId).GetHotspotModels();
			for (int i = 0; i < hotspotModels.Count; i++)
			{
				if (hotspotModels[i].Type == HotspotType.Defender && (hotspotState == HotspotState.DefenderSpawn_0 || hotspotState == HotspotState.DefenderSpawn_1 || hotspotState == HotspotState.DefenderSpawn_2))
				{
					return true;
				}
				if (hotspotModels[i].Type == HotspotType.Goal && (hotspotState == HotspotState.ResourceContainer || hotspotState == HotspotState.Flag))
				{
					return true;
				}
				if (hotspotModels[i].Type == HotspotType.Walker && hotspotState == HotspotState.Walker)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void UpdateSlices()
	{
		OutpostLevelModel editLevelModel = GameManager.Instance.playerModel.OutpostModel.EditLevelModel;
		if (Slices == null || (Slices.Count == 0 && editLevelModel != null))
		{
			CreateSlices();
			RefreshMarkers(editLevelModel);
		}
	}

	private void RefreshMarkers(OutpostLevelModel levelModel)
	{
		float y = -50f;
		int num = 1;
		OutpostDeploymentMarker outpostDeploymentMarker = null;
		int num2 = 0;
		int length = Enum.GetValues(typeof(WalkerType)).Length;
		int num3 = 0;
		WalkerType walkerType = WalkerType.WalkerArmored;
		for (num2 = 0; num2 < length; num2++)
		{
			walkerType = (WalkerType)num2;
			num3 = GetOwnedCount(walkerType);
			for (int i = 0; i < num3; i++)
			{
				num = levelModel.GetDeploymentCostForHotspot(HotspotState.Walker, walkerType);
				outpostDeploymentMarker = OutpostDeploymentMarker.CreateDeploymentMarker(DeployItemPrefab, DeploymentItemGrid.gameObject, new Vector3(0f, y, 0f));
				outpostDeploymentMarker.Set(HotspotState.Walker, walkerType, num);
				DeploymentItems.Add(outpostDeploymentMarker);
			}
		}
		HotspotState hotspotState = HotspotState.DefenderSpawn_0;
		num = levelModel.GetDeploymentCostForHotspot(hotspotState, WalkerType.WalkerNormal);
		outpostDeploymentMarker = OutpostDeploymentMarker.CreateDeploymentMarker(DeployItemPrefab, DeploymentItemGrid.gameObject, new Vector3(0f, y, 0f));
		outpostDeploymentMarker.Set(hotspotState, WalkerType.WalkerTank, num);
		DeploymentItems.Add(outpostDeploymentMarker);
		hotspotState = HotspotState.DefenderSpawn_1;
		num = levelModel.GetDeploymentCostForHotspot(hotspotState, WalkerType.WalkerNormal);
		outpostDeploymentMarker = OutpostDeploymentMarker.CreateDeploymentMarker(DeployItemPrefab, DeploymentItemGrid.gameObject, new Vector3(0f, y, 0f));
		outpostDeploymentMarker.Set(hotspotState, WalkerType.WalkerTank, num);
		DeploymentItems.Add(outpostDeploymentMarker);
		hotspotState = HotspotState.DefenderSpawn_2;
		num = levelModel.GetDeploymentCostForHotspot(hotspotState, WalkerType.WalkerNormal);
		outpostDeploymentMarker = OutpostDeploymentMarker.CreateDeploymentMarker(DeployItemPrefab, DeploymentItemGrid.gameObject, new Vector3(0f, y, 0f));
		outpostDeploymentMarker.Set(hotspotState, WalkerType.WalkerTank, num);
		DeploymentItems.Add(outpostDeploymentMarker);
		num = levelModel.GetDeploymentCostForHotspot(HotspotState.ResourceContainer, WalkerType.WalkerNormal);
		outpostDeploymentMarker = OutpostDeploymentMarker.CreateDeploymentMarker(DeployItemPrefab, DeploymentItemGrid.gameObject, new Vector3(0f, y, 0f));
		outpostDeploymentMarker.Set(HotspotState.ResourceContainer, WalkerType.WalkerNormal, num);
		DeploymentItems.Add(outpostDeploymentMarker);
		num = levelModel.GetDeploymentCostForHotspot(HotspotState.Flag, WalkerType.WalkerNormal);
		outpostDeploymentMarker = OutpostDeploymentMarker.CreateDeploymentMarker(DeployItemPrefab, DeploymentItemGrid.gameObject, new Vector3(0f, y, 0f));
		outpostDeploymentMarker.Set(HotspotState.Flag, WalkerType.WalkerNormal, num);
		DeploymentItems.Add(outpostDeploymentMarker);
	}

	private void DestroySlices()
	{
		for (int i = 0; i < DeploymentItems.Count; i++)
		{
			UnityEngine.Object.Destroy(DeploymentItems[i].gameObject);
		}
		DeploymentItems.Clear();
		Slices.Clear();
		if (!(SliceContainer != null))
		{
			return;
		}
		foreach (Transform item in SliceContainer.transform)
		{
			DeleteSlicePanel(item.gameObject.GetComponent<SlicePanel>());
		}
	}

	private void DeleteSlicePanel(SlicePanel slicePanel)
	{
		if (slicePanel != null)
		{
			slicePanel.OnSliceInteraction -= OnMainSliceInteraction;
			UnityEngine.Object.Destroy(slicePanel.gameObject);
		}
	}

	private void UpdateSelectedHotspotState(HotspotState newState)
	{
		if (selectedHotspot != null)
		{
			OutpostHotspotModel hotspotModel = selectedHotspot.HotspotModel;
			HotspotInfo hotspotInfo = GetHotspotInfo(selectedHotspot);
			if (hotspotInfo != null)
			{
				OutpostEditManager.CallModifyOutpostCommand(base.OutpostLevelModel.GetChosenSliceViewId(SelectedSlice), hotspotModel.ViewId, newState, hotspotInfo.WalkerType, hotspotInfo.Count, hotspotInfo.DefensiveMode);
			}
			UpdateHotspotProperties(hotspotModel);
			UpdateDeploymentItems();
		}
	}

	private void UpdateSelectedHotspotDefenderMode(AIMode newDefensiveMode)
	{
		if (selectedHotspot != null)
		{
			OutpostHotspotModel hotspotModel = selectedHotspot.HotspotModel;
			HotspotInfo hotspotInfo = GetHotspotInfo(selectedHotspot);
			if (hotspotInfo != null)
			{
				OutpostEditManager.CallModifyOutpostCommand(base.OutpostLevelModel.GetChosenSliceViewId(SelectedSlice), hotspotModel.ViewId, hotspotInfo.State, hotspotInfo.WalkerType, hotspotInfo.Count, newDefensiveMode);
			}
			UpdateHotspotProperties(hotspotModel);
			UpdateDeploymentItems();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_change_mode");
		}
	}

	private void UpdateSelectedHotspotWalkerCount(int walkerCountAdd)
	{
		if (selectedHotspot != null)
		{
			OutpostHotspotModel hotspotModel = selectedHotspot.HotspotModel;
			HotspotInfo hotspotInfo = GetHotspotInfo(selectedHotspot);
			if (hotspotInfo != null)
			{
				int width = 1;
				int height = 1;
				selectedHotspot.HotspotModel.GetDimensions(out width, out height);
				int max = width * height;
				int count = UtilsMath.Clamp(hotspotInfo.Count + walkerCountAdd, 1, max);
				OutpostEditManager.CallModifyOutpostCommand(base.OutpostLevelModel.GetChosenSliceViewId(SelectedSlice), hotspotModel.ViewId, hotspotInfo.State, hotspotInfo.WalkerType, count, hotspotInfo.DefensiveMode);
			}
			UpdateHotspotProperties(hotspotModel);
			UpdateDeploymentItems();
		}
	}

	public void OnSurvivorAClicked()
	{
		UpdateSelectedHotspotState(HotspotState.DefenderSpawn_0);
		UpdateAllHotspots();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/survivor_click");
	}

	public void OnSurvivorBClicked()
	{
		UpdateSelectedHotspotState(HotspotState.DefenderSpawn_1);
		UpdateAllHotspots();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/survivor_click");
	}

	public void OnSurvivorCClicked()
	{
		UpdateSelectedHotspotState(HotspotState.DefenderSpawn_2);
		UpdateAllHotspots();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/survivor_click");
	}

	public void OnStationaryClicked()
	{
		UpdateSelectedHotspotDefenderMode(AIMode.Stationary);
	}

	public void OnDefensiveClicked()
	{
		UpdateSelectedHotspotDefenderMode(AIMode.Defending);
	}

	public void OnAggressiveClicked()
	{
		UpdateSelectedHotspotDefenderMode(AIMode.Aggressive);
	}

	public void OnWalkerPlusClicked()
	{
		UpdateSelectedHotspotWalkerCount(1);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_increment");
	}

	public void OnWalkerMinusClicked()
	{
		UpdateSelectedHotspotWalkerCount(-1);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_decrement");
	}

	public void OnEditTeamClicked()
	{
		TeamSelectionPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MapTeamSelection) as TeamSelectionPopup;
		obj.SurvivorType = SurvivorContainerModel.SurvivorType.Outpost;
		obj.OnCloseCallback = (Callback)Delegate.Remove(obj.OnCloseCallback, new Callback(updateAfterEditTeam));
		obj.OnCloseCallback = (Callback)Delegate.Combine(obj.OnCloseCallback, new Callback(updateAfterEditTeam));
		obj.Open();
	}

	public void updateAfterEditTeam()
	{
		UpdateAllHotspots();
		UpdateHotspotProperties(selectedHotspot.HotspotModel);
	}

	public void RemoveTargetsHightlight()
	{
		SlicePanel currentSlicePanel = GetCurrentSlicePanel();
		if (currentSlicePanel != null)
		{
			currentSlicePanel.HighlightAllThatCanAccept(null);
		}
	}

	public void DragStart(OutpostDeploymentMarker marker)
	{
		SlicePanel currentSlicePanel = GetCurrentSlicePanel();
		if (currentSlicePanel != null)
		{
			currentSlicePanel.HighlightAllThatCanAccept(marker);
		}
	}

	public void DragCancel(OutpostDeploymentMarker marker)
	{
		UpdateDeploymentItems();
		RemoveTargetsHightlight();
	}

	public void DragCompleteAddNewHotspot(OutpostDeploymentMarker marker, OutpostSliceHotspot targetHotspot)
	{
		if (marker != null && marker.originalHotspot == null && targetHotspot != null)
		{
			selectedHotspot = targetHotspot;
			OutpostEditManager.CallModifyOutpostCommand(base.OutpostLevelModel.GetChosenSliceViewId(SelectedSlice), targetHotspot.HotspotModel.ViewId, marker.CurrentState, marker.CurrentWalkerType, marker.CurrentCount);
			UpdateHotspotProperties(targetHotspot.HotspotModel);
			SlicePanel currentSlicePanel = GetCurrentSlicePanel();
			if (currentSlicePanel != null)
			{
				currentSlicePanel.DeselectAllHotspots();
				currentSlicePanel = null;
			}
			UpdateDeploymentItems();
			UpdateDeploymentPoints();
			selectedHotspot.SetSelected(selected: true);
		}
		RemoveTargetsHightlight();
	}

	public void DragCompleteClearHotspot(OutpostDeploymentMarker marker)
	{
		if (marker != null && marker.originalHotspot != null)
		{
			selectedHotspot = marker.originalHotspot;
			selectedHotspot.SetSelected(selected: true);
			OutpostEditManager.CallModifyOutpostCommand(base.OutpostLevelModel.GetChosenSliceViewId(SelectedSlice), marker.originalHotspot.HotspotModel.ViewId, HotspotState.None, WalkerType.WalkerNormal, 0);
			marker.originalHotspot.DeploymentMarker = null;
			UpdateHotspotProperties(marker.originalHotspot.HotspotModel);
			SlicePanel currentSlicePanel = GetCurrentSlicePanel();
			if (currentSlicePanel != null)
			{
				currentSlicePanel.DeselectAllHotspots();
				currentSlicePanel = null;
			}
			UnityEngine.Object.Destroy(marker.gameObject);
			if (DeploymentItemGrid != null)
			{
				DeploymentItems.Clear();
				int num = 0;
				foreach (Transform item in DeploymentItemGrid.transform)
				{
					UnityEngine.Object.Destroy(item.gameObject);
					num++;
				}
				RefreshMarkers(GameManager.Instance.playerModel.OutpostModel.EditLevelModel);
			}
			UpdateDeploymentItems();
			UpdateDeploymentPoints();
		}
		RemoveTargetsHightlight();
		StartCoroutine(RepositionGrid());
	}

	private IEnumerator RepositionGrid()
	{
		yield return new WaitForEndOfFrame();
		DeploymentItemGrid.Reposition();
	}

	public void DragCompleteChangeHotspotPosition(OutpostDeploymentMarker marker, OutpostSliceHotspot newHotspot)
	{
		bool flag = false;
		if (marker != null && marker.originalHotspot != null && newHotspot != null)
		{
			selectedHotspot = newHotspot;
			selectedHotspot.SetSelected(selected: true);
			AIMode defensiveMode = ((marker.originalHotspot.HotspotModel != null && marker.originalHotspot.HotspotModel.DefenderModel != null) ? base.OutpostLevelModel.FindHotspotInfo(marker.originalHotspot.HotspotModel.ViewId) : null)?.DefensiveMode ?? AIMode.Aggressive;
			bool num = OutpostEditManager.CallModifyOutpostCommand(base.OutpostLevelModel.GetChosenSliceViewId(SelectedSlice), marker.originalHotspot.HotspotModel.ViewId, HotspotState.None, WalkerType.WalkerNormal, 0);
			if (num)
			{
				flag = OutpostEditManager.CallModifyOutpostCommand(base.OutpostLevelModel.GetChosenSliceViewId(SelectedSlice), newHotspot.HotspotModel.ViewId, marker.CurrentState, marker.CurrentWalkerType, marker.CurrentCount, defensiveMode);
			}
			if (num && flag)
			{
				SlicePanel currentSlicePanel = GetCurrentSlicePanel();
				if (currentSlicePanel != null)
				{
					currentSlicePanel.DeselectAllHotspots();
					currentSlicePanel = null;
				}
				marker.originalHotspot.DeploymentMarker = null;
				newHotspot.DeploymentMarker = marker;
				UpdateHotspotProperties(marker.originalHotspot.HotspotModel);
				UpdateHotspotProperties(newHotspot.HotspotModel);
				UpdateDeploymentItems();
				UpdateDeploymentPoints();
			}
		}
		RemoveTargetsHightlight();
	}

	public bool ClearHotspotAndUpdate(SlicePosition position, string hotspotViewId)
	{
		bool result = false;
		if (selectedHotspot != null)
		{
			result = OutpostEditManager.CallModifyOutpostCommand(base.OutpostLevelModel.GetChosenSliceViewId(position), hotspotViewId, HotspotState.None, WalkerType.WalkerNormal, 0);
			selectedHotspot.SetState(HotspotState.None, WalkerType.WalkerNormal, 0);
			selectedHotspot.SetSelected(selected: false);
			UpdateDeploymentItems();
			UpdateHotspotProperties(selectedHotspot.HotspotModel);
			UpdateDeploymentPoints();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_remove_item");
		}
		return result;
	}

	public void OnClearHotspotClicked()
	{
		if (selectedHotspot != null)
		{
			ClearHotspotAndUpdate(SelectedSlice, selectedHotspot.HotspotModel.ViewId);
		}
	}

	private void UpdateAllHotspots()
	{
		OutpostSliceHotspot[] array = UnityEngine.Object.FindObjectsOfType<OutpostSliceHotspot>();
		foreach (OutpostSliceHotspot outpostSliceHotspot in array)
		{
			HotspotInfo hotspotInfo = GetHotspotInfo(outpostSliceHotspot);
			if (hotspotInfo != null)
			{
				outpostSliceHotspot.SetState(hotspotInfo.State, hotspotInfo.WalkerType, hotspotInfo.Count);
			}
		}
	}

	private void UpdateHotspotProperties(OutpostHotspotModel hotspot)
	{
		SurvivorContainerModel survivorContainer = GameManager.Instance.playerModel.SurvivorContainer;
		HotspotInfo hotspotInfo = ((GameManager.Instance.playerModel.OutpostModel.EditLevelModel != null && hotspot != null) ? base.OutpostLevelModel.FindHotspotInfo(hotspot.ViewId) : null);
		if (hotspotInfo != null)
		{
			if (selectedHotspot == null)
			{
				Debug.LogError("Selected hot spot is null");
				return;
			}
			selectedHotspot.SetState(hotspotInfo.State, hotspotInfo.WalkerType, hotspotInfo.Count);
			if (hotspotInfo.IsDefenderSpawn)
			{
				ClearHotspotButton.gameObject.SetActive(value: true);
				BoxOrFlagMenu.SetActive(value: false);
				NoSelectionMenu.SetActive(value: false);
				SurvivorMainParent.SetActive(value: true);
				UpdateWalkerDetails(show: false, hotspotInfo);
				UpdateSurvivorCard(survivorContainer.GetOutpostDefendingSurvivor(hotspotInfo.GetDefenderIndex()));
				if (StateStationarySelected != null)
				{
					StateStationarySelected.SetActive(hotspotInfo.DefensiveMode == AIMode.Stationary);
				}
				if (StateDefensiveSelected != null)
				{
					StateDefensiveSelected.SetActive(hotspotInfo.DefensiveMode == AIMode.Defending);
				}
				if (StateOffensiveSelected != null)
				{
					StateOffensiveSelected.SetActive(hotspotInfo.DefensiveMode == AIMode.Aggressive);
				}
				if (StateAISelectedLabel != null)
				{
					StateAISelectedLabel.text = HelpersLocalization.GetDefensiveModeDescription(hotspotInfo.DefensiveMode);
				}
			}
			else if (hotspotInfo.IsWalkerSpawn)
			{
				ClearHotspotButton.gameObject.SetActive(value: true);
				BoxOrFlagMenu.SetActive(value: false);
				NoSelectionMenu.SetActive(value: false);
				SurvivorMainParent.SetActive(value: false);
				UpdateWalkerDetails(show: true, hotspotInfo);
				if (WalkerClassIcon != null && WalkerPortrait != null)
				{
					if (hotspotInfo.WalkerType == WalkerType.WalkerNormal)
					{
						WalkerClassIcon.spriteName = "Ui_Icon_Class_WalkerNormal";
						WalkerPortrait.spriteName = "Ui_Texture_Walker_Normal";
					}
					else if (hotspotInfo.WalkerType == WalkerType.WalkerTank)
					{
						WalkerClassIcon.spriteName = "Ui_Icon_Class_WalkerTank";
						WalkerPortrait.spriteName = "Ui_Texture_Walker_Tank";
					}
					else if (hotspotInfo.WalkerType == WalkerType.WalkerArmored)
					{
						WalkerClassIcon.spriteName = "Ui_Icon_Class_WalkerArmored";
						WalkerPortrait.spriteName = "Ui_Texture_Walker_Armored";
					}
				}
			}
			else
			{
				ClearHotspotButton.gameObject.SetActive(value: true);
				BoxOrFlagMenu.SetActive(value: true);
				NoSelectionMenu.SetActive(value: false);
				SurvivorMainParent.SetActive(value: false);
				UpdateWalkerDetails(show: false, hotspotInfo);
			}
		}
		else
		{
			ClearHotspotButton.gameObject.SetActive(value: false);
			BoxOrFlagMenu.SetActive(value: false);
			NoSelectionMenu.SetActive(value: true);
			SurvivorMainParent.SetActive(value: false);
			UpdateWalkerDetails(show: false, hotspotInfo);
		}
	}

	private void UpdateSurvivorCard(SurvivorModel survivor)
	{
		if (survivor == null)
		{
			return;
		}
		if (survivorCard == null && survivorCardPrefab != null && survivoCardParent != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(survivorCardPrefab);
			if (gameObject != null)
			{
				survivorCard = gameObject.GetComponent<SurvivorCard>();
				survivorCard.transform.parent = survivoCardParent.transform;
				survivorCard.transform.localPosition = Vector3.zero;
				survivorCard.transform.localScale = Vector3.one;
				UIButton component = survivorCard.GetComponent<UIButton>();
				if (component != null && !component.onClick.Contains(new EventDelegate(OnEditTeamClicked)))
				{
					component.onClick.Add(new EventDelegate(OnEditTeamClicked));
				}
			}
		}
		if (survivorCard != null)
		{
			survivorCard.Type = SurvivorCard.CardType.Basic;
			survivorCard.Item = survivor;
			survivorCard.UpdateUI();
		}
		else
		{
			Debug.LogWarning("Could not Instantiate Survivor Card");
		}
	}

	private void UpdateWalkerDetails(bool show, HotspotInfo info)
	{
		if (GameManager.Instance == null || GameManager.Instance.playerModel.OutpostModel == null)
		{
			return;
		}
		OutpostLevelModel editLevelModel = GameManager.Instance.playerModel.OutpostModel.EditLevelModel;
		if (WalkerMenu != null)
		{
			show = info != null && show;
			WalkerMenu.SetActive(show);
		}
		if (editLevelModel != null && WalkerMenu.activeSelf)
		{
			int deploymentCostForHotspot = editLevelModel.GetDeploymentCostForHotspot(info.State, info.WalkerType);
			OutpostWalkerModel walkerModel = GameManager.Instance.playerModel.OutpostModel.GetWalkerModel(info.WalkerType.ToString());
			if (WalkerNameLabel != null)
			{
				WalkerNameLabel.text = LocalizationManager.GetText("Walker.Class." + info.WalkerType);
			}
			if (WalkerLevelLabel != null && walkerModel != null)
			{
				WalkerLevelLabel.text = walkerModel.Level.ToString();
			}
			if (DeploymentCostLabel != null)
			{
				DeploymentCostLabel.text = deploymentCostForHotspot.ToString() ?? "";
			}
		}
	}

	private SlicePanel GetCurrentSlicePanel()
	{
		if (Slices != null && Slices.TryGetValue(SelectedSlice, out var value))
		{
			return value;
		}
		return null;
	}
}
