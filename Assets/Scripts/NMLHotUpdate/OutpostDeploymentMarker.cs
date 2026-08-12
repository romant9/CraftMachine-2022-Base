using TWDModel;
using UnityEngine;

public class OutpostDeploymentMarker : OutpostDragDropItem
{
	public delegate void MarkerClickedHandler(OutpostDeploymentMarker marker);

	public const string SurvivorClassIconPrefix = "Ui_Icon_Class_";

	public MarkerClickedHandler OnMarkerClicked;

	public UISprite IconWalkerRegular;

	public UISprite IconWalkerTank;

	public UISprite IconWalkerArmored;

	public GameObject IconSurvivorA;

	public GameObject IconSurvivorB;

	public GameObject IconSurvivorC;

	public UISprite IconContainer;

	public UISprite IconFlag;

	public UILabel Count;

	public HotspotState CurrentState;

	public WalkerType CurrentWalkerType;

	public int CurrentCount;

	[Header("Backgrounds")]
	[SerializeField]
	private GameObject SurvivorBackground;

	[SerializeField]
	private GameObject WalkerBackground;

	[SerializeField]
	private GameObject FlagBackground;

	[SerializeField]
	private GameObject ResourceContainerBackground;

	public static OutpostDeploymentMarker CreateDeploymentMarker(GameObject deploymentMarkerPrefab, GameObject container, Vector3 offset)
	{
		GameObject gameObject = Object.Instantiate(deploymentMarkerPrefab);
		OutpostDeploymentMarker component = gameObject.GetComponent<OutpostDeploymentMarker>();
		if (component != null)
		{
			gameObject.transform.parent = container.transform;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.transform.localPosition = offset;
			return component;
		}
		Object.Destroy(gameObject);
		return component;
	}

	public void Active()
	{
		base.gameObject.SetActive(value: true);
	}

	public void Deactive()
	{
		base.gameObject.SetActive(value: false);
	}

	public new void OnClick()
	{
		if (OnMarkerClicked != null)
		{
			OnMarkerClicked(this);
		}
	}

	private void DisableAll()
	{
		IconWalkerRegular.gameObject.SetActive(value: false);
		IconWalkerTank.gameObject.SetActive(value: false);
		IconWalkerArmored.gameObject.SetActive(value: false);
		IconSurvivorA.SetActive(value: false);
		IconSurvivorB.SetActive(value: false);
		IconSurvivorC.SetActive(value: false);
		IconContainer.gameObject.SetActive(value: false);
		IconFlag.gameObject.SetActive(value: false);
		if (Count != null)
		{
			Count.gameObject.SetActive(value: false);
		}
	}

	private void UpdateClassIcon(GameObject parentObject, int defenderIndex)
	{
		UISprite componentInChildren = parentObject.GetComponentInChildren<UISprite>();
		if (componentInChildren != null)
		{
			componentInChildren.spriteName = "Ui_Icon_Class_" + GameManager.Instance.playerModel.SurvivorContainer.GetOutpostDefendingSurvivor(defenderIndex).SurvivorClass;
		}
	}

	public void Set(HotspotState state, WalkerType walkerType, int count)
	{
		DisableAll();
		CurrentState = state;
		CurrentWalkerType = walkerType;
		CurrentCount = count;
		if (state == HotspotState.DefenderSpawn_0)
		{
			IconSurvivorA.SetActive(value: true);
			UpdateClassIcon(IconSurvivorA, 0);
		}
		else if (state == HotspotState.DefenderSpawn_1)
		{
			IconSurvivorB.SetActive(value: true);
			UpdateClassIcon(IconSurvivorB, 1);
		}
		else if (state == HotspotState.DefenderSpawn_2)
		{
			IconSurvivorC.SetActive(value: true);
			UpdateClassIcon(IconSurvivorC, 2);
		}
		else if (state == HotspotState.Flag)
		{
			IconFlag.gameObject.SetActive(value: true);
		}
		else if (state == HotspotState.ResourceContainer)
		{
			IconContainer.gameObject.SetActive(value: true);
		}
		else if (state == HotspotState.Walker && walkerType == WalkerType.WalkerNormal)
		{
			IconWalkerRegular.gameObject.SetActive(value: true);
			if (Count != null)
			{
				Count.gameObject.SetActive(value: true);
				Count.text = count.ToString();
			}
		}
		else if (state == HotspotState.Walker && walkerType == WalkerType.WalkerTank)
		{
			IconWalkerTank.gameObject.SetActive(value: true);
			if (Count != null)
			{
				Count.gameObject.SetActive(value: true);
				Count.text = "1";
			}
		}
		else if (state == HotspotState.Walker && walkerType == WalkerType.WalkerArmored)
		{
			IconWalkerArmored.gameObject.SetActive(value: true);
			if (Count != null)
			{
				Count.gameObject.SetActive(value: true);
				Count.text = "1";
			}
		}
		UpdateBackground();
	}

