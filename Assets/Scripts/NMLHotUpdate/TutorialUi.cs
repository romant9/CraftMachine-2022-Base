using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TWDModel;
using UnityEngine;

public class TutorialUi : HUDElement
{
	public const string Portrait_Daryl = "Portrait_Daryl";

	public const string Portrait_StoryTeller = "Portrait_StoryTeller";

	public const string Portrait_Info = "Portrait_Info";

	[SerializeField]
	[Tooltip("The arrow indicating what action to be done.")]
	private TutorialArrow arrow;

	[SerializeField]
	[Tooltip("Hand showing click & drag actions.")]
	private TutorialHand hand;

	[SerializeField]
	private TutorialDialogCharacter character;

	[SerializeField]
	[Tooltip("The animator of the bubble.")]
	private Animator bubbleAnimator;

	[SerializeField]
	[Tooltip("The enter animation of the bubble.")]
	private AnimationClip bubbleAnimationEnter;

	[SerializeField]
	[Tooltip("The exit animation of the bubble.")]
	private AnimationClip bubbleAnimationExit;

	[SerializeField]
	[Tooltip("Background sprite.")]
	private UISprite bgSprite;

	[SerializeField]
	private GameObject highlightedObjectContainer;

	private bool dismissed;

	private Vector3 bgClickCenter = new Vector3(0f, 200f, 0f);

	private Vector3 bgClickSize = new Vector3(1200f, 700f, 0f);

	private ActorModel actorTalking;

	private bool wasTutorialHandShownWhenOpeningPopup;

	public Transform HighlightedObjectContainer => highlightedObjectContainer.transform;

	public bool IsActorTalking
	{
		get
		{
			if (character != null)
			{
				return character.IsShown();
			}
			return false;
		}
	}

	public override void Open()
	{
		base.Open();
		GetComponent<BoxCollider>().enabled = false;
		character.Init();
		HideHand();
		HideBubbleInstant();
		UIEvent.OnUIEvent += OnUiEvent;
		EventManager.OnEvent += OnEvent;
	}

	public override void Close()
	{
		base.Close();
		UIEvent.OnUIEvent -= OnUiEvent;
		EventManager.OnEvent -= OnEvent;
	}

