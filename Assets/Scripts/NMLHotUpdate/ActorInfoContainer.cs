using BaseModel;
using TWDModel;
using UnityEngine;

public class ActorInfoContainer : HUDElement
{
	[Tooltip("Actor portrait")]
	public UITexture Portrait;

	[Tooltip("Actor class indicator")]
	public SurvivorClassIcon ClassIcon;

	[Tooltip("Actor's name label.")]
	public UILabel Name;

	[Tooltip("Actor's level label.")]
	public UILabel Level;

	[Tooltip("Actor selection sprite")]
	public UISprite SelectionSprite;

	[Tooltip("Glow effect sprite when charge equipment is selected")]
	public UISprite ChargeActiveSprite;

	[Tooltip("Icon of the charge ability shown when charge is not yet available")]
	public UISprite ChargeNotAvailableSprite;

	[Tooltip("Info container of the normal weapon")]
	public EquipmentInfoContainer weaponInfoContainer;

	[Tooltip("Info container of the charge equipment")]
	public EquipmentInfoContainer chargeEquipment;

	[Tooltip("Info container of the status info")]
	public ActorStatusInfoContainer statusInfoContainer;

	[SerializeField]
	private PortraitHealthBar portraitHealthBar;

	[Tooltip("Current actor ShieldHP progress bar.")]
	public UIProgressBar ShieldHPBar;

	public UILabel ShieldLable;

	[SerializeField]
	private UIGridExtended traitBuffsContainer;

	[SerializeField]
	private UISprite FocusModeIcon;

	[SerializeField]
	private UISprite OverloadIcon;

	[SerializeField]
	private SurvivorCardTraitBuffsInfoList buffsInfoList;

	private DoubleBooleanState showInfoContainer;

	private ActorModel actor;

	public ActorModel Actor
	{
		get
		{
			return actor;
		}
		set
		{
			if (actor != value)
			{
				if (actor != null)
				{
					actor.Changed -= OnActorModelChanged;
				}
				actor = value;
				portraitHealthBar.SetActorModel(actor);
				buffsInfoList.InitData(actor);
				buffsInfoList.UpdateUI();
				traitBuffsContainer.Reposition();
				if (actor != null)
				{
					showInfoContainer.SecondState = actor.UserCanControl;
				}
				if (actor != null)
				{
					actor.Changed += OnActorModelChanged;
				}
			}
		}
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	public void Clear()
	{
		Actor = null;
		showInfoContainer.FirstState = false;
		showInfoContainer.SecondState = false;
	}

	public void SetPortait(ActorModel actor)
	{
		PortraitManager instance = PortraitManager.Instance;
		if (instance == null)
		{
			return;
		}
		PortraitRenderSource portraitRenderSource = PortraitRenderSource.fromActorModel(actor);
		Texture portrait = instance.GetPortrait(portraitRenderSource);
		if (portrait == null)
		{
			ModularCharacter prefabForActor = ActorView.GetPrefabForActor(actor);
			if (portraitRenderSource != null)
			{
				PortraitManager.Instance.CreatePortrait(portraitRenderSource, prefabForActor, OnPortraitRendered);
			}
		}
		else
		{
			Portrait.mainTexture = portrait;
		}
		UpdateShieldHPBar();
		FreshFocusModeIcon();
	}

	private void OnPortraitRendered(IPortraitRenderSource info)
	{
		if (actor != null && base.gameObject != null && Portrait != null && info.ActorDefinitionId == actor.ActorDefinitionID)
		{
			Portrait.mainTexture = PortraitManager.Instance.GetPortrait(info);
		}
	}

	public void ShowInfoContainer(bool show)
	{
		showInfoContainer.FirstState = show;
		base.gameObject.SetActive(showInfoContainer);
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "BackupEndEvent")
		{
			FreshFocusModeIcon();
			FreshOverloadIcon();
			traitBuffsContainer.Reposition();
		}
	}

	private void OnActorModelChanged(ModelObject m, string changed, object args)
	{
		if (this == null || actor == null)
		{
			return;
		}
		switch (changed)
		{
		case "actorUserCanControlChanged":
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(null, delegate
			{
				_ = showInfoContainer;
				if (actor != null)
				{
					showInfoContainer.SecondState = actor.UserCanControl;
					if (!actor.UserCanControl && actor.IsStruggling)
					{
						showInfoContainer.SecondState = true;
					}
					base.gameObject.SetActive(showInfoContainer);
				}
			}));
			break;
		case "ShieldChanged":
			UpdateShieldHPBar();
			break;
		case "SkillIncreaseAttackChanged":
			buffsInfoList.UpdateUI();
			traitBuffsContainer.Reposition();
			break;
		case "GodWarSkillChange":
			buffsInfoList.UpdateUI();
			traitBuffsContainer.Reposition();
			break;
		case "FortunaMainTraitChange":
			buffsInfoList.UpdateUI();
			traitBuffsContainer.Reposition();
			break;
		case "TurnCountChangedEvent":
			FreshFocusModeIcon();
			FreshOverloadIcon();
			buffsInfoList.UpdateUI();
			traitBuffsContainer.Reposition();
			break;
		case "JoinFocusMode":
		case "AbortFocusMode":
		case "ShowsFocusModeBTN":
		case "HideFocusModeBTN":
			FreshFocusModeIcon();
			traitBuffsContainer.Reposition();
			break;
		case "OverLoadEvent":
			FreshOverloadIcon();
			traitBuffsContainer.Reposition();
			break;
		}
	}

	private void UpdateShieldHPBar()
	{
		if (Actor != null)
		{
			if (Actor.MaxShieldHitPoints <= 0)
			{
				Helpers.GameObjectSetActive(ShieldHPBar, value: false);
				Helpers.GameObjectSetActive(ShieldLable, value: false);
			}
			else if (ShieldHPBar != null && ShieldLable != null)
			{
				Helpers.GameObjectSetActive(ShieldHPBar, value: true);
				Helpers.GameObjectSetActive(ShieldLable, value: true);
				float value = (float)Actor.ShieldHitPoints / (float)Actor.MaxShieldHitPoints;
				ShieldHPBar.value = value;
				ShieldLable.text = Actor.ShieldHitPoints + "/" + Actor.MaxShieldHitPoints;
			}
		}
	}

	private void FreshFocusModeIcon()
	{
		if (Actor == null)
		{
			Helpers.GameObjectSetActive(FocusModeIcon, value: false);
			return;
		}
		Helpers.GameObjectSetActive(FocusModeIcon, Actor.FocusModeState);
		chargeEquipment.FreshAvailableIcon(Actor);
	}

	public void OnclickFocusModeIcon()
	{
		if (!(FocusModeIcon == null))
		{
			TooltipManager.OpenTextBoxWithText(FocusModeIcon.gameObject, LocalizationManager.GetText("Traits.FocusMode.Icon.Description"));
		}
	}

	private void FreshOverloadIcon()
	{
		if (Actor == null)
		{
			Helpers.GameObjectSetActive(OverloadIcon, value: false);
			return;
		}
		int num = Actor.OverloadStatusLeftTurns;
		if (num <= 0)
		{
			num = 0;
		}
		if (num > 0)
		{
			Helpers.GameObjectSetActive(OverloadIcon, value: true);
			OverloadIcon.transform.Find("txt").GetComponent<UILabel>().text = num.ToString();
		}
		else
		{
			Helpers.GameObjectSetActive(OverloadIcon, value: false);
		}
	}
}
