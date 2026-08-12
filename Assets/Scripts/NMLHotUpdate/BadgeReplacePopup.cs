using System;
using UnityEngine;

public class BadgeReplacePopup : HUDElement
{
	[SerializeField]
	private UIButtonExtended ScrapButton;

	[SerializeField]
	private UIButtonExtended RemoveButton;

	[SerializeField]
	private UIButtonExtended CloseButton;

	[SerializeField]
	private UILabel CostLabel;

	private Action scrapCallback;

	private Action removeCallback;

	public override void Open()
	{
		base.Open();
		if (CloseButton != null)
		{
			CloseButton.SetClickCallback(OnCloseButtonClicked);
		}
		if (ScrapButton != null)
		{
			ScrapButton.SetClickCallback(OnScrap);
		}
		if (RemoveButton != null)
		{
			RemoveButton.SetClickCallback(OnRemove);
		}
		int badgeReclaimCost = GameManager.Instance.playerModel.ActivityManager.GetBadgeReclaimCost(GameManager.Instance.gameEconomyData.ConfigData);
		HelpersUI.SetContentToLabel(CostLabel, badgeReclaimCost.ToString());
	}

	public void SetCallbacks(Action scrapCallback, Action removeCallback)
	{
		this.scrapCallback = scrapCallback;
		this.removeCallback = removeCallback;
	}

	public void OnScrap(UIButtonExtended button)
	{
		if (scrapCallback != null)
		{
			scrapCallback();
		}
	}

	public void OnRemove(UIButtonExtended button)
	{
		if (removeCallback != null)
		{
			removeCallback();
		}
	}

	public void OnCloseButtonClicked(UIButtonExtended button)
	{
		base.OnClickClose();
	}
}
