using TWDModel;
using UnityEngine;

public class SurvivorOutfitsView : MonoBehaviour
{
	[Header("Titles")]
	[SerializeField]
	private UILabel MainTitle;

	[SerializeField]
	private UILabel OutfitTitle;

	[SerializeField]
	private UILabel SeasonTitle;

	[SerializeField]
	private UILabel PurchasedMessageLabel;

	[SerializeField]
	private UISprite MessageBackground;

	[Header("Cancel and Buy")]
	[SerializeField]
	private UIButton CancelButton;

	[SerializeField]
	private GameObject BuyButtonParent;

	[SerializeField]
	private PayButton BuyButton;

	[Header("Change Button")]
	[SerializeField]
	private UIButton NextButton;

	[SerializeField]
	private UIButton PreviousButton;

	[SerializeField]
	private OutfitList CurrentOutfitList;

	private int CurrentDefinitionGEDIndex;

	public OutfitDefinition CurrentOutfitDefinition { get; set; }

	[ContextMenu("Show")]
	public void Show(string OutfitDefinitionID)
	{
		CurrentOutfitDefinition = findDefinitionById(OutfitDefinitionID);
		base.gameObject.SetActive(value: true);
		if (CurrentOutfitList != null)
		{
			CurrentOutfitList.CreateItems(CurrentOutfitDefinition);
		}
		if (BuyButtonParent != null && PurchasedMessageLabel != null && MessageBackground != null)
		{
			BuyButtonParent.SetActive(value: false);
			PurchasedMessageLabel.gameObject.SetActive(value: false);
			PurchasedMessageLabel.text = LocalizationManager.GetText("Popup.SurvivorInfoPopup.PurchaseMessage");
			MessageBackground.gameObject.SetActive(value: false);
		}
		if (MainTitle != null)
		{
			MainTitle.text = LocalizationManager.GetText("Popup.SurvivorInfoPopup.OutfitPreview");
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/open_shop");
		clearOutfitTexts();
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (CurrentOutfitDefinition != null)
		{
			if (OutfitTitle != null)
			{
				OutfitTitle.text = LocalizationManager.GetText(CurrentOutfitDefinition.TitleLocalizationKey);
			}
			if (SeasonTitle != null)
			{
				SeasonTitle.text = LocalizationManager.GetText(CurrentOutfitDefinition.SeasonLocalizationKey);
			}
			if (BuyButtonParent != null && PurchasedMessageLabel != null && MessageBackground != null)
			{
				bool flag = GameManager.Instance.playerModel.SurvivorContainer.HasOutfit(CurrentOutfitDefinition.ID);
				BuyButtonParent.SetActive(!flag);
				PurchasedMessageLabel.gameObject.SetActive(BuyButtonParent.activeSelf);
				MessageBackground.gameObject.SetActive(BuyButtonParent.activeSelf);
				if (!flag)
				{
					Cashier cashier = Cashier.CreateOneItemCashier(GameManager.Instance.modelManager, PurchaseType.None, CurrencyType.Diamonds, CurrentOutfitDefinition.Cost);
					BuyButton.UpdateUI(cashier, LocalizationManager.GetText("Popup.SurvivorInfoPopup.Button.Buy"));
				}
			}
		}
		else
		{
			if (BuyButtonParent != null && PurchasedMessageLabel != null && MessageBackground != null)
			{
				BuyButtonParent.SetActive(value: false);
				PurchasedMessageLabel.gameObject.SetActive(value: false);
				MessageBackground.gameObject.SetActive(value: false);
			}
			clearOutfitTexts();
		}
	}

	[ContextMenu("Hide")]
	public void Hide()
	{
		CurrentOutfitList.ClearCards();
		base.gameObject.SetActive(value: false);
		CurrentOutfitDefinition = null;
	}

	public void BuyOutfitClicked()
	{
		if (CurrentOutfitDefinition != null)
		{
			Cashier cashier = Cashier.CreateOneItemCashier(GameManager.Instance.modelManager, PurchaseType.None, CurrencyType.Diamonds, CurrentOutfitDefinition.Cost);
			if (cashier.CanAfford())
			{
				ConfirmationPopup confirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
				string text = LocalizationManager.GetText(CurrentOutfitDefinition.TitleLocalizationKey);
				confirmationPopup.SetContent(LocalizationManager.GetText("Popup.BuyOutfitConfirmation.Title{outfitTitle}", text), LocalizationManager.GetText("Popup.BuyOutfitConfirmation.Message{outfitTitle}", text));
				confirmationPopup.SetCurrencies(cashier);
				confirmationPopup.SetCallbacks(BuyOutfitConfirmed);
				confirmationPopup.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
				confirmationPopup.Open();
			}
			else
			{
				ShopPopupHelper.OpenForMissingCurrencyWithMissingAmount(cashier.GetMissing(CurrencyType.Diamonds));
			}
		}
	}

	private void BuyOutfitConfirmed()
	{
		if (CurrentOutfitDefinition != null && Helpers.ExecuteCommand(new BuyOutfitCommand(CurrentOutfitDefinition.ID)) == TWDModelResult.OK)
		{
			UIEvent.Send("OnNewOutfitBought", CurrentOutfitDefinition);
			SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.PermanentlySwitchToOutfit(CurrentOutfitDefinition, null);
			UpdateUI();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/purchase");
		}
	}

	public void NextClicked()
	{
		SelectNextOutfit(-1);
	}

	public void PreviousClicked()
	{
		SelectNextOutfit(1);
	}

	private void SelectNextOutfit(int step)
	{
		int num = GameManager.Instance.playerModel.gameEconomyData.GetAvailableOutfitDefinitions(GameManager.Instance.playerModel.UtcTimeStamp).Count - 1;
		CurrentDefinitionGEDIndex += step;
		CurrentDefinitionGEDIndex = ((CurrentDefinitionGEDIndex < 0) ? num : CurrentDefinitionGEDIndex);
		CurrentDefinitionGEDIndex = ((CurrentDefinitionGEDIndex <= num) ? CurrentDefinitionGEDIndex : 0);
		CurrentOutfitDefinition = GameManager.Instance.playerModel.gameEconomyData.GetAvailableOutfitDefinitions(GameManager.Instance.playerModel.UtcTimeStamp)[CurrentDefinitionGEDIndex];
		UIEvent.Send("OnNewOutfitSeleted", CurrentOutfitDefinition);
		UpdateUI();
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnNewOutfitSeleted")
		{
			if (parameter is OutfitDefinition currentOutfitDefinition)
			{
				CurrentOutfitDefinition = currentOutfitDefinition;
				CurrentDefinitionGEDIndex = GameManager.Instance.playerModel.gameEconomyData.GetAvailableOutfitDefinitions(GameManager.Instance.playerModel.UtcTimeStamp).IndexOf(CurrentOutfitDefinition);
				if (GameManager.Instance.playerModel.SurvivorContainer.HasOutfit(CurrentOutfitDefinition.ID))
				{
					SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.PermanentlySwitchToOutfit(CurrentOutfitDefinition, null);
				}
				SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.RequestSwitchOutfit(CurrentOutfitDefinition);
				UpdateUI();
			}
		}
		else if (type == "OnNewOutfitDeseleted")
		{
			SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.PermanentlySwitchBackToDefault(null);
			CurrentOutfitDefinition = null;
			UpdateUI();
		}
	}

	private void clearOutfitTexts()
	{
		if (OutfitTitle != null)
		{
			OutfitTitle.text = "";
		}
		if (SeasonTitle != null)
		{
			SeasonTitle.text = "";
		}
	}

	private OutfitDefinition findDefinitionById(string definitionID)
	{
		for (int i = 0; i < GameManager.Instance.playerModel.gameEconomyData.GetAvailableOutfitDefinitions(GameManager.Instance.playerModel.UtcTimeStamp).Count; i++)
		{
			if (GameManager.Instance.playerModel.gameEconomyData.GetAvailableOutfitDefinitions(GameManager.Instance.playerModel.UtcTimeStamp)[i].ID == definitionID)
			{
				return GameManager.Instance.playerModel.gameEconomyData.GetAvailableOutfitDefinitions(GameManager.Instance.playerModel.UtcTimeStamp)[i];
			}
		}
		return null;
	}
}
