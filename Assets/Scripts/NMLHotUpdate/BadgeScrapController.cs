using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class BadgeScrapController : MonoBehaviour
{
	public ResidenceBadgeInventoryTab BadgeInventoryTab;

	private List<BadgeModel> badgesToScrap = new List<BadgeModel>();

	[SerializeField]
	private UIButtonExtended scrapButton;

	[SerializeField]
	private UIButtonExtended scrapCancelButton;

	[SerializeField]
	private UIButtonExtended scrapConfirmButton;

	[SerializeField]
	private GameObject scrapOptionsContainer;

	[SerializeField]
	private UILabel scrappingTotalAmount;

	public bool ScrapModeActive { get; private set; }

	private void OnEnable()
	{
		scrapButton.SetClickCallback(delegate
		{
			SetScrapMode(enabled: true);
		});
		scrapConfirmButton.SetClickCallback(OnClickScrap);
		scrapCancelButton.SetClickCallback(OnCancelScrap);
	}

	private void OnDisable()
	{
		if (ScrapModeActive)
		{
			SetScrapMode(enabled: false);
		}
		scrapButton.Clear();
		scrapConfirmButton.Clear();
		scrapCancelButton.Clear();
	}

	public void OnBadgeClicked(BadgeInfo badge)
	{
		if (!ScrapModeActive)
		{
			return;
		}
		string eventName = "global/ui_drop";
		if (string.IsNullOrEmpty(badge.OwnerName))
		{
			badge.ScrapSelected = !badge.ScrapSelected;
			if (!badge.ScrapSelected)
			{
				eventName = "global/ui_drag";
			}
		}
		else
		{
			badge.ScrapSelected = false;
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(eventName);
		UpdateTotalScrapRefund();
	}

	private void UpdateTotalScrapRefund()
	{
		int num = 0;
		foreach (BadgeInfo allBadge in BadgeInventoryTab.allBadges)
		{
			if (allBadge != null && allBadge.ScrapSelected)
			{
				num += allBadge.Model.GetScrapCashier().GetTotalCost(CurrencyType.SurvivalPoints);
			}
		}
		HelpersUI.SetContentToLabel(scrappingTotalAmount, num.ToString());
	}

	private void OnClickScrap(UIButtonExtended button)
	{
		if (badgesToScrap == null)
		{
			badgesToScrap = new List<BadgeModel>();
		}
		else
		{
			badgesToScrap.Clear();
		}
		foreach (BadgeInfo allBadge in BadgeInventoryTab.allBadges)
		{
			if (allBadge != null && allBadge.ScrapSelected)
			{
				badgesToScrap.Add(allBadge.Model);
			}
		}
		if (badgesToScrap.Count > 0)
		{
			Cashier badgeListScrapCashier = GameManager.Instance.playerModel.Equipment.GetBadgeListScrapCashier(badgesToScrap);
			ConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
			obj.SetContent(LocalizationManager.GetText("Popup.ScrapConfirmationList.Badges.Title"), LocalizationManager.GetText("Popup.ScrapConfirmationList.Badges.Message"));
			obj.SetCurrencies(badgeListScrapCashier);
			obj.SetCallbacks(OnScrapBadgesConfirmed, OnScrapBadgesCancelled);
			obj.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
			obj.SetCancelButtonLabel(LocalizationManager.GetText("Button.Cancel"));
			obj.Open();
		}
	}

	private void OnCancelScrap(UIButtonExtended button)
	{
		OnScrapBadgesCancelled();
	}

	private void OnScrapBadgesConfirmed()
	{
		SetScrapMode(enabled: false);
		if (badgesToScrap.Count > 0)
		{
			List<int> badgeIdsToScrap = badgesToScrap.Select((BadgeModel x) => x.ModelId).ToList();
			if (Helpers.ExecuteCommand(new ScrapBadgesCommand
			{
				modelIds = badgeIdsToScrap
			}) == TWDModelResult.OK)
			{
				BadgeInventoryTab.allBadges.RemoveAll((BadgeInfo x) => badgeIdsToScrap.Contains(x.Model.ModelId));
			}
			badgesToScrap.Clear();
			BadgeInventoryTab.ForceUpdate();
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/workshop_equipment_scrap");
	}

	private void OnScrapBadgesCancelled()
	{
		SetScrapMode(enabled: false);
	}

	public void SetScrapMode(bool enabled)
	{
		ScrapModeActive = enabled;
		for (int i = 0; i < BadgeInventoryTab.allBadges.Count; i++)
		{
			BadgeInventoryTab.allBadges[i].ScrapModeEnabled = enabled;
			BadgeInventoryTab.allBadges[i].ScrapSelected = false;
		}
		BadgeInventoryTab.UpdateInventory();
		Helpers.GameObjectSetActive(scrapOptionsContainer, ScrapModeActive);
		HelpersUI.SetContentToLabel(scrappingTotalAmount, "0");
		Helpers.GameObjectSetActive(scrapButton, !ScrapModeActive);
	}
}
