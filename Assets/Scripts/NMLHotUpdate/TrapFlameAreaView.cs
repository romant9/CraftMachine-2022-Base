using BaseModel;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class TrapFlameAreaView : CombatModelView
{
	[SerializeField]
	private Transform areaTransform;

	[SerializeField]
	private GameObject areaEffectAlly;

	[SerializeField]
	private GameObject areaEffectOpponent;

	[SerializeField]
	private GameObject areaQuad_1_1;

	[SerializeField]
	private GameObject areaQuad_1_2;

	[SerializeField]
	private GameObject areaQuad_2_1;

	[SerializeField]
	private GameObject areaQuad_2_2;

	private bool isAlly;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		TrapFlameArea trapFlameArea = (TrapFlameArea)base.Model;
		isAlly = trapFlameArea.Faction == Faction.Survivor;
		Helpers.GameObjectSetActive(areaEffectAlly, isAlly);
		Helpers.GameObjectSetActive(areaQuad_1_1, isAlly);
		Helpers.GameObjectSetActive(areaQuad_1_2, isAlly);
		areaTransform.localScale = Vector3.one * (float)trapFlameArea.Radius;
		base.transform.position = GridView.Instance.GetPosition(trapFlameArea.EffectiveAreaGridCoordinate).ToVector3();
		if (DelayedActionGrenadeThrowVisualizationTask.TryDeferFlameTrapUntilDetonation(this, trapFlameArea.EffectiveAreaGridCoordinate))
		{
			SetFlameTrapVisible(visible: false);
		}
		else
		{
			SetFlameTrapVisible(visible: true);
		}
	}

	public void SetFlameTrapVisible(bool visible)
	{
		Helpers.GameObjectSetActive(areaEffectAlly, visible && isAlly);
		Helpers.GameObjectSetActive(areaQuad_1_1, visible && isAlly);
		Helpers.GameObjectSetActive(areaQuad_1_2, visible && isAlly);
		Helpers.GameObjectSetActive(areaEffectOpponent, visible && !isAlly);
		Helpers.GameObjectSetActive(areaQuad_2_1, visible && !isAlly);
		Helpers.GameObjectSetActive(areaQuad_2_2, visible && !isAlly);
	}

	public override void Kill()
	{
		VisualizationQueue.Instance.Add(new CustomVisualizationTask(delegate
		{
			base.Kill();
		}));
	}
}
