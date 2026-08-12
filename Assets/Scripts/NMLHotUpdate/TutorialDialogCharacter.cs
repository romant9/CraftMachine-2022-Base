using System;
using TWDModel;
using UnityEngine;

[Serializable]
public class TutorialDialogCharacter : MonoBehaviour
{
	private Animator characterAnimator;

	[SerializeField]
	[Tooltip("The enter screen animation of the character")]
	private AnimationClip characterAnimationEnter;

	[SerializeField]
	[Tooltip("The enter screen animation of the character")]
	private AnimationClip characterAnimationEnterCenter;

	[SerializeField]
	[Tooltip("The exit screen animation of the character")]
	private AnimationClip characterAnimationExit;

	[SerializeField]
	[Tooltip("The container of the character sprite.")]
	private GameObject characterSpriteContainer;

	[SerializeField]
	[Tooltip("The info icon container.")]
	private GameObject infoIcon;

	[SerializeField]
	[Tooltip("The sprite of the character.")]
	private UISprite characterSprite;

	[SerializeField]
	[Tooltip("The texture for the portrait of the character.")]
	private UITexture characterPortrait;

	[SerializeField]
	[Tooltip("Dark transparent background")]
	private GameObject grayedOutBackground;

	[SerializeField]
	[Tooltip("Dismiss button with normal colors")]
	private GameObject dismissButtonNormal;

	[SerializeField]
	[Tooltip("Highlighted dismiss button")]
	private GameObject dismissButtonHighlighted;

	[SerializeField]
	[Tooltip("Label of what the character says.")]
	private UILabel label;

	[SerializeField]
	[Tooltip("Inactive position of this character. Used when he is not talking.")]
	private Vector3 inactivePosition;

	private bool shown;

	private ActorModel actor;

	private bool showAtCenter;

	public GameObject DismissButton { get; private set; }

	public void Init()
	{
		characterAnimator = GetComponent<Animator>();
		EnableImprovedVersion(GameManager.Instance.gameEconomyData.GetFeature("ImprovedTutorialDialog").Enabled);
		HideInstant();
	}

	private void EnableImprovedVersion(bool ShowImprovedVersion)
	{
		Helpers.GameObjectSetActive(grayedOutBackground, ShowImprovedVersion);
		Helpers.GameObjectSetActive(dismissButtonNormal, !ShowImprovedVersion);
		Helpers.GameObjectSetActive(dismissButtonHighlighted, ShowImprovedVersion);
		DismissButton = (ShowImprovedVersion ? dismissButtonHighlighted : dismissButtonNormal);
	}

	public void Show()
	{
		if (!shown)
		{
			if (showAtCenter)
			{
				characterAnimator.Play(characterAnimationEnterCenter.name);
			}
			else
			{
				characterAnimator.Play(characterAnimationEnter.name);
			}
			shown = true;
		}
	}

	public bool IsShown()
	{
		return shown;
	}

	public void SetText(string text)
	{
		label.text = text;
	}

	public void Hide()
	{
		shown = false;
		showAtCenter = false;
		characterAnimator.Play(characterAnimationExit.name);
	}

	public void HideInstant()
	{
		shown = false;
		showAtCenter = false;
		characterAnimator.Play(characterAnimationExit.name, -1, 1f);
	}

	public void SetCharacter(string character)
	{
		characterPortrait.gameObject.SetActive(value: false);
		characterSprite.gameObject.SetActive(value: true);
		characterSprite.spriteName = character;
		bool flag = character == "Portrait_Info";
		if (characterSpriteContainer != null)
		{
			characterSpriteContainer.SetActive(!flag);
		}
		if (infoIcon != null)
		{
			infoIcon.SetActive(flag);
		}
	}

	public void SetShowAtCenter()
	{
		showAtCenter = true;
	}

	private void OnPortraitRendered(IPortraitRenderSource info)
	{
		if (base.gameObject != null && characterPortrait != null && actor != null && info.ActorDefinitionId == actor.ActorDefinitionID)
		{
			characterPortrait.mainTexture = PortraitManager.Instance.GetPortrait(info);
		}
	}

	public void SetCharacter(ActorModel actorModel)
	{
		characterSpriteContainer.SetActive(value: true);
		characterPortrait.gameObject.SetActive(value: true);
		characterSprite.gameObject.SetActive(value: false);
		actor = actorModel;
		if (PortraitManager.Instance != null)
		{
			Texture portrait = PortraitManager.Instance.GetPortrait(PortraitRenderSource.fromActorModel(actorModel));
			if (portrait == null)
			{
				ModularCharacter modularCharacter = ActorView.GetPrefabForActor(actorModel);
				if (modularCharacter == null)
				{
					modularCharacter = ActorView.SelectRandomPrefabForActor(actorModel);
				}
				if (modularCharacter != null)
				{
					PortraitManager.Instance.CreatePortrait(PortraitRenderSource.fromActorModel(actorModel), modularCharacter, OnPortraitRendered);
				}
			}
			else
			{
				characterPortrait.mainTexture = portrait;
			}
		}
		if (infoIcon != null)
		{
			infoIcon.SetActive(value: false);
		}
	}

	public void MoveToInactivePosition(float inactiveTransitionDuration)
	{
		TweenPosition.Begin(characterSprite.gameObject, inactiveTransitionDuration, inactivePosition);
	}
}
