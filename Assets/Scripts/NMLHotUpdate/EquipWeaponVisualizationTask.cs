using TWDModel;

public class EquipWeaponVisualizationTask : ActorVisualizationTask
{
	private CharacterAnimationController CharacterAnimationController => base.ActorView.GetComponent<CharacterAnimationController>();

	public EquipmentItemModel Equipment { get; private set; }

	public EquipWeaponVisualizationTask(ActorModel actor, EquipmentItemModel equipment)
		: base(null)
	{
		base.Actor = actor;
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		Equipment = equipment;
		AddFactionDependency(base.Actor.Faction);
		AddActorDependency(base.Actor);
	}

	public override void Start()
	{
		base.Start();
		if (Equipment != null && base.ActorView.CurrentWeapon != Equipment)
		{
			base.ActorView.RequestSwitchEquipment(Equipment);
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
