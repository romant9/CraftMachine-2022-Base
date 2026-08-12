using System;
using System.Collections.Generic;
using System.Linq;
using TWDModel;
using TWDModel.ContentTypes;
using UnityEngine;

public class EndlessModeGameDifficultySelectionPopup : HUDElement
{
	[SerializeField]
	private UIButton closeButtton;

	[SerializeField]
	private UIButton infoButton;

	[SerializeField]
	private UIButton normalGameDifficultyButton;

	[SerializeField]
	private UIButton expertGameDifficultyButton;

	[SerializeField]
	private UILabel normalModeScoreProgressLabel;

	[SerializeField]
	private UILabel normalModeRewardProgressLabel;

	[SerializeField]
	private UILabel normalModeCurrencyNumLabel;

	[SerializeField]
	private UILabel expertModeCurrencyNumLabel;

	[SerializeField]
	private UILabel expertModeCouncilLevelLockLabel;

	[SerializeField]
	private GameObject actorEntryContainer;

	[SerializeField]
	private GameObject actorTokenPrefab;

	[SerializeField]
	private GameObject lockedContainer;

	private readonly List<GameObject> actorEntries = new List<GameObject>();

	public override void Open()
	{
		base.Open();
		SetupOnClickCloseButton();
		SetupOnClickInfoButton();
		SetupOnClickNormalDifficultyButton();
		SetupButtonColor();
		HelpersUI.SetContentToLabel(normalModeCurrencyNumLabel, GameManager.Instance.playerModel.GetCurrency(CurrencyType.EndlessPassToken).Value.ToString());
		HelpersUI.SetContentToLabel(expertModeCurrencyNumLabel, GameManager.Instance.playerModel.GetCurrency(CurrencyType.EndlessPassExpertToken).Value.ToString());
		HelpersUI.SetContentToLabel(normalModeScoreProgressLabel, LocalizationManager.GetText("SurvivalMode_Selection_Normal_ScoreProgress_Title") + Math.Min(EndlessModeHelpers.GetAttemptsScoreNormal(), EndlessModeHelpers.GetMaxEndlessNormalModeScore()) + "/" + EndlessModeHelpers.GetMaxEndlessNormalModeScore());
		HelpersUI.SetContentToLabel(normalModeRewardProgressLabel, LocalizationManager.GetText("SurvivalMode_Selection_Normal_RewardProgress_Title") + EndlessModeHelpers.GetClaimedNormalProgressRewardIndex?.Count + "/" + GameManager.Instance.gameEconomyData.EndlessModeNormalRewardDefinitons.Length);
		if (EndlessModeHelpers.IsExpertMdeLockedByCouncilLevel || !EndlessModeHelpers.HasGeneratedExpertModeActors)
		{
			DisableExpertModeContainer();
			return;
		}
		SetupOnClickExpertDifficultyButton();
		SetupExpertModeActorTokens();
	}

	private void SetupExpertModeActorTokens()
	{
		List<ActorDefinition> getExpertModeActorDefinitions = EndlessModeHelpers.GetExpertModeActorDefinitions;
		if (getExpertModeActorDefinitions.Count == 0)
		{
			return;
		}
		getExpertModeActorDefinitions = getExpertModeActorDefinitions.OrderByDescending((ActorDefinition x) => GameManager.Instance.playerModel.SurvivorContainer.HasHero(x.ID)).ToList();
		ClearActorTokenEntries();
		UITable component = actorEntryContainer.GetComponent<UITable>();
		UIScrollView componentInParent = actorEntryContainer.GetComponentInParent<UIScrollView>();
		for (int num = 0; num < getExpertModeActorDefinitions.Count; num++)
		{
			ActorDefinition actorDefinition = getExpertModeActorDefinitions[num];
			if (actorDefinition != null)
			{
				GameObject gameObject = actorEntryContainer.AddChild(actorTokenPrefab);
				NGUITools.SetActive(gameObject, state: true);
				if (gameObject.TryGetComponent<UISprite>(out var component2))
				{
					component2.spriteName = HelpersGfx.GetTraitUpgradeCurrencyByActorDefinition(actorDefinition);
				}
				if (gameObject.transform.GetChild(0) != null)
				{
					GameObject obj = gameObject.transform.GetChild(0).gameObject;
					bool flag = EndlessModeHelpers.HasExpertModeHero(actorDefinition);
					obj.gameObject.SetActive(!flag);
				}
				actorEntries.Add(gameObject);
			}
		}
		component.Reposition();
		componentInParent.ResetPosition();
	}

	private void ClearActorTokenEntries()
	{
		for (int i = 0; i < actorEntries.Count; i++)
		{
			NGUITools.Destroy(actorEntries[i]);
		}
		actorEntries.Clear();
	}

