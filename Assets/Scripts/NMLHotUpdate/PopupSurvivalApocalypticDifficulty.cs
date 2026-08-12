using System;
using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class PopupSurvivalApocalypticDifficulty : HUDElement
{
	[SerializeField]
	private List<ApocalypticDifficultyContainer> apocalypticDifficultyContainers;

	[SerializeField]
	private UILabel refreshLabel;

	[SerializeField]
	private UISprite refreshButton;

	private ApocalypseWeeklyChallengeModel _challengeModel;

	[SerializeField]
	[Tooltip("Label show when there is some info")]
	private UILabel infoLabel;

	[SerializeField]
	private UISprite infoSprite;

	[SerializeField]
	[Tooltip("Time to show in seconds")]
	private float timeToShow = 2f;

	[SerializeField]
	private Color errorColor;

	[SerializeField]
	private Color normalColor;

	public static void TryOpenOnChallengeEnter()
	{
		if (!SingularityMonoBehaviour<HUDManager>.Instance.IsActive(UIType.OpenLootInUi) && !SingularityMonoBehaviour<HUDManager>.Instance.IsActive(UIType.OpenApocalypticLootInUi))
		{
			PopupSurvivalApocalypticDifficulty popupSurvivalApocalypticDifficulty = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.PopupSurvivalApocalypticDifficulty) as PopupSurvivalApocalypticDifficulty;
			if (popupSurvivalApocalypticDifficulty != null)
			{
				popupSurvivalApocalypticDifficulty.Open();
			}
		}
	}

	public override void Open()
	{
		base.Open();
		InternalHide();
		UpdateUI();
	}

	private new void UpdateUI()
	{
		_challengeModel = GameManager.Instance.playerModel.ApocalypseWeeklyChallenge;
		if (_challengeModel == null)
		{
			Debug.LogError("WeeklyChallenge is null, turn it off");
			OnClickClose();
			return;
		}
		List<WeeklyChallengeApocalypseBuff> list = null;
		if (_challengeModel.SkipPendingSelectApocalypseBuffs?.Count >= apocalypticDifficultyContainers.Count)
		{
			list = _challengeModel.SkipPendingSelectApocalypseBuffs.Skip(Math.Max(0, _challengeModel.SkipPendingSelectApocalypseBuffs.Count - 3)).Take(apocalypticDifficultyContainers.Count).ToList();
		}
		if (list == null || list.Count <= 0)
		{
			list = _challengeModel.PendingSelectApocalypseBuffs;
		}
		if (list == null)
		{
			Debug.LogError("PendingSelectApocalypseBuffs is null, turn it off");
			OnClickClose();
			return;
		}
		if (list.Count != apocalypticDifficultyContainers.Count)
		{
			Debug.LogError("The gain buff is not 3, turn it off");
			OnClickClose();
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			apocalypticDifficultyContainers[i].Init(list[i], i);
		}
		if (refreshLabel != null)
		{
			refreshLabel.text = _challengeModel.RerollRemainingCount.ToString() ?? "";
		}
		if (refreshButton != null)
		{
			refreshButton.color = ((_challengeModel.RerollRemainingCount > 0) ? Color.white : Color.gray);
		}
	}

	public override void Start()
	{
		base.Start();
		EventManager.OnClick += OnEventHubClick;
	}

	private void OnDestroy()
	{
		EventManager.OnClick -= OnEventHubClick;
	}

	private void OnEventHubClick(string clickType)
	{
		if (clickType.Equals("Hub"))
		{
			Close();
		}
		if (clickType.Equals("SelectApocalyptic"))
		{
			Close();
			if (GameManager.Instance.playerModel?.ApocalypseWeeklyChallenge?.SkipPendingSelectApocalypseBuffs?.Count >= apocalypticDifficultyContainers.Count)
			{
				TryOpenOnChallengeEnter();
			}
		}
	}

	public void OnClickRefreshButton()
	{
		if (_challengeModel == null)
		{
			HUDNotification.Error(LocalizationManager.GetText("Error.ErrorGeneric"));
		}
		else if (_challengeModel.RerollRemainingCount <= 0)
		{
			HUDNotification.Error(LocalizationManager.GetText("Popup.RefreshApocalypse.NotEnough"));
		}
		else if (Helpers.ExecuteCommand(new RerollApocalypseBuffsCommand()) == TWDModelResult.OK)
		{
			UpdateUI();
		}
	}

	private void ShowInfo(string text, bool isError = false)
	{
		InternalHide();
		SetInfoText(infoLabel, text);
		infoSprite.color = (isError ? errorColor : normalColor);
	}

	private void SetInfoText(UILabel label, string text)
	{
		if (label != null && label.gameObject != null)
		{
			label.gameObject.SetActive(value: true);
			label.text = text;
			CancelInvoke("InternalHide");
			Invoke("InternalHide", timeToShow);
		}
		else
		{
			Debug.LogError("HUDNotification: Could not show notification because label is NULL!");
		}
	}

	private void InternalHide()
	{
		if (infoLabel != null && infoLabel.gameObject != null)
		{
			infoLabel.gameObject.SetActive(value: false);
		}
	}
}
