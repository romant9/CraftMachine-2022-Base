using Client.Redeem;
using Decagames.Externals.SingularSDK;
using TWDModel;
using UnityEngine;

namespace TWD.Externals
{
	public class DeepLinkActionResolver : MonoBehaviour
	{
		private DeepLinkRedeemManager deepLinkRedeemManager;

		public void Initialize()
		{
			deepLinkRedeemManager = new DeepLinkRedeemManager();
			SingularityMonoBehaviour<SDKManager>.Instance.SingularSDKManager.OnDeepLinkAction += OnSingularDeepLinkReceived;
		}

		private void OnSingularDeepLinkReceived(string deepLink, SingularDeepLinkResult afDeepLinkResult)
		{
			if (afDeepLinkResult == SingularDeepLinkResult.ERROR)
			{
				Debug.LogError("DeepLink Error with " + deepLink);
			}
			if (afDeepLinkResult == SingularDeepLinkResult.FOUND || afDeepLinkResult == SingularDeepLinkResult.DEFERRED)
			{
				if (DeepLinkNavigation.HandleDeepLink(GetDeepLinkActionByDeepLink(deepLink)))
				{
					OnRedeemDeepLink(deepLink);
				}
			}
			else
			{
				DeepLinkNavigation.HandleNativeDeepLink(deepLink);
			}
		}

		private string GetDeepLinkActionByDeepLink(string deepLink)
		{
			GameManager.Instance.gameEconomyData.TryGetDeepLinkDefinition(deepLink, out var deepLinkDefinition);
			return deepLinkDefinition.DeepLinkAction;
		}

		public void OnRedeemDeepLink(string deepLink)
		{
			string value = null;
			IRedeemDefinition redeemDefinition;
			switch (deepLinkRedeemManager.RedeemCode(deepLink, out redeemDefinition))
			{
			case RedeemValidity.AlreadyClaimed:
				value = LocalizationManager.GetText("Popup.RedeemCode.Error.Duplicate");
				break;
			case RedeemValidity.Invalid:
				value = LocalizationManager.GetText("Popup.RedeemCode.Error.Invalid");
				break;
			case RedeemValidity.Expired:
				value = LocalizationManager.GetText("Popup.RedeemCode.Error.Expired");
				break;
			case RedeemValidity.LevelOffRange:
				if (redeemDefinition is DeepLinkDefinition deepLinkDefinition)
				{
					value = LocalizationManager.GetText("Popup.RedeemCode.Error.MaxCouncil{Parameter}", deepLinkDefinition.MaxCouncil);
				}
				break;
			case RedeemValidity.Valid:
			{
				IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
				if (iAPConfirmPopupNew != null)
				{
					iAPConfirmPopupNew.OpenForRewards(redeemDefinition.Rewards.RewardsList);
					iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.RedeemCode.Success"), null);
				}
				break;
			}
			}
			_ = !string.IsNullOrEmpty(value);
		}
	}
}
