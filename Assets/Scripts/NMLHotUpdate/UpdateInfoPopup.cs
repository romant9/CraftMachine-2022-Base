using TWDModel;
using UnityEngine;

public class UpdateInfoPopup : HUDElement
{
	[SerializeField]
	private GameObject bannerCloseArea;

	[SerializeField]
	private GameObject progressContainer;

	[SerializeField]
	private UIProgressBar progress;

	[SerializeField]
	private UIButton okButton;

	[SerializeField]
	private float cantCloseBeforeSeconds = 5f;

	private bool canClose;

	private float timeToClose;

	public override void Open()
	{
		progressContainer.SetActive(value: true);
		canClose = false;
		base.Open();
		timeToClose = cantCloseBeforeSeconds;
		if (!GameManager.Instance.Blackboard.IsToggleOn("Toggle.ToggleUpdateInfoPopupShown"))
		{
			Helpers.ExecuteCommand(new FeatureUnlockedSeenCommand("Toggle.ToggleUpdateInfoPopupShown"));
		}
		else
		{
			timeToClose = 0f;
			canClose = true;
			progressContainer.SetActive(value: false);
		}
		SetOkButtonState();
	}

	private void SetOkButtonState()
	{
		if (canClose)
		{
			okButton.SetState(UIButtonColor.State.Normal, true);
		}
		else
		{
			okButton.SetState(UIButtonColor.State.Disabled, true);
		}
	}

	public override void Update()
	{
		base.Update();
		if (timeToClose > 0f)
		{
			timeToClose -= Time.deltaTime;
			progress.value = timeToClose / cantCloseBeforeSeconds;
			if (timeToClose <= 0f)
			{
				progressContainer.SetActive(value: false);
				canClose = true;
				SetOkButtonState();
			}
		}
	}

	public override void OnClickClose()
	{
		if (canClose)
		{
			base.OnClickClose();
			TutorialView.Instance.StartNextTutorial();
			string updateGift = GameManager.Instance.gameEconomyData.ConfigData.UpdateGift;
			if (!string.IsNullOrEmpty(updateGift))
			{
				Rewards rewards = new Rewards(updateGift);
				CampView.Instance.BuildingsHud.CreateCollectAnim(rewards, null);
				Helpers.ExecuteCommand(new UpdateGiftCommand());
			}
		}
	}

	public void OnClickVideo()
	{
		Application.OpenURL(GameManager.Instance.gameEconomyData.ConfigData.UpdateInfoVideoUrl);
	}

	public void OnClickMoreInfo()
	{
		GameManager.Instance.PlayerHubManager.OpenNewsletter();
		Close();
	}
}
