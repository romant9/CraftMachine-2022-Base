using System;
using System.Collections.Generic;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class CombatEndFlowRewards : CombatEndFlowStep
{
	private enum AnimationState
	{
		AnimatingTitle = 0,
		OpeningBag = 1,
		AnimatingEquipmentCards = 2,
		AnimatingSurvivorCards = 3,
		OpeningSurvivorCards = 4,
		Invalid = 5
	}

	[Tooltip("Equipment card prefab")]
	[SerializeField]
	private GameObject equipmentCardPrefab;

	[Tooltip("Equipment bag prefab")]
	[SerializeField]
	private GameObject equipmentBagPrefab;

	[Tooltip("Survivor card prefab")]
	[SerializeField]
	private GameObject survivorCardPrefab;

	[Tooltip("Grid to place the equipment cards")]
	[SerializeField]
	private GameObject equipmentContainerGrid;

	[Tooltip("Grid to place the survivor cards")]
	[SerializeField]
	private GameObject mainContainerGrid;

	[Tooltip("Rewards title label.")]
	[SerializeField]
	private UILabel title;

	[Tooltip("Distance between each equipment card")]
	[SerializeField]
	private float cardOffset;

	private List<GameObject> survivorCards = new List<GameObject>();

	private List<GameObject> equipmentCards = new List<GameObject>();

	private List<CardsTransform> equipmentCardsStartTransforms = new List<CardsTransform>();

	private List<CardsTransform> equipmentCardsEndTransforms = new List<CardsTransform>();

	private GameObject bagObject;

	private float animationTime;

	private const float TITLE_REST_TIME = 0.5f;

	private const float BAG_REVEAL_TIME = 0.5f;

	private const float EQUIPMENT_CARD_MOVE_TIME = 0.5f;

	private const float SURVIVOR_CARD_REVEAL_TIME = 0.5f;

	private const float CARD_REST_TIME = 0.5f;

	private const float CARD_COUNTDOWN_TIME = 0.25f;

	private const float CARD_COUNTDOWN_REST_TIME = 0.25f;

	private const float CARDS_DISAPPEAR_TIME = 0.5f;

	private AnimationState currentState;

	public CombatEndFlowRewards()
	{
		DestroyAfterCompletion = false;
	}

	private void ChangeState(AnimationState newState)
	{
		if (currentState != newState)
		{
			CleanUpAfterOldState();
			currentState = newState;
			NewStateApplied();
			animationTime = 0f;
		}
	}

	private void CleanUpAfterOldState()
	{
		if (currentState == AnimationState.OpeningBag)
		{
			bagObject.SetActive(value: false);
		}
	}

	private void NewStateApplied()
	{
		if (currentState == AnimationState.OpeningBag)
		{
			int num = 0;
			foreach (GameObject equipmentCard in equipmentCards)
			{
				equipmentCard.SetActive(value: true);
				equipmentCard.transform.localPosition = equipmentCardsStartTransforms[num].position;
				num++;
			}
			bagObject = UnityEngine.Object.Instantiate(equipmentBagPrefab);
			bagObject.transform.parent = base.gameObject.transform;
			bagObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			bagObject.transform.localScale = new Vector3(1f, 1f, 1f);
			bagObject.GetComponentInChildren<EquipmentBag>().OnBagOpened += OnEquipmentOpened;
			return;
		}
		if (currentState == AnimationState.AnimatingEquipmentCards)
		{
			CombatEndFlowStep.InterpolateElements(equipmentCards, equipmentCardsStartTransforms, equipmentCardsEndTransforms, 0f, useLocalPosition: true);
			{
				foreach (GameObject equipmentCard2 in equipmentCards)
				{
					equipmentCard2.SetActive(value: true);
				}
				return;
			}
		}
		if (currentState == AnimationState.AnimatingSurvivorCards)
		{
			foreach (GameObject survivorCard in survivorCards)
			{
				survivorCard.SetActive(value: true);
				survivorCard.GetComponent<UIWidget>().alpha = 0f;
				UIButton componentInChildren = survivorCard.GetComponentInChildren<UIButton>();
				if ((bool)componentInChildren)
				{
					UIEventListener uIEventListener = UIEventListener.Get(componentInChildren.gameObject);
					uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnSurvivorCardOpened));
				}
			}
			return;
		}
		_ = currentState;
		_ = 4;
	}

	public override void StartFlow()
	{
		base.StartFlow();
		animationTime = 0f;
	}

	private void OnEnable()
	{
		title.gameObject.SetActive(value: false);
	}

	public void SetupRewards(List<EquipmentItemModel> primaryLootEquipment, List<ActorModel> rescuedSurvivors)
	{
		foreach (GameObject equipmentCard in equipmentCards)
		{
			UnityEngine.Object.Destroy(equipmentCard);
		}
		equipmentCards.Clear();
		equipmentCardsStartTransforms.Clear();
		equipmentCardsEndTransforms.Clear();
		if (primaryLootEquipment != null)
		{
			foreach (EquipmentItemModel item in primaryLootEquipment)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(equipmentCardPrefab);
				gameObject.transform.parent = base.gameObject.transform;
				gameObject.SetActive(value: false);
				equipmentCards.Add(gameObject);
				gameObject.GetComponent<CombatEquipmentInfoCard>().Setup(item);
			}
			List<CardsTransform> outTransforms = new List<CardsTransform>();
			CombatEndFlowStep.SetupElementsTransformsOnGrid(equipmentCards, equipmentContainerGrid, ref outTransforms, addToContainer: true, cardOffset, horizontalFill: false, useLocalPosition: true);
			CombatEndFlowStep.SetupElementsTransforms(equipmentCards, ref equipmentCardsStartTransforms, new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f));
		}
		foreach (GameObject survivorCard in survivorCards)
		{
			UnityEngine.Object.Destroy(survivorCard);
		}
		survivorCards.Clear();
		if (rescuedSurvivors != null)
		{
			foreach (SurvivorModel rescuedSurvivor in rescuedSurvivors)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(survivorCardPrefab);
				gameObject2.transform.parent = base.gameObject.transform;
				gameObject2.SetActive(value: false);
				survivorCards.Add(gameObject2);
				SurvivorCard component = gameObject2.GetComponent<SurvivorCard>();
				component.Item = rescuedSurvivor;
				component.UpdateUI();
				component.SetupForUnrevealed();
				component.SetPicture(PortraitManager.Instance.GetPortrait(PortraitRenderSource.fromActorModel(rescuedSurvivor.manager.CombatModel.MissionRoster[0])));
			}
		}
		List<GameObject> list = new List<GameObject>();
		if (equipmentCards.Count > 0)
		{
			list.Add(equipmentContainerGrid);
		}
		list.AddRange(survivorCards);
		UnityUtils.AlignItemsInsideContainerLine(list, mainContainerGrid, cardOffset, addToContainer: false, 1f);
		equipmentCardsEndTransforms.Clear();
		foreach (GameObject equipmentCard2 in equipmentCards)
		{
			equipmentCard2.transform.parent = base.gameObject.transform;
			CardsTransform cardsTransform = new CardsTransform();
			cardsTransform.position = equipmentCard2.transform.localPosition;
			cardsTransform.scale = equipmentCard2.transform.localScale;
			equipmentCardsEndTransforms.Add(cardsTransform);
		}
		currentState = AnimationState.Invalid;
		ChangeState(AnimationState.AnimatingTitle);
	}

	public override void ForceFlowEnd()
	{
		base.ForceFlowEnd();
		foreach (GameObject survivorCard in survivorCards)
		{
			survivorCard.SetActive(value: true);
			survivorCard.GetComponent<UIWidget>().alpha = 1f;
		}
	}

	private void OnEquipmentOpened(EquipmentBag equipmentBag)
	{
		ChangeState(AnimationState.AnimatingEquipmentCards);
	}

	private void OnSurvivorCardOpened(GameObject buttonObject)
	{
		UIEventListener uIEventListener = UIEventListener.Get(buttonObject);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnSurvivorCardOpened));
		buttonObject.GetComponentInParent<SurvivorCard>().RevealCard();
		foreach (GameObject survivorCard in survivorCards)
		{
			if (survivorCard.GetComponent<SurvivorCard>().IsUnrevealed())
			{
				return;
			}
		}
		ForceFlowEnd();
	}

	private void UpdateTitleAnimation()
	{
		if (animationTime < 0.5f)
		{
			float alpha = animationTime / 0.5f;
			title.alpha = alpha;
			return;
		}
		title.alpha = 1f;
		if (equipmentCards != null && equipmentCards.Count > 0)
		{
			ChangeState(AnimationState.OpeningBag);
		}
		else
		{
			ChangeState(AnimationState.AnimatingSurvivorCards);
		}
	}

	private void UpdateAnimatingEquipmentCards()
	{
		float a = MathExtensions.EaseCubicIn(animationTime, 0f, 1f, 0.5f);
		a = Mathf.Max(0f, Mathf.Min(a, 1f));
		CombatEndFlowStep.InterpolateElements(equipmentCards, equipmentCardsStartTransforms, equipmentCardsEndTransforms, a, useLocalPosition: true);
		if (a >= 1f)
		{
			if (survivorCards.Count > 0)
			{
				ChangeState(AnimationState.AnimatingSurvivorCards);
			}
			else
			{
				ForceFlowEnd();
			}
		}
	}

	private void UpdateAnimatingSurvivorCards()
	{
		float b = animationTime / 0.5f;
		b = Mathf.Max(0f, Mathf.Min(1f, b));
		foreach (GameObject survivorCard in survivorCards)
		{
			survivorCard.SetActive(value: true);
			survivorCard.GetComponent<UIWidget>().alpha = b;
		}
		if (b >= 1f)
		{
			ChangeState(AnimationState.OpeningSurvivorCards);
		}
	}

	public override void Update()
	{
		base.Update();
		if (flowStarted && !flowEnded)
		{
			animationTime += Time.deltaTime;
			title.gameObject.SetActive(value: true);
			if (currentState == AnimationState.AnimatingTitle)
			{
				UpdateTitleAnimation();
			}
			else if (currentState == AnimationState.AnimatingEquipmentCards)
			{
				UpdateAnimatingEquipmentCards();
			}
			else if (currentState == AnimationState.AnimatingSurvivorCards)
			{
				UpdateAnimatingSurvivorCards();
			}
		}
	}
}