	private void OnDestroy()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
		EventManager.OnEvent -= OnEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if ((!(type == "OnPopUpOpen") && !(type == "OnPopUpClose")) || (!(parameter is SettingsPopup) && !(parameter is LinkDevicePopup) && !(parameter is CombatFleeScreen)))
		{
			return;
		}
		if (type == "OnPopUpOpen")
		{
			wasTutorialHandShownWhenOpeningPopup = IsHandActive();
			if (wasTutorialHandShownWhenOpeningPopup)
			{
				ShowHandGameObject(show: false);
			}
		}
		else if (type == "OnPopUpClose")
		{
			ShowHandGameObject(wasTutorialHandShownWhenOpeningPopup);
		}
	}

	public void ShowArrow(GameObject objectToFollow, bool downwards = true)
	{
		if (arrow != null)
		{
			arrow.Show(objectToFollow, downwards);
		}
	}

	public void HideArrow()
	{
		if (arrow != null && arrow.gameObject != null)
		{
			arrow.gameObject.SetActive(value: false);
		}
	}

	public bool IsArrowActive()
	{
		if (arrow != null)
		{
			return arrow.gameObject.activeSelf;
		}
		return false;
	}

	public void ShowHand(GameObject clickTarget)
	{
		hand.ShowClick(clickTarget);
	}

	public void ShowHand(Vector3 clickTarget)
	{
		hand.ShowClick(clickTarget);
	}

	public void ShowHand(Vector3 startDrag, Vector3 endDrag)
	{
		hand.ShowDrag(startDrag, endDrag);
	}

	public void HideHand()
	{
		hand.SetActive(active: false);
	}

	public bool IsEnabled()
	{
		if (GetComponent<BoxCollider>() != null)
		{
			return GetComponent<BoxCollider>().enabled;
		}
		return false;
	}

	public void ShowHandGameObject(bool show)
	{
		hand.gameObject.SetActive(show);
	}

	public bool IsHandActive()
	{
		return hand.gameObject.activeSelf;
	}

	public Coroutine Say(string character, string textId, bool waitForClick = true, bool showDialogAtCenter = false, object argument = null)
	{
		if (GameManager.Instance.gameEconomyData.RookieConfigData != null && GameManager.Instance.gameEconomyData.RookieConfigData.DeleteDialogue01 != null && GameManager.Instance.gameEconomyData.RookieConfigData.DeleteDialogue01.Count > 0)
		{
			foreach (string item in GameManager.Instance.gameEconomyData.RookieConfigData.DeleteDialogue01)
			{
				if (textId == item)
				{
					return null;
				}
			}
			return StartCoroutine(ShowCharacter(character, textId, waitForClick, showDialogAtCenter, argument));
		}
		return StartCoroutine(ShowCharacter(character, textId, waitForClick, showDialogAtCenter, argument));
	}

	public Coroutine HideCharacter()
	{
		return StartCoroutine(HideAllCharacters());
	}

	public void OnDismissClicked()
	{
		dismissed = true;
	}

	private IEnumerator HideAllCharacters()
	{
		if (actorTalking != null)
		{
			if (CombatView.Instance != null)
			{
				ActorView actorViewFromModel = CombatView.Instance.GetActorViewFromModel(actorTalking);
				if (actorViewFromModel != null)
				{
					actorViewFromModel.SetSpeechBubble(enabled: false);
				}
			}
			actorTalking = null;
		}
		if (character != null)
		{
			character.Hide();
		}
		yield return new WaitForSeconds(0.25f);
	}

	public static string TryParsePortraitInLocalization(string localization, out string character)
	{
		Match match = new Regex("<<(.*)>>").Match(localization);
		if (match.Groups.Count > 1)
		{
			character = "Portrait_" + match.Groups[1].ToString();
			return localization.Replace(match.Groups[0].ToString(), "");
		}
		character = "";
		return localization;
	}

	private IEnumerator ShowCharacter(string character, string textId, bool waitForClick = true, bool showDialogAtCenter = false, object argument = null)
	{
		dismissed = false;
		string text = null;
		if (textId != null)
		{
			text = LocalizationManager.GetText(textId, argument);
			string text2 = "";
			text = TryParsePortraitInLocalization(text, out text2);
			if (text2 != "")
			{
				character = text2;
			}
		}
		GetComponent<BoxCollider>().enabled = waitForClick;
		bgSprite.gameObject.GetComponent<BoxCollider>().enabled = waitForClick;
		if (GameManager.Instance.gameEconomyData.RookieConfigData != null && GameManager.Instance.gameEconomyData.RookieConfigData.ScreenClickToDismissDialogue)
		{
			bgSprite.gameObject.GetComponent<BoxCollider>().center = bgClickCenter;
			bgSprite.gameObject.GetComponent<BoxCollider>().size = bgClickSize;
		}
		if (actorTalking != null)
		{
			ActorView actorView = CombatView.Instance?.GetActorViewFromModel(actorTalking);
			if (actorView != null)
			{
				actorView.SetSpeechBubble(enabled: false);
			}
			actorTalking = null;
		}
		if (CombatView.Instance != null)
		{
			switch (character)
			{
			case "CombatRandom":
				actorTalking = CombatView.Instance.Model.MissionRoster[UnityEngine.Random.Range(0, CombatView.Instance.Model.MissionRoster.Count)];
				break;
			case "Survivor_A":
				actorTalking = CombatView.Instance.Model.MissionRoster[0];
				break;
			case "Survivor_B":
				actorTalking = CombatView.Instance.Model.MissionRoster[1];
				break;
			case "Survivor_C":
				actorTalking = CombatView.Instance.Model.MissionRoster[2];
				break;
			default:
				if (character.Contains("Tag_"))
				{
					string value = character.Substring(character.IndexOf('_') + 1);
					List<ActorModel> actorsWithTag = CombatView.Instance.Model.GetActorsWithTag(Convert.ToInt32(value));
					if (actorsWithTag.Count > 0)
					{
						actorTalking = actorsWithTag[0];
					}
				}
				break;
			}
		}
		if (actorTalking == null)
		{
			this.character.SetCharacter(character);
		}
		else
		{
			ActorView actorView2 = CombatView.Instance?.GetActorViewFromModel(actorTalking);
			if (actorView2 != null)
			{
				if (!CombatHUD.IsSpeedUpEnabled)
				{
					PlayerInputManager.Instance.GetHandler<CameraInputHandler>().FrameActorToView(actorTalking);
				}
				actorView2.SetSpeechBubble(enabled: true);
			}
			this.character.SetCharacter(actorTalking);
		}
		if (showDialogAtCenter)
		{
			this.character.SetShowAtCenter();
		}
		this.character.Show();
		TweenManager.PlayTweenGroup(this.character.DismissButton, 0, forward: true, OnDismissTweenPlayed);
		if (text != null)
		{
			this.character.SetText(text);
			if (waitForClick)
			{
				yield return StartCoroutine(WaitClick());
				UIEvent.Send("OnTutorialDialogClicked");
			}
		}
		GetComponent<BoxCollider>().enabled = false;
	}

	private void OnDismissTweenPlayed()
	{
	}

	private IEnumerator WaitClick()
	{
		while (!CanClickDialogs() || !dismissed)
		{
			yield return null;
		}
	}

	private bool CanClickDialogs()
	{
		HUDElement popupOnTop = SingularityMonoBehaviour<HUDManager>.Instance.GetPopupOnTop();
		if (popupOnTop == null)
		{
			return true;
		}
		HUDElementConfig hudElementConfig = SingularityMonoBehaviour<HUDManager>.Instance.GetHudElementConfig(popupOnTop);
		if (hudElementConfig != null)
		{
			return !hudElementConfig.BlockTutorialDialogs;
		}
		return true;
	}

	public void ShowBubble(string textId)
	{
		bubbleAnimator.Play(bubbleAnimationEnter.name);
	}

	public void HideBubble()
	{
		bubbleAnimator.Play(bubbleAnimationExit.name);
	}

	public void HideBubbleInstant()
	{
		bubbleAnimator.Play(bubbleAnimationExit.name, -1, 1f);
	}

	public void ClearHighlightedObjects()
	{
		highlightedObjectContainer.RemoveAllChildren();
	}

	private void OnEvent(EventManager.EventType eventtype, object parameter)
	{
		if (eventtype != EventManager.EventType.TutorialPartOver)
		{
			return;
		}
		bool flag = GameManager.Instance.playerModel.Tutorial.HasCompletedPart("EndTutorial");
		bool flag2 = GameManager.Instance.playerModel.Tutorial.HasCompletedPart("HeroUnlock");
		bool flag3 = GameManager.Instance.playerModel.Tutorial.HasCompletedPart("ScavengeMode");
		bool flag4 = GameManager.Instance.playerModel.Tutorial.HasCompletedPart("SeasonsMode");
		if (!GameManager.Instance.playerModel.Tutorial.HasCompletedPart("HeroTrait"))
		{
			StoryTellerModel storyTeller = GameManager.Instance.playerModel.SurvivorContainer.StoryTeller;
			bool flag5 = storyTeller.GetCurrentUncompletedQuestDefinition() != null && storyTeller.GetCurrentUncompletedQuestDefinition().Order > 0;
			if (flag && !flag3 && !flag2)
			{
				TutorialView.Instance.StartPart("ScavengeMode");
			}
			if (flag2 && !flag4 && flag5)
			{
				TutorialView.Instance.StartPart("SeasonsMode");
			}
		}
	}
}
