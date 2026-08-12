using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Client.Utils;
using TWDModel;
using TWDModel.ContentTypes;
using UnityEngine;

public class CombatHUD : HUDElement
{
	public delegate void AbilitySelectedCallback(AbilityModel ability, ActorModel sourceActor);

	public enum SkillType
	{
		Weapon = 0,
		Survivor = 1
	}

	[Tooltip("Enemy icon position.")]
	[SerializeField]
	private UISprite enemyIconPosition;

	private GameObject enemyIconObject;

	[Tooltip("Enemy name label.")]
	[SerializeField]
	private UILabel enemyName;

	[SerializeField]
	private UIButton menuButton;

	[SerializeField]
	private UIButton backupButton;

	[SerializeField]
	private UILabel turnsLabel;

	[SerializeField]
	private UIButton consumableButton;

	[SerializeField]
	private GameObject cancelConsumablesButton;

	[SerializeField]
	private UILabel consumablesCooldownLabel;

	[SerializeField]
	private UIButton completeMissionButton;

	[SerializeField]
	private UILabel completeMissionLabel;

	[Tooltip("All player info container.")]
	[SerializeField]
	private GameObject playerInfoContainer;

	[Tooltip("Enemy info container.")]
	[SerializeField]
	private GameObject enemyInfoContainer;

	[SerializeField]
	[Tooltip("Combat objectives container.")]
	private GameObject objectivesContainer;

	[Tooltip("Player ability button prefab")]
	[SerializeField]
	private GameObject abilityButtonPrefab;

	[SerializeField]
	[Tooltip("Objective button prefab.")]
	private GameObject objectiveButtonPrefab;

	[Tooltip("Endless Mode Info Container")]
	[SerializeField]
	private GameObject endlessModeInfoContainer;

	[SerializeField]
	private GameObject consumablesPlightButton;

	[Tooltip("Endless Mode Info")]
	[SerializeField]
	private GameObject endlessModeExpertModeTagContainer;

	[SerializeField]
	[Tooltip("Passive ability button prefab.")]
	private CombatPassiveAbilityButton passiveAbilityButtonPrefab;

	[SerializeField]
	[Tooltip("Container for screen notifications.")]
	private GameObject notificationsContainer;

	[SerializeField]
	[Tooltip("SpeedUp Button.")]
	private UIButtonToggle speedUpButton;

	[Tooltip("Container for threat meter.")]
	[SerializeField]
	private GameObject threatMeterContainer;

	[Tooltip("Container for charge meter.")]
	[SerializeField]
	private GameObject chargeMeterContainer;

	[Tooltip("Container for threat wave notification.")]
	[SerializeField]
	private GameObject waveNotificaitonContainer;

	[Tooltip("Survivors health bar prefab.")]
	[SerializeField]
	private GameObject survivorHealthBarPrefab;

	[SerializeField]
	[Tooltip("Walkers health bar prefab.")]
	private GameObject walkerHealthBarPrefab;

	[SerializeField]
	[Tooltip("Raiders health bar prefab.")]
	private GameObject raiderHealthBarPrefab;

	[SerializeField]
	[Tooltip("Actors notification element prefab for generic messages.")]
	private GameObject actorDefaultNotificationElementPrefab;

	[SerializeField]
	[Tooltip("Actors notification element prefab for attack event messages.")]
	private GameObject actorAttackNotificationElementPrefab;

	[SerializeField]
	[Tooltip("Actors notification element prefab for damage messages.")]
	private GameObject actorDamageNotificationElementPrefab;

	[SerializeField]
	[Tooltip("Actors notification element prefab for fire damage messages.")]
	private GameObject actorDamageFireNotificationElementPrefab;

	[SerializeField]
	[Tooltip("Actors notification element prefab for poison damage messages.")]
	private GameObject actorDamagePoisonNotificationElementPrefab;

	[SerializeField]
	[Tooltip("Actors notification element prefab for poison damage messages.")]
	private GameObject actorDamageGrenadeFragmentNotificationElementPrefab;

	[Tooltip("Actors notification element prefab for bleeding damage messages.")]
	[SerializeField]
	private GameObject actorDamageBleedingNotificationElementPrefab;

	[SerializeField]
	[Tooltip("Actors notification element prefab for SP gain messages.")]
	private GameObject actorCurrencySPNotificationElementPrefab;

	[Tooltip("Actors notification element prefab for supply gain messages.")]
	[SerializeField]
	private GameObject actorCurrencySuppliesNotificationElementPrefab;

	[SerializeField]
	[Tooltip("Actors notification element prefab for Key found messages.")]
	private GameObject actorCurrencyKeyNotificationElementPrefab;

	[SerializeField]
	[Tooltip("Actors notification element prefab for silver loot found messages.")]
	private GameObject actorLootSilverNotificationElementPrefab;

	[SerializeField]
	[Tooltip("Actors notification element prefab for silver loot found messages.")]
	private GameObject actorLootGoldNotificationElementPrefab;

	[Tooltip("Actors notification element prefab for Charge Point gained messages.")]
	[SerializeField]
	private GameObject actorChargePointNotificationElementPrefab;

	[Tooltip("Actors notification prefab for Trait trigger messages.")]
	[SerializeField]
	private GameObject actorActionNotificationPrefab;

	[SerializeField]
	[Tooltip("Actors notification prefab for Timed Effect trigger messages.")]
	private GameObject actorTimedEffectNotificationPrefab;

	[Tooltip("Action Indicator prefab.")]
	[SerializeField]
	private GameObject actionIndicatorPrefab;

	[Tooltip("Action Indicator prefab.")]
	[SerializeField]
	private GameObject moveActionIndicatorPrefab;

	[SerializeField]
	[Tooltip("Action Indicator prefab.")]
	private GameObject moveGroundIndicatorPrefab;

	[SerializeField]
	[Tooltip("Cover move indicator prefab.")]
	private GameObject coverMoveIndicatorPrefab;

	[Tooltip("Actor info popup prefab.")]
	[SerializeField]
	private GameObject actorInfoPopupPrefab;

	[Tooltip("Actor info popup prefab.")]
	[SerializeField]
	private GameObject interactiveObjectInfoPopupPrefab;

	[Tooltip("Turn Count Indicator prefab.")]
	[SerializeField]
	private GameObject turnCountPrefab;

	[SerializeField]
	[Tooltip("Delayed action grenade turn count indicator prefab.")]
	private GameObject grenadeTurnCountPrefab;

	[SerializeField]
	[Tooltip("Threat Meter prefab.")]
	private GameObject threatMeterPrefab;

	[Tooltip("Turn Panel prefab.")]
	[SerializeField]
	private GameObject turnPanelPrefab;

	[SerializeField]
	[Tooltip("Turn Panel prefab.")]
	private GameObject endlessModeTurnPanel;

	[SerializeField]
	[Tooltip("Threat Meter Overlay Indicator.")]
	public ThreatMeterOverlay ThreatMeterOverlay;

	[SerializeField]
	[Tooltip("Charge Meter prefab.")]
	private GameObject chargeMeterPrefab;

	[SerializeField]
	[Tooltip("Wave notification prefab.")]
	private GameObject waveNotificationPrefab;

	[SerializeField]
	[Tooltip("Time notification prefab.")]
	private GameObject timeNotificationPrefab;

	[SerializeField]
	[Tooltip("Turn notification prefab.")]
	private GameObject turnNotificationPrefab;

	[SerializeField]
	[Tooltip("Combat end notification prefab.")]
	private GameObject fadeOutNotificationPrefab;

	[Tooltip("Walker turn notification prefab.")]
	[SerializeField]
	private GameObject walkerTurnNotificationPrefab;

	[SerializeField]
	[Tooltip("Battle Pass Currency Notification prefab.")]
	private GameObject battlePassCurrencyNotificationPrefab;

	[SerializeField]
	[Tooltip("Actor speech bubble prefab.")]
	private GameObject speechBubblePrefab;

	[Tooltip("Indicator prefabs")]
	[SerializeField]
	public List<IndicatorPrefabInfo> IndicatorPrefabs = new List<IndicatorPrefabInfo>();

	[Tooltip("Endless Mode Wave Count Label")]
	[SerializeField]
	private UILabel EndlessModeWaveCountLabel;

	[Tooltip("Endless Mode Wave Score Label")]
	[SerializeField]
	private UILabel EndlessModeScoreLabel;

	[SerializeField]
	[Tooltip("Endless Mode Wave Kill Score Multiplier")]
	private UILabel EndlessModeKillScoreMultiplier;

	[Tooltip("Endless Mode Expert Kill Multiplier tag")]
	[SerializeField]
	private UILabel EndlessModeExpertKillScoreMultiplierTag;

	[SerializeField]
	[Tooltip("List of actor info containers.")]
	private List<ActorInfoContainer> actorInfoContainers;

	[SerializeField]
	private CombatSupportsUIView combatSupportsUIView;

	[SerializeField]
	private int CloseBackUpTime = 5;

	[SerializeField]
	private TraitInfoContainer traitInfoContainer;

	[Tooltip("Expert Mode Color for Endless Mode")]
	public Color ExpertModeColor;

	[Tooltip("Normal Mode Color for Endless Mode")]
	public Color NormalModeColor;

	private MoveActionIndicator moveActionIndicator;

	private CoverIndicator coverMoveIndicator;

	private float normalSpeed = 1f;

	private float highSpeed = 2f;

	private float lastClickedTime;

	[SerializeField]
	[Tooltip("Portrait normal shader.")]
	private Shader actorPortraitNormalShader;

	[SerializeField]
	[Tooltip("Portrait turn complete shader.")]
	private Shader actorPortraitTurnCompleteShader;

	[SerializeField]
	private UIButton skillLeftArrow;

	[SerializeField]
	private UIButton skillRightArrow;

	[SerializeField]
	private UILabel activeSkillWeaponName;

	[SerializeField]
	private GameObject traitPart;

	[Tooltip("Equipment sprite")]
	[SerializeField]
	private UITexture EquipmentTexture;

	[SerializeField]
	private GameObject skillPart1;

	[SerializeField]
	private UILabel Skill1APNum;

	[SerializeField]
	private UISprite skill1Icon1;

	[SerializeField]
	private UISprite skill1Icon1Corlor1;

	[SerializeField]
	private UISprite skill1Icon1BG1;

	[SerializeField]
	private GameObject skill1Short;

	[SerializeField]
	private GameObject skill1ShortAP;

	[SerializeField]
	private GameObject skill1ShortAction;

	[SerializeField]
	private UILabel skill1ShortActionNum;

	[SerializeField]
	private GameObject skillPart2;

	[SerializeField]
	private UILabel Skill2APNum;

	[SerializeField]
	private UISprite skill2Icon1;

	[SerializeField]
	private UISprite skill2Icon1Corlor1;

	[SerializeField]
	private UISprite skill2Icon1BG1;

	[SerializeField]
	private GameObject skill2Short;

	[SerializeField]
	private GameObject skill2ShortAP;

	[SerializeField]
	private GameObject skill2ShortAction;

	[SerializeField]
	private UILabel skill2ShortActionNum;

	[SerializeField]
	private GameObject toolTip1;

	[SerializeField]
	private UILabel toolTip1Name;

	[SerializeField]
	private UILabel toolTip1Des;

	[SerializeField]
	private UILabel toolTip1CDNum;

	[SerializeField]
	private UILabel toolTip1APNum;

	[SerializeField]
	private UISprite toolTip1Icon1;

	[SerializeField]
	private UISprite toolTip1Icon1Corlor1;

	[SerializeField]
	private UISprite toolTip1Icon1BG1;

	[SerializeField]
	private GameObject toolTip2;

	[SerializeField]
	private UILabel toolTip2Name;

	[SerializeField]
	private UILabel toolTip2Des;

	[SerializeField]
	private UILabel toolTip2CDNum;

	[SerializeField]
	private UILabel toolTip2APNum;

	[SerializeField]
	private UISprite toolTip2Icon1;

	[SerializeField]
	private UISprite toolTip2Icon1Corlor1;

	[SerializeField]
	private UISprite toolTip2Icon1BG1;

	[SerializeField]
	private GameObject activeSkillContent;

	[SerializeField]
	private GameObject skilArrowContent;

	[SerializeField]
	private GameObject activeSkillOperate;

	[SerializeField]
	private GameObject MainContainer;

	[SerializeField]
	private GameObject SkillContainer;

	[SerializeField]
	private UILabel EndlessLoseLabel;

	[SerializeField]
	private GameObject DebuffDamagePerRoundTips;

	private CommandSkillModelManager commandSkillModelManager;

	private ActorModel selectionActor;

	private int selectionSkillIndex;

	[SerializeField]
	private UIButton okbutton;

	private WaveNotification waveNotification;

	private bool activateChargeOnChange;

	private bool isPopUpEndlessNormalModeExit;

	public List<ActorModel> ActiveSkillActors = new List<ActorModel>();

	private UIWidget notificationParentWidget;

	private List<UIButton> abilityButtons = new List<UIButton>();

	[SerializeField]
	private GameObject CooldownAnimationsParent;

	private const int tweenerGroupCooldown = 4;

	private const int tweenerGroupReady = 5;

	private int previousCooldown = -1;

	private bool ignoreDeselectAbilityButtons;

	private DoubleBooleanState ShowMenuButtonState;

