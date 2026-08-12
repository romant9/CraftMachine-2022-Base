using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class BadgeRerollPopup : BadgeReceivePopup
{
	[SerializeField]
	private UILabel rerollCostLabel;

	[SerializeField]
	private UILabel rerollHeaderLabel;

	[SerializeField]
	private UILabel rerollTypeLabel;

	[SerializeField]
	private UIButton rerollButton;

	[Header("Trait reroll meter")]
	[SerializeField]
	private HUDMeter traitRerollMeter;

	private BadgeReroll reroll;

	public const string RerolledSlot = "Popup.Badges.Reroll.RerolledSlot";

	public const string RerolledSet = "Popup.Badges.Reroll.RerolledSet";

	public const string RerolledBonus = "Popup.Badges.Reroll.RerolledBonus";

	public const string RerollSlot = "Popup.Badges.Reroll.RerollSlot";

	public const string RerollSet = "Popup.Badges.Reroll.RerollSet";

	public const string RerollBonus = "Popup.Badges.Reroll.RerollBonus";

	private readonly Dictionary<BadgeReroll, string> rerollHeaderLocalizationKeys = new Dictionary<BadgeReroll, string>
	{
		{
			BadgeReroll.Slot,
			"Popup.Badges.Reroll.RerolledSlot"
		},
		{
			BadgeReroll.Set,
			"Popup.Badges.Reroll.RerolledSet"
		},
		{
			BadgeReroll.Bonus,
			"Popup.Badges.Reroll.RerolledBonus"
		}
	};

	private readonly Dictionary<BadgeReroll, string> rerollButtonLocalizationKeys = new Dictionary<BadgeReroll, string>
	{
		{
			BadgeReroll.Slot,
			"Popup.Badges.Reroll.RerollSlot"
		},
		{
			BadgeReroll.Set,
			"Popup.Badges.Reroll.RerollSet"
		},
		{
			BadgeReroll.Bonus,
			"Popup.Badges.Reroll.RerollBonus"
		}
	};

	[SerializeField]
	private Color enoughCurrency = Color.white;

	[SerializeField]
	private Color notEnoughCurrency = new Color(0.6313726f, 0.18431373f, 0.101960786f);

	private static IEnumerator cor;

	public override void Open()
	{
		base.Open();
		int badgeReRollCost = GameManager.Instance.playerModel.LootManager.GetBadgeReRollCost(model.ModelId, reroll);
		rerollCostLabel.text = badgeReRollCost.ToString();
		int currencyAmount = GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.TraitRerollToken);
		traitRerollMeter.SetCurrencyType(CurrencyType.TraitRerollToken);
		traitRerollMeter.SetValue(currencyAmount);
		rerollCostLabel.color = ((currencyAmount >= badgeReRollCost) ? enoughCurrency : notEnoughCurrency);
		HelpersUI.SetButtonState(rerollButton, (currencyAmount < badgeReRollCost) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
		rerollHeaderLabel.text = LocalizationManager.GetText(rerollHeaderLocalizationKeys[reroll]);
		rerollTypeLabel.text = LocalizationManager.GetText(rerollButtonLocalizationKeys[reroll]);
	}

	public override void OnClickClose()
	{
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BadgeDetailsPopup);
		if (!(hUDElement == null))
		{
			hUDElement.OpenForModel(GameManager.Instance.playerModel.LastCraftedBadge);
			base.OnClickClose();
		}
	}

	public void RerollAgain()
	{
		OpenConfirmationPopup(reroll);
	}

	private void ExecuteReRollCommand()
	{
		if (Helpers.ExecuteCommand(new RerollBadgeCommand
		{
			RerollType = reroll,
			BadgeModelId = model.ModelId
		}) == TWDModelResult.OK)
		{
			GameManager.Instance.CheckConnectionReachability(showPopup: true, "RerollBadgeCommand");
			Close();
			Helpers.StartCoroutine(GameManager.Instance, Reroll(), ref cor);
		}
	}

	private IEnumerator Reroll()
	{
		while (SingularityMonoBehaviour<HUDManager>.Instance.GetOpenPopupsList().Any((HUDElement x) => x is BadgeRerollPopup))
		{
			yield return new WaitForEndOfFrame();
		}
		BadgeRerollPopup badgeRerollPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BadgeRerollPopup) as BadgeRerollPopup;
		badgeRerollPopup.SetRerollType(reroll);
		if (badgeRerollPopup != null)
		{
			badgeRerollPopup.OpenForModel(GameManager.Instance.playerModel.LastCraftedBadge);
		}
		UIEvent.Send("OnBadgeRerolled");
	}

	public void SetRerollType(BadgeReroll reroll)
	{
		this.reroll = reroll;
	}

	private void OpenConfirmationPopup(BadgeReroll reroll)
	{
		int badgeReRollCost = GameManager.Instance.playerModel.LootManager.GetBadgeReRollCost(model.ModelId, reroll);
		CurrencyType currencyType = CurrencyType.TraitRerollToken;
		BuyResourcesPopup obj = (BuyResourcesPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup);
		obj.GetComponent<UIPanel>().depth = BuyResourcesPopup.BadgeRerollDepth;
		obj.SetConfirmContent(LocalizationManager.GetText("Popup.BuyResources.TradeCrate"), LocalizationManager.GetText("Popup.Badges.Details.RerollTitle"), badgeReRollCost, currencyType);
		obj.SetCallbacks(delegate
		{
			ExecuteReRollCommand();
		});
		obj.Open();
	}
}
