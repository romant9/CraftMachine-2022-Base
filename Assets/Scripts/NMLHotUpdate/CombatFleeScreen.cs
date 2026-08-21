using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class CombatFleeScreen : HUDElement
{
	[Tooltip("Survivor card prefab")]
	[SerializeField]
	private GameObject survivorCardPrefab;

	[Tooltip("Grid to place the casualties cards")]
	[SerializeField]
	private GameObject casualtiesContainerGrid;

	[Tooltip("Distance between each survivor card")]
	[SerializeField]
	private float survivorCardContainerOffset;

	[SerializeField]
	private GameObject deadlyMissionContainer;

	[SerializeField]
	private GameObject outpostMissionContainer;

	[SerializeField]
	private GameObject normalMissionContainer;

	[SerializeField]
	private GameObject survivalMissionContainer;

	[SerializeField]
	private UIButton confirmButton;

	[SerializeField]
	private GameObject buttonsContainer;

	[SerializeField]
	private GameObject mainContainer;

	[SerializeField]
	private GameObject nextButton;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel subtitleLabel;

	[SerializeField]
	private UILabel outpostConfirmLabel;

	private int numberAnimationOver;

	private bool ClosingAnimation;

	private List<GameObject> survivorCards = new List<GameObject>();

	private SurvivorCard survivorSelectedCard;

	private bool isDeadly;

	private bool isOutpost;

	private bool isSurvivalMission;

	private bool canSelectSurvivor;

	private bool fled;

	private bool clickedNextButton;

	private List<SurvivorModel> incapacitatedSurvivors = new List<SurvivorModel>();

	private bool incapacatitatedSurvivorLeftBehind;

	public bool AnimationOver
	{
		get
		{
			if (survivorCards != null)
			{
				return numberAnimationOver >= survivorCards.Count;
			}
			return false;
		}
	}

	public override void Open()
	{
		base.Open();
		survivorSelectedCard = null;
		fled = false;
		clickedNextButton = false;
		numberAnimationOver = 0;
		ClosingAnimation = false;
		if (nextButton != null)
		{
			nextButton.SetActive(value: false);
		}
		buttonsContainer.SetActive(value: true);
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		isDeadly = combat.IsDeadly;
		isOutpost = combat.HasPvPRules;
		isSurvivalMission = combat.IsSurvivalMission;
		IMapMissionModel attackTargetMissionModel = GameManager.Instance.playerModel.GetAttackTargetMissionModel();
		if (attackTargetMissionModel != null)
		{
			canSelectSurvivor = attackTargetMissionModel.MaxTeamSize > 0 && !combat.IsEndlessBattleMission;
		}
		deadlyMissionContainer.SetActive(isDeadly && !isOutpost);
		outpostMissionContainer.SetActive((isOutpost && !isDeadly) || !canSelectSurvivor);
		normalMissionContainer.SetActive(!isDeadly && !isOutpost && !isSurvivalMission && canSelectSurvivor);
		Helpers.GameObjectSetActive(survivalMissionContainer, isSurvivalMission);
		if (isOutpost && outpostMissionContainer.activeSelf)
		{
			outpostConfirmLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.FleeConfirmation.MessageOutpost");
		}
		else if (outpostMissionContainer.activeSelf && !canSelectSurvivor)
		{
			if (combat.IsEndlessBattleMission)
			{
				outpostConfirmLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.FleeConfirmation.MessageEndlessMode");
			}
			else
			{
				outpostConfirmLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.FleeConfirmation.MessageNoOwnSurvivors");
			}
		}
		if (isOutpost || !canSelectSurvivor)
		{
			confirmButton.isEnabled = true;
		}
		else
		{
			confirmButton.isEnabled = false;
		}
		incapacitatedSurvivors.Clear();
		IEnumerable enumerable = null;
		enumerable = ((GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors == null || GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors.Count <= 0) ? ((IEnumerable<ActorModel>)combat.Survivors) : ((IEnumerable<ActorModel>)GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors));
		foreach (SurvivorModel item in enumerable)
		{
			if (!item.IsDead)
			{
				GameObject gameObject = Object.Instantiate(survivorCardPrefab);
				gameObject.transform.parent = base.gameObject.transform;
				NGUITools.SetLayer(gameObject, base.gameObject.layer);
				survivorCards.Add(gameObject);
				SurvivorCard component = gameObject.GetComponent<SurvivorCard>();
				component.IsMissionSurvivor = GameManager.Instance.playerModel.MapContainerModel?.AttackTargetMissionModel?.IsFixedSurvivorSeasonMission == true;
				component.Item = item;
				component?.SetExtraAttackLabel();
				component.OnlyShowSurvivorTop();
				if ((item.IsStruggling || item.IsBleedingOut) && !IsSpecialMissionCharacter(item, attackTargetMissionModel as MapMissionModel) && !combat.IsEndlessBattleMission)
				{
					incapacitatedSurvivors.Add(item);
					survivorSelectedCard = component;
					survivorSelectedCard.ShowSacrifice(show: true, isDeadly, item.ExclusiveTimedEffect.Duration - item.ExclusiveTimedEffect.Counter, GameManager.Instance.playerModel.Combat.IsSurvivalMission);
				}
			}
		}
		incapacatitatedSurvivorLeftBehind = incapacitatedSurvivors.Count > 0;
		if (isOutpost || !canSelectSurvivor)
		{
			titleLabel.text = LocalizationManager.GetText("Popup.FleeConfirmation.Title_Outpost");
			subtitleLabel.text = "";
		}
		else if (incapacatitatedSurvivorLeftBehind)
		{
			confirmButton.isEnabled = true;
			titleLabel.text = LocalizationManager.GetText("Popup.FleeConfirmation.Title_Incapacitated");
			subtitleLabel.text = ((!isSurvivalMission) ? LocalizationManager.GetText("Popup.FleeConfirmation.Subtitle") : "");
		}
		else if (isSurvivalMission)
		{
			subtitleLabel.text = "";
		}
		else
		{
			titleLabel.text = LocalizationManager.GetText("Popup.FleeConfirmation.Title");
			subtitleLabel.text = LocalizationManager.GetText("Popup.FleeConfirmation.Subtitle");
		}
		Helpers.GameObjectSetActive(subtitleLabel, combat == null || !combat.HasGuildBossRules);
		UnityUtils.AlignItemsInsideContainerLine(survivorCards, casualtiesContainerGrid, survivorCardContainerOffset, addToContainer: true, 1f);
		UIEvent.OnUIEvent += OnUIEvent;
		TweenManager.RemoveCallback(base.gameObject, 10, Close);
		TweenManager.PlayTweenGroup(base.gameObject, 10);
	}

	private bool IsSpecialMissionCharacter(SurvivorModel survivor, MapMissionModel map)
	{
		if (survivor != null && map != null && map.MissionData != null && map.MissionData.ExtraData != null && map.MissionData.ExtraData.InUse && map.MissionData.ExtraData.PlayableSurvivors != null)
		{
			for (int i = 0; i < map.MissionData.ExtraData.PlayableSurvivors.Count; i++)
			{
				if (map.MissionData.ExtraData.PlayableSurvivors[i].ActorID == survivor.ActorDefinitionID)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void OnDisable()
	{
		foreach (GameObject survivorCard in survivorCards)
		{
			survivorCard.GetComponent<CacheableObject>().Destroy();
		}
		survivorCards.Clear();
	}

	public override void Close()
	{
		base.Close();
		ClosingAnimation = false;
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	public void OnCloseButton(GameObject button)
	{
		if (!ClosingAnimation)
		{
			ClosingAnimation = true;
			TweenManager.PlayTweenGroup(base.gameObject, 10, forward: false, Close);
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		}
	}

	public void OnFleeButton(GameObject button)
	{
		if (survivorSelectedCard == null && !isOutpost && canSelectSurvivor)
		{
			confirmButton.isEnabled = false;
			return;
		}
		buttonsContainer.SetActive(fled);
		Helpers.ExecuteCommand(new FleeStep1Command(incapacitatedSurvivors));
		foreach (GameObject survivorCard in survivorCards)
		{
			SurvivorCard component = survivorCard.GetComponent<SurvivorCard>();
			component.UpdateUIForEndScreenStatus(survivorAnimOver, IsSpecialMissionCharacter(component.Item, GameManager.Instance.playerModel.MapContainerModel.AttackTargetMissionModel), isSurvivalMission);
		}
		fled = true;
		confirmButton.isEnabled = false;
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	public override void OnBackButtonClicked()
	{
		if (!fled)
		{
			OnCloseButton(null);
		}
		else
		{
			checkNextState();
		}
	}

	private void survivorAnimOver()
	{
		numberAnimationOver++;
	}

	public override void Update()
	{
		base.Update();
		if (nextButton != null && !nextButton.activeInHierarchy && AnimationOver)
		{
			nextButton.SetActive(value: true);
		}
		if (fled && Input.GetMouseButtonUp(0))
		{
			checkNextState();
		}
	}

	private void checkNextState()
	{
		if (AnimationOver && !clickedNextButton)
		{
			clickedNextButton = true;
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			TweenManager.PlayTweenGroup(mainContainer, 20, forward: true, SendFleeCall);
		}
	}

	private void SendFleeCall()
	{
		Helpers.ExecuteCommand(new FleeStep2Command());
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (incapacatitatedSurvivorLeftBehind || !(type == "OnNewSurvivorSelected") || fled || isOutpost || !canSelectSurvivor)
		{
			return;
		}
		if (survivorSelectedCard != null)
		{
			survivorSelectedCard.ShowSacrifice(show: false, isDeadly, 0, GameManager.Instance.playerModel.Combat.IsSurvivalMission);
		}
		SurvivorModel survivorModel = parameter as SurvivorModel;
		incapacitatedSurvivors.Clear();
		incapacitatedSurvivors.Add(survivorModel);
		foreach (GameObject survivorCard in survivorCards)
		{
			SurvivorCard component = survivorCard.GetComponent<SurvivorCard>();
			if (component.Item == survivorModel)
			{
				survivorSelectedCard = component;
				component.ShowSacrifice(show: true, isDeadly, 0, GameManager.Instance.playerModel.Combat.IsSurvivalMission);
				confirmButton.isEnabled = true;
			}
		}
	}
}
