using TWDModel;

public class SurvivorInfoStateTrainPreview : SurvivorInfoStateBase
{
	public override void Init()
	{
		base.Init();
		CurrentState = States.SurvivorTrainPreview;
	}

	public int GetDamage(int level)
	{
		if (base.SurvivorModel.manager == null || base.SurvivorModel.manager.Player == null)
		{
			return 0;
		}
		SurvivalManualManager survivalManualManager = base.SurvivorModel.manager.Player.SurvivalManualManager;
		int num = 0;
		int num2 = 0;
		if (survivalManualManager != null)
		{
			num = survivalManualManager.GetAttackClinet(base.SurvivorModel);
			num2 = survivalManualManager.GetPrivateAttackRatioClient(base.SurvivorModel) + survivalManualManager.GetAttributeAttackRatioClient();
		}
		float num3 = (float)base.SurvivorModel.GetDamageForPreferredWeaponForLevel(level) / 100f * (float)(100 + num2) + (float)num;
		if (base.SurvivorModel.manager.Player.Tutorial.HasCompletedPart("Phone") && base.SurvivorModel.FeaturedDefinition != null)
		{
			num3 += num3 * ((float)base.SurvivorModel.FeaturedDefinition.DamageBoostMultiplier / 100f);
		}
		return (int)num3;
	}

	public int GetHeal(int level)
	{
		if (base.SurvivorModel.manager == null || base.SurvivorModel.manager.Player == null)
		{
			return 0;
		}
		SurvivalManualManager survivalManualManager = base.SurvivorModel.manager.Player.SurvivalManualManager;
		int num = 0;
		int num2 = 0;
		if (survivalManualManager != null)
		{
			num = survivalManualManager.GetHPClinet(base.SurvivorModel);
			num2 = survivalManualManager.GetPrivateHpRatioClient(base.SurvivorModel) + survivalManualManager.GetAttributeHpRatioClient();
		}
		float num3 = (float)base.SurvivorModel.GetHitpointsForLevel(level) / 100f * (float)(100 + num2) + (float)num;
		if (base.SurvivorModel.manager.Player.Tutorial.HasCompletedPart("Phone") && base.SurvivorModel.FeaturedDefinition != null)
		{
			num3 += num3 * ((float)base.SurvivorModel.FeaturedDefinition.HealthBoostMultiplier / 100f);
		}
		return (int)num3;
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.SurvivorModel != null)
		{
			if (base.UpgradePanel != null)
			{
				base.UpgradePanel.SetDamageValue(GetDamage(base.SurvivorModel.Level + 1));
				base.UpgradePanel.SetHealthValue(GetHeal(base.SurvivorModel.Level + 1));
				base.UpgradePanel.SetLevelValue(base.SurvivorModel.Level + 1);
			}
			if (base.TrainButton != null && base.TrainButton.GetComponent<PayButton>() != null)
			{
				Cashier upgradeCashier = base.SurvivorModel.GetUpgradeCashier(instantUpgrade: false);
				string text = LocalizationManager.GetText("Popup.SurvivorLevelUp.Button.LevelUp");
				int upgradeTime = base.SurvivorModel.UpgradeTime;
				base.TrainButton.GetComponent<PayButton>().UpdateUI(upgradeCashier, text, upgradeTime);
			}
			bool value = TutorialView.Instance != null && TutorialView.Instance.Model != null && (!TutorialView.Instance.Running || TutorialView.Instance.Model.GetCurrentStepDefinition.Id >= 6);
			if (base.TrainInstanButton != null && base.TrainInstanButton.GetComponent<PayButton>() != null && Helpers.GameObjectSetActive(base.TrainInstanButton, value))
			{
				base.TrainInstanButton.GetComponent<PayButton>().UpdateUI(base.SurvivorModel.GetUpgradeCashier(instantUpgrade: true, addInitialSurvivorPoints: true), LocalizationManager.GetText("Popup.SurvivorLevelUp.Button.LevelUp"), base.SurvivorModel.UpgradeTime);
			}
			if (base.TrainInstantWithTokensButton != null && base.TrainInstantWithTokensButton.TryGetComponent<PayButton>(out var component) && Helpers.GameObjectSetActive(base.TrainInstantWithTokensButton, value))
			{
				component.UpdateUI(base.SurvivorModel.GetUpgradeCashier(instantUpgrade: true, addInitialSurvivorPoints: false, useTokens: true), LocalizationManager.GetText("Popup.SurvivorLevelUp.Button.LevelUp"), base.SurvivorModel.UpgradeTime);
			}
		}
	}

	public override void Enter()
	{
		base.Enter();
		if (base.SurvivorModel != null && !base.SurvivorModel.CanUpgrade)
		{
			SetState(States.SurvivorOverview);
			return;
		}
		if (base.UpgradePanel != null)
		{
			Helpers.GameObjectSetActive(base.UpgradePanel.gameObject, value: true);
			base.SurvivorStatistics.HideFeaturedHeroContainer();
		}
		PlayAnchorTween(base.SurvivorStatistics, TweenAnchorId.Show);
		PlayAnchorTween(base.SurvivorRightSidePanel, TweenAnchorId.Hide);
	}
}
