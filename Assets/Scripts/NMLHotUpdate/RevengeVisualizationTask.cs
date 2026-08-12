using System.Collections;
using TWDModel;

public class RevengeVisualizationTask : ActorVisualizationTask
{
	private bool started;

	public float Delay { get; set; }

	public ActorModel TargetActor { get; set; }

	public ActorModel RevengedActor { get; set; }

	private ActorView TargetView { get; set; }

	private IEnumerator ShowVisualHandle { get; set; }

	private bool Done { get; set; }

	public RevengeVisualizationTask(RevengeAction action)
		: base(action)
	{
		base.Actor = GameManager.Instance.modelManager.GetModel<ActorModel>(action.ModelId);
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		TargetActor = action.TargetActor;
		RevengedActor = action.RevengedActor;
		if (TargetActor != null)
		{
			AddDependency(TargetActor, reserve: false);
			TargetView = GameManager.Instance.GetViewForModel(TargetActor) as ActorView;
		}
		if (base.Actor != null)
		{
			AddActorDependency(base.Actor);
		}
		if (action.RevengedActor != null)
		{
			AddActorDependency(action.RevengedActor);
		}
	}

	public void StartVisualization()
	{
		Start();
		ReleaseDependency(base.Actor);
		ReleaseDependency(TargetActor);
		ReleaseDependency(RevengedActor);
		Done = false;
		ShowVisualHandle = ShowVisual();
		GameManager.Instance.StartCoroutine(ShowVisualHandle);
		Done = true;
	}

	private IEnumerator ShowVisual()
	{
		base.ActorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.Revenge"), "Ui_Icon_Trait_Revenge"));
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
