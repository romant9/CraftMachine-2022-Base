using TWDModel;

public class ChangeWeaponPoseVisualizationTask : ActorVisualizationTask
{
	private SurvivorAnimationController SurvivorAnimationController
	{
		get
		{
			if (!(base.ActorView != null))
			{
				return null;
			}
			return base.ActorView.CharacterAnimationController as SurvivorAnimationController;
		}
	}

	private WeaponPose TargetPose { get; set; }

	public ChangeWeaponPoseVisualizationTask(ActorModel actor, WeaponPose targetPose)
		: base(null)
	{
		base.Actor = actor;
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		TargetPose = targetPose;
		AddFactionDependency(base.Actor.Faction);
		AddActorDependency(base.Actor);
	}

	public override void Start()
	{
		base.Start();
		if (SurvivorAnimationController != null)
		{
			SurvivorAnimationController.DesiredWeaponPose = TargetPose;
		}
	}

	public override bool Update(float deltaTime)
	{
		if (base.ActorView == null)
		{
			return false;
		}
		if (base.Actor.IsDead)
		{
			return false;
		}
		if (SurvivorAnimationController != null)
		{
			if (SurvivorAnimationController.IsInDeath || SurvivorAnimationController.IsDeathRequested)
			{
				return false;
			}
			if (RunTime > 10f)
			{
				if (TargetPose == WeaponPose.Raised)
				{
					SurvivorAnimationController.ForceRaiseWeapon();
					return false;
				}
				SurvivorAnimationController.ForceIdle();
				return false;
			}
			SurvivorAnimationController.DesiredWeaponPose = TargetPose;
			return SurvivorAnimationController.CurrentWeaponPose != TargetPose;
		}
		return false;
	}
}
