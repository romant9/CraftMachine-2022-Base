using TWDModel;

public class SurvivorInfoStateHeroPreview : SurvivorInfoStateBase
{
	public override void Init()
	{
		base.Init();
		CurrentState = States.SurvivorHeroPreview;
	}

	public override void Enter()
	{
		base.Enter();
		SurvivorInfoPopup.AllowWeapons = false;
		PlayAnchorTween(base.SurvivorStatistics, TweenAnchorId.Show);
		PlayAnchorTween(base.SurvivorRightSidePanel, TweenAnchorId.Show);
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.SurvivorNamePanel != null)
		{
			base.SurvivorNamePanel.EnableNameInput(value: false);
		}
		UpdateAndShowSurvivorNavigationButtons();
		if (base.SurvivorModel != null)
		{
			bool num = base.SurvivorModel.SurvivorRarityLevel > base.SurvivorModel.StartingRarityLevel;
			bool flag = base.SurvivorModel.Level > base.SurvivorModel.StartingLevel;
			bool flag2 = num || flag;
			Helpers.GameObjectSetActive(base.PreviewMaxStatsButton, !flag2);
			Helpers.GameObjectSetActive(base.PreviewMaxStatsReturnButton, flag2);
			CurrencyType traitUpgradeCurrency = base.SurvivorModel.Definition.TraitUpgradeCurrency;
			Cashier heroUnlockCashier = GameManager.Instance.playerModel.SurvivorContainer.GetHeroUnlockCashier(traitUpgradeCurrency);
			bool flag3 = heroUnlockCashier.CanAfford();
			int totalCost = heroUnlockCashier.GetTotalCost(traitUpgradeCurrency);
			string content = GameManager.Instance.playerModel.GetCurrency(traitUpgradeCurrency).Value + "/" + totalCost;
			if (flag3)
			{
				Helpers.GameObjectSetActive(base.UnlockUiButton, value: true);
				base.UnlockUiButton.SetContentToIconOne(HelpersGfx.GetCurrencyIconName(traitUpgradeCurrency));
				base.UnlockUiButton.SetContentToLabelTwo(content);
			}
			else
			{
				Helpers.GameObjectSetActive(base.MoreTokensUiButton, value: true);
				base.MoreTokensUiButton.SetContentToIconOne(HelpersGfx.GetCurrencyIconName(traitUpgradeCurrency));
				base.MoreTokensUiButton.SetContentToLabelTwo(content);
			}
			HelpersUI.SetContentToLabel(base.SurvivorDescriptionLabel, HelpersLocalization.GetHeroDescription(base.SurvivorModel.Definition));
		}
	}

	protected override void UpdateAndShowBadges()
	{
		if (base.SurvivorRightSidePanel != null)
		{
			base.SurvivorRightSidePanel.SetActiveButtons(value: false);
		}
	}
}
