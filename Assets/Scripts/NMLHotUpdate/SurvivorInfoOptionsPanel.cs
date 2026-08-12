using TWDModel;
using UnityEngine;

public class SurvivorInfoOptionsPanel : MonoBehaviour
{
	[Tooltip("Survivor Instant Train")]
	[SerializeField]
	private PayButton instantTrainButton;

	[Tooltip("Survivor Train")]
	[SerializeField]
	private PayButton trainButton;

	[Tooltip("Survivor Retire")]
	[SerializeField]
	private UIButton retireButton;

	[SerializeField]
	private UILabel retireLabel;

	[Tooltip("Survivor Change Outfit")]
	[SerializeField]
	private UIButton changeOutfitButton;

	[Tooltip("Back Button")]
	[SerializeField]
	private UIButton backButton;

	[SerializeField]
	private UILabel backLabel;

	[Tooltip("Promote Button")]
	[SerializeField]
	private PayButton PromoteButton;

	[Tooltip("Upgrade Info Parent")]
	[SerializeField]
	private GameObject messageParent;

	[Tooltip("Upgrade Info")]
	[SerializeField]
	private UILabel messageLabel;

	[Header("Share")]
	[SerializeField]
	private UIButton shareButton;

	[SerializeField]
	private GameObject sharePanel;

	[SerializeField]
	private UITexture shareBadge;

	public void Start()
	{
		if (retireLabel != null)
		{
			retireLabel.text = LocalizationManager.GetText("Popup.SurvivorInfoPopup.Button.Demote");
		}
		if (backLabel != null)
		{
			backLabel.text = LocalizationManager.GetText("Popup.SurvivorInfoPopup.Button.Back");
		}
	}

	public void SetPayButton(string labelText, Cashier cashier, int upgradeTime = -1)
	{
		if (!(trainButton != null) || cashier == null)
		{
			return;
		}
		trainButton.UpdateUI(cashier, labelText, upgradeTime);
		if (!TutorialView.Instance.Running)
		{
			Transform transform = trainButton.transform.Find("TutorialSuggest");
			if (transform != null)
			{
				transform.gameObject.SetActive(value: false);
			}
		}
	}

	public void SetUpgradeTraitButton(string labelText, Cashier cashier, bool active)
	{
		if (PromoteButton != null && cashier != null)
		{
			Helpers.GameObjectSetActive(PromoteButton.gameObject, active);
			PromoteButton.UpdateUI(cashier, labelText);
		}
	}

	public void SetInstantPayButton(Cashier cashier)
	{
		if (instantTrainButton != null && cashier != null)
		{
			instantTrainButton.UpdateUI(cashier, LocalizationManager.GetText("Popup.DefaultPopup.Button.Instant"));
		}
	}

	public void SetBackButton(bool enabled)
	{
		setActive(backButton.gameObject, enabled);
	}

	public void ShowOutfitButton(bool show = true)
	{
		if (changeOutfitButton != null)
		{
			if (!GameManager.Instance.playerModel.gameEconomyData.ConfigData.BetaFlag_Outfits)
			{
				changeOutfitButton.gameObject.SetActive(value: false);
			}
			else
			{
				changeOutfitButton.gameObject.SetActive(show);
			}
		}
	}

	public void showPayTrainButtons(bool showPayButton = true, bool showInstantPayButton = true)
	{
		setActive(trainButton.gameObject, showPayButton);
		setActive(instantTrainButton.gameObject, showInstantPayButton);
	}

	public void showRetire()
	{
		setActive(retireButton.gameObject, value: true);
	}

	public void hideRetire()
	{
		setActive(retireButton.gameObject, value: false);
	}

	public void hidePayTrainButtons()
	{
		setActive(trainButton.gameObject, value: false);
		setActive(instantTrainButton.gameObject, value: false);
	}

	public void hideAllButtons()
	{
		hidePayTrainButtons();
		if (retireButton != null && changeOutfitButton != null && messageParent != null)
		{
			setActive(retireButton.gameObject, value: false);
			setActive(changeOutfitButton.gameObject, value: false);
			setActive(backButton.gameObject, value: false);
			setActive(messageParent.gameObject, value: false);
		}
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
			Helpers.GameObjectSetActive(obj, value);
		}
	}

	private void ShowUiForScreenshot(bool show)
	{
		sharePanel.SetActive(show);
	}

	public void OnClickShare()
	{
		StartCoroutine(GetComponent<ScreenshotShare>().TakeScreenshot("Survivor", shareButton, shareBadge, ShowUiForScreenshot));
	}

	public void Show()
	{
		ShowUiForScreenshot(show: false);
	}
}
