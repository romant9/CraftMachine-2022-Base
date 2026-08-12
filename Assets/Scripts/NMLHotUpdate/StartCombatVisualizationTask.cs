using System;
using TWDModel;

public class StartCombatVisualizationTask : VisualizationTask
{
	private const float timeToShowStartingSurvivorsNotifications = 0f;

	private float delayTimer;

	public ActorModel Actor { get; protected set; }

	public ActorView ActorView { get; protected set; }

	public StartCombatVisualizationTask()
		: base(null)
	{
		delayTimer = 0f;
	}

	public override bool Update(float deltaTime)
	{
		if (GameManager.Instance.playerModel.Combat.MissionCompleted)
		{
			return false;
		}
		if (SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatMissionObjectivesPopUp).IsOpen || SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatQuickTipPopup).IsOpen || SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.Tutorial).IsOpen)
		{
			delayTimer = 0f;
			return true;
		}
		delayTimer -= deltaTime;
		if (delayTimer <= 0f)
		{
			CombatModel model = CombatView.Instance.Model;
			bool flag = model.Walkers.Count > 0 || model.Raiders.Count > 0;
			for (int i = 0; i < model.Survivors.Count; i++)
			{
				SurvivorModel survivorModel = model.Survivors[i] as SurvivorModel;
				ActorView actorView = GameManager.Instance.GetViewForModel((ActorModel)survivorModel) as ActorView;
				if (survivorModel == null || !(actorView != null))
				{
					continue;
				}
				bool flag2 = false;
				bool flag3 = false;
				if (model.manager.Player.ActivityManager.TryGetActivityParam(ActivityType.Classstartscharged, out var activityParams))
				{
					if (activityParams[0] == "Survivors")
					{
						if (!survivorModel.IsHero)
						{
							flag2 = true;
						}
					}
					else if (survivorModel.SurvivorClass == (SurvivorClass)Enum.Parse(typeof(SurvivorClass), activityParams[0]))
					{
						flag3 = true;
					}
				}
				if (survivorModel.NumberChargePointAtStart > 0 || flag3 || flag2)
				{
					string text = "";
					if ((flag3 || flag2) && !model.IsGuildBattleMission && !model.IsSurvivalMission)
					{
						text = "Notification.EventCharge";
						actorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(text)));
					}
					else if (survivorModel.NumberChargePointAtStart > 0)
					{
						text = "Traits.LeaderBuffReadyForAction";
						actorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(text), "Ui_Icon_Trait_LeaderBuffReadyForAction"));
					}
				}
				if (survivorModel.HasAnyLevelTrait("LeaderBuffMarkEnemy") && flag && !actorView.MarkedNotificationShown)
				{
					actorView.MarkedNotificationShown = true;
					survivorModel.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffMarkEnemy", false });
				}
				if (survivorModel.ChargeMeter != null && survivorModel.ChargeMeter.EXMaxLevel > 0)
				{
					survivorModel.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffOverload", false });
				}
			}
			return false;
		}
		return true;
	}
}
