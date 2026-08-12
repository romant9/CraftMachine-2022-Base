using System;
using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class MissionObjectiveView : ModelView<MissionObjective>
{
	public float ChangeEffectTime = 5f;

	private float changeEffectTimer;

	private UILabel descriptionLabel;

	private List<UISprite> lootTokens = new List<UISprite>();

	private GameObject deadlyContainer;

	private UIWidget bg;

	private GameObject keysContainer;

	private GameObject key1;

	private GameObject key2;

	private GameObject key3;

	private GameObject pvpObjectivesContainer;

	private GameObject pvpObjectiveFlag;

	private GameObject pvpObjectiveLoot;

	private GameObject pvpObjectiveDefenders;

	private GameObject pvpTimerObject;

	private GameObject pvpTimeOutObject;

	private WaveNotification pvpTimerWarningNotification;

	private bool pvpFlagCompleted;

	private bool pvpLootCompleted;

	private bool pvpDefendersCompleted;

	private bool pvpTimerWarningShown;

	private EffectSparkle objectiveChangeEffect;

	private GameObject objectiveChangedObject;

	private bool firstTimeChangingObjective = true;

	private Color objectiveCompletedColor = new Color(1f, 1f, 1f, 1f);

	private long previousTimeInSeconds;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		base.Model.manager.RegisterDelayedEventListener(base.Model, OnModelChange);
		base.Model.manager.RegisterDelayedEventListener(base.Model.manager.Player.Combat.MissionStatistics, OnMissionStatisticsChange);
		SetupVisuals();
	}

	private GameObject GetChild(GameObject parent, string childName)
	{
		Transform transform = parent.transform.Find(childName);
		if (transform != null)
		{
			return transform.gameObject;
		}
		return null;
	}

	public void ShowKeys(bool show)
	{
		if (keysContainer != null)
		{
			keysContainer.SetActive(show && GameManager.Instance.playerModel.Combat.MissionType != MissionType.Rescue);
		}
	}

	public void SetCombatTimeLeft(long timeInSeconds)
	{
		if (pvpTimerObject != null)
		{
			pvpTimerObject.GetComponent<UILabel>().text = ((timeInSeconds > 0) ? Helpers.FormatTime(timeInSeconds * 1000) : ("0" + LocalizationManager.GetText("Generic.Time.SecondSmall")));
		}
		if (pvpTimeOutObject != null && !pvpTimerWarningShown && timeInSeconds <= 30)
		{
			pvpTimerWarningShown = true;
			pvpTimeOutObject.SetActive(value: true);
			if (pvpTimerWarningNotification != null)
			{
				pvpTimerWarningNotification.Reset();
			}
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/timer_warning");
		}
		if (timeInSeconds < previousTimeInSeconds)
		{
			if (timeInSeconds <= 10)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/timer_tick");
			}
			if (timeInSeconds <= 3)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/timer_tick_warning");
			}
		}
		previousTimeInSeconds = timeInSeconds;
	}

	public void Reset()
	{
		pvpTimerWarningShown = false;
		pvpTimeOutObject.SetActive(value: false);
		SingularityMonoBehaviour<AudioManager>.Instance.StopEvent("combat_ui/timer_warning");
	}

	private void Update()
	{
		if (objectiveChangeEffect != null)
		{
			changeEffectTimer -= Time.deltaTime;
			objectiveChangeEffect.enabled = changeEffectTimer > 0f;
		}
	}

	private void SetupVisuals()
	{
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		string text = "";
		text = ((!combat.HasPvPRules) ? "ObjectiveLabel" : "ObjectiveLabel_Outpost");
		GameObject gameObject = (GameObject)UnityUtils.LoadFromAssetBundle(text, HUDElementConfig.BundleName);
		if (gameObject == null)
		{
			Debug.LogError("Could not find resource: " + text);
			return;
		}
		if ((bool)gameObject)
		{
			GameObject gameObject2 = Helpers.InstantiateToParent(gameObject, base.transform.gameObject);
			keysContainer = GetChild(gameObject2, "Keys");
			if (keysContainer != null)
			{
				key1 = GetChild(keysContainer, "Key_Icon_1");
				key2 = GetChild(keysContainer, "Key_Icon_2");
				key3 = GetChild(keysContainer, "Key_Icon_3");
				keysContainer.SetActive(combat.MissionType != MissionType.Rescue && !combat.HasPvPRules);
			}
			if (combat.HasPvPRules)
			{
				pvpObjectiveFlag = GetChild(gameObject2, "Icon_Flag");
				pvpObjectiveLoot = GetChild(gameObject2, "Icon_Crate");
				pvpObjectiveDefenders = GetChild(gameObject2, "Icon_Survivor");
				pvpTimerObject = GetChild(gameObject2, "Time_Label");
				pvpTimeOutObject = GetChild(gameObject2, "Time_Out");
				pvpTimerWarningNotification = CombatView.Instance.CombatHUD.CreateTimerNotificationIndicator();
			}
			GameObject child = GetChild(gameObject2, "DescriptionLabel");
			deadlyContainer = GetChild(gameObject2, "Deadly");
			bg = GetChild(gameObject2, "Bg").GetComponent<UIWidget>();
			UIEventListener uIEventListener = UIEventListener.Get(GetChild(gameObject2, "Bg").GetComponent<UIButton>().gameObject);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClicked));
			string text2 = "Not_Found";
			GameObject child2 = GetChild(gameObject2, "Loot");
			if (child2 != null)
			{
				for (int i = 1; i <= 3; i++)
				{
					string childName = text2 + i;
					GameObject child3 = GetChild(child2, childName);
					lootTokens.Add(child3.GetComponent<UISprite>());
					child3.SetActive(value: false);
				}
			}
			descriptionLabel = child.GetComponent<UILabel>();
			objectiveChangeEffect = gameObject2.GetComponentInChildren<EffectSparkle>();
			objectiveChangedObject = GetChild(gameObject2, "ObjectiveUpdated");
		}
		firstTimeChangingObjective = true;
		RefreshVisuals();
	}

	private void OnClicked(GameObject button)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatMissionObjectivesPopUp).Open();
	}

	private void RefreshVisuals()
	{
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (descriptionLabel != null)
		{
			if (combat.HasPvPRules)
			{
				string text = LocalizationManager.GetText("Popup.Social.WeeklyChallenge.EndsIn");
				descriptionLabel.text = text;
				TweenManager.PlayTweenGroup(objectiveChangedObject, 1);
			}
			else
			{
				string text2 = descriptionLabel.text;
				CombatExitModel model = base.Model.manager.CombatModel.GetModel<CombatExitModel>();
				if (model != null && model.Enabled)
				{
					text2 = LocalizationManager.GetText("MissionObjective.ReachExit", base.Model.CustomText1, base.Model.CustomText2);
					if (SingularityMonoBehaviour<AudioManager>.Instance != null && !SingularityMonoBehaviour<AudioManager>.Instance.IsEventPlaying("combat_ui/combat_start") && SingularityMonoBehaviour<AudioManager>.Instance.combatSfxLoaded)
					{
						SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/exit_ready");
					}
				}
				else if (base.Model.Description != null && base.Model.Description.Length > 0)
				{
					text2 = LocalizationManager.GetText("MissionObjective." + base.Model.Description, base.Model.CustomText1, base.Model.CustomText2);
				}
				if (text2 != descriptionLabel.text)
				{
					descriptionLabel.text = text2;
					changeEffectTimer = ChangeEffectTime;
					TweenManager.PlayTweenGroup(objectiveChangedObject, 1);
				}
			}
		}
		bg.leftAnchor.absolute = (base.Model.manager.CombatModel.IsDeadly ? (-36) : (-10));
		if (deadlyContainer != null)
		{
			deadlyContainer.SetActive(base.Model.manager.CombatModel.IsDeadly);
		}
		UpdateObjectives();
	}

	public void UpdateViewFromTask()
	{
		RefreshVisuals();
	}

	private void UpdateUponChange(bool showObjectivesPopup)
	{
		if (base.Model != null && base.Model.manager != null && base.Model.manager.CombatModel != null)
		{
			List<TWDModelObject> models = base.Model.manager.CombatModel.GetModels<CombatExitModel>();
			if (VisualizationQueue.Instance != null)
			{
				VisualizationQueue.Instance.Add(new MissionObjectiveVisualizationTask(base.Model, models, showObjectivesPopup && firstTimeChangingObjective));
				firstTimeChangingObjective = false;
			}
		}
	}

	private void OnMissionStatisticsChange(ModelObject model, string changed, object args)
	{
		if (changed == "LootCollected")
		{
			UpdateUponChange(showObjectivesPopup: false);
		}
	}

	private void OnModelChange(ModelObject model, string changed, object args)
	{
		if (changed == "Status" || changed == "Description")
		{
			bool showObjectivesPopup = false;
			if (changed == "Description" && args != null)
			{
				showObjectivesPopup = (bool)args;
			}
			UpdateUponChange(showObjectivesPopup);
		}
	}

	private void UpdateObjectives()
	{
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (combat.HasPvPRules)
		{
			if (pvpObjectiveFlag != null && !pvpFlagCompleted && combat.IsPvPFlagCollected)
			{
				pvpFlagCompleted = true;
				pvpObjectiveFlag.GetComponent<UISprite>().color = objectiveCompletedColor;
				TweenManager.PlayTweenGroup(pvpObjectiveFlag, 0);
			}
			if (pvpObjectiveLoot != null && !pvpLootCompleted && combat.IsPvPLootCollected)
			{
				pvpLootCompleted = true;
				pvpObjectiveLoot.GetComponent<UISprite>().color = objectiveCompletedColor;
				TweenManager.PlayTweenGroup(pvpObjectiveLoot, 0);
			}
			if (pvpObjectiveDefenders != null && !pvpDefendersCompleted && combat.IsPvpDefendersKilled)
			{
				pvpDefendersCompleted = true;
				pvpObjectiveDefenders.GetComponent<UISprite>().color = objectiveCompletedColor;
				TweenManager.PlayTweenGroup(pvpObjectiveDefenders, 0);
			}
		}
		else
		{
			if (key1 != null)
			{
				key1.SetActive(base.Model.manager.Player.LootManager.AvailableKeys > 0);
			}
			if (key2 != null)
			{
				key2.SetActive(base.Model.manager.Player.LootManager.AvailableKeys > 1);
			}
			if (key3 != null)
			{
				key3.SetActive(base.Model.manager.Player.LootManager.AvailableKeys > 2);
			}
		}
	}

	public void UpdateUI()
	{
		UpdateUponChange(showObjectivesPopup: false);
	}

	private void OnEnable()
	{
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
	}

	private void OnDisable()
	{
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		RefreshVisuals();
	}
}
