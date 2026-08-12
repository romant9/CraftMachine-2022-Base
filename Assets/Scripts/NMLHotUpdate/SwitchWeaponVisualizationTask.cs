using TWDModel;

public class SwitchWeaponVisualizationTask : ActorVisualizationTask
{
	private CharacterAnimationController CharacterAnimationController => base.ActorView.CharacterAnimationController;

	public bool SwitchToMelee { get; private set; }

	public SwitchWeaponVisualizationTask(ActorModel actor, bool switchToMelee)
		: base(null)
	{
		base.Actor = actor;
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		SwitchToMelee = switchToMelee;
		AddFactionDependency(base.Actor.Faction);
		AddActorDependency(base.Actor);
	}

	public override void Start()
	{
		base.Start();
		if (base.ActorView.IsMeleeWeaponEquipped != SwitchToMelee)
		{
			base.ActorView.RequestWeaponSwitch(SwitchToMelee);
		}
	}

	public override bool Update(float deltaTime)
	{
		if (base.ActorView == null)
		{
			return false;
		}
		if (!base.ActorView.SwitchingWeapon)
		{
			return !base.ActorView.CharacterAnimationController.IsIdle;
		}
		return true;
	}
}
