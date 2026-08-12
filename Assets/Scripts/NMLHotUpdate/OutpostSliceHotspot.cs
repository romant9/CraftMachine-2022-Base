using TWDModel;
using UnityEngine;

public class OutpostSliceHotspot : MonoBehaviour
{
	public string SpriteHotspotSingleActor = "Ui_Tile_Zone_Survivor";

	public string SpriteHotspotMultiActor = "Ui_Tile_Zone_Walker";

	public string SpriteHotspotObject = "Ui_Tile_Zone_Object";

	public UISprite Background;

	public UISprite BackgroundHightlight;

	public GameObject SelectedParent;

	public GameObject HightlightParent;

	public GameObject DeploymentMarkerPrefab;

	public OutpostHotspotModel HotspotModel { get; private set; }

	public OutpostDeploymentMarker DeploymentMarker { get; set; }

	public event HotspotClickedHandler OnHotspotClicked;

	private void NotifyHotspotClicked()
	{
		this.OnHotspotClicked?.Invoke(this);
	}

	public void EnableColliders(bool enabled)
	{
		BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
		if (component != null)
		{
			component.enabled = enabled;
		}
		if (DeploymentMarker != null)
		{
			component = DeploymentMarker.gameObject.GetComponent<BoxCollider>();
			if (component != null)
			{
				component.enabled = enabled;
			}
		}
	}

	public void SetSelected(bool selected)
	{
		if (SelectedParent != null)
		{
			SelectedParent.SetActive(selected);
		}
	}

	public void SetHighlight(bool value)
	{
		if (HightlightParent != null)
		{
			HightlightParent.SetActive(value);
		}
	}

	public string GetBackgroundSpriteName(HotspotType hotspotType)
	{
		return hotspotType switch
		{
			HotspotType.SingleActor => SpriteHotspotSingleActor, 
			HotspotType.MultiActor => SpriteHotspotMultiActor, 
			HotspotType.Goal => SpriteHotspotObject, 
			HotspotType.Defender => SpriteHotspotSingleActor, 
			HotspotType.Walker => SpriteHotspotMultiActor, 
			_ => null, 
		};
	}

	public void Set(OutpostHotspotModel hotspotModel)
	{
		HotspotModel = hotspotModel;
		if (Background != null)
		{
			Background.spriteName = GetBackgroundSpriteName(hotspotModel?.Type ?? HotspotType.Goal);
		}
		if (BackgroundHightlight != null)
		{
			BackgroundHightlight.spriteName = GetBackgroundSpriteName(hotspotModel?.Type ?? HotspotType.Goal);
		}
	}

	public void SetState(HotspotState state, WalkerType walkerType, int count)
	{
		if (state == HotspotState.None && DeploymentMarker != null)
		{
			Object.Destroy(DeploymentMarker.gameObject);
		}
		else if (state != HotspotState.None && DeploymentMarker == null && DeploymentMarkerPrefab != null)
		{
			GameObject gameObject = Object.Instantiate(DeploymentMarkerPrefab);
			DeploymentMarker = gameObject.GetComponent<OutpostDeploymentMarker>();
			if (DeploymentMarker != null)
			{
				DeploymentMarker.restriction = UIDragDropItem.Restriction.None;
				DeploymentMarker.OnMarkerClicked = MarkerClicked;
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			else
			{
				Object.Destroy(gameObject);
			}
		}
		if (DeploymentMarker != null)
		{
			DeploymentMarker.Set(state, walkerType, count);
			DeploymentMarker.OnMarkerClicked = MarkerClicked;
		}
	}

	private void MarkerClicked(OutpostDeploymentMarker marker)
	{
		if (HotspotModel != null)
		{
			NotifyHotspotClicked();
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_select_area");
	}

	public void OnSliceHotspotClicked()
	{
	}
}
