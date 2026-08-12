using System;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class StruggleVisualizationTask : ActorVisualizationTask
{
	private ActorModel Target { get; set; }

	private ActorView TargetView { get; set; }

	public StruggleVisualizationTask(StruggleAction action)
		: base(action, affectsCovers: true)
	{
		base.Actor = GameManager.Instance.modelManager.GetModel<ActorModel>(action.ModelId);
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		Target = action.Target;
		TargetView = GameManager.Instance.GetViewForModel(action.Target) as ActorView;
		AddFactionDependency(base.Actor.Faction);
		AddActorDependency(base.Actor);
		AddActorDependency(Target);
	}

	public StruggleVisualizationTask(ActorModel source, ActorModel target)
		: base(null, affectsCovers: true)
	{
		base.Actor = source;
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		Target = target;
		TargetView = GameManager.Instance.GetViewForModel(Target) as ActorView;
		AddFactionDependency(base.Actor.Faction);
		AddActorDependency(base.Actor);
		AddActorDependency(Target);
	}

	public override void Start()
	{
		StruggleAction obj = base.Action as StruggleAction;
		if (obj == null || !obj.Avoided)
		{
			TargetView.CharacterAnimationController.EnsureIdle();
		}
	}

	public override bool Update(float deltaTime)
	{
		if (base.ActorView == null || TargetView == null)
		{
			return false;
		}
		StruggleAction obj = base.Action as StruggleAction;
		if (obj != null && obj.Avoided)
		{
			TargetView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.StruggleAvoided"), "Ui_Icon_StatusEffect_Struggling", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
			return false;
		}
		CharacterAnimationController characterAnimationController = base.ActorView.CharacterAnimationController;
		CharacterAnimationController characterAnimationController2 = TargetView.CharacterAnimationController;
		if (!characterAnimationController.IsIdle || !characterAnimationController2.IsIdle)
		{
			if (!characterAnimationController.IsIdle && !characterAnimationController.IsInTransition)
			{
				characterAnimationController.EnsureIdle();
			}
			if (!characterAnimationController2.IsIdle && !characterAnimationController2.IsInTransition)
			{
				characterAnimationController2.EnsureIdle();
			}
			return true;
		}
		TargetView.SetWeaponActive(active: false);
		GridCoordinate gridCoordinate = base.Actor.GridCoordinate;
		GridCoordinate gridCoordinate2 = Target.GridCoordinate;
		FixedVec3 position = GridView.Instance.GetPosition(gridCoordinate);
		FixedVec3 position2 = GridView.Instance.GetPosition(gridCoordinate2);
		Vector3 vector = new Vector3(0f, 1f, 0f);
		Vector3 normalized = (position2 - position).ToVector3().normalized;
		Vector3 a = new Vector3(0f, 0f, 1f);
		float angle = a.SignedAngle(normalized, vector);
		float angle2 = a.SignedAngle(-normalized, vector);
		base.ActorView.transform.position = position.ToVector3();
		TargetView.transform.position = position2.ToVector3();
		base.ActorView.transform.rotation = Quaternion.AngleAxis(angle, vector);
		TargetView.transform.rotation = Quaternion.AngleAxis(angle2, vector);
		characterAnimationController.EnterStruggle();
		characterAnimationController2.EnterStruggle();
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			string name = Enum.GetName(typeof(Faction), base.Actor.Faction);
			string text = "combat_" + name + "/" + name + "_enter_struggle";
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(text.ToLower(), base.ActorView.gameObject);
			if (base.Actor.Faction == Faction.Walker)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_walker/walker_struggle", base.ActorView.gameObject);
			}
			if (Target.IsHuman)
			{
				if (Target.Gender == ActorGender.Male)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_survivor/survivor_male_struggle", TargetView.gameObject);
				}
				else
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_survivor/survivor_female_struggle", TargetView.gameObject);
				}
			}
		}
		return false;
	}
}