	private DoubleBooleanState ShowObjectivesState;

	private DoubleBooleanState ShowChargeState;

	private DoubleBooleanState ShowSkipTurnState;

	private DoubleBooleanState ShowSpeedUpState;

	private DoubleBooleanState ShowThreatTurnState;

	private DoubleBooleanState ShowKeysState;

	private bool endTurnEnabled = true;

	private Dictionary<GameObject, IndicatorInstanceInfo> locationIndicators = new Dictionary<GameObject, IndicatorInstanceInfo>();

	public bool IsSkillSelectableStatus { get; private set; }

	public bool CanSelectSkill { get; private set; }

	public SkillType CurSkillType { get; private set; }

	private ActorModel ActiveActor => GameManager.Instance.playerModel.Combat?.ActiveActor;

	public ActorModel ActiveSkillActor { get; private set; }

	public GridCoordinate ActiveSkillGridCell { get; private set; } = GridCoordinate.Invalid;

	private bool shouldHideConsumableButtons
	{
		get
		{
			PlayerModel playerModel = GameManager.Instance.playerModel;
			CombatModel combat = playerModel.Combat;
			IMapMissionModel attackTargetMissionModel = playerModel.GetAttackTargetMissionModel();
			if (!(combat.SuggestedInteractionTargetCoordinate != GridCoordinate.Invalid) && !TutorialView.Instance.InCombatTutorial && (attackTargetMissionModel == null || attackTargetMissionModel.MaxTeamSize != 0) && (!TutorialView.Instance.Running || playerModel.Tutorial.ShowDiamondsHud))
			{
				if (combat.MapCategory == MapCategory.Outpost)
				{
					return playerModel.OutpostTutorialState != OutpostTutorialState.Done;
				}
				return false;
			}
			return true;
		}
	}

	public static bool IsSpeedUpEnabled { get; private set; }

	public GameObject ObjectivesContainer => objectivesContainer;

	public event AbilitySelectedCallback OnAbilitySelected;

	private ActorInfoContainer GetActorInfoContainer(ActorModel actor)
	{
		return actorInfoContainers.Find((ActorInfoContainer x) => x.Actor == actor);
	}

	public void OnSpeedUpClicked()
	{
		SetSpeedUpState(!IsSpeedUpEnabled);
		GameManager.Instance.Settings.CombatSpeedUp = IsSpeedUpEnabled;
		string text = "combat_ui/speed_";
		text += (IsSpeedUpEnabled ? "up" : "down");
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(text);
	}

	public void SetSpeedUpState(bool enabled)
	{
		IsSpeedUpEnabled = enabled;
		if (speedUpButton != null)
		{
			speedUpButton.SetToggled(IsSpeedUpEnabled);
			UILabel componentInChildren = speedUpButton.GetComponentInChildren<UILabel>();
			if (componentInChildren != null)
			{
				UITweener[] components = componentInChildren.GetComponents<UITweener>();
				if (components != null && componentInChildren != null)
				{
					componentInChildren.text = (IsSpeedUpEnabled ? "x 2" : "x 1");
					for (int i = 0; i < components.Length; i++)
					{
						components[i].ResetToBeginning();
						components[i].PlayForward();
					}
				}
			}
		}
		float num = (IsSpeedUpEnabled ? highSpeed : normalSpeed);
		if (num != Time.timeScale)
		{
			Time.timeScale = num;
		}
	}

	public void SetSurvivorTurnHUD(ActorModel actor)
	{
		OnClickSkillLeft();
		UpdateTheActiveSKill(actor);
		if (actor != null && actor.Faction == Faction.Survivor)
		{
			SetupSurvivorPortraits();
		}
	}

	public void UpdateTheActiveSKill(ActorModel actor)
	{
		if (ActiveActor == null || actor == null || ActiveActor != actor)
		{
			return;
		}
		if (actor.CommandSkillModelManager.CommandSkills.Count > 0)
		{
			traitPart.SetActive(value: false);
			activeSkillOperate.SetActive(value: false);
			UpdateSkillParts(actor);
			EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
			if (weaponEquipment != null)
			{
				EquipmentTexture.mainTexture = HelpersGfx.GetEquipmentIconTexture(weaponEquipment);
				if (weaponEquipment.Definition.UseSpecialMaterial)
				{
					Material specialMaterial = HelpersGfx.GetEquipmentResourceEntry(weaponEquipment).specialMaterial;
					EquipmentTexture.material = specialMaterial;
				}
			}
			activeSkillWeaponName.text = HelpersLocalization.GetEquipmentName(weaponEquipment);
		}
		else
		{
			Helpers.GameObjectSetActive(skilArrowContent, value: false);
			Helpers.GameObjectSetActive(activeSkillContent, value: false);
			Helpers.GameObjectSetActive(activeSkillOperate, value: false);
		}
	}

	public void UpdateSkillParts(ActorModel actor)
	{
		commandSkillModelManager = actor.CommandSkillModelManager;
		switch (actor.CommandSkillModelManager.CommandSkills.Count)
		{
		case 1:
		{
			skillPart2.SetActive(value: false);
			BaseCommandSkill baseCommandSkill3 = actor.CommandSkillModelManager.CommandSkills[0];
			CommandSkillDefinition definition3 = baseCommandSkill3.Definition;
			Skill1APNum.text = definition3.APCost.ToString();
			skill1Icon1.spriteName = definition3.Icon;
			toolTip1Name.text = LocalizationManager.GetText(definition3.Name);
			toolTip1Des.text = LocalizationManager.GetText(definition3.Desc);
			toolTip1CDNum.text = LocalizationManager.GetText("UI.Battle.CommandSkill.Des.CD", definition3.Cooldown);
			toolTip1APNum.text = LocalizationManager.GetText("UI.Battle.CommandSkill.Des.AP", definition3.APCost);
			toolTip1Icon1.spriteName = definition3.Icon;
			if (ColorUtility.TryParseHtmlString(definition3.IconColour, out var color3))
			{
				skill1Icon1Corlor1.color = color3;
				toolTip1Icon1Corlor1.color = color3;
			}
			else
			{
				Debug.LogError("Неверный формат цвета");
			}
			if (ColorUtility.TryParseHtmlString(definition3.IconBGColour, out color3))
			{
				skill1Icon1BG1.color = color3;
				toolTip1Icon1BG1.color = color3;
			}
			else
			{
				Debug.LogError("Неверный формат цвета");
			}
			if (baseCommandSkill3.LeftCooldownTurns == 0)
			{
				skill1ShortAction.SetActive(value: false);
				if (baseCommandSkill3.CanExecuteWhereAPEnough())
				{
					skill1Short.SetActive(value: false);
					skill1ShortAP.SetActive(value: false);
				}
				else
				{
					skill1Short.SetActive(value: true);
					skill1ShortAP.SetActive(value: true);
				}
			}
			else
			{
				skill1Short.SetActive(value: true);
				skill1ShortAP.SetActive(value: false);
				skill1ShortAction.SetActive(value: true);
				skill1ShortActionNum.text = baseCommandSkill3.LeftCooldownTurns.ToString();
			}
			break;
		}
		case 2:
		{
			skillPart2.SetActive(value: true);
			BaseCommandSkill baseCommandSkill = actor.CommandSkillModelManager.CommandSkills[0];
			BaseCommandSkill baseCommandSkill2 = actor.CommandSkillModelManager.CommandSkills[1];
			CommandSkillDefinition definition = baseCommandSkill.Definition;
			CommandSkillDefinition definition2 = baseCommandSkill2.Definition;
			Skill1APNum.text = definition.APCost.ToString();
			skill1Icon1.spriteName = definition.Icon;
			toolTip1Name.text = LocalizationManager.GetText(definition.Name);
			toolTip1Des.text = LocalizationManager.GetText(definition.Desc);
			toolTip1CDNum.text = LocalizationManager.GetText("UI.Battle.CommandSkill.Des.CD", definition.Cooldown);
			toolTip1APNum.text = LocalizationManager.GetText("UI.Battle.CommandSkill.Des.AP", definition.APCost);
			toolTip1Icon1.spriteName = definition.Icon;
			if (ColorUtility.TryParseHtmlString(definition.IconColour, out var color))
			{
				skill1Icon1Corlor1.color = color;
				toolTip1Icon1Corlor1.color = color;
			}
			else
			{
				Debug.LogError("Неверный формат цвета");
			}
			if (ColorUtility.TryParseHtmlString(definition.IconBGColour, out color))
			{
				skill1Icon1BG1.color = color;
				toolTip1Icon1BG1.color = color;
			}
			else
			{
				Debug.LogError("Неверный формат цвета");
			}
			if (baseCommandSkill.LeftCooldownTurns == 0)
			{
				skill1ShortAction.SetActive(value: false);
				if (baseCommandSkill.CanExecuteWhereAPEnough())
				{
					skill1Short.SetActive(value: false);
					skill1ShortAP.SetActive(value: false);
				}
				else
				{
					skill1Short.SetActive(value: true);
					skill1ShortAP.SetActive(value: true);
				}
			}
			else
			{
				skill1Short.SetActive(value: true);
				skill1ShortAP.SetActive(value: false);
				skill1ShortAction.SetActive(value: true);
				skill1ShortActionNum.text = baseCommandSkill.LeftCooldownTurns.ToString();
			}
			Skill2APNum.text = definition2.APCost.ToString();
			skill2Icon1.spriteName = definition2.Icon;
			toolTip2Name.text = LocalizationManager.GetText(definition2.Name);
			toolTip2Des.text = LocalizationManager.GetText(definition2.Desc);
			toolTip2CDNum.text = LocalizationManager.GetText("UI.Battle.CommandSkill.Des.CD", definition2.Cooldown);
			toolTip2APNum.text = LocalizationManager.GetText("UI.Battle.CommandSkill.Des.AP", definition2.APCost);
			toolTip2Icon1.spriteName = definition2.Icon;
			if (ColorUtility.TryParseHtmlString(definition2.IconColour, out var color2))
			{
				skill2Icon1Corlor1.color = color2;
				toolTip2Icon1Corlor1.color = color2;
			}
			else
			{
				Debug.LogError("Неверный формат цвета");
			}
			if (ColorUtility.TryParseHtmlString(definition2.IconBGColour, out color2))
			{
				skill2Icon1BG1.color = color2;
				toolTip2Icon1BG1.color = color2;
			}
			else
			{
				Debug.LogError("Неверный формат цвета");
			}
			if (baseCommandSkill2.LeftCooldownTurns == 0)
			{
				skill2ShortAction.SetActive(value: false);
				if (baseCommandSkill2.CanExecuteWhereAPEnough())
				{
					skill2Short.SetActive(value: false);
					skill2ShortAP.SetActive(value: false);
				}
				else
				{
					skill2Short.SetActive(value: true);
					skill2ShortAP.SetActive(value: true);
				}
			}
			else
			{
				skill2Short.SetActive(value: true);
				skill2ShortAP.SetActive(value: false);
				skill2ShortAction.SetActive(value: true);
				skill2ShortActionNum.text = baseCommandSkill2.LeftCooldownTurns.ToString();
			}
			break;
		}
		}
	}

	public void SetupSurvivorPortraits()
	{
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (combat != null)
		{
			for (int i = 0; i < actorInfoContainers.Count; i++)
			{
				actorInfoContainers[i].ShowInfoContainer(show: false);
			}
			List<ActorModel> factionActors = combat.GetFactionActors(Faction.Survivor);
			for (int j = 0; j < factionActors.Count; j++)
			{
				ActorModel survivor = factionActors[j];
				SetupSurvivorInfoContainer(survivor, j);
			}
		}
	}

	public void EnablePlayerInfoContainer()
	{
		Helpers.GameObjectSetActive(playerInfoContainer, value: true);
		Helpers.GameObjectSetActive(SkillContainer, value: true);
		StartCoroutine(UpdateConsumableButtonAnimations());
	}

	private IEnumerator UpdateConsumableButtonAnimations()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		int cooldown = GameManager.Instance.playerModel.Combat.GetCooldown(EquipmentModel.ConsumableType.Grenade);
		int cooldown2 = GameManager.Instance.playerModel.Combat.GetCooldown(EquipmentModel.ConsumableType.MedKit);
		int cooldown3 = GameManager.Instance.playerModel.Combat.GetCooldown(EquipmentModel.ConsumableType.Flare);
		int cooldown4 = GameManager.Instance.playerModel.Combat.GetCooldown(EquipmentModel.ConsumableType.BlastGrenade);
		int num = Mathf.Min(GameManager.Instance.playerModel.Combat.GetCooldown(EquipmentModel.ConsumableType.Gore), Mathf.Min(Mathf.Min(cooldown3, cooldown4), Mathf.Min(cooldown, cooldown2)));
		int tweenToPlay = -1;
		if (previousCooldown == -1)
		{
			tweenToPlay = ((num == 0) ? 5 : 4);
		}
		else
		{
			if (num == 0 && previousCooldown != 0)
			{
				tweenToPlay = 5;
			}
			if (num != 0 && previousCooldown == 0)
			{
				tweenToPlay = 4;
			}
		}
		if (tweenToPlay != -1)
		{
			CooldownAnimationsParent.GetComponentsInChildren<UITweener>(includeInactive: true).ToList().ForEach(delegate(UITweener x)
			{
				if (x.tweenGroup == tweenToPlay)
				{
					x.PlayForward();
					if (previousCooldown == -1)
					{
						x.ResetToEnd();
					}
					else
					{
						x.ResetToBeginning();
					}
				}
			});
		}
		HelpersUI.SetButtonState(consumableButton, (num != 0) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
		previousCooldown = num;
		consumablesCooldownLabel.text = num.ToString();
	}

