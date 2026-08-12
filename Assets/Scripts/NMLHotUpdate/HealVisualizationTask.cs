using System.Collections;
using TWDModel;
using UnityEngine;

public class HealVisualizationTask : ActorVisualizationTask
{
	private bool started;

	public float Delay { get; set; }

	public ActorModel TargetActor { get; set; }

	private ActorView TargetView { get; set; }

	private IEnumerator ShowVisualHandle { get; set; }

	private bool Done { get; set; }

	public HealVisualizationTask(HealAction action)
		: base(action)
	{
		base.Actor = GameManager.Instance.modelManager.GetModel<ActorModel>(action.ModelId);
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		TargetActor = action.TargetActor;
		if (TargetActor != null)
		{
			AddDependency(TargetActor, reserve: false);
			TargetView = GameManager.Instance.GetViewForModel(TargetActor) as ActorView;
		}
	}

	public void StartVisualization()
	{
		Start();
		ReleaseDependency(TargetActor);
		Done = false;
		ShowVisualHandle = ShowVisual();
		GameManager.Instance.StartCoroutine(ShowVisualHandle);
		Done = true;
	}

	private IEnumerator ShowVisual()
	{
		HealAction healAction = base.Action as HealAction;
		ActorHitEffects component = TargetView.GetComponent<ActorHitEffects>();
		if (component != null && healAction.AmountHealed > 0)
		{
			component.SpawnHealEffects(healAction.SourceActor);
			TargetView.AddNotification(new ActorNotificationMessage(Mathf.Abs(healAction.AmountHealed).ToString(), ActorNotificationType.Heal));
		}
		Done = true;
		yield return null;
	}

	public override void Stop()
	{
		if (ShowVisualHandle != null && GameManager.Instance != null)
		{
			GameManager.Instance.StopCoroutine(ShowVisualHandle);
		}
	}

	public override bool Update(float deltaTime)
	{
		Delay -= deltaTime;
		if (Delay > 0f)
		{
			return true;
		}
		if (!started)
		{
			StartVisualization();
			started = true;
		}
		return !Done;
	}
}
