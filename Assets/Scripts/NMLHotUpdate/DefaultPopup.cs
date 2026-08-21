using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class DefaultPopup : HUDElement
{
	private HUDElement popup;

	[SerializeField]
	public UIButton actionButton;

	[SerializeField]
	private UILabel actionButtonLabel;

	[SerializeField]
	private GameObject whiteBackgroundContainer;

	[SerializeField]
	private GameObject whiteBackgroundContainerAnchorTarget;

	[SerializeField]
	private UISprite background;

	[SerializeField]
	private GameObject normalBackgroundContainer;

	[SerializeField]
	private GameObject lockedPanel;

	[SerializeField]
	private UILabel lockedPanelLabel;

	[SerializeField]
	private UIButton closeButton;

	[SerializeField]
	private UIButton notificationOkButton;

	[SerializeField]
	private GameObject payPanel;

	[SerializeField]
	private GameObject instantPayPanel;

	[SerializeField]
	private PayButton instantPayWithTokensButton;

	[SerializeField]
	private UILabel instantPayWithTokensButtonLabel;

	[SerializeField]
	private PayButton instantPayButton;

	[SerializeField]
	private UILabel instantPayButtonLabel;

	[SerializeField]
	private PayButton payButton;

	[SerializeField]
	private UILabel payButtonLabel;

	[SerializeField]
	private UIButton cannotPayButton;

	[SerializeField]
	private GameObject cornersContainer;

	[SerializeField]
	[Tooltip("The area around the popup  that you can click to close the pop-up.")]
	private GameObject closeArea;

	[Header("Simple buttons")]
	[SerializeField]
	private GameObject containerSimpleButtons;

	[SerializeField]
	private UIButton positveButton;

	[SerializeField]
	private UILabel positveButtonLabel;

	[SerializeField]
	private UIButton negativeButton;

	[SerializeField]
	private UILabel negativeButtonLabel;

	[SerializeField]
	private UILabel questionLabel;

	[SerializeField]
	private UILabel offerTimeLeft;

	[SerializeField]
	private UILabel BTLevelLabel;

	[SerializeField]
	private UIButton BreakThroughBtn;

	[SerializeField]
	private GameObject BreakThroughContainer;

	[SerializeField]
	private GameObject BreakThroughMaxContainer;

	[SerializeField]
	private GameObject LockedInfoContainer;

	private Cashier tokenCashier;

	public static int DefaultWidth = 760;

	public static int DefaultHeightSmall = 445;

	public static int DefaultHeightBig = 550;

	public static int GuildInfoPopupDepth = 44;

	private Stack<HUDElement> popupStack = new Stack<HUDElement>();

	private void OnEnable()
	{
		lockedPanel.SetActive(value: false);
		HideAllPayButtons();
		AllowNormalClosing(active: true);
		if (offerTimeLeft != null)
		{
			offerTimeLeft.gameObject.SetActive(value: false);
		}
	}

	public void SetOfferTime(string timeLeft)
	{
		if (offerTimeLeft != null)
		{
			if (!offerTimeLeft.gameObject.activeSelf)
			{
				offerTimeLeft.gameObject.SetActive(value: true);
			}
			offerTimeLeft.text = timeLeft;
		}
	}

	public void AddPopUp(HUDElement popup)
	{
		if (this.popup != null)
		{
			this.popup.gameObject.SetActive(value: false);
			popupStack.Push(this.popup);
		}
		this.popup = popup;
		popup.gameObject.SetActive(value: true);
		GetComponent<UIPanel>().depth = popup.GetComponent<UIPanel>().depth - 1;
		SetSize(popup.DefaultPopUpWidth, popup.DefaultPopUpHeight);
		if (popup.ForcePositionY == -1)
		{
			base.transform.localPosition = Vector3.zero;
		}
		else
		{
			UIPanel component = GetComponent<UIPanel>();
			Vector3 localPosition = new Vector3(0f, (component.root.activeHeight - popup.DefaultPopUpHeight) / 2 - popup.ForcePositionY, 0f);
			base.transform.localPosition = localPosition;
			popup.transform.localPosition = localPosition;
		}
		containerSimpleButtons.SetActive(value: false);
		if (notificationOkButton != null)
		{
			notificationOkButton.gameObject.SetActive(popup.UseWhiteBackground);
		}
		if (cornersContainer != null)
		{
			cornersContainer.SetActive(!popup.UseWhiteBackground);
		}
		if (closeButton != null)
		{
			closeButton.gameObject.SetActive(!popup.UseWhiteBackground);
		}
		SetWhiteBackground(popup.UseWhiteBackground);
		if (actionButton != null)
		{
			actionButton.gameObject.SetActive(value: false);
		}
	}

	public override void Close()
	{
		base.Close();
		if (popup != null)
		{
			popup.Close();
			popup = null;
		}
		if (popupStack.Count > 0)
		{
			base.gameObject.SetActive(value: true);
			AddPopUp(popupStack.Pop());
		}
	}

	public void SetSize(int w, int h)
	{
		if ((bool)background)
		{
			background.width = w;
			background.height = h;
			BoxCollider component = background.GetComponent<BoxCollider>();
			Vector3 size = component.size;
			size.x = w;
			size.y = h;
			component.size = size;
		}
	}

	public void SetWhiteBackground(bool useWhiteBackground)
	{
		if (background != null)
		{
			background.gameObject.SetActive(!useWhiteBackground);
		}
		if (normalBackgroundContainer != null)
		{
			normalBackgroundContainer.SetActive(!useWhiteBackground);
		}
		if (whiteBackgroundContainer != null)
		{
			whiteBackgroundContainer.SetActive(useWhiteBackground);
		}
		if (closeButton != null)
		{
			if (useWhiteBackground)
			{
				closeButton.gameObject.GetComponent<UIWidget>().SetAnchor(whiteBackgroundContainerAnchorTarget.transform);
			}
			else
			{
				closeButton.gameObject.GetComponent<UIWidget>().SetAnchor(background.transform);
			}
		}
	}

	public void AllowNormalClosing(bool active)
	{
		Helpers.GameObjectSetActive(closeArea, active);
		Helpers.GameObjectSetActive(closeButton, active);
	}

	public void ShowLockedPanel(string text)
	{
		if (text == null)
		{
			lockedPanel.SetActive(value: false);
			return;
		}
		lockedPanel.SetActive(value: true);
		lockedPanelLabel.text = text;
		HideAllPayButtons();
	}

	public void SetBreakthtroughInfoActive(bool active, bool showMax, EquipmentItemModel equipmentItemModel)
	{
		Helpers.GameObjectSetActive(BreakThroughContainer, value: false);
		if (!active || !Helpers.CanShowBreakthroughBtn(equipmentItemModel))
		{
			return;
		}
		Helpers.GameObjectSetActive(BreakThroughMaxContainer, showMax);
		Helpers.GameObjectSetActive(BreakThroughContainer, value: true);
		Helpers.GameObjectSetActive(LockedInfoContainer, value: false);
		BreakThroughBtn.onClick.Clear();
		BreakThroughBtn.onClick.Add(new EventDelegate(delegate
		{
			BreakThroughPopup breakThroughPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BreakThroughPopup) as BreakThroughPopup;
			if (breakThroughPopup != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
				breakThroughPopup.OpenForModel(equipmentItemModel);
			}
		}));
		BTLevelLabel.text = LocalizationManager.GetText("Popup.EquipmentLevelUp.BreakthroughsLevel{Parameter}", equipmentItemModel.BreakthroughLevel);
	}

	public void ShowPayButtons()
	{
		lockedPanel.SetActive(value: false);
		instantPayPanel.gameObject.SetActive(value: true);
		payButton.gameObject.SetActive(value: true);
	}

	public void ShowPayOnlyWithCurrencyButton(bool canAfford)
	{
		lockedPanel.SetActive(value: false);
		payButton.gameObject.SetActive(value: true);
		if (!canAfford)
		{
			UIButton component = payButton.GetComponent<UIButton>();
			component.SetState(UIButtonColor.State.Disabled, true);
			component.isEnabled = false;
			component.onClick.Clear();
		}
	}

	public void ShowCannotPayButton()
	{
		Helpers.GameObjectSetActive(cannotPayButton, value: true);
	}

	public void HideAllPayButtons()
	{
		SetInstantPayPanel(active: false);
		HidePayButton();
		HideCannotPayButton();
	}

	public void SetInstantPayPanel(bool active)
	{
		instantPayPanel.gameObject.SetActive(active);
	}

	public void HideInstantPayButton()
	{
		instantPayButton.gameObject.SetActive(value: false);
	}

	public void HidePayButton()
	{
		payButton.gameObject.SetActive(value: false);
	}

	public void HideCannotPayButton()
	{
		Helpers.GameObjectSetActive(cannotPayButton, value: false);
	}

	public void ShowActionButton(bool show)
	{
		if (actionButton != null)
		{
			actionButton.gameObject.SetActive(show);
		}
	}

	public void SetActionButton(bool available, string text, EventDelegate.Callback callback)
	{
		if (actionButton != null && actionButtonLabel != null)
		{
			actionButton.gameObject.SetActive(available);
			actionButtonLabel.text = text;
		}
		if (callback != null)
		{
			SetActionButtonClickCallback(callback);
		}
	}

	public void SetPayButton(string labelText, Cashier cashier, int upgradeTime = -1, bool twoCurrenciesPayment = false)
	{
		payButton.UpdateUI(cashier, labelText, upgradeTime, null, twoCurrenciesPayment);
	}

	public void SetInstantPayButton(Cashier cashier)
	{
		instantPayButton.UpdateUI(cashier, LocalizationManager.GetText("Popup.DefaultPopup.Button.Instant"));
	}

	public void SetInstantPayWithTokensButton(Cashier cashier)
	{
		instantPayWithTokensButton.UpdateUI(cashier);
		tokenCashier = cashier;
	}

	public void SetPayButtonClickCallback(EventDelegate.Callback callback)
	{
		UIButton component = payButton.GetComponent<UIButton>();
		component.onClick.Clear();
		component.onClick.Add(new EventDelegate(callback));
		component.onClick.Add(new EventDelegate(OnClickPayButton));
		if (!component.isEnabled)
		{
			component.SetState(UIButtonColor.State.Normal, true);
			component.isEnabled = true;
		}
	}

	public void SetCannotPayClickCallback(EventDelegate.Callback callback)
	{
		cannotPayButton.onClick.Clear();
		cannotPayButton.onClick.Add(new EventDelegate(callback));
	}

	public void SetInstantPayButtonClickCallback(EventDelegate.Callback callback)
	{
		UIButton component = instantPayButton.GetComponent<UIButton>();
		component.onClick.Clear();
		component.onClick.Add(new EventDelegate(callback));
		component.onClick.Add(new EventDelegate(OnClickPayInstantButton));
	}

	public void SetInstantPayWithTokensButtonClickCallback(EventDelegate.Callback callback)
	{
		UIButton component = instantPayWithTokensButton.GetComponent<UIButton>();
		component.onClick.Clear();
		component.onClick.Add(new EventDelegate(OnClickPayInstantWithTokensButton));
		component.onClick.Add(new EventDelegate(callback));
		component.onClick.Add(new EventDelegate(OnClickPayInstantButton));
	}

	public void SetActionButtonClickCallback(EventDelegate.Callback callback)
	{
		UIButton component = actionButton.GetComponent<UIButton>();
		component.onClick.Clear();
		component.onClick.Add(new EventDelegate(callback));
	}

	public void OnClickPayButton()
	{
		EventManager.NotifyClick("Buy");
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	public void OnClickPayInstantButton()
	{
		EventManager.NotifyClick("BuyInstant");
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	public void OnClickPayInstantWithTokensButton()
	{
		if (!tokenCashier.CanAfford())
		{
			TooltipManager.OpenTextBoxWithText(instantPayWithTokensButton.gameObject, LocalizationManager.GetText("Tooltip.BattlePass.SpeedupToken.Missing"));
		}
	}

	public void ShowOkButton(bool show)
	{
		notificationOkButton.gameObject.SetActive(show);
	}

	public void ShowSimpleButtons(bool show)
	{
		containerSimpleButtons.SetActive(show);
	}

	public void SetSimplePositiveButton(bool available, string text = null, EventDelegate.Callback callback = null)
	{
		SetSimpleButton(positveButton, positveButtonLabel, available, text, callback);
	}

	public void SetSimpleNegativeButton(bool available, string text = null, EventDelegate.Callback callback = null)
	{
		SetSimpleButton(negativeButton, negativeButtonLabel, available, text, callback);
	}

	public void SetSimpleButton(UIButton button, UILabel label, bool available, string text, EventDelegate.Callback callback)
	{
		button.gameObject.SetActive(available);
		label.text = text;
		if (callback != null)
		{
			button.onClick.Clear();
			button.onClick.Add(new EventDelegate(callback));
		}
	}

	public void SetQuestion(string text)
	{
		questionLabel.gameObject.SetActive(text != null);
		questionLabel.text = text;
	}

	public void ShowCommandError(TWDModelResult result, bool closeOnSuccess = true)
	{
		if (result == TWDModelResult.OK)
		{
			if (closeOnSuccess)
			{
				Close();
			}
		}
		else
		{
			HUDNotification.Error(LocalizationManager.GetText("Error." + result));
		}
	}

	#region mycode
	public void SetPositiveButtonSize(int width, int height)
	{
		var sprite = positveButton.tweenTarget.GetComponent<UISprite>();

		if (sprite)
		{
			sprite.width = width;
			sprite.height = height;
		}
	}
	#endregion
}
