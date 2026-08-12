using System;
using TWDModel;

public static class BattlePassClientHelpers
{
	public static void StartPremiumActivationFlow(Action postPurchasePopupAction = null)
	{
		ConfirmationPopup confirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BattlePassPurchasedPopup) as ConfirmationPopup;
		if ((bool)confirmationPopup)
		{
			confirmationPopup.SetCallbacks(PremiumPurchasePanelClosed, PremiumPurchasePanelClosed);
			confirmationPopup.Open();
		}
		else
		{
			PremiumPurchasePanelClosed();
		}
		void PremiumPurchasePanelClosed()
		{
			BattlePassModel battlePass = GameManager.Instance.playerModel.BattlePass;
			Rewards lastClaimedRewards = battlePass.LastClaimedRewards;
			if (lastClaimedRewards != null && lastClaimedRewards.Count > 0)
			{
				IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
				if ((bool)iAPConfirmPopupNew)
				{
					iAPConfirmPopupNew.OpenForRewards(battlePass.LastClaimedRewards.RewardsList);
					iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.BattlePass.Premium.RewardAutoClaim.Title"), LocalizationManager.GetText("Popup.BattlePass.Premium.RewardAutoClaim.Subtitle"));
					iAPConfirmPopupNew.SetCloseAnimOverCallback(ShowGuildPerkPopup);
				}
			}
			else
			{
				ShowGuildPerkPopup();
			}
			postPurchasePopupAction?.Invoke();
		}
		static void ShowGuildPerkPopup()
		{
			IAPConfirmPopupNew obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			obj.OpenForCurrency(new RewardCurrency
			{
				CurrencyType = CurrencyType.FreeGuildGiftPerk,
				Amount = 1
			});
			obj.SetContent(LocalizationManager.GetText("Popup.BattlePass.Premium.GuildPerkClaim.Title"), LocalizationManager.GetText("Popup.BattlePass.Premium.GuildPerkClaim.Subtitle"));
		}
	}

	public static bool IsInPreBeginnerBattlePassState(this PlayerModel playerModel)
	{
		GameEconomyData gameEconomyData = playerModel.manager.GameEconomyData;
		if (playerModel.BeginnerBattlePassInfo.State == BeginnerBattlePassState.NotStarted)
		{
			return playerModel.CouncilLevel >= gameEconomyData.BeginnerBattlePassConfig.CouncilLockLevel;
		}
		return false;
	}
}
