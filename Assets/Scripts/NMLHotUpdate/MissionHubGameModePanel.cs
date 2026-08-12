using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class MissionHubGameModePanel : MissionHubPanelBase
{
	[Header("Title")]
	[SerializeField]
	public UILabel titleLabel;

	[SerializeField]
	public UILabel titleSubLabel;

	[Header("Location Texture")]
	public UITexture locationTexture;

	[Header("Progress Bar")]
	[SerializeField]
	public UIProgressBarExtended progressBar;

	[Header("Timer")]
	[SerializeField]
	public GameObject timerGameobject;

	[SerializeField]
	public UILabel timerLabel;

	[Header("Buttons")]
	[SerializeField]
	public UIButtonWithLabelAndIcon buttonMain;

	[SerializeField]
	public UIButtonWithLabelAndIcon buttonInfo;

	[Header("Preview List Of Rewards")]
	[SerializeField]
	public UIRewardsList rewardsPreview;

	[Header("Preview Single Reward")]
	[SerializeField]
	public RewardIcon rewardsPreviewIcon;

	[Header("Locked")]
	[SerializeField]
	public GameObject lockedGameObject;

	[SerializeField]
	public UILabel lockedLabel;

	[SerializeField]
	public UISprite lockedSprite;

	[Header("Unlocked aka NEW")]
	[SerializeField]
	public GameObject unlockedEffect;

	[SerializeField]
	public string UsesFeature;

	protected static MapVisualData mapVisualData;

	protected string timeLabelLocalisation = "";

	private const long delayOnCompleteMillisec = -1000L;

	protected long gameModeTimeLeft = -1000L;

	private bool pauseUpdateInQueue;

	public override void Awake()
	{
		base.Awake();
		DebugIdString = "MissionHubGameModePanel";
	}

	public override void Start()
	{
		base.Start();
		UpdatePanelLocked();
		AddListeners();
		HelpersUI.SetContentToLabel(timerLabel, "");
		pauseUpdateInQueue = false;
	}

	private void OnEnable()
	{
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
	}

	private void OnDisable()
	{
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		SetTitleLocalisation();
	}

	public override void Update()
	{
		base.Update();
		if (gameModeTimeLeft > -1000)
		{
			gameModeTimeLeft -= (long)(Time.deltaTime * 1000f);
			if (timerLabel != null)
			{
				SetContentToTimerLabel(timeLabelLocalisation + FormatTimeLeft(gameModeTimeLeft));
			}
			if (gameModeTimeLeft <= -1000)
			{
				gameModeTimeLeft = -1001L;
				UpdateUI();
			}
		}
		Helpers.GameObjectSetActive(timerGameobject, gameModeTimeLeft > -1);
	}

	public virtual void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus && !pauseUpdateInQueue)
		{
			pauseUpdateInQueue = true;
			StartCoroutine(ResumeFromPause(0.5f));
		}
	}

	private IEnumerator ResumeFromPause(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (base.gameObject != null && base.gameObject.activeInHierarchy)
		{
			DebugLog("Delayed ResumeFromPause UpdateUI");
			UpdateUI();
		}
		pauseUpdateInQueue = false;
	}

	public virtual void SetContentToTimerLabel(string value)
	{
		HelpersUI.SetContentToLabel(timerLabel, value);
	}

	public override void Clear()
	{
		base.Clear();
		mapVisualData = null;
		RemoveListeners();
	}

	public virtual void SetTitleLocalisation()
	{
		if (missionHubContent != null)
		{
			HelpersUI.SetContentToLabel(titleLabel, LocalizationManager.GetText(missionHubContent.TitleLocalizationKey));
		}
	}

	public virtual void CheckLockedState()
	{
	}

	public void UpdateLockedState(bool locked)
	{
		base.isLocked = locked;
		UpdateLockedStateIfDisabled();
		UpdatePanelLocked();
	}

	protected virtual void UpdateLockedStateIfDisabled()
	{
		Feature usedFeature = GetUsedFeature();
		base.isLocked |= !usedFeature.Enabled;
	}

	protected virtual Feature GetUsedFeature()
	{
		return GameManager.Instance.gameEconomyData.GetFeature(UsesFeature);
	}

	protected void UpdatePanelLocked()
	{
		Helpers.GameObjectSetActive(lockedGameObject, base.isLocked);
		Helpers.GameObjectSetActive(titleSubLabel, !base.isLocked);
	}

	protected virtual void SetLocationTexture(MapMissionGroupModel mapMissionGroupModel)
	{
		if (!(locationTexture != null))
		{
			return;
		}
		if (mapVisualData == null)
		{
			mapVisualData = UnityUtils.LoadFromAssetBundle<MapVisualData>("MapVisualData", "scriptableobjects");
		}
		if (mapMissionGroupModel != null && mapVisualData != null)
		{
			GameObject detailMapItemPrefab = mapVisualData.GetDetailMapItemPrefab(mapMissionGroupModel.MissionSpawnPointGroup.MapId);
			if (detailMapItemPrefab != null)
			{
				Renderer componentInChildren = detailMapItemPrefab.GetComponentInChildren<Renderer>();
				if (componentInChildren != null)
				{
					locationTexture.material = componentInChildren.sharedMaterial;
					Helpers.GameObjectSetActive(locationTexture, value: true);
					return;
				}
			}
		}
		Helpers.GameObjectSetActive(locationTexture, value: false);
	}

	protected virtual void PreviewRewardsList(List<IReward> rewards)
	{
		if (rewardsPreview != null)
		{
			rewardsPreview.SetPreviewRewards(rewards);
		}
	}

	protected virtual void PreviewRewardsList(List<DropEventDefinition.DropEventTag> rewards)
	{
		if (rewardsPreview != null)
		{
			rewardsPreview.SetPreviewRewards(rewards);
		}
	}

	protected virtual void PreviewSingleReward(IReward reward)
	{
		if (rewardsPreviewIcon != null)
		{
			rewardsPreviewIcon.SetReward(reward);
		}
	}

	protected virtual void OpenDialog()
	{
	}

	protected virtual void ButtonMainClicked(UIButtonExtended button)
	{
		Feature usedFeature = GetUsedFeature();
		if (!usedFeature.Enabled && usedFeature.ShowPopup)
		{
			OptionalUpdatePopup.OpenFeatureLockedContent();
		}
		else
		{
			OpenDialog();
		}
	}

	protected virtual bool UsesDisabledFeature(out bool showPopup)
	{
		Feature usedFeature = GetUsedFeature();
		showPopup = false;
		if (!usedFeature.Enabled)
		{
			showPopup = usedFeature.ShowPopup;
		}
		return !usedFeature.Enabled;
	}

	protected virtual void ButtonInfoClicked(UIButtonExtended button)
	{
		DebugLog("ButtonInfoClicked: Show Tooltip Tip?");
	}

	protected virtual void AddListeners()
	{
		if (buttonMain != null)
		{
			buttonMain.SetClickCallback(ButtonMainClicked);
		}
		if (buttonInfo != null)
		{
			buttonInfo.SetClickCallback(ButtonInfoClicked);
		}
	}

	protected virtual void RemoveListeners()
	{
		if (buttonMain != null)
		{
			buttonMain.Clear();
		}
		if (buttonInfo != null)
		{
			buttonInfo.Clear();
		}
	}

	protected static string FormatTimeLeft(long timeLeft)
	{
		if (timeLeft <= 0)
		{
			return "0";
		}
		return Helpers.FormatTimeNoZero(timeLeft);
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		UpdateUI();
	}
}
