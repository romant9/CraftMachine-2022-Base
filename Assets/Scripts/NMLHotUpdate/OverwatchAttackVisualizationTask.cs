using System.Collections;
using TWDModel;

public class OverwatchAttackVisualizationTask : ActorVisualizationTask
{
	private bool started;

	public float Delay { get; set; }

	public ActorModel TargetActor { get; set; }

	private ActorView TargetView { get; set; }

	private IEnumerator ShowVisualHandle { get; set; }

	private bool Done { get; set; }

	public OverwatchAttackVisualizationTask(OverwatchAttackAction action)
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
		if (base.Action is OverwatchAttackAction overwatchAttackAction && !TargetActor.IsDead && TargetActor == overwatchAttackAction.TargetActor && base.Actor.GetTraitsThatContain("Interruptor").Count > 0)
		{
			string textId = (overwatchAttackAction.Interrupted ? "ActorNotification.Interrupted" : "ActorNotification.Interrupt.Avoided");
			TargetView?.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(textId)));
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