	public bool CanPlaceOnTarget(OutpostSliceHotspot targetHotspot)
	{
		if (originalHotspot == null)
		{
			if (targetHotspot != null && targetHotspot.DeploymentMarker == null)
			{
				if (targetHotspot.HotspotModel.Type == HotspotType.Defender)
				{
					if (CurrentState == HotspotState.DefenderSpawn_0 || CurrentState == HotspotState.DefenderSpawn_1 || CurrentState == HotspotState.DefenderSpawn_2)
					{
						return true;
					}
				}
				else if (targetHotspot.HotspotModel.Type == HotspotType.Goal)
				{
					if (CurrentState == HotspotState.Flag || CurrentState == HotspotState.ResourceContainer)
					{
						return true;
					}
				}
				else if (targetHotspot.HotspotModel.Type == HotspotType.Walker && CurrentState == HotspotState.Walker)
				{
					return true;
				}
			}
			return false;
		}
		if (originalHotspot != null && targetHotspot != null && originalHotspot.HotspotModel.Type == targetHotspot.HotspotModel.Type && targetHotspot.DeploymentMarker == null)
		{
			return true;
		}
		return false;
	}

	protected void UpdateBackground()
	{
		Helpers.GameObjectSetActive(SurvivorBackground, value: false);
		Helpers.GameObjectSetActive(WalkerBackground, value: false);
		Helpers.GameObjectSetActive(FlagBackground, value: false);
		Helpers.GameObjectSetActive(ResourceContainerBackground, value: false);
		if (CurrentState == HotspotState.DefenderSpawn_0 || CurrentState == HotspotState.DefenderSpawn_1 || CurrentState == HotspotState.DefenderSpawn_2)
		{
			Helpers.GameObjectSetActive(SurvivorBackground, value: true);
		}
		else if (CurrentState == HotspotState.Walker)
		{
			Helpers.GameObjectSetActive(WalkerBackground, value: true);
		}
		else if (CurrentState == HotspotState.Flag)
		{
			Helpers.GameObjectSetActive(FlagBackground, value: true);
		}
		else if (CurrentState == HotspotState.ResourceContainer)
		{
			Helpers.GameObjectSetActive(ResourceContainerBackground, value: true);
		}
	}

	protected override bool CanPlace()
	{
		UIDragDropContainer destinationDropContainer = GetDestinationDropContainer();
		if (destinationDropContainer != null)
		{
			if (destinationDropContainer is OutpostDeploymentDropArea)
			{
				return true;
			}
			OutpostSliceHotspot component = destinationDropContainer.GetComponent<OutpostSliceHotspot>();
			return CanPlaceOnTarget(component);
		}
		return base.CanPlace();
	}

	protected override void OnDragDropStart()
	{
		base.OnDragDropStart();
		OutpostStateSlicePlaceItems outpostStateSlicePlaceItems = OutpostEditPopup.CurrentActiveStatePanel.OutpostStateBase as OutpostStateSlicePlaceItems;
		if (outpostStateSlicePlaceItems != null)
		{
			outpostStateSlicePlaceItems.DragStart(this);
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_drag");
		}
	}

	protected override void OnDragDropEnd(GameObject surface)
	{
		base.OnDragDropEnd(surface);
		HideIndicators();
		OutpostStateSlicePlaceItems outpostStateSlicePlaceItems = OutpostEditPopup.CurrentActiveStatePanel.OutpostStateBase as OutpostStateSlicePlaceItems;
		if (outpostStateSlicePlaceItems != null)
		{
			OutpostSliceHotspot destinationHotspot = GetDestinationHotspot();
			if (destinationHotspot != null && originalHotspot != null && (destinationHotspot == originalHotspot || !CanPlace()))
			{
				base.transform.localPosition = Vector3.zero;
				outpostStateSlicePlaceItems.DragCancel(this);
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_drag");
			}
			else if (destinationHotspot != null && originalHotspot != null)
			{
				outpostStateSlicePlaceItems.DragCompleteChangeHotspotPosition(this, destinationHotspot);
				base.transform.localPosition = Vector3.zero;
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_drop");
			}
			else if (destinationHotspot == null && originalHotspot != null)
			{
				outpostStateSlicePlaceItems.DragCompleteClearHotspot(this);
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_invalid_drop");
			}
			else if (destinationHotspot != null && originalHotspot == null && CanPlace())
			{
				outpostStateSlicePlaceItems.DragCompleteAddNewHotspot(this, destinationHotspot);
				base.transform.localPosition = Vector3.zero;
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_drop");
			}
			else
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_invalid_drop");
				outpostStateSlicePlaceItems.DragCancel(this);
			}
		}
	}
}
