using System.Collections.Generic;
using TWD.Externals;
using TWDModel;
using UnityEngine;

public class WeeklyChallengeActivityPopup : MonoBehaviour
{
	[SerializeField]
	private UITexture bgTexture;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UISprite tokenIcon;

	[SerializeField]
	private UILabel tokenLabel;

	[SerializeField]
	private UIGridExtended grid;

	[SerializeField]
	private WeeklyChallengeActivityReward weeklyChallengeReward;

	[SerializeField]
	private UIButton jumpButton;

	[SerializeField]
	private UILabel jumpLabel;

	[SerializeField]
	private UILabel timeLabel;

	private WeeklyChallengeClassTeamActivityModel _weeklyChallengeClassTeamActivity;

	private List<WeeklyChallengeActivityReward> _rewards = new List<WeeklyChallengeActivityReward>();

	private long _gameModeTimeLeft;

	private Vector3 _jumpPos = new Vector3(0f, 31f, 0f);

	public void Awake()
	{
		_weeklyChallengeClassTeamActivity = GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity;
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "WeeklyChallengeActivityRewardEvent")
		{
			SetTokenLabel();
		}
	}

	private void Update()
	{
		if (_gameModeTimeLeft >= 0)
		{
			_gameModeTimeLeft -= (long)(Time.deltaTime * 1000f);
			if (_gameModeTimeLeft <= 0)
			{
				_gameModeTimeLeft = 0L;
			}
		}
		if (timeLabel != null)
		{
			string text = LocalizationManager.GetText("UI_Roulette_Countdown", FormatTimeLeft(_gameModeTimeLeft));
			HelpersUI.SetContentToLabel(timeLabel, text);
		}
	}

	public void Open()
	{
		if (_weeklyChallengeClassTeamActivity == null)
		{
			_weeklyChallengeClassTeamActivity = GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity;
		}
		_gameModeTimeLeft = _weeklyChallengeClassTeamActivity.CurrentDefinition.EndTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp;
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		UpdateUI();
	}

	public void Close()
	{
		ClearRewards();
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	private void UpdateUI()
	{
		if (_weeklyChallengeClassTeamActivity.Id == -1)
		{
			Close();
		}
		List<ClassTeamExchangeDefinition> exchangeDefinitions = _weeklyChallengeClassTeamActivity.Shop.GetExchangeDefinitions();
		List<SurvivorClass> classes = _weeklyChallengeClassTeamActivity.CurrentDefinition.GetClasses();
		Object obj = UnityUtils.LoadFromAssetBundle(_weeklyChallengeClassTeamActivity.CurrentDefinition.Pic_Bg, "itemgraphics");
		if (obj != null)
		{
			bgTexture.mainTexture = (Texture)obj;
		}
		tokenIcon.spriteName = HelpersGfx.GetCurrencyIconName(_weeklyChallengeClassTeamActivity.CurrentDefinition.StarCurrencyType);
		string survivorClassName = HelpersLocalization.GetSurvivorClassName(classes[0]);
		HelpersUI.SetContentToLabel(titleLabel, LocalizationManager.GetText("WeeklyChallengeClassTeamChallenge.Title", survivorClassName));
		foreach (ClassTeamExchangeDefinition item in exchangeDefinitions)
		{
			WeeklyChallengeActivityReward component = grid.gameObject.AddChild(weeklyChallengeReward.gameObject).GetComponent<WeeklyChallengeActivityReward>();
			component.UpdateUI(item);
			_rewards.Add(component);
		}
		if (WeeklyChallengeHelper.GetWeeklyChallengeModel() != null)
		{
			Helpers.GameObjectSetActive(jumpButton, WeeklyChallengeHelper.IsChallengeOngoing());
			jumpLabel.transform.localPosition = (WeeklyChallengeHelper.IsChallengeOngoing() ? _jumpPos : Vector3.zero);
		}
		else
		{
			Helpers.GameObjectSetActive(jumpButton, value: true);
			jumpLabel.transform.localPosition = _jumpPos;
		}
		string content = (jumpButton.gameObject.activeSelf ? LocalizationManager.GetText("WeeklyChallengeClassTeamChallenge.Desc", survivorClassName, survivorClassName) : LocalizationManager.GetText("WeeklyChallengeClassTeamChallenge.Tips.Desc2", survivorClassName));
		HelpersUI.SetContentToLabel(jumpLabel, content);
		SetTokenLabel();
		grid.enabled = true;
	}

	private void ClearRewards()
	{
		if (_rewards.Count > 0)
		{
			for (int num = _rewards.Count - 1; num >= 0; num--)
			{
				NGUITools.Destroy(_rewards[num].gameObject);
			}
			_rewards.Clear();
		}
	}

	private string FormatTimeLeft(long timeLeft)
	{
		if (timeLeft <= 0)
		{
			return "0";
		}
		return Helpers.FormatTimeNoZero(timeLeft);
	}

	public void OnJumpButtonClicked()
	{
		ActivityPopup activityPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ActivityPopup) as ActivityPopup;
		if (activityPopup != null)
		{
			activityPopup.OnClickClose();
		}
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		if (WeeklyChallengeHelper.IsLockedByCouncilLevelOrTutorial())
		{
			DeepLinkNavigation.HandleDeepLink("MISSION_HUB");
		}
		else if (!weeklyChallengeModel.OpenedApocalypseWeeklyChallenge)
		{
			DeepLinkNavigation.HandleDeepLink("MISSION_HUB");
			if (WeeklyChallengeHelper.IsLockedByCouncilLevelOrTutorial())
			{
				FeatureLockedPopup.Open(FeatureLockedPopup.FeatureType.Challenge, locked: true);
				return;
			}
			WeeklyChallengeDifficultyPopup weeklyChallengeDifficultyPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WeeklyChallengeDifficulty) as WeeklyChallengeDifficultyPopup;
			if (weeklyChallengeDifficultyPopup != null)
			{
				weeklyChallengeDifficultyPopup.Open();
			}
		}
		else if (WeeklyChallengeHelper.IsApocalypticUnlocked)
		{
			MissionHubNavigation.TryOpenApocalypticChallengeMap();
		}
	}

	public void TipsButtonClicked()
	{
		WeeklyChallengeActivityInfo weeklyChallengeActivityInfo = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WeeklyChallengeActivityInfo, HUDManager.Instance.UIContainerTopCameras) as WeeklyChallengeActivityInfo;
		if (weeklyChallengeActivityInfo != null)
		{
			weeklyChallengeActivityInfo.Open();
		}
	}

	private void SetTokenLabel()
	{
		string currencyName = HelpersLocalization.GetCurrencyName(_weeklyChallengeClassTeamActivity.CurrentDefinition.StarCurrencyType);
		int currencyAmount = GameManager.Instance.playerModel.GetCurrencyAmount(_weeklyChallengeClassTeamActivity.CurrentDefinition.StarCurrencyType);
		HelpersUI.SetContentToLabel(tokenLabel, currencyName + " : " + currencyAmount);
	}
}
