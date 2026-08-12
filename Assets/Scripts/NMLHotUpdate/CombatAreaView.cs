using BaseModel;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class CombatAreaView : CombatModelView
{
	[SerializeField]
	private Transform areaTransform;

	[SerializeField]
	private GameObject areaEffectAlly;

	[SerializeField]
	private GameObject areaEffectOpponent;

	private bool isAlly;

	private GameObject AreaEffect
	{
		get
		{
			if (!isAlly)
			{
				return areaEffectOpponent;
			}
			return areaEffectAlly;
		}
	}

	public void SetNewScale(float newScale)
	{
		areaTransform.Find("PitfallAreaAlly").Find("Circle").localScale *= newScale;
		areaTransform.Find("PitfallAreaAlly").Find("DecalImpact").localScale *= newScale;
		areaTransform.Find("PitfallAreaAlly").Find("ExplosionDistortion").localScale *= newScale;
		areaTransform.Find("PitfallAreaAlly").Find("ContinousDistortion").localScale *= newScale;
		areaTransform.Find("PitfallAreaAlly").Find("Expansive Wave").localScale *= newScale;
	}

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		CombatArea combatArea = (CombatArea)base.Model;
		isAlly = combatArea.Faction == Faction.Survivor;
		Helpers.GameObjectSetActive(areaEffectAlly, isAlly);
		Helpers.GameObjectSetActive(areaEffectOpponent, !isAlly);
		areaTransform.localScale = Vector3.one * (float)combatArea.Radius;
		base.transform.position = GridView.Instance.GetPosition(combatArea.Coordinate).ToVector3();
		TweenManager.PlayTweenGroup(AreaEffect, 1);
	}

	public override void Kill()
	{
		TweenManager.PlayTweenGroup(AreaEffect, 2, forward: true, delegate
		{
			base.Kill();
		});
	}
}