	public void ShowSurvivorStatusTooltips()
	{
		for (int i = 0; i < actorInfoContainers.Count; i++)
		{
			if (actorInfoContainers[i].gameObject.activeSelf)
			{
				actorInfoContainers[i].statusInfoContainer.ShowHealTooltip();
			}
		}
	}

	public void SetActionMoveIndicator(GridCoordinate coordinate, MoveActionType type, int turnCount, int apCount, GridCoordinate actionFromCoordinate)
	{
		if (moveActionIndicator == null)
		{
			moveActionIndicator = CreateMoveActionIndicator();
		}
		if (moveActionIndicator != null)
		{
			Vector3 position = GridView.Instance.GetPosition(coordinate).ToVector3();
			moveActionIndicator.SetPosition(position);
			if (type == MoveActionType.Cover || type == MoveActionType.Melee || type == MoveActionType.Shoot || type == MoveActionType.Loot || type == MoveActionType.Examine || type == MoveActionType.Interact || type == MoveActionType.Move || type == MoveActionType.MoveSprint || type == MoveActionType.BuddyAid)
			{
				moveActionIndicator.SetAPCount(apCount);
				moveActionIndicator.SetTurnCount(turnCount);
				moveActionIndicator.ShowIndicator(type, coordinate, actionFromCoordinate);
				moveActionIndicator.ShowGroundIndicator(type);
			}
			else
			{
				moveActionIndicator.ShowGroundIndicator(type);
				moveActionIndicator.HideIndicator();
			}
		}
	}

	public void SetCoverMoveIndicator(GridCoordinate coordinate)
	{
		if (coverMoveIndicator == null)
		{
			coverMoveIndicator = CreateCoverMoveIndicator();
		}
		if (coverMoveIndicator != null)
		{
			CombatModel combat = GameManager.Instance.playerModel.Combat;
			List<CoverDirection> coverDirections = combat.GetCoverDirections(coordinate);
			if (coverDirections != null && coverDirections.Count > 0)
			{
				Vector3 position = GridView.Instance.GetPosition(coordinate).ToVector3();
				coverMoveIndicator.SetPosition(position);
				CoverIconState coverState = ((!combat.IsCoverFlanked(coordinate, combat.TurnManager.ActiveActor)) ? CoverIconState.HalfCover : CoverIconState.Flanked);
				coverMoveIndicator.SetCoverDirections(coverDirections, coverState);
				coverMoveIndicator.gameObject.SetActive(value: true);
			}
		}
	}

	public void FreshChargePointActivated(ActorModel actor)
	{
		ActorInfoContainer actorInfoContainer = GetActorInfoContainer(actor);
		if (!(actorInfoContainer != null))
		{
			return;
		}
		List<UISprite> activationPointIcons = actorInfoContainer.chargeEquipment.ActivationPointIcons;
		for (int i = 0; i <= activationPointIcons.Count - 1; i++)
		{
			if (activationPointIcons[i] != null)
			{
				activationPointIcons[i].spriteName = actor.ChargeMeter.GetLevelSpriteName(i);
			}
		}
	}

	public void ResetChargePoint(ActorModel actor)
	{
		try
		{
			ActorInfoContainer actorInfoContainer = GetActorInfoContainer(actor);
			if (!(actorInfoContainer != null))
			{
				return;
			}
			int maxLevel = actor.ChargeMeter.MaxLevel;
			int chargeLevel = actor.ChargeMeter.ChargeLevel;
			List<UISprite> activationPointIcons = actorInfoContainer.chargeEquipment.ActivationPointIcons;
			int count = activationPointIcons.Count;
			for (int i = 0; i < count; i++)
			{
				UISprite uISprite = activationPointIcons[i];
				if (!(uISprite == null))
				{
					bool value = i < maxLevel;
					if (Helpers.GameObjectSetActive(uISprite, value))
					{
						uISprite.spriteName = actor.ChargeMeter.GetLevelSpriteName(i);
					}
				}
			}
		}
		catch (Exception arg)
		{
			Debug.LogError($"ResetChargePoint fail:{arg}");
		}
	}

	public void SetChargeButtonEnabled(ActorModel actor, bool enabled)
	{
		ActorInfoContainer actorInfoContainer = GetActorInfoContainer(actor);
		if (!(actorInfoContainer == null))
		{
			actorInfoContainer.chargeEquipment.SetAvailableIcon(actor, enabled);
			if (actor.FocusModeStateChargeCD)
			{
				actorInfoContainer.chargeEquipment.ActivationPointContainer.SetActive(value: true);
			}
			else
			{
				actorInfoContainer.chargeEquipment.ActivationPointContainer.SetActive(!enabled);
			}
			if (actorInfoContainer.chargeEquipment.EquipmentIcon != null)
			{
				actorInfoContainer.chargeEquipment.EquipmentIcon.gameObject.SetActive(!enabled);
			}
			if (enabled)
			{
				TweenManager.PlayTweenGroup(actorInfoContainer.chargeEquipment.GetAvailableIcon().gameObject, 0);
			}
		}
	}

	public void SetActorPortraitTurnCompleted(ActorModel actor, bool completed)
	{
		ActorInfoContainer actorInfoContainer = GetActorInfoContainer(actor);
		if (actorInfoContainer != null)
		{
			actorInfoContainer.Portrait.shader = (completed ? actorPortraitTurnCompleteShader : actorPortraitNormalShader);
		}
	}

	public void HideMoveActionIndicator()
	{
		if (moveActionIndicator != null)
		{
			moveActionIndicator.HideIndicator();
			moveActionIndicator.HideGroundIndicator();
		}
	}

	public void HideCoverMoveIndicator()
	{
		if (coverMoveIndicator != null)
		{
			coverMoveIndicator.gameObject.SetActive(value: false);
		}
	}

