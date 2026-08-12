using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class DetailMapScrollItem : MonoBehaviour
{
	[SerializeField]
	private UILabel title;

	[SerializeField]
	private GameObject star1;

	[SerializeField]
	private GameObject star2;

	[SerializeField]
	private GameObject star3;

	[SerializeField]
	private Vector3 star1OriginLocation;

	[SerializeField]
	private Vector3 star1CenteredLocation;

	[SerializeField]
	private Vector3 star1Centered2Location;

	[SerializeField]
	private Vector3 star2OriginLocation;

	[SerializeField]
	private Vector3 star2CenteredLocation;

	[SerializeField]
	private Vector3 star3OriginLocation;

	[SerializeField]
	private GameObject star1Background;

	[SerializeField]
	private GameObject star2Background;

	[SerializeField]
	private GameObject star3Background;

	[SerializeField]
	private List<TweenScale> star1Tweens;

	[SerializeField]
	private List<TweenScale> star2Tweens;

	[SerializeField]
	private List<TweenScale> star3Tweens;

	[SerializeField]
	private GameObject lockedItem;

	[SerializeField]
	private GameObject comingSoon;

	[SerializeField]
	private GameObject starContainer;

	[SerializeField]
	private GameObject selectedContainer;

	[SerializeField]
	private Color selectedColor;

	[SerializeField]
	private Color normalColor;

	[SerializeField]
	private Color lockedColor;

	[SerializeField]
	private Vector3 selectedItemScale;

	[SerializeField]
	private GameObject seasonFeature;

	[SerializeField]
	private UILabel seasonUnlockTime;

	[SerializeField]
	private GameObject seasonUnlockTimeContainer;

	public UIWidget LeftSide;

	public UIWidget RightSide;

	[SerializeField]
	private float secondsBeforeStarTweens = 0.5f;

	[SerializeField]
	private float secondsBetweenStarTweens = 0.5f;

	[SerializeField]
	private UILabel trialResetLabel;

	[HideInInspector]
	public MapMissionGroupModel EpisodeModel;

	private MissionHighlight cachedNextHighlight;

	private string trialsLocalisationString = "";

	private long trialsResetTimeLeft;

	[HideInInspector]
	public bool IsSelected;

	private long timeToUnlock = -1L;

	private bool unlocked;

	public void SetItem(MapMissionGroupModel model, string title, MissionHighlight nextHighlight = null)
	{
		base.gameObject.name = "Item " + title;
		this.title.text = title;
		EpisodeModel = model;
		cachedNextHighlight = nextHighlight;
	}

	public void UpdateUI()
	{
		if (seasonUnlockTimeContainer != null)
		{
			seasonUnlockTimeContainer.SetActive(value: false);
		}
		comingSoon.SetActive(value: false);
		lockedItem.SetActive(value: false);
		star3.SetActive(value: false);
		star2.SetActive(value: false);
		star1.SetActive(value: false);
		star3Background.SetActive(value: false);
		star2Background.SetActive(value: false);
		star1Background.SetActive(value: true);
		if (EpisodeModel == null)
		{
			starContainer.SetActive(value: false);
			lockedItem.SetActive(value: true);
			title.color = lockedColor;
			return;
		}
		if (EpisodeModel.GetOriginalDifficultyMapMissionGroupModel().AreAllStoryMissionsCompleted())
		{
			star1.SetActive(value: true);
			star2Background.SetActive(value: true);
		}
		if (EpisodeModel.MissionSpawnPointGroup.EpisodeDifficultyLevel == 3)
		{
			star2.SetActive(value: true);
			star3Background.SetActive(value: true);
		}
		if (EpisodeModel.MissionSpawnPointGroup.EpisodeDifficultyLevel == 3 && EpisodeModel.AreAllStoryMissionsCompleted())
		{
			star3.SetActive(value: true);
		}
		if (EpisodeModel.GetOriginalDifficultyMapMissionGroupModel() == EpisodeModel)
		{
			star2.SetActive(value: false);
			star3.SetActive(value: false);
			star2Background.SetActive(value: false);
			star3Background.SetActive(value: false);
		}
		CenterStars();
		lockedItem.SetActive(EpisodeModel.IsLocked);
		starContainer.SetActive(!EpisodeModel.IsLocked && EpisodeModel.MissionSpawnPointGroup.Category != MapCategory.Season);
		selectedContainer.SetActive(IsSelected);
		if (IsSelected)
		{
			title.color = selectedColor;
			base.gameObject.transform.localScale = selectedItemScale;
		}
		else
		{
			base.gameObject.transform.localScale = Vector3.one;
			if (EpisodeModel.IsLocked)
			{
				title.color = lockedColor;
			}
			else
			{
				title.color = normalColor;
			}
		}
		if (seasonFeature != null)
		{
			seasonFeature.SetActive(EpisodeModel.IsFeaturedData != null);
		}
		if (seasonUnlockTime != null && EpisodeModel.MissionSpawnPointGroup.Category == MapCategory.Season)
		{
			long isNextToUnlockTime = EpisodeModel.IsNextToUnlockTime;
			if (seasonUnlockTimeContainer != null && isNextToUnlockTime != -1)
			{
				lockedItem.SetActive(value: false);
				seasonUnlockTimeContainer.SetActive(value: true);
				timeToUnlock = isNextToUnlockTime;
				unlocked = false;
			}
		}
	}

	private void CenterStars()
	{
		if (star3Background.activeSelf || star3.activeSelf)
		{
			star1.transform.localPosition = star1OriginLocation;
			star2.transform.localPosition = star2OriginLocation;
			star3.transform.localPosition = star3OriginLocation;
			star3Background.transform.localPosition = star3OriginLocation;
		}
		else if (star2Background.activeSelf || star2.activeSelf)
		{
			star1.transform.localPosition = star1Centered2Location;
			star2.transform.localPosition = star2CenteredLocation;
			star2Background.transform.localPosition = star2CenteredLocation;
			star1Background.transform.localPosition = star1Centered2Location;
		}
		else
		{
			star1.transform.localPosition = star1CenteredLocation;
			star1Background.transform.localPosition = star1CenteredLocation;
		}
	}

	public void Update()
	{
		if (!unlocked)
		{
			timeToUnlock -= (long)(Time.deltaTime * 1000f);
			seasonUnlockTime.text = Helpers.FormatTime(timeToUnlock);
			if (timeToUnlock < -1000)
			{
				timeToUnlock = 0L;
				unlocked = true;
				DetailMapPopUp detailMapPopUp = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DetailMapPopUp) as DetailMapPopUp;
				if (detailMapPopUp.CurrentMap != null && detailMapPopUp.CurrentMap.MissionSpawnPointGroup != null && detailMapPopUp.CurrentMap.MissionSpawnPointGroup.Category == MapCategory.Season)
				{
					detailMapPopUp.UpdateSelectedItem();
				}
			}
		}
		if (!(trialResetLabel != null) || !(trialResetLabel.gameObject != null) || !(GameManager.Instance != null))
		{
			return;
		}
		Helpers.GameObjectSetActive(trialResetLabel, cachedNextHighlight != null);
		if (cachedNextHighlight != null)
		{
			trialsResetTimeLeft = cachedNextHighlight.GetTimeUntilStart(GameManager.Instance.playerModel.UtcTimeStamp);
			trialsLocalisationString = LocalizationManager.GetText("SeasonSeven.EpisodeNumber.TrialTimer{Parameter}", Helpers.FormatTime(trialsResetTimeLeft));
			HelpersUI.SetContentToLabel(trialResetLabel, trialsLocalisationString);
			if (trialsResetTimeLeft <= 0)
			{
				cachedNextHighlight = null;
				Helpers.GameObjectSetActive(trialResetLabel, value: false);
			}
		}
	}

	public void OnClick()
	{
		if (EpisodeModel == null || EpisodeModel.IsLocked)
		{
			HUDNotification.Info(LocalizationManager.GetText((EpisodeModel.MissionSpawnPointGroup.Category == MapCategory.Season) ? "Notification.Episode.Locked" : "Notification.Chapter.Locked"));
		}
		else if (!IsSelected)
		{
			(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DetailMapPopUp) as DetailMapPopUp).LoadEpisode(EpisodeModel.MissionSpawnPointGroupId);
		}
	}

	public void SetSelected(bool selected)
	{
		IsSelected = selected;
		UpdateUI();
	}

	public void TriggerTween()
	{
		if (base.gameObject != null && base.gameObject.activeInHierarchy)
		{
			StartCoroutine(TweenStars());
		}
	}

	private IEnumerator TweenStars()
	{
		bool star1Visible = star1.activeSelf;
		bool star2Visible = star2.activeSelf;
		bool star3Visible = star3.activeSelf;
		if (star1Visible)
		{
			for (int i = 0; i < star1Tweens.Count; i++)
			{
				star1Tweens[i].enabled = true;
			}
			Helpers.GameObjectSetActive(star1, value: false);
		}
		if (star2Visible)
		{
			for (int j = 0; j < star2Tweens.Count; j++)
			{
				star2Tweens[j].enabled = true;
			}
			Helpers.GameObjectSetActive(star2, value: false);
		}
		if (star3Visible)
		{
			for (int k = 0; k < star3Tweens.Count; k++)
			{
				star3Tweens[k].enabled = true;
			}
			Helpers.GameObjectSetActive(star3, value: false);
		}
		yield return new WaitForSeconds(secondsBeforeStarTweens);
		if (star1Visible)
		{
			Helpers.GameObjectSetActive(star1, value: true);
		}
		if (star2Visible)
		{
			yield return new WaitForSeconds(secondsBetweenStarTweens);
			Helpers.GameObjectSetActive(star2, value: true);
		}
		if (star3Visible)
		{
			yield return new WaitForSeconds(secondsBetweenStarTweens);
			Helpers.GameObjectSetActive(star3, value: true);
		}
	}
}
