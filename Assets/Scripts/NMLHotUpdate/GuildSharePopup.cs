using System.Collections;
using BaseModel;
using TWDModel;
using UnityEngine;

public class GuildSharePopup : HUDElement
{
	[SerializeField]
	private UISprite rewardSpriteComplete;

	[SerializeField]
	private UILabel titleLabel;

	[Header("Share screen")]
	[SerializeField]
	private UIButton shareButton;

	[SerializeField]
	private UILabel URLLabel;

	[SerializeField]
	private UIButton copyURLButton;

	[SerializeField]
	private GameObject sharePanel;

	[SerializeField]
	private UITexture shareBadge;

	[SerializeField]
	private UISprite normalImage;

	private bool seasonRewardMode;

	private GuildModel guild;

	public override void OpenForModel(ModelObject model)
	{
		base.OpenForModel(model);
		guild = ((GuildModelWrapper)model).GuildModel;
		UpdateUI();
	}

	public override void UpdateUI()
	{
		if (!seasonRewardMode)
		{
			if (normalImage != null)
			{
				normalImage.enabled = true;
			}
			if (URLLabel != null && guild != null)
			{
				URLLabel.text = "getnomansland.com?guild=" + guild.Id;
			}
			titleLabel.text = LocalizationManager.GetText("Popup.GuildShare.Title{GuildName}", guild.Name);
		}
	}

	public void OnShareClick()
	{
		string shareURL = ((URLLabel != null) ? URLLabel.text : "");
		string shareText = ((URLLabel != null) ? LocalizationManager.GetText("Popup.GuildShare.ShareGuildText{GuildURL}", URLLabel.text) : LocalizationManager.GetText("Popup.Share.DefaultMessage"));
		StartCoroutine(GetComponent<ScreenshotShare>().TakeScreenshot("MissionReward", shareButton, shareBadge, ShowUiForScreenshot, ShareCompleteCallback, skipPreview: true, shareURL, shareText));
	}

	private void ShareCompleteCallback()
	{
		Close();
	}

	private void ShowUiForScreenshot(bool show)
	{
		sharePanel.SetActive(show);
		if ((bool)shareButton)
		{
			shareButton.gameObject.SetActive(!show);
		}
		if (URLLabel != null)
		{
			URLLabel.gameObject.SetActive(!show);
		}
		if (copyURLButton != null)
		{
			copyURLButton.gameObject.SetActive(!show);
		}
	}

	public void CopyURLClick()
	{
		if (URLLabel != null)
		{
			GUIUtility.systemCopyBuffer = URLLabel.text;
		}
	}

	private IEnumerator DelayedClose(float delay)
	{
		yield return new WaitForSeconds(delay);
		Close();
	}
}