	private void SetupOnClickCloseButton()
	{
		EventDelegate.Set(closeButtton.onClick, OnClickCloseButton);
	}

	private void OnClickCloseButton()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_dialog_exit");
		Close();
	}

	private void SetupOnClickInfoButton()
	{
		EventDelegate.Set(infoButton.onClick, OnClickInfoButton);
	}

	private void SetupOnClickNormalDifficultyButton()
	{
		EventDelegate.Set(normalGameDifficultyButton.onClick, delegate
		{
			OnClickGameDifficultyButton(EndlessModeGameModeType.Normal);
		});
	}

	private void SetupOnClickExpertDifficultyButton()
	{
		EventDelegate.Set(expertGameDifficultyButton.onClick, delegate
		{
			OnClickGameDifficultyButton(EndlessModeGameModeType.Expert);
		});
	}

	private void OnClickInfoButton()
	{
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EndlessModeExpertModeDifficultyInfoPopup);
		if (hUDElement != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			hUDElement.Open();
		}
	}

	private void OnClickGameDifficultyButton(EndlessModeGameModeType endlessModeGameModeType)
	{
		if (endlessModeGameModeType == EndlessModeGameModeType.Normal)
		{
			if (GameManager.Instance.gameEconomyData.ConfigData.EndlessNormalSwitch)
			{
				EndlessNormalMissionHubPopup endlessNormalMissionHubPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EndlessNormalMissionHubPopup) as EndlessNormalMissionHubPopup;
				if (endlessNormalMissionHubPopup != null)
				{
					SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
					endlessNormalMissionHubPopup.Open();
				}
			}
			else
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.HUDNotification).Open();
				HUDNotification.Info(LocalizationManager.GetText("Tips.EndlessMode.SwitchOff"));
			}
		}
		if (endlessModeGameModeType != EndlessModeGameModeType.Expert)
		{
			return;
		}
		if (GameManager.Instance.gameEconomyData.ConfigData.EndlessExpertSwitch)
		{
			EndlessMissionHubPopup endlessMissionHubPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EndlessMissionHubPopup) as EndlessMissionHubPopup;
			if (endlessMissionHubPopup != null)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
				endlessMissionHubPopup.Open();
			}
		}
		else
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.HUDNotification).Open();
			HUDNotification.Info(LocalizationManager.GetText("Tips.EndlessMode.SwitchOff"));
		}
	}

	private void SetupButtonColor()
	{
		if (GameManager.Instance.gameEconomyData.ConfigData.EndlessNormalSwitch)
		{
			normalGameDifficultyButton.normalSprite = "Ui_Regular_Button_Bg";
			normalGameDifficultyButton.hoverSprite = "Ui_Regular_Button_Bg";
			normalGameDifficultyButton.pressedSprite = "Ui_Regular_Button_Bg_Pressed";
		}
		else
		{
			normalGameDifficultyButton.normalSprite = "Ui_Regular_Gray_Button_Bg";
			normalGameDifficultyButton.hoverSprite = "Ui_Regular_Gray_Button_Bg";
			normalGameDifficultyButton.pressedSprite = "Ui_Regular_Gray_Button_Pressed_Bg";
		}
		if (GameManager.Instance.gameEconomyData.ConfigData.EndlessExpertSwitch)
		{
			expertGameDifficultyButton.normalSprite = "Ui_Regular_Button_Bg";
			expertGameDifficultyButton.hoverSprite = "Ui_Regular_Button_Bg";
			expertGameDifficultyButton.pressedSprite = "Ui_Regular_Button_Bg_Pressed";
		}
		else
		{
			expertGameDifficultyButton.normalSprite = "Ui_Regular_Gray_Button_Bg";
			expertGameDifficultyButton.hoverSprite = "Ui_Regular_Gray_Button_Bg";
			expertGameDifficultyButton.pressedSprite = "Ui_Regular_Gray_Button_Pressed_Bg";
		}
	}

	private void DisableExpertModeContainer()
	{
		NGUITools.SetActiveChildren(lockedContainer.transform.parent.gameObject, state: false);
		Helpers.GameObjectSetActive(lockedContainer, value: true);
		HelpersUI.SetContentToLabel(expertModeCouncilLevelLockLabel, LocalizationManager.GetText("Popup.MissionHub.OutpostUnlockAtLevel{CouncilLevel}", GameManager.Instance.gameEconomyData.EndlessModeConfig.ExpertModeCouncilLockLevel));
	}

	public void OnProfessionRuleClick()
	{
		PopupProfessionTip popupProfessionTip = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EndlessProfessionTipPopup) as PopupProfessionTip;
		if (popupProfessionTip != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			popupProfessionTip.Open();
			popupProfessionTip.SetTipContent(EndlessModeGameModeType.Expert);
		}
	}
}