	private void SetupSurvivorInfoContainer(ActorModel survivor, int containerIndex)
	{
		if (containerIndex < 0 || containerIndex >= actorInfoContainers.Count)
		{
			Debug.LogError("Survivor info container index out of bounds!");
		}
		else
		{
			if (survivor.IsDead)
			{
				return;
			}
			ActorInfoContainer actorInfoContainer = actorInfoContainers[containerIndex];
			if (!(actorInfoContainer != null))
			{
				return;
			}
			actorInfoContainer.Actor = survivor;
			actorInfoContainer.SetPortait(survivor);
			if (actorInfoContainer.Portrait != null)
			{
				actorInfoContainer.Portrait.shader = (survivor.TurnComplete ? actorPortraitTurnCompleteShader : actorPortraitNormalShader);
			}
			if (actorInfoContainer.Name != null)
			{
				actorInfoContainer.Name.text = survivor.Name;
			}
			if (actorInfoContainer.Level != null)
			{
				actorInfoContainer.Level.text = survivor.Level.ToString();
			}
			if (actorInfoContainer.ClassIcon != null)
			{
				actorInfoContainer.ClassIcon.ActorDefinition = survivor.Definition;
			}
			if (actorInfoContainer.SelectionSprite != null)
			{
				bool flag = ActiveActor == survivor;
				if (flag)
				{
					if (survivor.SelectedEquipment != null && survivor.SelectedEquipment.IsChargeEquipment)
					{
						if (actorInfoContainer.SelectionSprite != null)
						{
							actorInfoContainer.ChargeActiveSprite.gameObject.SetActive(value: true);
						}
						if (actorInfoContainer.ChargeActiveSprite != null)
						{
							actorInfoContainer.SelectionSprite.gameObject.SetActive(value: false);
						}
					}
					else
					{
						if (actorInfoContainer.SelectionSprite != null)
						{
							actorInfoContainer.ChargeActiveSprite.gameObject.SetActive(value: false);
						}
						if (actorInfoContainer.ChargeActiveSprite != null)
						{
							actorInfoContainer.SelectionSprite.gameObject.SetActive(value: true);
						}
					}
				}
				else
				{
					if (actorInfoContainer.SelectionSprite != null)
					{
						actorInfoContainer.ChargeActiveSprite.gameObject.SetActive(value: false);
					}
					if (actorInfoContainer.ChargeActiveSprite != null)
					{
						actorInfoContainer.SelectionSprite.gameObject.SetActive(value: false);
					}
				}
				if (actorInfoContainer.ChargeActiveSprite == null || !actorInfoContainer.ChargeActiveSprite.gameObject.activeSelf)
				{
					actorInfoContainer.SelectionSprite.gameObject.SetActive(flag);
				}
			}
			EquipmentItemModel weaponEquipment = survivor.GetWeaponEquipment();
			if (weaponEquipment != null)
			{
				actorInfoContainer.weaponInfoContainer.EquipmentTexture.mainTexture = HelpersGfx.GetEquipmentIconTexture(weaponEquipment);
				if (weaponEquipment.Definition.UseSpecialMaterial)
				{
					Material specialMaterial = HelpersGfx.GetEquipmentResourceEntry(weaponEquipment).specialMaterial;
					actorInfoContainer.weaponInfoContainer.EquipmentTexture.material = specialMaterial;
				}
			}
			ShowChargeMeter(survivor, containerIndex, ShowChargeState);
			ShowStatusInfo(survivor, containerIndex);
			actorInfoContainer.ShowInfoContainer(show: true);
			UIEventListener uIEventListener = UIEventListener.Get(actorInfoContainer.gameObject);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnSurvivorPortraitClick));
			UIEventListener uIEventListener2 = UIEventListener.Get(actorInfoContainer.gameObject);
			uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnSurvivorPortraitClick));
		}
	}

	public void SetEnemyTurnHUD(Faction faction)
	{
	}

	public void HideSurvivorTurnHUD()
	{
		Helpers.GameObjectSetActive(playerInfoContainer, value: false);
		Helpers.GameObjectSetActive(SkillContainer, value: false);
		HelpersUI.SetButtonState(consumableButton, UIButtonColor.State.Disabled);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllElementsOfType(UIType.ConsumablesCombatPopup);
		if (!shouldHideConsumableButtons)
		{
			consumableButton.gameObject.SetActive(value: true);
			cancelConsumablesButton.gameObject.SetActive(value: false);
		}
		SetSkipTurnEnabled(enabled: false);
	}

	private void HideConsumableUIElements()
	{
		HelpersUI.SetButtonState(consumableButton, UIButtonColor.State.Disabled);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllElementsOfType(UIType.ConsumablesCombatPopup);
		consumableButton.gameObject.SetActive(value: false);
		cancelConsumablesButton.gameObject.SetActive(value: false);
	}

	public void HideEnemyTurnHUD()
	{
		enemyInfoContainer.SetActive(value: false);
	}

	public void ToggleChargeEquipment(ActorModel actor)
	{
		if (actor.Faction == Faction.Survivor)
		{
			if (actor.SelectedEquipment.IsChargeEquipment)
			{
				UnequipChargeEquipment(actor);
			}
			else
			{
				EquipChargeEquipment(actor);
			}
		}
	}

	public void EquipChargeEquipment(ActorModel actor)
	{
		if (actor.Faction != Faction.Survivor)
		{
			return;
		}
		EquipmentItemModel selectedEquipment = actor.SelectedEquipment;
		EquipmentItemModel chargeEquipment = selectedEquipment.ChargeEquipment;
		if (selectedEquipment == null || chargeEquipment == null || Helpers.ExecuteCommand(new EquipEquipmentCommand(actor, EquipmentCategory.ChargeEquipment)) != TWDModelResult.OK)
		{
			return;
		}
		NotifyAbilitySelected(actor, actor.SelectedEquipment.Ability);
		ActorInfoContainer actorInfoContainer = GetActorInfoContainer(actor);
		if (actorInfoContainer != null)
		{
			if (actorInfoContainer.ChargeActiveSprite != null)
			{
				actorInfoContainer.ChargeActiveSprite.gameObject.SetActive(value: true);
			}
			if (actorInfoContainer.SelectionSprite != null)
			{
				actorInfoContainer.SelectionSprite.gameObject.SetActive(value: false);
			}
			actorInfoContainer.chargeEquipment.SetAvailableIcon(actor, show: false);
			if (actorInfoContainer.chargeEquipment.ActivatedIcon != null)
			{
				actorInfoContainer.chargeEquipment.ActivatedIcon.gameObject.SetActive(value: true);
				TweenManager.PlayTweenGroup(actorInfoContainer.chargeEquipment.ActivatedIcon.gameObject, 0);
			}
			if (actorInfoContainer.chargeEquipment.ActivatedEquipmentInfoLabel != null)
			{
				string localizedText = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Equipment.ChargeLabel." + chargeEquipment.Definition.ID);
				actorInfoContainer.chargeEquipment.ActivatedEquipmentInfoLabel.text = localizedText;
			}
			if (actorInfoContainer.chargeEquipment.AvailableEquipmentInfoLabel != null)
			{
				string localizedText2 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Equipment.ChargeLabel." + chargeEquipment.Definition.ID);
				actorInfoContainer.chargeEquipment.AvailableEquipmentInfoLabel.text = localizedText2;
			}
		}
	}

	public void UnequipChargeEquipment(ActorModel actor)
	{
		if (actor.Faction != Faction.Survivor)
		{
			return;
		}
		EquipmentItemModel selectedEquipment = actor.SelectedEquipment;
		if (selectedEquipment == null || selectedEquipment.IsConsumable || Helpers.ExecuteCommand(new EquipEquipmentCommand(actor, EquipmentCategory.RangeWeapon)) != TWDModelResult.OK)
		{
			return;
		}
		NotifyAbilitySelected(actor, actor.SelectedEquipment.Ability);
		ActorInfoContainer actorInfoContainer = GetActorInfoContainer(actor);
		if (!(actorInfoContainer != null))
		{
			return;
		}
		if (actorInfoContainer.ChargeActiveSprite != null)
		{
			actorInfoContainer.ChargeActiveSprite.gameObject.SetActive(value: false);
		}
		if (actorInfoContainer.SelectionSprite != null)
		{
			actorInfoContainer.SelectionSprite.gameObject.SetActive(value: true);
		}
		actorInfoContainer.chargeEquipment.SetAvailableIcon(actor, actor.ChargeMeter.ChargeAvailable);
		if (actorInfoContainer.chargeEquipment.AvailableEquipmentInfoLabel != null)
		{
			EquipmentItemModel chargeEquipment = actor.GetChargeEquipment();
			if (chargeEquipment != null)
			{
				string localizedText = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Equipment.ChargeLabel." + chargeEquipment.Definition.ID);
				actorInfoContainer.chargeEquipment.AvailableEquipmentInfoLabel.text = localizedText;
			}
		}
		if (actorInfoContainer.chargeEquipment.ActivatedIcon != null)
		{
			actorInfoContainer.chargeEquipment.ActivatedIcon.gameObject.SetActive(value: false);
		}
	}

	public void ResetEquipmentSelectionIndicator(ActorModel actor)
	{
		if (actor.Faction != Faction.Survivor)
		{
			return;
		}
		EquipmentItemModel selectedEquipment = actor.SelectedEquipment;
		ActorInfoContainer actorInfoContainer = GetActorInfoContainer(actor);
		if (selectedEquipment == null || !(actorInfoContainer != null))
		{
			return;
		}
		if (actor == CombatView.Instance.Model.ActiveActor)
		{
			NotifyAbilitySelected(actor, actor.SelectedEquipment.Ability);
		}
		EquipmentResourceEntry equipmentResourceEntry = HelpersGfx.GetEquipmentResourceEntry(actor.SelectedEquipment);
		if (equipmentResourceEntry == null)
		{
			Debug.LogError("Could not load equipment resources prefab " + actor.SelectedEquipment.Definition.ID + "!");
		}
		else
		{
			if (!(actorInfoContainer.chargeEquipment != null))
			{
				return;
			}
			if (actorInfoContainer.chargeEquipment.EquipmentIcon != null)
			{
				actorInfoContainer.chargeEquipment.EquipmentIcon.spriteName = equipmentResourceEntry.IconSprite;
			}
			actorInfoContainer.chargeEquipment.SetAvailableIcon(actor, actor.ChargeMeter.ChargeAvailable);
			if (actorInfoContainer.chargeEquipment.ActivatedIcon != null)
			{
				actorInfoContainer.chargeEquipment.ActivatedIcon.gameObject.SetActive(value: false);
			}
			if (actorInfoContainer.ChargeActiveSprite != null)
			{
				actorInfoContainer.ChargeActiveSprite.gameObject.SetActive(value: false);
				if (actor == CombatView.Instance.Model.ActiveActor)
				{
					actorInfoContainer.SelectionSprite.gameObject.SetActive(value: true);
				}
			}
		}
	}

	public HealthIndicator CreateHealthIndicator(Faction faction)
	{
		GameObject prefab = survivorHealthBarPrefab;
		switch (faction)
		{
		case Faction.Walker:
		case Faction.Environmental:
			prefab = walkerHealthBarPrefab;
			break;
		case Faction.Raider:
			prefab = raiderHealthBarPrefab;
			break;
		}
		return Helpers.InstantiateToParent(prefab, notificationsContainer).GetComponent<HealthIndicator>();
	}

	public ActionIndicator CreateActionIndicator()
	{
		return Helpers.InstantiateToParent(actionIndicatorPrefab, notificationsContainer).GetComponent<ActionIndicator>();
	}

	public TurnCountIndicator CreateTurnCountIndicator()
	{
		return Helpers.InstantiateToParent(turnCountPrefab, notificationsContainer).GetComponent<TurnCountIndicator>();
	}

	public TurnCountIndicator CreateGrenadeTurnCountIndicator()
	{
		return Helpers.InstantiateToParent((grenadeTurnCountPrefab != null) ? grenadeTurnCountPrefab : turnCountPrefab, notificationsContainer).GetComponent<TurnCountIndicator>();
	}

	public MoveActionIndicator CreateMoveActionIndicator()
	{
		return Helpers.InstantiateToParent(moveActionIndicatorPrefab, notificationsContainer).GetComponent<MoveActionIndicator>();
	}

	public CoverIndicator CreateCoverMoveIndicator()
	{
		return Helpers.InstantiateToParent(coverMoveIndicatorPrefab, notificationsContainer).GetComponent<CoverIndicator>();
	}

	public GameObject CreateMoveGroundIndicator()
	{
		return UnityEngine.Object.Instantiate(moveGroundIndicatorPrefab);
	}

	public GameObject CreateActorInfoPopup()
	{
		return Helpers.InstantiateToParent(actorInfoPopupPrefab, notificationsContainer);
	}

	public GameObject CreateInteractiveObjectInfoPopup()
	{
		return Helpers.InstantiateToParent(interactiveObjectInfoPopupPrefab, notificationsContainer);
	}

	public ActorNotificationElement CreateActorNotificationElement(ActorNotificationType type)
	{
		if (type == ActorNotificationType.LootGold || type == ActorNotificationType.LootSilver)
		{
			if (GameManager.Instance.gameEconomyData.ConfigData.HideMissionGoldSilverChest)
			{
				return null;
			}
			if (TutorialView.Instance != null)
			{
				switch (type)
				{
				case ActorNotificationType.LootSilver:
					TutorialView.Instance.StartPart("SilverCrate");
					break;
				case ActorNotificationType.LootGold:
					TutorialView.Instance.StartPart("GoldCrate");
					break;
				}
			}
		}
		GameObject gameObject = null;
		gameObject = type switch
		{
			ActorNotificationType.Generic => actorDefaultNotificationElementPrefab,
			ActorNotificationType.AttackEvent => actorAttackNotificationElementPrefab,
			ActorNotificationType.Damage => actorDamageNotificationElementPrefab,
			ActorNotificationType.DamageFire => actorDamageFireNotificationElementPrefab,
			ActorNotificationType.Heal => ((PrefabResource)UnityUtils.LoadAsset("Combat/HealNotificationElement")).GetPrefab(),
			ActorNotificationType.DamageFlame => actorDamageFireNotificationElementPrefab,
			ActorNotificationType.IgniteBoost => actorActionNotificationPrefab,
			ActorNotificationType.DamagePoison => actorDamagePoisonNotificationElementPrefab,
			ActorNotificationType.DamageGrenade => actorDamageGrenadeFragmentNotificationElementPrefab,
			ActorNotificationType.DamageBleeding => actorDamageBleedingNotificationElementPrefab,
			ActorNotificationType.CurrencySP => actorCurrencySPNotificationElementPrefab,
			ActorNotificationType.CurrencySupplies => actorCurrencySuppliesNotificationElementPrefab,
			ActorNotificationType.CurrencyKey => actorCurrencyKeyNotificationElementPrefab,
			ActorNotificationType.LootGold => actorLootGoldNotificationElementPrefab,
			ActorNotificationType.LootSilver => actorLootSilverNotificationElementPrefab,
			ActorNotificationType.ChargePoint => actorChargePointNotificationElementPrefab,
			ActorNotificationType.ActionNotification => actorActionNotificationPrefab,
			ActorNotificationType.TimedEffectNotification => actorTimedEffectNotificationPrefab,
			ActorNotificationType.BattlePassCurrencyNotification => battlePassCurrencyNotificationPrefab,
			_ => actorDefaultNotificationElementPrefab,
		};
		ActorNotificationElement result = null;
		if (gameObject != null)
		{
			result = Helpers.InstantiateToParent(gameObject, notificationsContainer).GetComponent<ActorNotificationElement>();
		}
		return result;
	}

	public ThreatMeterIndicator CreateThreatMeterIndicator()
	{
		return Helpers.InstantiateToParent(threatMeterPrefab, threatMeterContainer).GetComponent<ThreatMeterIndicator>();
	}

	public CombatTurnPanel CreateTurnPanel()
	{
		return Helpers.InstantiateToParent(GameManager.Instance.playerModel.Combat.IsEndlessBattleMission ? endlessModeTurnPanel : turnPanelPrefab, threatMeterContainer).GetComponent<CombatTurnPanel>();
	}

	public ChargeMeterIndicator CreateChargeMeterIndicator()
	{
		GameObject obj = Helpers.InstantiateToParent(chargeMeterPrefab, chargeMeterContainer);
		ChargeMeterIndicator component = obj.GetComponent<ChargeMeterIndicator>();
		UIEventListener uIEventListener = UIEventListener.Get(obj.GetComponent<UIButton>().gameObject);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnChargeMeterClick));
		return component;
	}

	public WaveNotification CreateWaveNotificationIndicator()
	{
		GameObject obj = Helpers.InstantiateToParent(waveNotificationPrefab, waveNotificaitonContainer);
		WaveNotification component = obj.GetComponent<WaveNotification>();
		obj.SetActive(value: false);
		return component;
	}

	public WaveNotification CreateTimerNotificationIndicator()
	{
		GameObject obj = Helpers.InstantiateToParent(timeNotificationPrefab, waveNotificaitonContainer);
		WaveNotification component = obj.GetComponent<WaveNotification>();
		obj.SetActive(value: false);
		return component;
	}

	public WaveNotification CreateTurnNotificationIndicator()
	{
		GameObject obj = Helpers.InstantiateToParent(turnNotificationPrefab, waveNotificaitonContainer);
		WaveNotification component = obj.GetComponent<WaveNotification>();
		obj.SetActive(value: false);
		return component;
	}

	public FadeOutNotification CreateFadeOutObject()
	{
		GameObject obj = Helpers.InstantiateToParent(fadeOutNotificationPrefab, waveNotificaitonContainer);
		FadeOutNotification component = obj.GetComponent<FadeOutNotification>();
		obj.SetActive(value: false);
		return component;
	}

	public WalkerTurnNotification CreateWalkerTurnNotificationIndicator()
	{
		GameObject obj = Helpers.InstantiateToParent(walkerTurnNotificationPrefab, waveNotificaitonContainer);
		WalkerTurnNotification component = obj.GetComponent<WalkerTurnNotification>();
		obj.SetActive(value: false);
		return component;
	}

	public SpeechBubble CreateSpeechBubble()
	{
		return Helpers.InstantiateToParent(speechBubblePrefab, notificationsContainer).GetComponent<SpeechBubble>();
	}

	private void OnAbilityClick(GameObject button)
	{
	}

	private void OnSurvivorPortraitClick(GameObject infoContainerObject)
	{
		ActorModel actor = infoContainerObject.GetComponent<ActorInfoContainer>().Actor;
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (actor == null || combat == null)
		{
			return;
		}
		OnConsumablesCancelClick();
		ActorView actorView = GameManager.Instance.GetViewForModel(actor) as ActorView;
		if (actorView != null)
		{
			Vector3 position = actorView.gameObject.transform.position;
			PlayerInputManager.Instance.GetHandler<CameraInputHandler>().FocusCameraOnTargetIfFarFromCenter(position);
			if (combat.TurnManager.ActiveActor != actor && !actor.TurnComplete && !actor.IsStruggling && !actor.IsDead)
			{
				Helpers.ExecuteCommand(new SetActiveActorCommand(actor));
				if (SingularityMonoBehaviour<AudioManager>.Instance != null)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/select_survivor");
				}
			}
		}
		List<ActorModel> factionActors = combat.GetFactionActors(Faction.Survivor);
		for (int i = 0; i < factionActors.Count; i++)
		{
			if (factionActors[i].ChargeMeter.ChargeEnabled)
			{
				Helpers.ExecuteCommand(new EnableChargeCommand(factionActors[i], enabled: false));
				UnequipChargeEquipment(factionActors[i]);
			}
		}
		combatSupportsUIView.OnCancelClick();
	}

	private void ShowChargeMeter(ActorModel survivor, int containerIndex, bool show)
	{
		ActorInfoContainer actorInfoContainer = actorInfoContainers[containerIndex];
		EquipmentItemModel chargeEquipment = survivor.GetChargeEquipment();
		if (chargeEquipment != null)
		{
			EquipmentResourceEntry equipmentResourceEntry = HelpersGfx.GetEquipmentResourceEntry(chargeEquipment);
			if (equipmentResourceEntry == null)
			{
				Debug.LogError("Could not load equipment resources prefab " + chargeEquipment.Definition.ID + "!");
			}
			else if (actorInfoContainer.chargeEquipment != null)
			{
				if (actorInfoContainer.ChargeNotAvailableSprite != null)
				{
					actorInfoContainer.ChargeNotAvailableSprite.spriteName = equipmentResourceEntry.IconSprite;
				}
				if (actorInfoContainer.chargeEquipment.EquipmentIcon != null)
				{
					actorInfoContainer.chargeEquipment.EquipmentIcon.spriteName = equipmentResourceEntry.IconSprite;
					actorInfoContainer.chargeEquipment.EquipmentIcon.gameObject.SetActive(!survivor.ChargeMeter.ChargeAvailable && show);
				}
				if (actorInfoContainer.chargeEquipment.GetAvailableIcon() != null)
				{
					actorInfoContainer.chargeEquipment.AvailableEquipmentIcon.spriteName = equipmentResourceEntry.IconSprite;
					actorInfoContainer.chargeEquipment.SetAvailableIcon(survivor, survivor.ChargeMeter.ChargeAvailable && show && (!activateChargeOnChange || survivor != ActiveActor));
					actorInfoContainer.chargeEquipment.ActivatedEquipmentIcon.spriteName = equipmentResourceEntry.IconSprite;
					actorInfoContainer.chargeEquipment.ActivatedIcon.gameObject.SetActive(survivor == ActiveActor && activateChargeOnChange);
					if (survivor == ActiveActor)
					{
						activateChargeOnChange = false;
					}
					if (survivor.FocusModeStateChargeCD)
					{
						actorInfoContainer.chargeEquipment.ActivationPointContainer.SetActive(value: true);
					}
					else
					{
						actorInfoContainer.chargeEquipment.ActivationPointContainer.SetActive(!survivor.ChargeMeter.ChargeAvailable && show);
					}
					UIEventListener uIEventListener = UIEventListener.Get(actorInfoContainer.chargeEquipment.GetAvailableIcon().gameObject);
					uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnChargeActivatedClicked));
					UIEventListener uIEventListener2 = UIEventListener.Get(actorInfoContainer.chargeEquipment.GetAvailableIcon().gameObject);
					uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnChargeActivatedClicked));
					UIEventListener uIEventListener3 = UIEventListener.Get(actorInfoContainer.chargeEquipment.ActivatedIcon.gameObject);
					uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnChargeActivatedClicked));
					UIEventListener uIEventListener4 = UIEventListener.Get(actorInfoContainer.chargeEquipment.ActivatedIcon.gameObject);
					uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnChargeActivatedClicked));
				}
				int maxLevel = survivor.ChargeMeter.MaxLevel;
				_ = survivor.ChargeMeter.ChargeLevel;
				List<UISprite> activationPointIcons = actorInfoContainer.chargeEquipment.ActivationPointIcons;
				int count = activationPointIcons.Count;
				for (int i = 0; i < count; i++)
				{
					UISprite uISprite = activationPointIcons[i];
					if (!(uISprite == null))
					{
						bool value = i < maxLevel;
						if (Helpers.GameObjectSetActive(uISprite, value))
						{
							uISprite.spriteName = survivor.ChargeMeter.GetLevelSpriteName(i);
						}
					}
				}
				if (actorInfoContainer.chargeEquipment.ActivationPointGrid != null)
				{
					actorInfoContainer.chargeEquipment.ActivationPointGrid.enabled = true;
				}
				if (actorInfoContainer.chargeEquipment.ActivatedEquipmentInfoLabel != null)
				{
					string localizedText = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Equipment.ChargeLabel." + chargeEquipment.Definition.ID);
					actorInfoContainer.chargeEquipment.ActivatedEquipmentInfoLabel.text = localizedText;
				}
				if (actorInfoContainer.chargeEquipment.AvailableEquipmentInfoLabel != null)
				{
					string localizedText2 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Equipment.ChargeLabel." + chargeEquipment.Definition.ID);
					actorInfoContainer.chargeEquipment.AvailableEquipmentInfoLabel.text = localizedText2;
				}
			}
		}
		if (actorInfoContainer != null && actorInfoContainer.chargeEquipment != null)
		{
			actorInfoContainer.chargeEquipment.gameObject.SetActive(chargeEquipment != null && show);
		}
		ActorView actorView = GameManager.Instance.GetViewForModel(survivor) as ActorView;
		if (actorView != null && actorView.HealthIndicator != null && actorView.HealthIndicator.ChargePointContainer != null)
		{
			actorView.HealthIndicator.ChargePointContainer.SetActive(show && !survivor.IsAIControlled);
		}
	}

	private void ShowStatusInfo(ActorModel survivor, int containerIndex)
	{
		ActorStatusInfoContainer statusInfoContainer = actorInfoContainers[containerIndex].statusInfoContainer;
		if (statusInfoContainer != null)
		{
			statusInfoContainer.Actor = survivor;
			List<ActorStatusInfo> list = new List<ActorStatusInfo>();
			if (survivor.HasTrait("Bleeding"))
			{
				list.Add(new ActorStatusInfo(ActorStatusType.Bleeding));
			}
			if (survivor.HasTrait("Burning"))
			{
				list.Add(new ActorStatusInfo(ActorStatusType.Burning));
			}
			if (survivor.HasTrait("StaggerActive"))
			{
				list.Add(new ActorStatusInfo(ActorStatusType.StaggerActive));
			}
			if (survivor.IsStunned)
			{
				int turnCount = survivor.ExclusiveTimedEffect.Duration - survivor.ExclusiveTimedEffect.Counter;
				list.Add(new ActorStatusInfo(ActorStatusType.Stunned, turnCount));
			}
			if (survivor.IsRooted)
			{
				int turnCount2 = survivor.ExclusiveTimedEffect.Duration - survivor.ExclusiveTimedEffect.Counter;
				list.Add(new ActorStatusInfo(ActorStatusType.Rooted, turnCount2));
			}
			if (survivor.IsStruggling)
			{
				int turnCount3 = survivor.ExclusiveTimedEffect.Duration - survivor.ExclusiveTimedEffect.Counter;
				list.Add(new ActorStatusInfo(ActorStatusType.Struggling, turnCount3));
			}
			if (survivor.IsReloading)
			{
				int remainingTurnsToReload = survivor.SelectedEquipment.RemainingTurnsToReload;
				list.Add(new ActorStatusInfo(ActorStatusType.Reloading, remainingTurnsToReload));
			}
			if (survivor.IsInvisible)
			{
				TraitEntry trait = survivor.TraitContainer.GetTrait("Gore");
				list.Add(new ActorStatusInfo(ActorStatusType.IsInvisible, (int)trait.TraitDuration));
			}
			if (list.Count > 0)
			{
				ShowChargeMeter(survivor, containerIndex, show: false);
			}
			statusInfoContainer.SetStatusInfo(list);
			UIEventListener uIEventListener = UIEventListener.Get(statusInfoContainer.HealButtonContainer);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnStatusInfoHealClicked));
			UIEventListener uIEventListener2 = UIEventListener.Get(statusInfoContainer.HealButtonContainer);
			uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnStatusInfoHealClicked));
		}
	}

	private void OnStatusInfoHealClicked(GameObject activatedIconObject)
	{
		ActorInfoContainer actorInfoContainer = activatedIconObject.FindComponentInParents<ActorInfoContainer>();
		if (!(actorInfoContainer != null))
		{
			return;
		}
		ActorModel actor = actorInfoContainer.Actor;
		Helpers.ExecuteCommand(new HealActorStatusCommand(actor));
		List<ActorModel> factionActors = GameManager.Instance.playerModel.Combat.GetFactionActors(Faction.Survivor);
		for (int i = 0; i < factionActors.Count; i++)
		{
			if (factionActors[i] == actor)
			{
				ShowStatusInfo(actor, i);
				break;
			}
		}
	}

	private void OnChargeActivatedClicked(GameObject activatedIconObject)
	{
		ActorInfoContainer actorInfoContainer = activatedIconObject.FindComponentInParents<ActorInfoContainer>();
		if (actorInfoContainer == null)
		{
			return;
		}
		ActorModel actor = actorInfoContainer.Actor;
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (actor == null || combat == null || actor.TurnComplete || actor.IsStruggling || actor.IsDead)
		{
			return;
		}
		if (actor.UsedChargeAttackThisTurn)
		{
			TooltipManager.OpenTextBoxWithText(activatedIconObject, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Tooltip.ChargeAttackAlreadyUsed{actorName}", actor.Name));
			return;
		}
		bool flag = combat.TurnManager.ActiveActor != actor;
		if (combat.TurnManager.ActiveActor != actor)
		{
			ActorView actorView = GameManager.Instance.GetViewForModel(actor) as ActorView;
			if (actorView != null)
			{
				Vector3 position = actorView.gameObject.transform.position;
				PlayerInputManager.Instance.GetHandler<CameraInputHandler>().FocusCameraOnTargetIfFarFromCenter(position);
				if (combat.TurnManager.ActiveActor != actor && !actor.TurnComplete && !actor.IsStruggling && !actor.IsDead)
				{
					flag = Helpers.ExecuteCommand(new SetActiveActorCommand(actor)) == TWDModelResult.OK;
					activateChargeOnChange = true;
					if (SingularityMonoBehaviour<AudioManager>.Instance != null)
					{
						SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/select_survivor");
					}
				}
			}
		}
		ChargeMeterModel chargeMeter = actor.ChargeMeter;
		bool flag2 = actor.GetChargeEquipment() != null;
		if (chargeMeter != null && chargeMeter.ChargeAvailable && (!chargeMeter.ChargeEnabled || flag) && flag2)
		{
			Helpers.ExecuteCommand(new EnableChargeCommand(actor, enabled: true));
			EquipChargeEquipment(actor);
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/charge_on");
			}
		}
		else
		{
			Helpers.ExecuteCommand(new EnableChargeCommand(actor, enabled: false));
			UnequipChargeEquipment(actor);
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/charge_off");
			}
		}
		combatSupportsUIView.OnCancelClick();
	}

	private void OnChargeMeterClick(GameObject button)
	{
		if (ActiveActor == null)
		{
			return;
		}
		ChargeMeterModel chargeMeter = ActiveActor.ChargeMeter;
		bool flag = ActiveActor.GetChargeEquipment() != null;
		if (chargeMeter != null && chargeMeter.ChargeAvailable && !chargeMeter.ChargeEnabled && flag)
		{
			Helpers.ExecuteCommand(new EnableChargeCommand(ActiveActor, enabled: true));
			EquipChargeEquipment(ActiveActor);
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/charge_on");
			}
		}
		else
		{
			Helpers.ExecuteCommand(new EnableChargeCommand(ActiveActor, enabled: false));
			UnequipChargeEquipment(ActiveActor);
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/charge_off");
			}
		}
	}

	public void DeselectAbilityButtons()
	{
		if (!ignoreDeselectAbilityButtons)
		{
			ignoreDeselectAbilityButtons = true;
			NotifyAbilitySelected(null, null);
			ignoreDeselectAbilityButtons = false;
		}
	}

	public bool IsMenuButtonActive()
	{
		if (menuButton != null)
		{
			return menuButton.gameObject.activeInHierarchy;
		}
		return false;
	}

	public void ChangeMissionButtonState(bool enabled, ECombatResult currentResult)
	{
		if (menuButton != null)
		{
			menuButton.gameObject.SetActive(!enabled);
		}
		if (completeMissionButton != null)
		{
			completeMissionButton.gameObject.SetActive(enabled);
			switch (currentResult)
			{
			case ECombatResult.Draw:
				completeMissionLabel.text = LocalizationManager.GetText("Combat.Button.DrawOutpost");
				break;
			case ECombatResult.Successful:
				completeMissionLabel.text = LocalizationManager.GetText("Combat.Button.CompleteOutpost");
				break;
			}
		}
	}

	private void OnEnable()
	{
		if (menuButton != null)
		{
			UIEventListener uIEventListener = UIEventListener.Get(menuButton.gameObject);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnGoToMenu));
		}
		if (backupButton != null)
		{
			UIEventListener uIEventListener2 = UIEventListener.Get(backupButton.gameObject);
			uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnBackUp));
		}
		if (completeMissionButton != null)
		{
			UIEventListener uIEventListener3 = UIEventListener.Get(completeMissionButton.gameObject);
			uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnCompleteMission));
		}
		notificationParentWidget = null;
		ShowMenuButtonState = true;
		ShowObjectivesState = true;
		ShowChargeState = true;
		ShowSkipTurnState = true;
		ShowThreatTurnState = true;
		ShowKeysState = true;
		ShowSpeedUpState = true;
		normalSpeed = Mathf.Clamp((float)GameManager.Instance.gameEconomyData.ConfigData.CombatNormalSpeed, 1f, 5f);
		highSpeed = Mathf.Clamp((float)GameManager.Instance.gameEconomyData.ConfigData.CombatHighSpeed, 1f, 5f);
		SetSpeedUpState(GameManager.Instance.Settings.CombatSpeedUp);
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (shouldHideConsumableButtons)
		{
			HideConsumableUIElements();
		}
		else
		{
			ActorModel activeActor = combat.TurnManager.ActiveActor;
			if (activeActor != null && activeActor.SelectedEquipment.IsConsumable)
			{
				ConsumableSelected();
			}
			else
			{
				ConsumableUnselected();
			}
		}
		combat.Changed += OnModelChange;
		endlessModeInfoContainer.SetActive(combat.IsEndlessBattleMission);
		consumablesPlightButton.SetActive(value: false);
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		if (weeklyChallengeModel != null && WeeklyChallengeHelper.IsChallengeOngoing() && weeklyChallengeModel.IsDebufCycles() && combat.MapCategory == MapCategory.Challenge)
		{
			consumablesPlightButton.SetActive(value: true);
		}
		ApocalypseWeeklyChallengeModel weeklyApocalypticChallengeModel = WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel();
		if (weeklyApocalypticChallengeModel != null && WeeklyChallengeHelper.IsChallengeOngoing())
		{
			WeeklyChallengeApocalypseConfig currentCircleDefinition = weeklyApocalypticChallengeModel.CurrentCircleDefinition;
			if (currentCircleDefinition == null || !(currentCircleDefinition.DebuffConfigs?.Count > 0))
			{
				List<WeeklyChallengeApocalypseBuff> weeklyChallengeApocalypseBuffs = weeklyApocalypticChallengeModel.weeklyChallengeApocalypseBuffs;
				if (weeklyChallengeApocalypseBuffs == null || weeklyChallengeApocalypseBuffs.Count <= 0)
				{
					goto IL_02b5;
				}
			}
			if (combat.MapCategory == MapCategory.ApocalypticChallenge)
			{
				consumablesPlightButton.SetActive(value: true);
			}
		}
		goto IL_02b5;
		IL_02b5:
		endlessModeExpertModeTagContainer.SetActive(combat.IsEndlessBattleMission && EndlessModeHelpers.IsEndlessExpertMode());
		objectivesContainer.SetActive(!combat.IsEndlessBattleMission);
		if (combat.IsEndlessBattleMission)
		{
			RefreshEndlessModeScores(playAnimation: false);
			RefreshEndlessModeWaveCount(playAnimation: false);
			if (EndlessModeHelpers.IsEndlessExpertMode())
			{
				HelpersUI.SetContentToLabel(EndlessModeExpertKillScoreMultiplierTag, "X" + EndlessModeHelpers.GetExpertModeFinalScoreMultiplier);
				HelpersUI.SetColor(EndlessModeExpertKillScoreMultiplierTag, ExpertModeColor);
			}
		}
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
		CloseTraitInfoContainer();
	}

	private void OnDisable()
	{
		if (menuButton != null)
		{
			UIEventListener uIEventListener = UIEventListener.Get(menuButton.gameObject);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnGoToMenu));
		}
		if (backupButton != null)
		{
			UIEventListener uIEventListener2 = UIEventListener.Get(backupButton.gameObject);
			uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnBackUp));
		}
		if (completeMissionButton != null)
		{
			UIEventListener uIEventListener3 = UIEventListener.Get(completeMissionButton.gameObject);
			uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnCompleteMission));
		}
		DestroyLocationIndicators();
		for (int i = 0; i < actorInfoContainers.Count; i++)
		{
			actorInfoContainers[i].Clear();
		}
		Time.timeScale = 1f;
		GameManager.Instance.playerModel.Combat.Changed -= OnModelChange;
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		SetupSurvivorPortraits();
	}

	private void OnModelChange(ModelObject model, string changed, object args)
	{
		if (changed == "suggestedInteractionTargetChanged")
		{
			if (shouldHideConsumableButtons)
			{
				HideConsumableUIElements();
			}
			else
			{
				ConsumableUnselected();
			}
		}
		if (changed == "supportExecuted" && model is CombatModel combatModel && args is WalkerMikeSupportExecution)
		{
			for (int i = 0; i < combatModel.GetFactionActors(Faction.Survivor).Count; i++)
			{
				ShowStatusInfo(combatModel.GetFactionActors(Faction.Survivor)[i], i);
			}
		}
		if (changed == "suggestedInteractionTargetChanged")
		{
			if (shouldHideConsumableButtons)
			{
				HideConsumableUIElements();
			}
			else
			{
				ConsumableUnselected();
			}
		}
		if (changed == "UpdateSurvivalGameEvent" || changed == "UpdateShadowedGuardEvent")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(null, delegate
			{
				combatSupportsUIView.UpdateUI();
			}));
		}
		if (!(changed == "turnEnded"))
		{
			return;
		}
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (combat.IsEndlessBattleMission)
		{
			int warningTextSpacesAmount = EndlessModeHelpers.EndlessModeConfig.WarningTextSpacesAmount;
			int count = combat.Walkers.Count;
			int num = ((!EndlessModeHelpers.IsEndlessExpertMode()) ? (GameManager.Instance.playerModel.EndlessModeManager.CurrentEndlessModeCalendarDefinition.MaxWalkerAmount - count) : (GameManager.Instance.playerModel.EndlessModeManager.CurrentEndlessModeCalendarDefinition.MaxWalkerAmountExpert - count));
			if (num <= warningTextSpacesAmount)
			{
				Helpers.GameObjectSetActive(EndlessLoseLabel.gameObject, value: true);
				HelpersUI.SetContentToLabel(EndlessLoseLabel, LocalizationManager.GetText("SurvivalMode_Combat_NoSpace_Altert{Parameter}", num.ToString()));
				EndlessLoseLabel.GetComponent<UILabelShowAndFade>().StartFade();
			}
		}
	}

	public void ShowKeys(bool show, bool tutorial = false)
	{
		ShowKeysState[tutorial ? 1 : 0] = show;
		MissionObjectiveView componentInChildren = objectivesContainer.GetComponentInChildren<MissionObjectiveView>();
		if (componentInChildren != null)
		{
			componentInChildren.ShowKeys(ShowKeysState);
		}
	}

	public void ShowFlee(bool show, bool tutorial = false)
	{
		if (!show || !completeMissionButton.gameObject.activeInHierarchy)
		{
			ShowMenuButtonState[tutorial ? 1 : 0] = show;
			menuButton.gameObject.SetActive(ShowMenuButtonState);
		}
	}

	public void ShowSpeedUp(bool show, bool tutorial = false)
	{
		ShowSpeedUpState[tutorial ? 1 : 0] = show;
		if (speedUpButton != null)
		{
			speedUpButton.gameObject.SetActive(ShowSpeedUpState);
		}
	}

	public void ShowObjectives(bool show, bool tutorial = false)
	{
		ShowObjectivesState[tutorial ? 1 : 0] = show;
		objectivesContainer.SetActive(ShowObjectivesState);
	}

	public void ShowCharge(bool show, bool tutorial = false)
	{
		ShowChargeState[tutorial ? 1 : 0] = show;
		chargeMeterContainer.SetActive(ShowChargeState);
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (combat != null)
		{
			List<ActorModel> factionActors = combat.GetFactionActors(Faction.Survivor);
			for (int i = 0; i < factionActors.Count; i++)
			{
				ActorModel survivor = factionActors[i];
				ShowChargeMeter(survivor, i, ShowChargeState);
			}
		}
	}

	public bool ShowCharge()
	{
		return ShowChargeState;
	}

	public void ShowSkipTurn(bool show, bool tutorial = false)
	{
		if (OfflineManager.IsTutorialDisable || !TutorialView.Instance.InCombatTutorial)
		{
			ShowSkipTurnState[tutorial ? 1 : 0] = show;
			CombatView.Instance.TurnPanel.SurvivorTurnButton.SetActive(ShowSkipTurnState);
		}
		else
		{
			CombatView.Instance.TurnPanel.SurvivorTurnButton.SetActive(value: false);
		}
	}

	public void ShowThreatTurnCount(bool show, bool tutorial = false)
	{
		ShowThreatTurnState[tutorial ? 1 : 0] = show;
		CombatView.Instance.TurnPanel.SetMonsterClosetVisible(ShowThreatTurnState);
	}

	public void OnGoToMenu(GameObject button)
	{
		CombatFleeScreen combatFleeScreen = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatMenuFleeConfirmation) as CombatFleeScreen;
		if (!combatFleeScreen.IsOpen)
		{
			combatFleeScreen.Open();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		}
	}

	public void OnConsumablesClick()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConsumablesCombatPopup).Open();
	}

	public void OnConsumablesPlightClick()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel != null && playerModel.MapContainerModel?.AttackTargetMissionModel?.IsInWeeklyChallenge == true)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConsumablesPlightCombatPopupNormal).Open();
		}
		PlayerModel playerModel2 = GameManager.Instance.playerModel;
		if (playerModel2 != null && playerModel2.MapContainerModel?.AttackTargetMissionModel?.IsInApocalyptiWeeklyChallenge == true)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConsumablesPlightCombatPopup).Open();
		}
	}

	public void OnConsumablesCancelClick()
	{
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		ActorModel activeActor = combat.TurnManager.ActiveActor;
		bool? obj;
		if (activeActor == null)
		{
			obj = null;
		}
		else
		{
			EquipmentItemModel selectedEquipment = activeActor.SelectedEquipment;
			obj = ((selectedEquipment != null) ? new bool?(!selectedEquipment.IsConsumable) : ((bool?)null));
		}
		bool? flag = obj;
		if (flag.HasValue && flag != true && Helpers.ExecuteCommand(new UnequipConsumableCommand(combat.TurnManager.ActiveActor)) == TWDModelResult.OK)
		{
			ConsumableUnselected();
		}
	}

	public void ConsumableSelected()
	{
		consumableButton.gameObject.SetActive(value: false);
		cancelConsumablesButton.SetActive(value: true);
	}

	public void ConsumableUnselected()
	{
		if (!shouldHideConsumableButtons)
		{
			consumableButton.gameObject.SetActive(value: true);
			cancelConsumablesButton.SetActive(value: false);
			StartCoroutine(UpdateConsumableButtonAnimations());
		}
	}

	public void OnCompleteMission(GameObject button)
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		if (!SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatMissionObjectivesPopUp).IsOpen)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatMissionObjectivesPopUp).Open();
		}
	}

	public void ConfirmCompleteMission()
	{
		Helpers.ExecuteCommand(new EndCombatCommand());
	}

	public void SetSkipTurnEnabled(bool enabled)
	{
		endTurnEnabled = enabled;
		ShowSkipTurn(endTurnEnabled);
	}

	public void OnSkipTurn(GameObject button)
	{
		if (Time.realtimeSinceStartup - lastClickedTime < 2f)
		{
			return;
		}
		lastClickedTime = Time.realtimeSinceStartup;
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (combat == null || combat.ActiveActor == null || combat.ActiveActor.Faction != Faction.Survivor || !endTurnEnabled)
		{
			return;
		}
		SetSkipTurnEnabled(enabled: false);
		if (Helpers.ExecuteCommand(new SkipTurnCommand()) != TWDModelResult.OK)
		{
			SetSkipTurnEnabled(enabled: true);
			return;
		}
		for (int i = 0; i < combat.Survivors.Count; i++)
		{
			ActorModel actorModel = combat.Survivors[i];
			UnequipChargeEquipment(actorModel);
			ActorView survivorView = GameManager.Instance.GetViewForModel(actorModel) as ActorView;
			if (!(survivorView != null))
			{
				continue;
			}
			if (!actorModel.IsDead && actorModel.TurnComplete && !actorModel.IsInvisible && actorModel.HadActionPointsAtEndOfTurn && !actorModel.SelectedEquipment.NeedsReloading)
			{
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(actorModel, delegate
				{
					survivorView.SetOverwatchIndicator(enabled: true);
				}));
			}
			else
			{
				survivorView.SetOverwatchIndicator(enabled: false);
			}
		}
	}

	public override void Update()
	{
		base.Update();
		if (CombatView.Instance != null)
		{
			UpdateLocationIndicators();
			float num = (IsSpeedUpEnabled ? highSpeed : normalSpeed);
			if (num != Time.timeScale)
			{
				Time.timeScale = num;
			}
			if (CanBackUp())
			{
				Helpers.GameObjectSetActive(backupButton, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(backupButton, value: false);
			}
		}
		Helpers.GameObjectSetActive(turnsLabel, value: false);
	}

	private void NotifyAbilitySelected(ActorModel actor, AbilityModel ability)
	{
		this.OnAbilitySelected?.Invoke(ability, actor);
	}

	public void OnClickSettings()
	{
		SettingsPopup settingsPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SettingsPopup) as SettingsPopup;
		if (settingsPopup != null)
		{
			settingsPopup.Open();
			settingsPopup.SetHelpNotification(SingularityMonoBehaviour<SDKManager>.Instance.ZendeskManager.UnreadMessageCount);
		}
	}

	public void OnClickSkillLeft()
	{
		skillRightArrow.gameObject.SetActive(value: true);
		skillLeftArrow.gameObject.SetActive(value: false);
		if (ActiveActor != null && ActiveActor.CommandSkillModelManager.CommandSkills.Count > 0)
		{
			Helpers.GameObjectSetActive(skilArrowContent, value: true);
			Helpers.GameObjectSetActive(activeSkillContent, value: true);
		}
	}

	public void OnClickSkillRight()
	{
		skillRightArrow.gameObject.SetActive(value: false);
		skillLeftArrow.gameObject.SetActive(value: true);
		activeSkillContent.SetActive(value: false);
	}

	public void OnClickSkillOk()
	{
		if (!HasActiveSkillTarget())
		{
			return;
		}
		List<ActorModel> factionActors = GameManager.Instance.playerModel.Combat.GetFactionActors(Faction.Survivor);
		if (GoExecuteSkillCommand() == TWDModelResult.OK)
		{
			for (int i = 0; i < factionActors.Count; i++)
			{
				ActorModel actorModel = factionActors[i];
				if (ActiveActor == actorModel)
				{
					UpdateTheActiveSKill(actorModel);
					break;
				}
			}
			SetupSurvivorPortraits();
		}
		SetCommandSkillSelectableStatus(newSet: false);
	}

	public TWDModelResult GoExecuteSkillCommand()
	{
		TWDModelResult tWDModelResult = TWDModelResult.Error;
		GridCoordinate activeSkillTargetCell = GetActiveSkillTargetCell();
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (CurSkillType == SkillType.Survivor)
		{
			return Helpers.ExecuteCommand(new PerformActorCommandSkillCommand(combat.ActiveActor.ModelId, activeSkillTargetCell));
		}
		return Helpers.ExecuteCommand(new PerformCommandSkillCommand(commandSkillModelManager.CommandSkills[selectionSkillIndex].ModelId, activeSkillTargetCell));
	}

	public void OnClickSkillCancel()
	{
		SetCommandSkillSelectableStatus(newSet: false);
	}

	public void OnClickSkillInfo()
	{
		toolTip1.SetActive(skillPart1.activeSelf);
		toolTip2.SetActive(skillPart2.activeSelf);
		traitPart.SetActive(value: true);
	}

	public void OnClickCloseSkillInfo()
	{
		traitPart.SetActive(value: false);
	}

	public void OnClickSkillUse1()
	{
		CurSkillType = SkillType.Weapon;
		selectionSkillIndex = 0;
		SetCommandSkillSelectableStatus(newSet: true);
	}

	public void OnClickSkillUse2()
	{
		CurSkillType = SkillType.Weapon;
		selectionSkillIndex = 1;
		SetCommandSkillSelectableStatus(newSet: true);
	}

	public WaveNotification DisplayWaveNotification(string heading, string body)
	{
		if (waveNotification == null)
		{
			waveNotification = CreateWaveNotificationIndicator();
		}
		waveNotification.Reset();
		waveNotification.SetMessage(heading, body);
		return waveNotification;
	}

	public WaveNotification DisplayWaveNotification(string heading, string body, Color headingColor, Color bodyColor)
	{
		WaveNotification obj = DisplayWaveNotification(heading, body);
		obj.SetColor(headingColor, bodyColor);
		return obj;
	}

	public void RefreshEndlessModeScores(bool playAnimation)
	{
		EndlessModeCombatModel endlessModeCombatModel = GameManager.Instance.playerModel.Combat.EndlessModeCombatModel;
		EndlessModeManagerModel endlessModeManager = GameManager.Instance.playerModel.EndlessModeManager;
		if (endlessModeCombatModel == null || endlessModeManager == null)
		{
			return;
		}
		long currentAttemptScore = EndlessModeHelpers.GetCurrentAttemptScore();
		if (!EndlessModeHelpers.IsEndlessExpertMode() && !isPopUpEndlessNormalModeExit && EndlessModeHelpers.GetAttemptsScoreNormal() < EndlessModeHelpers.GetMaxEndlessNormalModeScore() && EndlessModeHelpers.IsScoreGetMaxReward(currentAttemptScore))
		{
			ConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
			obj.SetContent(LocalizationManager.GetText("SurvivalMode_Normal_ScoreMaxout_Title"), LocalizationManager.GetText("SurvivalMode_Normal_ScoreMaxout_Desc"));
			obj.SetCallbacks(delegate
			{
				CombatFleeScreen combatFleeScreen = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatMenuFleeConfirmation) as CombatFleeScreen;
				if (!combatFleeScreen.IsOpen)
				{
					combatFleeScreen.Open();
					combatFleeScreen.OnFleeButton(null);
					isPopUpEndlessNormalModeExit = false;
				}
			});
			obj.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
			obj.SetCancelButtonLabel(LocalizationManager.GetText("Button.Cancel"));
			obj.Open();
			isPopUpEndlessNormalModeExit = true;
		}
		FixedPoint currentKillScoreMultiplier = endlessModeCombatModel.CurrentKillScoreMultiplier;
		Color color = ((endlessModeManager.EndlessModeGameModeType == EndlessModeGameModeType.Expert) ? ExpertModeColor : NormalModeColor);
		HelpersUI.SetContentToLabel(EndlessModeKillScoreMultiplier, EndlessModeHelpers.GetFormattedScoreMultiplier(currentKillScoreMultiplier));
		HelpersUI.SetContentToLabel(EndlessModeScoreLabel, EndlessModeHelpers.GetFormattedScoreText(currentAttemptScore));
		HelpersUI.SetColor(EndlessModeScoreLabel, color);
		if (playAnimation)
		{
			TweenManager.PlayTweenGroup(endlessModeInfoContainer, 5);
		}
	}

	public void RefreshEndlessModeWaveCount(bool playAnimation)
	{
		EndlessModeCombatModel endlessModeCombatModel = GameManager.Instance.playerModel.Combat.EndlessModeCombatModel;
		if (endlessModeCombatModel != null)
		{
			HelpersUI.SetContentToLabel(EndlessModeWaveCountLabel, endlessModeCombatModel.GetCurrentOverAllWaveIndex.ToString());
		}
		if (playAnimation)
		{
			TweenManager.PlayTweenGroup(endlessModeInfoContainer, 6);
		}
	}

	private GameObject GetIndicatorPrefab(IndicatorType indicatorType, bool right)
	{
		for (int i = 0; i < IndicatorPrefabs.Count; i++)
		{
			if (IndicatorPrefabs[i].IndicatorType == indicatorType)
			{
				if (!right)
				{
					return IndicatorPrefabs[i].IndicatorPrefabLeft;
				}
				return IndicatorPrefabs[i].IndicatorPrefabRight;
			}
		}
		return null;
	}

	public void ShowLocationIndicator(GameObject gameObjectToIndicate, IndicatorType indicatorType)
	{
		if (!(notificationsContainer == null) && !locationIndicators.ContainsKey(gameObjectToIndicate))
		{
			IndicatorInstanceInfo value = new IndicatorInstanceInfo(indicatorType);
			locationIndicators.Add(gameObjectToIndicate, value);
		}
	}

	public void HideLocationIndicator(GameObject gameObjectToIndicate)
	{
		if (locationIndicators.ContainsKey(gameObjectToIndicate))
		{
			UnityEngine.Object.Destroy(locationIndicators[gameObjectToIndicate].IndicatorInstance);
			locationIndicators.Remove(gameObjectToIndicate);
		}
	}

	private void CreateIndicatorInstance(IndicatorInstanceInfo info, bool rightSide)
	{
		DestroyIndicatorInstance(info);
		info.IndicatorInstance = Helpers.InstantiateToParent(GetIndicatorPrefab(info.IndicatorType, rightSide), notificationsContainer);
		info.InstanceOnRightSide = rightSide;
	}

	private void DestroyIndicatorInstance(IndicatorInstanceInfo info)
	{
		if (info.IndicatorInstance != null)
		{
			UnityEngine.Object.Destroy(info.IndicatorInstance);
		}
	}

	private void DestroyLocationIndicators()
	{
		foreach (IndicatorInstanceInfo value in locationIndicators.Values)
		{
			UnityEngine.Object.Destroy(value.IndicatorInstance);
		}
		locationIndicators.Clear();
		notificationParentWidget = null;
	}

	private UIWidget FindParentWidget(GameObject go)
	{
		GameObject gameObject = go;
		UIWidget uIWidget = null;
		while (uIWidget == null && go.transform.parent != null)
		{
			gameObject = gameObject.transform.parent.gameObject;
			uIWidget = gameObject.GetComponent<UIWidget>();
		}
		return uIWidget;
	}

	private void UpdateLocationIndicators()
	{
		if (Camera.main == null || locationIndicators == null)
		{
			return;
		}
		foreach (KeyValuePair<GameObject, IndicatorInstanceInfo> locationIndicator in locationIndicators)
		{
			GameObject key = locationIndicator.Key;
			IndicatorInstanceInfo value = locationIndicator.Value;
			if (!(key != null) || value == null)
			{
				continue;
			}
			Vector3 vector = Camera.main.WorldToScreenPoint(key.transform.position);
			bool flag = vector.x > (float)Camera.main.pixelWidth || vector.y > (float)Camera.main.pixelHeight || vector.y < 0f;
			bool flag2 = vector.x < 0f || vector.y > (float)Camera.main.pixelHeight || vector.y < 0f;
			GameObject gameObject = value.IndicatorInstance;
			if (gameObject == null)
			{
				if (flag)
				{
					CreateIndicatorInstance(value, rightSide: true);
				}
				else if (flag2)
				{
					CreateIndicatorInstance(value, rightSide: false);
				}
				gameObject = value.IndicatorInstance;
			}
			else if (!flag && value.InstanceOnRightSide)
			{
				DestroyIndicatorInstance(value);
				gameObject = null;
			}
			else if (!flag2 && !value.InstanceOnRightSide)
			{
				DestroyIndicatorInstance(value);
				gameObject = null;
			}
			if (!(gameObject == null))
			{
				if (notificationParentWidget == null)
				{
					notificationParentWidget = FindParentWidget(gameObject);
				}
				if (notificationParentWidget != null)
				{
					float num = notificationParentWidget.width;
					float num2 = notificationParentWidget.height;
					Vector3 vector2 = new Vector3(num * 0.5f, num2 * 0.5f, 0f);
					Vector2 vector3 = vector - vector2;
					Vector3 vector4 = vector - vector2;
					Vector3 localPosition = new Vector3(Mathf.Clamp(vector4.x, (0f - num) * 0.5f, num * 0.5f), Mathf.Clamp(vector4.y, (0f - num2) * 0.5f, num2 * 0.5f), vector4.z);
					gameObject.transform.localPosition = localPosition;
					float z = (0f - Mathf.Atan2(0f - vector3.y, vector3.x)) / MathF.PI * 180f + (value.InstanceOnRightSide ? 0f : 180f);
					gameObject.transform.eulerAngles = new Vector3(0f, 0f, z);
				}
			}
		}
	}

	public void OnBackUp(GameObject button)
	{
		if (!CanBackUp())
		{
			return;
		}
		ConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
		obj.SetContent(LocalizationManager.GetText("Popup.CombatRevert.Confirm.Title"), LocalizationManager.GetText("Popup.CombatRevert.Confirm.Description"));
		obj.SetCallbacks(delegate
		{
			int turn = 2;
			if (Helpers.ExecuteCommand(new CombatBackUpCommand(turn)
			{
				Turn = turn
			}) == TWDModelResult.OK)
			{
				PlayerInputManager.Instance.GetHandler<ObjectInfoInputHandler>()?.ClearInfoPopups();
				GameManager.Instance.Backup();
				OpenBackUpCover();
			}
		});
		obj.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
		obj.SetCancelButtonLabel(LocalizationManager.GetText("Button.Cancel"));
		obj.Open();
	}

	public bool CanBackUp()
	{
		if (GameManager.Instance == null)
		{
			return false;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel == null)
		{
			return false;
		}
		CombatModel combat = playerModel.Combat;
		if (combat == null)
		{
			return false;
		}
		if (playerModel.SubscriptionManager == null)
		{
			return false;
		}
		if (!playerModel.SubscriptionManager.IsSubscriptionActive)
		{
			return false;
		}
		if (combat == null || combat.MissionCompleted)
		{
			return false;
		}
		if (combat.BackUpCount > 0)
		{
			return false;
		}
		if (!Helpers.IsBackUpPassConfig(combat.CurrentMissionId))
		{
			return false;
		}
		if (combat.IsGuildBattleMission)
		{
			return false;
		}
		if (endTurnEnabled && combat.TurnManager.TurnCount > 0)
		{
			return true;
		}
		return false;
	}

	private void OpenBackUpCover()
	{
		CombatBackUpCover combatBackUpCover = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BackUpCoverPopup) as CombatBackUpCover;
		if (!combatBackUpCover.IsOpen)
		{
			combatBackUpCover.Open();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		}
	}

	private void SetCommandSkillSelectableStatus(bool newSet)
	{
		ClearActiveSkillTarget();
		CancelShadowedGuardSkill();
		IsSkillSelectableStatus = newSet;
		CanSelectSkill = newSet;
		SetCommandSkillSelectableIndicator(newSet);
		SetMainContainer_OnlyNotifi(newSet);
		Helpers.GameObjectSetActive(activeSkillContent, value: false);
		Helpers.GameObjectSetActive(skilArrowContent, value: false);
		if (ActiveActor.CommandSkillModelManager.CommandSkills.Count > 0 && !newSet)
		{
			Helpers.GameObjectSetActive(activeSkillContent, value: true);
			Helpers.GameObjectSetActive(skilArrowContent, value: true);
		}
		Helpers.GameObjectSetActive(activeSkillOperate, newSet);
		SetInfoTxt(newSet);
	}

	private void SetCommandSkillSelectableIndicator(bool newSet)
	{
		CombatModel combat = GameManager.Instance.playerModel.manager.CombatModel;
		BaseCommandSkill curBaseCommandSkill = GetCurBaseCommandSkill();
		List<ActorModel> list = new List<ActorModel>();
		if (CurSkillType == SkillType.Survivor)
		{
			if (curBaseCommandSkill.Definition.TargetType.Contains(CommandSkillTargetType.Enemy))
			{
				list.AddRange(combat.ActiveActor.GridCoordinate.GetEnemiesByDistanceAndFaction(combat.ActiveActor.GridCoordinate, combat, curBaseCommandSkill.Definition.Range, combat.ActiveActor.Faction));
				list.RemoveAll((ActorModel t) => t.Faction != Faction.Walker && t.Faction != Faction.Survivor && t.Faction != Faction.Raider);
				list.RemoveAll((ActorModel t) => !combat.IsGridCellVisibleByAnySurvivor(t.GridCoordinate));
			}
			if (curBaseCommandSkill.Definition.TargetType.Contains(CommandSkillTargetType.ActorItself))
			{
				list.Add(combat.Survivors[0]);
			}
			if (curBaseCommandSkill.Definition.TargetType.Contains(CommandSkillTargetType.Friendly))
			{
				list.AddRange(combat.Survivors.Where((ActorModel t) => t != combat.Survivors[0]));
			}
		}
		else
		{
			list = combat.Survivors.Models;
		}
		foreach (ActorModel item in list)
		{
			ActorView actorView = GameManager.Instance.GetViewForModel(item) as ActorView;
			GridCoordinate gridCoordinate = item.GridCoordinate;
			bool flag = curBaseCommandSkill.CanExecute(gridCoordinate);
			if (newSet && flag)
			{
				actorView.CreateCommandSkillSelectableIndicator();
			}
			else
			{
				actorView.DestroyCommandSkillSelectableIndicator();
			}
		}
		RefreshCommandSkillGridHighlights(newSet, ActiveSkillGridCell);
	}

	public BaseCommandSkill GetCurBaseCommandSkill()
	{
		CombatModel combatModel = GameManager.Instance.playerModel.manager.CombatModel;
		if (CurSkillType == SkillType.Survivor)
		{
			return combatModel.ActiveActor.CommandSkillModelManager.ActorCommandSkill;
		}
		return commandSkillModelManager.CommandSkills[selectionSkillIndex];
	}

	public void SetActiveSkillGridCell(GridCoordinate gridCell)
	{
		if (gridCell.IsValid)
		{
			BaseCommandSkill curBaseCommandSkill = GetCurBaseCommandSkill();
			CombatModel combat = GameManager.Instance.playerModel.Combat;
			GridCoordinate sourceCell = CommandSkillGridHelpers.GetSourceCell(curBaseCommandSkill, ActiveActor);
			if (combat != null && !CommandSkillGridHelpers.IsGridCellOnPlayableMap(combat, gridCell))
			{
				RefreshCommandSkillGridHighlightsIfActive();
				return;
			}
			if (combat != null && !CommandSkillGridHelpers.IsGridCellVisibleFrom(combat, sourceCell, gridCell))
			{
				RefreshCommandSkillGridHighlightsIfActive();
				return;
			}
			if (!curBaseCommandSkill.CanExecute(gridCell))
			{
				RefreshCommandSkillGridHighlightsIfActive();
				return;
			}
			if (gridCell == ActiveSkillGridCell)
			{
				OnClickSkillOk();
				return;
			}
			ClearActiveSkillTarget();
			ActiveSkillGridCell = gridCell;
			CanSelectSkill = true;
			RefreshCommandSkillGridHighlights(show: true, gridCell);
		}
	}

	public void SetActiveSkillActor(ActorModel actor)
	{
		if (actor == null)
		{
			return;
		}
		ActorView actorView = GameManager.Instance.GetViewForModel(actor) as ActorView;
		if (actorView == null)
		{
			return;
		}
		BaseCommandSkill curBaseCommandSkill = GetCurBaseCommandSkill();
		GridCoordinate gridCoordinate = actor.GridCoordinate;
		if (curBaseCommandSkill.CanExecute(gridCoordinate))
		{
			if (actor == ActiveSkillActor)
			{
				OnClickSkillOk();
				return;
			}
			ClearActiveSkillTarget();
			actorView.DestroyCommandSkillSelectedIndicator();
			actorView.CreateCommandSkillSelectedIndicator();
			ActiveSkillActor = actor;
			CanSelectSkill = true;
		}
	}

	private bool HasActiveSkillTarget()
	{
		if (ActiveSkillActor == null)
		{
			return ActiveSkillGridCell.IsValid;
		}
		return true;
	}

	private GridCoordinate GetActiveSkillTargetCell()
	{
		if (ActiveSkillGridCell.IsValid)
		{
			return ActiveSkillGridCell;
		}
		if (ActiveSkillActor == null)
		{
			return GridCoordinate.Invalid;
		}
		return ActiveSkillActor.GridCoordinate;
	}

	private bool IsGridTargetCommandSkill(BaseCommandSkill baseCommandSkill)
	{
		if (baseCommandSkill?.Definition?.TargetType != null)
		{
			return baseCommandSkill.Definition.TargetType.Contains(CommandSkillTargetType.Grid);
		}
		return false;
	}

	private void TryAddCommandSkillGridHighlight(CombatModel combat, BaseCommandSkill baseCommandSkill, GridCoordinate coord, GridCoordinate selectedCell, List<GridCoordinate> coordinates, List<Color> colors, List<int> indices)
	{
		GridCoordinate sourceCell = CommandSkillGridHelpers.GetSourceCell(baseCommandSkill, ActiveActor);
		if (CommandSkillGridHelpers.IsGridCellVisibleFrom(combat, sourceCell, coord) && baseCommandSkill.CanExecute(coord))
		{
			coordinates.Add(coord);
			bool flag = selectedCell.IsValid && selectedCell == coord;
			colors.Add(flag ? Color.yellow : Color.green);
			indices.Add(flag ? 1 : 0);
		}
	}

	public void RefreshCommandSkillGridHighlightsIfActive()
	{
		if (IsSkillSelectableStatus)
		{
			RefreshCommandSkillGridHighlights(show: true);
		}
	}

	private void RefreshCommandSkillGridHighlights(bool show)
	{
		RefreshCommandSkillGridHighlights(show, ActiveSkillGridCell);
	}

	private void RefreshCommandSkillGridHighlights(bool show, GridCoordinate selectedCell)
	{
		if (!show)
		{
			GridView.Instance.ClearHighlights();
			return;
		}
		BaseCommandSkill curBaseCommandSkill = GetCurBaseCommandSkill();
		if (!IsGridTargetCommandSkill(curBaseCommandSkill))
		{
			return;
		}
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (combat == null)
		{
			return;
		}
		List<GridCoordinate> list = new List<GridCoordinate>();
		List<Color> colors = new List<Color>();
		List<int> indices = new List<int>();
		int range = curBaseCommandSkill.Definition.Range;
		GridCoordinate gridCoordinate = (curBaseCommandSkill.OwnActorModel ?? ActiveActor).GridCoordinate;
		GridField<FixedPoint> playableField = CommandSkillGridHelpers.CreatePlayableMapDistanceField(combat);
		foreach (GridCoordinate coordinate in combat.Grid.Coordinates)
		{
			if ((range < 0 || gridCoordinate.ChebyshevDistance(coordinate) <= range) && CommandSkillGridHelpers.IsGridCellOnPlayableMap(combat, coordinate, playableField))
			{
				TryAddCommandSkillGridHighlight(combat, curBaseCommandSkill, coordinate, selectedCell, list, colors, indices);
			}
		}
		if (list.Count > 0)
		{
			GridView.Instance.HighlightCoordinates(list, colors, indices);
		}
		else
		{
			GridView.Instance.ClearHighlights();
		}
	}

	private void ClearActiveSkillTarget()
	{
		ClearActiveSkillActor();
		ActiveSkillGridCell = GridCoordinate.Invalid;
		GridView.Instance.ClearHighlights();
	}

	private void ClearActiveSkillActor()
	{
		if (ActiveSkillActor != null)
		{
			(GameManager.Instance.GetViewForModel(ActiveSkillActor) as ActorView).DestroyCommandSkillSelectedIndicator();
		}
		ActiveSkillActor = null;
	}

	public void SetShadowedGuardSkill()
	{
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (!combat.Survivors[0].HasAnyLevelTrait("LeaderBuffShadowedGuard"))
		{
			return;
		}
		List<ActorModel> factionActors = GameManager.Instance.playerModel.Combat.GetFactionActors(Faction.Survivor);
		for (int i = 0; i < factionActors.Count; i++)
		{
			ActorView actorView = GameManager.Instance.GetViewForModel(factionActors[i]) as ActorView;
			if (actorView == null)
			{
				return;
			}
			BaseCommandSkill curBaseCommandSkill = GetCurBaseCommandSkill();
			GridCoordinate gridCoordinate = factionActors[i].GridCoordinate;
			if (!curBaseCommandSkill.CanExecute(gridCoordinate))
			{
				return;
			}
			actorView.DestroyCommandSkillSelectedIndicator();
			actorView.CreateCommandSkillSelectedIndicator();
			ActiveSkillActors.Add(factionActors[i]);
		}
		ActiveSkillActor = combat.Survivors[0];
		CanSelectSkill = false;
		HelpersUI.SetContentToLabel(activeSkillOperate.transform.Find("Info").GetComponentInChildren<UILabel>(), LocalizationManager.GetText("BattleNotice_CommandSkill_Lydia_Leader"));
	}

	private void CancelShadowedGuardSkill()
	{
		if (ActiveSkillActors.Count > 0)
		{
			foreach (ActorModel activeSkillActor in ActiveSkillActors)
			{
				(GameManager.Instance.GetViewForModel(activeSkillActor) as ActorView).DestroyCommandSkillSelectedIndicator();
			}
		}
		ActiveSkillActors.Clear();
		CanSelectSkill = true;
	}

	private void SetMainContainer_OnlyNotifi(bool active)
	{
		CombatMainContainer component = MainContainer.GetComponent<CombatMainContainer>();
		if (component != null)
		{
			component.SetOnlyNotifi(active);
		}
	}

	private void SetTraitInfoContainer(bool show)
	{
		Helpers.GameObjectSetActive(traitInfoContainer, show);
	}

	public void CloseTraitInfoContainer()
	{
		SetTraitInfoContainer(show: false);
	}

	public void OpenTraitInfoContainer(ActorModel actor)
	{
		SetTraitInfoContainer(show: true);
		traitInfoContainer.UpdateUI(actor);
	}

	public void OnClickSurvivorSkillUse()
	{
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (combat == null || !CheckSkillCD())
		{
			return;
		}
		BaseCommandSkill actorCommandSkill = ActiveActor.CommandSkillModelManager.ActorCommandSkill;
		if (actorCommandSkill.Definition.SkillFunc == CommandSkillFuncType.Charge && actorCommandSkill.Type == CommandSkillType.CommandSkillShadowedGuard)
		{
			FixedPoint value = 0L;
			combat.manager.Player.AbilityManager.VisitParameter("LeaderBuffShadowedGuard_Charge_MaxNum", ref value, ActiveActor);
			if (ActiveActor.ChargeNum < value)
			{
				return;
			}
		}
		Helpers.ExecuteCommand(new EnableChargeCommand(ActiveActor, enabled: false));
		UnequipChargeEquipment(ActiveActor);
		CurSkillType = SkillType.Survivor;
		SetCommandSkillSelectableStatus(newSet: true);
		SetShadowedGuardSkill();
	}

	private void SetInfoTxt(bool show)
	{
		Transform transform = activeSkillOperate.transform.Find("Info");
		if (transform == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(transform.gameObject, value: false);
		if (show && ActiveActor != null && ActiveActor.CommandSkillModelManager != null && ActiveActor.CommandSkillModelManager.ActorCommandSkill != null && ActiveActor.CommandSkillModelManager.ActorCommandSkill.Definition != null)
		{
			string text = LocalizationManager.GetText(ActiveActor.CommandSkillModelManager.ActorCommandSkill.Definition.UseDesc);
			if (!string.IsNullOrEmpty(text))
			{
				transform.GetComponentInChildren<UILabel>().text = text;
				Helpers.GameObjectSetActive(transform.gameObject, value: true);
			}
		}
	}

	public void OnSupportClicked(int actorSlotIndex)
	{
		combatSupportsUIView.OnSupportClick(actorSlotIndex);
		Helpers.ExecuteCommand(new EnableChargeCommand(ActiveActor, enabled: false));
		UnequipChargeEquipment(ActiveActor);
	}

	public void ShowDebuffDamagePerRoundTips()
	{
		Helpers.GameObjectSetActive(DebuffDamagePerRoundTips, value: true);
		DebuffDamagePerRoundTips.GetComponent<UILabelShowAndFade>().StartFade();
	}

	public bool CheckSkillCD()
	{
		CommandSkillType type = ActiveActor.CommandSkillModelManager.ActorCommandSkill.Definition.Type;
		switch (type)
		{
		case CommandSkillType.CommandSkillSurvivalGame:
			if (ActiveActor.SurvivalGameLeftCD > 0)
			{
				return false;
			}
			break;
		case CommandSkillType.CommandSkillShadowedGuard:
		{
			ShadowedGuardSkill shadowedGuardSkill = ActiveActor.CommandSkillModelManager?.GetActorCommandSkill<ShadowedGuardSkill>(type);
			if (shadowedGuardSkill != null && shadowedGuardSkill.LeftCooldownTurns > 0)
			{
				return false;
			}
			break;
		}
		}
		return true;
	}
}
