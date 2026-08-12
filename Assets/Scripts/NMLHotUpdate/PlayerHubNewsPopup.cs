using System;
using TWDModel;
using UnityEngine;

public class PlayerHubNewsPopup : HUDElement
{
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel contentLabel;

	[SerializeField]
	private GameObject timerContainer;

	[SerializeField]
	private UILabel timerLabel;

	[SerializeField]
	private UILabel alreadyParticipatedLabel;

	[SerializeField]
	private UITexture imageTexture;

	[SerializeField]
	private int thumbnailTextureMaxHeight = 150;

	[SerializeField]
	private UIButton[] buttons;

	[SerializeField]
	private UILabel[] buttonTexts;

	[SerializeField]
	private UIScrollView contentScrollView;

	private long timeUntilNotValid;

	private string[] possibleAnswers;

	public PlayerHubNewsItem Item { get; set; }

	public override void Open()
	{
		base.Open();
		InitPossibleAnswers();
		UpdateUI();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_open_info");
		float y = contentScrollView.panel.clipSoftness.y;
		contentScrollView.MoveRelative(new Vector3(0f, 0f - y, 0f));
	}

	private void InitPossibleAnswers()
	{
		possibleAnswers = null;
		if (Item.NavigationLink == "POLL" || Item.NavigationLink == "QUIZ")
		{
			possibleAnswers = Item.PromoAttributes.Split(';');
		}
	}

	public bool ShowCounter()
	{
		if ((Item.NavigationLink == "POLL" || Item.NavigationLink == "QUIZ") && GameManager.Instance.playerModel.NewsLetterItemsInteracted.Contains(Item.EntryId))
		{
			return false;
		}
		if (Item.NavigationLink == "MORE_INFO" && !string.IsNullOrEmpty(Item.PromoAttributes))
		{
			return false;
		}
		return Item.ShowCounter;
	}

	public override void UpdateUI()
	{
		if (titleLabel != null)
		{
			titleLabel.text = Item.Title;
		}
		contentLabel.text = Item.Content;
		if (timerContainer != null)
		{
			timerContainer.SetActive(ShowCounter());
		}
		timeUntilNotValid = Math.Max((long)(Item.EndUnixTime.FromUnixTimeSeconds() - DateTime.UtcNow).TotalMilliseconds, 0L);
		LoadImageFromUrl component = GetComponent<LoadImageFromUrl>();
		if (Item.ImageUrl != null && component != null)
		{
			component.LoadImage(Item.ImageUrl, imageTexture, thumbnailTextureMaxHeight);
		}
		else
		{
			imageTexture.gameObject.SetActive(value: false);
		}
		InitButtons();
	}

	private void InitButtons()
	{
		int num = 1;
		if (possibleAnswers != null)
		{
			num = possibleAnswers.Length;
		}
		alreadyParticipatedLabel.gameObject.SetActive(value: false);
		if ((Item.NavigationLink == "POLL" || Item.NavigationLink == "QUIZ") && GameManager.Instance.playerModel.NewsLetterItemsInteracted.Contains(Item.EntryId))
		{
			num = 0;
			possibleAnswers = null;
			alreadyParticipatedLabel.gameObject.SetActive(value: true);
		}
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].gameObject.SetActive(i < num);
		}
		switch (num)
		{
		case 0:
			buttonTexts[2].text = LocalizationManager.GetText("Button.Ok");
			buttons[2].gameObject.SetActive(value: true);
			break;
		case 1:
			buttonTexts[0].text = LocalizationManager.GetText("Button.Ok");
			if (Item.NavigationLink == "MORE_INFO" && !string.IsNullOrEmpty(Item.PromoAttributes))
			{
				buttons[1].gameObject.SetActive(value: true);
				buttonTexts[1].text = LocalizationManager.GetText("Popup.Hub.ReadMore");
			}
			break;
		default:
		{
			for (int j = 0; j < num; j++)
			{
				buttonTexts[j].text = possibleAnswers[j];
			}
			break;
		}
		}
	}

	public override void Update()
	{
		base.Update();
		if (ShowCounter())
		{
			timeUntilNotValid -= (long)(Time.deltaTime * 1000f);
			if (timeUntilNotValid < 0)
			{
				timeUntilNotValid = 0L;
			}
			if (timerLabel != null)
			{
				timerLabel.text = Helpers.FormatTimeNoZero(timeUntilNotValid);
			}
		}
	}

	public void OnButtonPressedOk()
	{
		OnButtonPressed(0);
	}

	public void OnButtonPressedB()
	{
		OnButtonPressed(1);
	}

	public void OnButtonPressedC()
	{
		OnButtonPressed(2);
	}

	private void OnButtonPressed(int buttonIndex)
	{
		if (Item.NavigationLink == "MORE_INFO" && !string.IsNullOrEmpty(Item.PromoAttributes) && buttonIndex == 1)
		{
			Application.OpenURL(Item.PromoAttributes);
		}
		else if (Item.NavigationLink == "POLL")
		{
			if (possibleAnswers != null && possibleAnswers.Length > 1 && buttonIndex < possibleAnswers.Length)
			{
				Helpers.ExecuteCommand(new PlayerHubCommand
				{
					EventName = "Poll_" + possibleAnswers[buttonIndex],
					ItemId = Item.EntryId
				});
				if (Helpers.ExecuteCommand(new InteractNewsletterItemCommand
				{
					ItemId = Item.EntryId,
					DeepLinkType = Item.NavigationLink
				}) == TWDModelResult.OK)
				{
					AlertPopup.ShowPopupGetText("Popup.Quizz.Thanks.Title", "Popup.Quizz.Thanks.Message", "Button.Ok", null);
				}
			}
		}
		else if (Item.NavigationLink == "QUIZ" && possibleAnswers != null && possibleAnswers.Length > 1 && buttonIndex < possibleAnswers.Length)
		{
			TWDModelResult tWDModelResult = Helpers.ExecuteCommand(new InteractNewsletterItemCommand
			{
				ItemId = Item.EntryId,
				DeepLinkType = Item.NavigationLink,
				ButtonPressedIndex = buttonIndex
			});
			string eventName = "QuizError";
			switch (tWDModelResult)
			{
			case TWDModelResult.OK:
			{
				eventName = "QuizCorrect";
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.PlayerHubPopup);
				OpenLootInUi openLootInUi = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi) as OpenLootInUi;
				if (openLootInUi != null)
				{
					openLootInUi.OpenForModel(GameManager.Instance.playerModel);
				}
				break;
			}
			case TWDModelResult.Wrong:
				eventName = "QuizWrong";
				AlertPopup.ShowPopupGetText(LocalizationManager.GetText("Quiz.Wrong.Title"), LocalizationManager.GetText("Quiz.Wrong.Text"), LocalizationManager.GetText("Button.Ok"), null);
				break;
			}
			Helpers.ExecuteCommand(new PlayerHubCommand
			{
				EventName = eventName,
				ItemId = Item.EntryId
			});
		}
		OnClickClose();
	}
}
