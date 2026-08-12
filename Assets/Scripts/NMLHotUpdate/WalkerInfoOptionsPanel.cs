using TWDModel;
using UnityEngine;

public class WalkerInfoOptionsPanel : MonoBehaviour
{
	[Tooltip("Instant Train")]
	[SerializeField]
	private PayButton instantTrainButton;

	[Tooltip("Train")]
	[SerializeField]
	private PayButton trainButton;

	[SerializeField]
	private PayButton upgradeAmountButton;

	[Tooltip("Back Button")]
	[SerializeField]
	private UIButton backButton;

	[SerializeField]
	private UILabel backLabel;

	[Tooltip("Upgrade Info Parent")]
	[SerializeField]
	private GameObject messageParent;

	[Tooltip("Upgrade Info")]
	[SerializeField]
	private UILabel messageLabel;

	[SerializeField]
	private GameObject messageAmountLockedParent;

	[SerializeField]
	private UILabel messageAmountLockedLabel;

	[Header("Share")]
	[SerializeField]
	private UIButton shareButton;

	[SerializeField]
	private GameObject sharePanel;

	[SerializeField]
	private UITexture shareBadge;

	public void Start()
	{
		if (backLabel != null)
		{
			backLabel.text = LocalizationManager.GetText("Popup.SurvivorLevelUp.Button.Back");
		}
	}

	public void SetPayButton(string labelText, Cashier cashier, int upgradeTime = -1)
	{
		if (trainButton != null && cashier != null)
		{
			trainButton.UpdateUI(cashier, labelText, upgradeTime);
		}
	}

	public void SetInstantPayButton(Cashier cashier, string text = null)
	{
		if (instantTrainButton != null && cashier != null)
		{
			if (text == null)
			{
				LocalizationManager.GetText("Popup.DefaultPopup.Button.Instant");
			}
			instantTrainButton.UpdateUI(cashier, text);
		}
	}

	public void SetBackButton(bool enabled)
	{
		setActive(backButton.gameObject, enabled);
	}

	public void showPayTrainButtons(bool showPayButton = true, bool showInstantPayButton = true)
	{
		setActive(trainButton.gameObject, showPayButton);
		setActive(instantTrainButton.gameObject, showInstantPayButton);
	}

	public void hidePayTrainButtons()
	{
		setActive(trainButton.gameObject, value: false);
		setActive(instantTrainButton.gameObject, value: false);
	}

	public void hideAllButtons()
	{
		hidePayTrainButtons();
		if (messageParent != null)
		{
			setActive(backButton.gameObject, value: false);
			setActive(messageParent.gameObject, value: false);
		}
	}

	public void SetUpgradeAmountPayButton(Cashier cashier)
	{
		if (upgradeAmountButton != null && cashier != null)
		{
			upgradeAmountButton.UpdateUI(cashier);
		}
	}

	public void showUpgradeAmountButton(bool show)
	{
		setActive(upgradeAmountButton.gameObject, show);
	}

	public void showUpgradeAmountLocked(string text)
	{
		setActive(messageAmountLockedParent, text != null);
		messageAmountLockedLabel.text = text;
	}

	public void showMessage(string messageText)
	{
		if (messageLabel != null)
		{
			if (messageText != "")
			{
				setActive(messageParent, value: true);
			}
			else
			{
				setActive(messageParent, value: false);
			}
			messageLabel.text = messageText;
		}
	}

	private void setActive(GameObject obj, bool value)
	{
		if (obj != null)
		{
			obj.gameObject.SetActive(value);
		}
	}

	private void ShowUiForScreenshot(bool show)
	{
		sharePanel.SetActive(show);
	}

	public void OnClickShare()
	{
		StartCoroutine(GetComponent<ScreenshotShare>().TakeScreenshot("Walker", shareButton, shareBadge, ShowUiForScreenshot));
	}

	public void Show()
	{
		ShowUiForScreenshot(show: false);
	}
}
