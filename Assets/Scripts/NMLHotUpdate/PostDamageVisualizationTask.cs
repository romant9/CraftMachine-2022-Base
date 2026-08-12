using System.Collections.Generic;
using TWDModel;

public class PostDamageVisualizationTask : ActorVisualizationTask
{
	private ActorModel DamagerActor { get; set; }

	private ActorView DamagerView { get; set; }

	public PostDamageVisualizationTask(PostDamageAction action)
		: base(action)
	{
		base.Actor = GameManager.Instance.modelManager.GetModel<ActorModel>(action.ModelId);
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		DamagerActor = action.DamagerActor;
		if (DamagerActor != null)
		{
			AddDependency(DamagerActor, reserve: false);
			DamagerView = GameManager.Instance.GetViewForModel(DamagerActor) as ActorView;
		}
	}

	public override void Start()
	{
		base.Start();
		ReleaseDependency(DamagerActor);
	}

	public override bool Update(float deltaTime)
	{
		if (OfflineManager.IsLoadDataManager && OfflineManager.IsTutorialDisable) return false;
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		PostDamageAction postDamageAction = base.Action as PostDamageAction;
		if (combat != null)
		{
			List<TWDModelObject> models = combat.GetModels<CoverModel>();
			if (models != null && models.Count > 0 && GameManager.Instance.SmartTutorialData.HasShown(SmartTutorialType.CoverPart1) && !GameManager.Instance.SmartTutorialData.HasShown(SmartTutorialType.CoverPart2) && DamagerActor != null && DamagerActor.Faction == Faction.Survivor && base.Actor.IsHuman && combat.HasCover(base.Actor.GridCoordinate) && postDamageAction.DamageAction.DamageType == DamageType.Ranged)
			{
				GameManager.Instance.SmartTutorialData.StartSmartTutorial(SmartTutorialType.CoverPart2);
			}
		}
		return false;
	}
}
