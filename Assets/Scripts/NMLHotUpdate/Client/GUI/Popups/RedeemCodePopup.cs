using Client.Redeem;
using TWDModel;
using UnityEngine;

namespace Client.GUI.Popups
{
	public class RedeemCodePopup : HUDElement
	{
		[Header("Redeem Code Popup")]
		[SerializeField]
		private UIInput codeInput;

		[SerializeField]
		private UILabel errorMessageLabel;

		[SerializeField]
		private GameObject successMessageObject;

		[SerializeField]
		private GameObject placeHolderTextObject;

		private IRedeemManager redeemManager;

		public override void Start()
		{
			redeemManager = new GiftCodeRedeemManager();
			codeInput.OnSelected += delegate(bool selected)
			{
				Helpers.GameObjectSetActive(placeHolderTextObject, codeInput.value.Length <= 0 && !selected);
			};
		}

		public override void Open()
		{
			base.Open();
			Helpers.GameObjectSetActive(errorMessageLabel, value: false);
			Helpers.GameObjectSetActive(successMessageObject, value: false);
		}

		public void OnRedeem()
		{
			string text = null;
			IRedeemDefinition redeemDefinition;
			switch (redeemManager.RedeemCode(codeInput.value, out redeemDefinition))
			{
			case RedeemValidity.AlreadyClaimed:
				text = LocalizationManager.GetText("Popup.RedeemCode.Error.Duplicate");
				break;
			case RedeemValidity.Invalid:
				text = LocalizationManager.GetText("Popup.RedeemCode.Error.Invalid");
				break;
			case RedeemValidity.Expired:
				text = LocalizationManager.GetText("Popup.RedeemCode.Error.Expired");
				break;
			case RedeemValidity.LevelOffRange:
				if (redeemDefinition is GiftCodeDefinition giftCodeDefinition)
				{
					text = LocalizationManager.GetText("Popup.RedeemCode.Error.MaxCouncil{Parameter}", giftCodeDefinition.MaxCouncil);
				}
				break;
			case RedeemValidity.Valid:
			{
				IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
				if ((bool)iAPConfirmPopupNew)
				{
					iAPConfirmPopupNew.OpenForRewards(redeemDefinition.Rewards.RewardsList);
					iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.RedeemCode.Success"), null);
				}
				break;
			}
			}
			bool flag = !string.IsNullOrEmpty(text);
			Helpers.GameObjectSetActive(errorMessageLabel, flag);
			Helpers.GameObjectSetActive(successMessageObject, !flag);
			if (flag)
			{
				errorMessageLabel.text = text;
			}
		}
	}
}
