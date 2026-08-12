using System.Collections;
using System.Linq;
using TWDModel;
using UnityEngine;

namespace Client.BlackMarket
{
	public class RefreshBlackMarketHeroPopup : HUDElement
	{
		[SerializeField]
		private UILabel costLabel;

		private BlackMarketHeroSlot heroSlot;

		private readonly WaitForSeconds oneSecondWait = new WaitForSeconds(1f);

		private Coroutine timeLeftCoroutine;

		public void OpenFor(BlackMarketHeroSlot heroSlot)
		{
			this.heroSlot = heroSlot;
			costLabel.text = GameManager.Instance.gameEconomyData.ConfigData.BlackMarketRefreshCost.ToString();
			if (timeLeftCoroutine != null)
			{
				timeLeftCoroutine = StartCoroutine(UpdateTimeLeft());
			}
			Open();
		}

		public void RefreshHero()
		{
			if (heroSlot.CanRefresh(GameManager.Instance.playerModel))
			{
				if (GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.Diamonds) >= GameManager.Instance.modelManager.GameEconomyData.ConfigData.BlackMarketRefreshCost)
				{
					OpenConfirmationPopup();
					return;
				}
				MiniShopPopup.OpenWithTotalRequiredCurrencyAmount(CurrencyType.Diamonds, GameManager.Instance.gameEconomyData.ConfigData.BlackMarketRefreshCost);
				OnClickClose();
			}
		}

		private void OpenConfirmationPopup()
		{
			int blackMarketRefreshCost = GameManager.Instance.modelManager.GameEconomyData.ConfigData.BlackMarketRefreshCost;
			CurrencyType currencyType = CurrencyType.Diamonds;
			BuyResourcesPopup obj = (BuyResourcesPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup);
			obj.SetConfirmContent(LocalizationManager.GetText("Popup.BuyResources.TradeCrate"), LocalizationManager.GetText("Popup.Shop.BlackMarket.Refresh"), blackMarketRefreshCost, currencyType);
			obj.SetCallbacks(ExecuteRefreshCommand);
			obj.Open();
		}

		private void ExecuteRefreshCommand()
		{
			RefreshBlackMarketSlotCommand refreshCommand = new RefreshBlackMarketSlotCommand
			{
				ActorId = BlackMarketShopController.Instance.ActiveHero.ActiveActorDefinitionID
			};
			BlackMarketHeroSlot blackMarketHeroSlot = GameManager.Instance.playerModel.BlackMarket.Slots.FirstOrDefault((BlackMarketHeroSlot x) => x.ActiveActorDefinitionID == refreshCommand.ActorId);
			if (Helpers.ExecuteCommand(refreshCommand) == TWDModelResult.OK)
			{
				BlackMarketShopController.Instance.RefreshedHero(blackMarketHeroSlot?.ActiveActorDefinitionID);
				OnClickClose();
			}
		}

		private IEnumerator UpdateTimeLeft()
		{
			while (true)
			{
				if (!heroSlot.CanRefresh(GameManager.Instance.playerModel))
				{
					Close();
				}
				yield return oneSecondWait;
			}
		}

		public override void Close()
		{
			base.Close();
			if (timeLeftCoroutine != null)
			{
				StopCoroutine(timeLeftCoroutine);
				timeLeftCoroutine = null;
			}
		}
	}
}
