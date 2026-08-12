using System.Collections.Generic;
using TWDModel;
using UnityEngine;

namespace Client.HCoin
{
	public class HCoinShopController : MonoBehaviour
	{
		[SerializeField]
		private UITexture heroImage;

		[SerializeField]
		private UIButton closeButton;

		[SerializeField]
		private UILabel title;

		[Header("Main List")]
		[SerializeField]
		private NUIScrollableList scrollableList;

		[SerializeField]
		private GameObject container;

		[SerializeField]
		private ShopPopup shopPopup;

		public static HCoinShopController Instance;

		public HillTopStoreSlot ActiveHero;

		private void Awake()
		{
			Instance = this;
		}

		private void OnDisable()
		{
		}

		public void UpdateUI()
		{
			HillTopStoreSlotDefinition hillTopStoreSlotDefinition = GameManager.Instance.gameEconomyData.GetHillTopStoreSlotDefinition(ActiveHero.SlotType);
			string textId = "None";
			string text = "None";
			if (hillTopStoreSlotDefinition != null)
			{
				text = hillTopStoreSlotDefinition.CoverImagePath;
				textId = hillTopStoreSlotDefinition.CoverLocalizationKey;
			}
			title.text = LocalizationManager.GetText(textId);
			heroImage.mainTexture = (Texture)UnityUtils.LoadFromAssetBundle(text, "itemgraphics");
		}

		public void HideContent()
		{
			container.SetActive(value: false);
			ShopPopupHelper.UpdateListWithData(scrollableList, new List<BundleStoreDefinition>(), resetScrollPosition: true, isTabsIndexFeaturedShop: false);
		}

		public void OnCloseEventHandler()
		{
			HideContent();
			shopPopup.ShowMainContent();
			shopPopup.UpdateCardUIs();
			shopPopup.SetToggleSetVisibility(show: true);
		}

		public void ShowFor(HillTopStoreSlot hillTopStoreSlot)
		{
			ActiveHero = hillTopStoreSlot;
			UpdateUI();
			List<HillTopStoreDefinition> activeOffers = hillTopStoreSlot.GetActiveOffers();
			ShopPopupHelper.UpdateListWithData(scrollableList, activeOffers, resetScrollPosition: true, isTabsIndexFeaturedShop: false);
			shopPopup.HideMainContent();
			shopPopup.SetToggleSetVisibility(show: false);
			container.SetActive(value: true);
			scrollableList.SortAndReset();
		}
	}
}
