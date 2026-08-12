using UnityEngine;

public class NewBieQuestsButton : MonoBehaviour
{
	[SerializeField]
	private GameObject button;

	[SerializeField]
	private UILabel timerLabel;

	[SerializeField]
	private UISprite buttonSprite;

	private float timeNum;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnEvent;
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null)
		{
			if (GameManager.Instance.playerModel.NewbieSenvenQuest.IsOpen && GameManager.Instance.playerModel.gameEconomyData.ConfigData.NewbieSevenQuestSwich)
			{
				return;
			}
			button.SetActive(value: false);
		}
		UIEvent.Send("CampBottomLeftFreshEvent");
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnEvent;
	}

	private void OnEvent(string type, object parameter)
	{
	}

	private void Update()
	{
		bool activeSelf = button.activeSelf;
		timeNum += Time.deltaTime;
		timerLabel.text = timeNum.ToString();
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null)
		{
			if (!GameManager.Instance.playerModel.NewbieSenvenQuest.IsOpen || !GameManager.Instance.playerModel.gameEconomyData.ConfigData.NewbieSevenQuestSwich)
			{
				button.SetActive(value: false);
				SentEvent(activeSelf);
				return;
			}
			long num = GameManager.Instance.playerModel.NewbieSenvenQuest.StartTime + GameManager.Instance.playerModel.gameEconomyData.ConfigData.NewbieSevenQuestDuration - GameManager.Instance.playerModel.UtcTimeStamp;
			if (num > 0)
			{
				timerLabel.text = Helpers.FormatTimeNoZero(num);
				button.SetActive(value: true);
			}
			else
			{
				button.SetActive(value: false);
			}
		}
		if (TutorialView.Instance.RunningButNotSuggesting)
		{
			button.SetActive(value: false);
		}
		SentEvent(activeSelf);
	}

	public void OnClick()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		NewBieQuestsPopup.OpenQuestsPopup();
	}

	private void SentEvent(bool isActive)
	{
		if (isActive != button.activeSelf)
		{
			UIEvent.Send("CampBottomLeftFreshEvent");
		}
	}
}
