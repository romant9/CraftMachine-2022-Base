using TWDModel;

public class ActionCameraVisualizationTask : ActorVisualizationTask
{
	private ActorModel SourceActor { get; set; }

	private ActorView SourceView { get; set; }

	public ActionCameraVisualizationTask(ActorModel sourceActor, ActorModel targetActor)
		: base(null)
	{
		if (targetActor != null)
		{
			AddFactionDependency(targetActor.Faction);
		}
		AddDependency(targetActor, reserve: false);
		AddDependency(sourceActor, reserve: false);
		SourceActor = sourceActor;
		SourceView = GameManager.Instance.GetViewForModel(SourceActor) as ActorView;
		base.Actor = targetActor;
		base.ActorView = ((base.Actor != null) ? (GameManager.Instance.GetViewForModel(base.Actor) as ActorView) : null);
	}

	public override void Start()
	{
		base.Start();
		if (!GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.ToggleCombatCameraEnabled") || !(ActionCamera.Instance != null) || CombatHUD.IsSpeedUpEnabled || base.Actor == null)
		{
			return;
		}
		DamageVisualizationTask nextActorTask = VisualizationQueue.Instance.GetNextActorTask<DamageVisualizationTask>(base.Actor);
		bool flag = ActionCamera.Instance.AlwaysTrigger || (nextActorTask?.IsCritical ?? false);
		bool num = nextActorTask?.IsFollowThrough ?? false;
		bool flag2 = (base.Actor.Faction == Faction.Survivor || base.Actor.Faction == Faction.Civilian) && (base.Actor.IsDead || base.Actor.IsStruggling);
		if (!num && ((base.Actor.IsDead && flag) || flag2))
		{
			ActionCameraType actionCameraType = (base.Actor.IsDead ? ActionCameraType.CriticalKill : ActionCameraType.CriticalDamage);
			if (flag2)
			{
				actionCameraType = ActionCameraType.SurvivorDeath;
			}
			if (GameManager.Instance.playerModel.Combat.SceneName == "Tutorial_Camp_001_Tutorial_Simplified")
			{
				if (GameManager.Instance.gameEconomyData == null || !GameManager.Instance.gameEconomyData.RookieConfigData.Delete3DCamera01)
				{
					ActionCamera.Instance.RequestActionCamera(base.ActorView.transform.position, actionCameraType, base.Actor.ModelId);
				}
			}
			else
			{
				ActionCamera.Instance.RequestActionCamera(base.ActorView.transform.position, actionCameraType, base.Actor.ModelId);
			}
		}
		else if (CombatView.Instance.Model.MissionCompleted && CombatView.Instance.Model.MissionResult == ECombatResult.Successful)
		{
			ActionCameraType actionCameraType2 = ActionCameraType.CombatExitLocation;
			ActionCamera.Instance.RequestActionCamera(base.ActorView.transform.position, actionCameraType2, base.Actor.ModelId);
		}
	}

	public override bool Update(float deltaTime)
	{
		return false;
	}
}
