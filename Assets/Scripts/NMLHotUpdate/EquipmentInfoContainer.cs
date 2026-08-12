using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class EquipmentInfoContainer : HUDElement
{
	[Tooltip("Equipment sprite")]
	public UITexture EquipmentTexture;

	[Tooltip("Equipment sprite")]
	public UISprite EquipmentIcon;

	[SerializeField]
	[Tooltip("Equipment available icon")]
	private UISprite AvailableIcon;

	[Tooltip("Equipment activated icon")]
	public UISprite ActivatedIcon;

	[Tooltip("Available equipment sprite")]
	public UISprite AvailableEquipmentIcon;

	[Tooltip("Available equipment info label")]
	public UILabel AvailableEquipmentInfoLabel;

	[Tooltip("Activated equipment sprite")]
	public UISprite ActivatedEquipmentIcon;

	[Tooltip("Activated equipment info label")]
	public UILabel ActivatedEquipmentInfoLabel;

	[Tooltip("Container object for the activation point icons.")]
	public GameObject ActivationPointContainer;

	[Tooltip("List of activation point icons, in ascending order")]
	public List<UISprite> ActivationPointIcons;

	[Tooltip("Grid for the activation point icons.")]
	public UIGrid ActivationPointGrid;

	public void SetAvailableIcon(ActorModel actor, bool show)
	{
		if (actor != null)
		{
			if (actor.FocusModeStateChargeCD)
			{
				Helpers.GameObjectSetActive(AvailableIcon, value: false);
			}
			else
			{
				Helpers.GameObjectSetActive(AvailableIcon, show);
			}
		}
	}

	public void FreshAvailableIcon(ActorModel actor)
	{
		if (actor != null)
		{
			if (actor.FocusModeStateChargeCD)
			{
				Helpers.GameObjectSetActive(AvailableIcon, value: false);
			}
			else
			{
				Helpers.GameObjectSetActive(AvailableIcon, actor.ChargeMeter.ChargeAvailable);
			}
		}
	}

	public UISprite GetAvailableIcon()
	{
		return AvailableIcon;
	}
}
